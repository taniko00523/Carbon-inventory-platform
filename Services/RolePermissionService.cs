using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.EntityFrameworkCore;

namespace Carbon_inventory_platform.Services
{
    /// <summary>
    /// 讀取使用者可見的「畫面」清單，供選單顯示使用。
    /// 之前的版本用 x.Permissions（集合導航 + [ForeignKey]）查詢，
    /// 在資料庫裡實際對應到 EF 自動產生的中介表，永遠查不到資料。
    /// 再之前修好那個問題後的版本又只讀 UserPermissions，完全沒有看 RolePermissions——
    /// 而 UserPermission 目前沒有任何維護介面、資料表永遠是空的，等於選單邏輯必定回傳空清單。
    /// 現在改成跟 HasPermissionAsync 一致：使用者個別權限（UserPermission）與角色權限
    /// （RolePermission）皆可授予可見性，兩者取聯集（個別設定可以「多授予」畫面，但目前的
    /// 資料結構無法表達「個別收回角色已授予的畫面」）。
    /// </summary>
    public class RolePermissionService
    {
        private readonly ApplicationDbContext _context;

        public RolePermissionService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>取得使用者（含其角色）被授權的「畫面」(Function) 清單，依選單排序。</summary>
        public async Task<List<Function>> GetVisibleFunctionsAsync(string? userId, IEnumerable<string> roleIds)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new List<Function>();
            }

            var userFunctionIds = await _context.UserPermissions
                .AsNoTracking()
                .Where(up => up.UserId == userId
                             && up.Permission != null
                             && up.Permission.IsDeleted == 0
                             && up.Permission.FunctionAction!.Name == "Index")
                .Select(up => up.Permission!.FunctionId)
                .ToListAsync();

            var ids = roleIds.ToList();
            var roleFunctionIds = ids.Count == 0
                ? new List<int>()
                : await _context.RolePermissions
                    .AsNoTracking()
                    .Where(rp => ids.Contains(rp.RoleId)
                                 && rp.Permission != null
                                 && rp.Permission.IsDeleted == 0
                                 && rp.Permission.FunctionAction!.Name == "Index")
                    .Select(rp => rp.Permission!.FunctionId)
                    .ToListAsync();

            var functionIds = userFunctionIds.Union(roleFunctionIds).ToHashSet();
            if (functionIds.Count == 0)
            {
                return new List<Function>();
            }

            return await _context.Functions
                .AsNoTracking()
                .Where(f => functionIds.Contains(f.Id) && f.IsShow == 1)
                .OrderBy(f => f.Sort)
                .ToListAsync();
        }

        /// <summary>判斷使用者（含其角色）是否具有指定畫面/動作的權限。</summary>
        public async Task<bool> HasPermissionAsync(string? userId, IEnumerable<string> roleIds, string controller, string action)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }

            var permissionId = await _context.Permissions
                .Where(p => p.IsDeleted == 0
                            && p.Function!.Name == controller
                            && p.FunctionAction!.Name == action)
                .Select(p => (int?)p.Id)
                .FirstOrDefaultAsync();

            if (permissionId == null)
            {
                return false;
            }

            if (await _context.UserPermissions.AnyAsync(up => up.UserId == userId && up.PermissionId == permissionId))
            {
                return true;
            }

            var ids = roleIds.ToList();
            return ids.Count > 0
                   && await _context.RolePermissions.AnyAsync(rp => ids.Contains(rp.RoleId) && rp.PermissionId == permissionId);
        }
    }
}
