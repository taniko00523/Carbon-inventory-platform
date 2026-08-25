using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Filters;
using Carbon_inventory_platform.Models;
using Carbon_inventory_platform.Services;
using Carbon_inventory_platform.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Carbon_inventory_platform.Controllers
{
    [AllowAnonymous]
    [CheckSubscriptionData]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AreaTrendService _trend;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, AreaTrendService trend)
        {
            _context = context;
            _userManager = userManager;
            _trend = trend;
        }

        // B3：登入後首頁依角色顯示摘要資訊；未登入者維持原本的宣傳頁（View 內以 Model == null 判斷）。
        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return View((HomeDashboardViewModel?)null);
            }

            bool isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            var model = isAdmin ? await BuildAdminDashboardAsync() : await BuildInventoryDashboardAsync();
            return View(model);
        }

        private async Task<HomeDashboardViewModel> BuildAdminDashboardAsync()
        {
            var companies = await _context.Companies.AsNoTracking().ToListAsync();
            // 剛建立、還沒填寫任何資料的公司 Name 會是空字串，連結上要有文字可以點，不能顯示空白。
            var incomplete = companies
                .Where(c => !CompanyReportProgress.IsComplete(c))
                .Select(c => (c.Id, string.IsNullOrWhiteSpace(c.Name) ? "(尚未填寫公司名稱)" : c.Name))
                .ToList();

            var recentFeedbacks = await _context.Feedbacks
                .AsNoTracking()
                .OrderByDescending(f => f.CreatedAt)
                .Take(5)
                .ToListAsync();

            // 即將到期：30 天內到期、尚未過期的帳號。
            var soon = DateTime.Now.AddDays(30);
            var expiringSoon = await _context.Users
                .AsNoTracking()
                .Where(u => u.UserLimitData != null && u.UserLimitData <= soon && u.UserLimitData >= DateTime.Now)
                .OrderBy(u => u.UserLimitData)
                .Select(u => new { u.UserName, u.UserLimitData })
                .Take(10)
                .ToListAsync();

            return new HomeDashboardViewModel
            {
                IsAdmin = true,
                TotalCompanies = companies.Count,
                CompletedReportCount = companies.Count - incomplete.Count,
                IncompleteCompanies = incomplete,
                RecentFeedbacks = recentFeedbacks,
                ExpiringSoonAccounts = expiringSoon.Select(u => (u.UserName ?? "(未命名帳號)", u.UserLimitData!.Value)).ToList(),
            };
        }

        private async Task<HomeDashboardViewModel> BuildInventoryDashboardAsync()
        {
            var model = new HomeDashboardViewModel { IsAdmin = false };

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return model;
            }

            var company = await _context.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.UserId == userId);
            if (company == null)
            {
                return model;
            }
            model.HasCompany = true;

            var areas = await _context.Areas.AsNoTracking()
                .Where(a => a.CompanyId == company.Id)
                .OrderByDescending(a => a.Year)
                .ToListAsync();
            model.HasArea = areas.Count > 0;
            if (areas.Count == 0)
            {
                return model;
            }

            var areaIds = areas.Select(a => a.Id).ToList();
            var devices = await _context.Devices.AsNoTracking()
                .Where(d => areaIds.Contains(d.AreaId))
                .Include(d => d.ActivityDatas)
                .ToListAsync();
            model.HasDevice = devices.Count > 0;
            model.HasActivityData = devices.Any(d => d.ActivityDatas != null && d.ActivityDatas.Count > 0);
            model.MissingActivityDataDeviceCount = devices.Count(d => d.ActivityDatas == null || d.ActivityDatas.Count == 0);

            // 依年度排序後最新一筆，作為首頁「本年度」摘要的代表廠區。
            var currentArea = areas[0];
            model.CurrentAreaId = currentArea.Id;
            model.CurrentAreaYear = currentArea.Year;
            model.CurrentAreaTrend = await _trend.GetSinglePointAsync(currentArea.Id);

            return model;
        }

        public IActionResult OutOfLimitTime()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
