using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.EntityFrameworkCore;

namespace Carbon_inventory_platform.Services
{
    /// <summary>
    /// 讀取「使用者個別權限」表，供選單顯示使用。
    /// 之前的版本用 x.Permissions（集合導航 + [ForeignKey]）查詢，
    /// 在資料庫裡實際對應到 EF 自動產生的中介表，永遠查不到資料。
    /// </summary>
    public class RolePermissionService
    {
        private readonly ApplicationDbContext _context;

        public RolePermissionService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>取得使用者被授權的「畫面」(Function) 清單，依選單排序。</summary>
        public async Task<List<Function>> GetVisibleFunctionsAsync(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new List<Function>();
            }

            return await _context.UserPermissions
                .AsNoTracking()
                .Where(up => up.UserId == userId
                             && up.Permission != null
                             && up.Permission.IsDeleted == 0
                             && up.Permission.FunctionAction!.Name == "Index")
                .Select(up => up.Permission!.Function!)
                .Where(f => f.IsShow == 1)
                .Distinct()
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
