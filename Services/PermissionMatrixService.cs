using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Carbon_inventory_platform.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace Carbon_inventory_platform.Services
{
    /// <summary>
    /// B1 階段 3／4：把「角色 × 權限」與「使用者個別權限」原本一次一筆的 CRUD
    /// （RolePermissionsController／不存在的 UserPermission 介面），
    /// 改成「一個角色／使用者一個畫面」的矩陣勾選。這裡只放資料存取與比對邏輯，
    /// 不依賴 MVC，方便單元測試。
    /// </summary>
    public class PermissionMatrixService
    {
        private readonly ApplicationDbContext _context;

        public PermissionMatrixService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>建立矩陣的骨架：畫面（列）× 動作（欄），只列出實際有定義權限的組合。</summary>
        private async Task<(List<Function> functions, List<FunctionAction> actions, List<Permission> permissions)> LoadCatalogAsync()
        {
            var permissions = await _context.Permissions
                .AsNoTracking()
                .Include(p => p.Function)
                .Include(p => p.FunctionAction)
                .Where(p => p.Function != null && p.FunctionAction != null)
                .ToListAsync();

            var functions = permissions
                .Select(p => p.Function!)
                .GroupBy(f => f.Id)
                .Select(g => g.First())
                .OrderBy(f => f.Sort)
                .ThenBy(f => f.Id)
                .ToList();

            var actions = permissions
                .Select(p => p.FunctionAction!)
                .GroupBy(a => a.Id)
                .Select(g => g.First())
                .OrderBy(a => a.Id)
                .ToList();

            return (functions, actions, permissions);
        }

        /// <summary>建立某個角色的權限矩陣，勾選狀態反映該角色目前的 RolePermission。</summary>
        public async Task<PermissionMatrixViewModel> BuildRoleMatrixAsync(string roleId)
        {
            var (functions, actions, permissions) = await LoadCatalogAsync();

            var grantedIds = (await _context.RolePermissions
                .AsNoTracking()
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.PermissionId)
                .ToListAsync())
                .ToHashSet();

            var matrix = BuildMatrix(functions, actions, permissions, grantedIds, lockedPermissionIds: null);
            matrix.Baseline = ComputeBaseline(grantedIds);
            return matrix;
        }

        /// <summary>
        /// 建立某個使用者的權限矩陣。角色已授予的權限顯示為勾選且鎖住（Locked=true）——
        /// 目前的資料結構無法表達「個別收回角色已授予的權限」，畫面上不能在這裡取消。
        /// </summary>
        public async Task<PermissionMatrixViewModel> BuildUserMatrixAsync(string userId, IEnumerable<string> roleIds)
        {
            var (functions, actions, permissions) = await LoadCatalogAsync();

            var individualIds = (await _context.UserPermissions
                .AsNoTracking()
                .Where(up => up.UserId == userId)
                .Select(up => up.PermissionId)
                .ToListAsync())
                .ToHashSet();

            var roleGrantedIds = await GetRoleGrantedPermissionIdsAsync(roleIds);

            var grantedIds = new HashSet<int>(individualIds);
            grantedIds.UnionWith(roleGrantedIds);

            var matrix = BuildMatrix(functions, actions, permissions, grantedIds, roleGrantedIds);
            // 基準值只涵蓋「可異動範圍」——跟 SaveUserMatrixAsync 比對／異動的範圍必須完全一致，
            // 否則基準值一律對不上，存檔會被誤判成永遠衝突。
            matrix.Baseline = ComputeBaseline(individualIds.Where(id => !roleGrantedIds.Contains(id)));
            return matrix;
        }

        private async Task<HashSet<int>> GetRoleGrantedPermissionIdsAsync(IEnumerable<string> roleIds)
        {
            var ids = roleIds.ToList();
            if (ids.Count == 0)
            {
                return new HashSet<int>();
            }

            return (await _context.RolePermissions
                .AsNoTracking()
                .Where(rp => ids.Contains(rp.RoleId))
                .Select(rp => rp.PermissionId)
                .ToListAsync())
                .ToHashSet();
        }

        /// <summary>排序後以逗號串接，作為「當下已授權集合」的簡單指紋，用來偵測併發修改。</summary>
        private static string ComputeBaseline(IEnumerable<int> ids)
            => string.Join(",", ids.OrderBy(id => id));

        private static PermissionMatrixViewModel BuildMatrix(
            List<Function> functions, List<FunctionAction> actions, List<Permission> permissions,
            HashSet<int> checkedPermissionIds, HashSet<int>? lockedPermissionIds)
        {
            var byFunctionAndAction = permissions.ToDictionary(p => (p.FunctionId, p.FunctionActionId), p => p);

            var vm = new PermissionMatrixViewModel
            {
                Columns = actions.Select(a => new PermissionMatrixColumn { FunctionActionId = a.Id, CName = a.CName }).ToList(),
            };

            foreach (var function in functions)
            {
                var row = new PermissionMatrixRow { FunctionId = function.Id, CName = function.CName };
                foreach (var action in actions)
                {
                    if (byFunctionAndAction.TryGetValue((function.Id, action.Id), out var permission))
                    {
                        row.Cells.Add(new PermissionMatrixCell
                        {
                            PermissionId = permission.Id,
                            Checked = checkedPermissionIds.Contains(permission.Id),
                            Locked = lockedPermissionIds?.Contains(permission.Id) ?? false,
                        });
                    }
                    else
                    {
                        row.Cells.Add(null);
                    }
                }
                vm.Rows.Add(row);
            }

            return vm;
        }

        /// <summary>
        /// 把角色的權限重設成剛好等於 checkedPermissionIds：多的刪掉、少的補上。
        ///
        /// expectedBaseline（可省略）是呼叫端在畫面渲染當下看到的授權集合指紋
        /// （見 <see cref="PermissionMatrixViewModel.Baseline"/>）。如果送出時資料庫的實際狀態
        /// 已經跟這個指紋不一樣——代表存檔前有別的管理員也改過同一個角色——
        /// 直接套用這次的勾選內容會悄悄蓋掉對方剛存的異動，所以改為拒絕存檔並回報衝突，
        /// 由呼叫端請使用者重新整理後再試一次。省略這個參數則維持舊行為（不檢查、直接覆蓋）。
        /// </summary>
        public async Task<PermissionMatrixSaveResult> SaveRoleMatrixAsync(string roleId, IEnumerable<int> checkedPermissionIds, string? expectedBaseline = null)
        {
            var checkedSet = checkedPermissionIds.ToHashSet();
            // 只在「權限目錄裡真的存在」的範圍內比較，避免表單被竄改夾帶不存在的 Id。
            var validIds = (await _context.Permissions.AsNoTracking().Select(p => p.Id).ToListAsync()).ToHashSet();
            checkedSet.IntersectWith(validIds);

            for (var attempt = 1; attempt <= 2; attempt++)
            {
                var existing = await _context.RolePermissions
                    .Where(rp => rp.RoleId == roleId)
                    .ToListAsync();
                var existingIds = existing.Select(rp => rp.PermissionId).ToHashSet();

                // 基準值只在第一次嘗試檢查——第二次是為了從下面的併發衝突自動復原，
                // 不是使用者剛送出的新請求，這裡不該再拒絕一次。
                if (attempt == 1 && expectedBaseline != null && ComputeBaseline(existingIds) != expectedBaseline)
                {
                    return PermissionMatrixSaveResult.Conflict;
                }

                var toRemove = existing.Where(rp => !checkedSet.Contains(rp.PermissionId)).ToList();
                var toAddIds = checkedSet.Where(id => !existingIds.Contains(id)).ToList();

                if (toRemove.Count == 0 && toAddIds.Count == 0)
                {
                    return PermissionMatrixSaveResult.Saved;
                }

                if (toRemove.Count > 0)
                {
                    _context.RolePermissions.RemoveRange(toRemove);
                }
                if (toAddIds.Count > 0)
                {
                    _context.RolePermissions.AddRange(toAddIds.Select(id => new RolePermission { RoleId = roleId, PermissionId = id }));
                }

                try
                {
                    await _context.SaveChangesAsync();
                    return PermissionMatrixSaveResult.Saved;
                }
                catch (DbUpdateException) when (attempt == 1)
                {
                    // 兩個請求幾乎同時送出同一份表單（雙擊送出、兩個分頁）時，各自都以為
                    // 自己讀到的是最新狀態而搶著新增／刪除，後送達的一方會撞到
                    // (RoleId, PermissionId) 的唯一索引，或試圖刪除已經被對方刪掉的資料列。
                    // 清空變更追蹤器、重新查一次資料庫目前的實際狀態，用最新資料重算一次差異再試一次；
                    // 如果第二次還是失敗，才真的把例外往外丟（這種情況已經不是單純的競態問題了）。
                    _context.ChangeTracker.Clear();
                }
            }

            // 不會執行到這裡：第二次嘗試若成功會在迴圈內 return，若失敗則因為
            // when (attempt == 1) 不成立而不會被上面的 catch 攔截，例外會直接往外傳。
            throw new InvalidOperationException("SaveRoleMatrixAsync 重試邏輯出現非預期狀態。");
        }

        /// <summary>
        /// 把使用者的「個別」權限（UserPermission）重設成剛好等於 checkedPermissionIds，
        /// 但只在「不是由角色授予」的範圍內比較與異動。角色已授予的權限一律排除在外：
        /// 即使表單被竄改夾帶進來，也不會多存一筆；已存在的個別授權（不論是否與角色重複）
        /// 也不會因為畫面上被鎖住、沒有送出而被誤刪。
        ///
        /// expectedBaseline 的用途與 <see cref="SaveRoleMatrixAsync"/> 相同：偵測「存檔前有別的
        /// 管理員也改過這個使用者的個別權限」，避免悄悄蓋掉對方的異動。
        /// </summary>
        public async Task<PermissionMatrixSaveResult> SaveUserMatrixAsync(string userId, IEnumerable<string> roleIds, IEnumerable<int> checkedPermissionIds, string? expectedBaseline = null)
        {
            var checkedSet = checkedPermissionIds.ToHashSet();
            var validIds = (await _context.Permissions.AsNoTracking().Select(p => p.Id).ToListAsync()).ToHashSet();
            checkedSet.IntersectWith(validIds);

            var roleGrantedIds = await GetRoleGrantedPermissionIdsAsync(roleIds);
            checkedSet.ExceptWith(roleGrantedIds);

            for (var attempt = 1; attempt <= 2; attempt++)
            {
                var existing = await _context.UserPermissions
                    .Where(up => up.UserId == userId && !roleGrantedIds.Contains(up.PermissionId))
                    .ToListAsync();
                var existingIds = existing.Select(up => up.PermissionId).ToHashSet();

                if (attempt == 1 && expectedBaseline != null && ComputeBaseline(existingIds) != expectedBaseline)
                {
                    return PermissionMatrixSaveResult.Conflict;
                }

                var toRemove = existing.Where(up => !checkedSet.Contains(up.PermissionId)).ToList();
                var toAddIds = checkedSet.Where(id => !existingIds.Contains(id)).ToList();

                if (toRemove.Count == 0 && toAddIds.Count == 0)
                {
                    return PermissionMatrixSaveResult.Saved;
                }

                if (toRemove.Count > 0)
                {
                    _context.UserPermissions.RemoveRange(toRemove);
                }
                if (toAddIds.Count > 0)
                {
                    _context.UserPermissions.AddRange(toAddIds.Select(id => new UserPermission { UserId = userId, PermissionId = id }));
                }

                try
                {
                    await _context.SaveChangesAsync();
                    return PermissionMatrixSaveResult.Saved;
                }
                catch (DbUpdateException) when (attempt == 1)
                {
                    // 見 SaveRoleMatrixAsync 的相同註解：清空變更追蹤器後用最新資料重算一次差異再試一次。
                    _context.ChangeTracker.Clear();
                }
            }

            throw new InvalidOperationException("SaveUserMatrixAsync 重試邏輯出現非預期狀態。");
        }
    }
}
