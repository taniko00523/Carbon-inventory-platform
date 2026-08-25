using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Carbon_inventory_platform.Services
{
    /// <summary>
    /// 「這個使用者能不能存取這家公司/這個廠區的資料」的擁有權檢查。
    /// 原本 AreasController／DevicesController／EmissionController 各自複製一份幾乎一樣的邏輯，
    /// 而且三份並不一致：AreasController 的版本只放行 Admin、且只認得使用者「第一個」查到的公司
    /// （GetCallerCompanyIdAsync 用 FirstOrDefaultAsync），若同一使用者名下有第二家公司會被誤擋；
    /// Devices／Emission 的版本則放行 Admin 與 SuperAdmin，並檢查使用者名下「所有」公司。
    /// 統一成這裡一份，並以較完整的版本為準（Admin/SuperAdmin 皆放行、比對使用者名下所有公司）。
    /// </summary>
    public class CompanyOwnershipService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CompanyOwnershipService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private static bool IsAdmin(ClaimsPrincipal user) => user.IsInRole("Admin") || user.IsInRole("SuperAdmin");

        private async Task<List<Guid>> GetOwnedCompanyIdsAsync(ClaimsPrincipal user)
        {
            var userId = _userManager.GetUserId(user);
            if (string.IsNullOrEmpty(userId))
            {
                return new List<Guid>();
            }
            return await _context.Companies
                .Where(c => c.UserId == userId)
                .Select(c => c.Id)
                .ToListAsync();
        }

        /// <summary>取得登入者自己名下第一家公司的 Id，用於「預設帶入自己的公司」等畫面情境。</summary>
        public async Task<Guid?> GetCallerCompanyIdAsync(ClaimsPrincipal user)
        {
            var userId = _userManager.GetUserId(user);
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            return await _context.Companies
                .Where(c => c.UserId == userId)
                .Select(c => (Guid?)c.Id)
                .FirstOrDefaultAsync();
        }

        /// <summary>管理員（Admin/SuperAdmin）可存取所有公司，其他人只能存取自己名下的公司。</summary>
        public async Task<bool> CanAccessCompanyAsync(ClaimsPrincipal user, Guid? companyId)
        {
            if (companyId == null || companyId == Guid.Empty)
            {
                return false;
            }
            if (IsAdmin(user))
            {
                return true;
            }
            var companyIds = await GetOwnedCompanyIdsAsync(user);
            return companyIds.Contains(companyId.Value);
        }

        /// <summary>管理員（Admin/SuperAdmin）可存取所有廠區，其他人只能存取自己名下公司的廠區。</summary>
        public async Task<bool> CanAccessAreaAsync(ClaimsPrincipal user, Guid? areaId)
        {
            if (areaId == null || areaId == Guid.Empty)
            {
                return false;
            }
            if (IsAdmin(user))
            {
                return true;
            }
            var companyIds = await GetOwnedCompanyIdsAsync(user);
            if (companyIds.Count == 0)
            {
                return false;
            }
            // isDeleted 由 ApplicationDbContext 的全域查詢過濾器處理，不需要重複寫。
            return await _context.Areas.AnyAsync(a => a.Id == areaId && companyIds.Contains(a.CompanyId));
        }
    }
}
