using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Filters;
using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Carbon_inventory_platform.Controllers
{
    [CheckSubscriptionData]
    [Authorize]
    public class EmissionController : CountController
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Services.CompanyOwnershipService _ownership;
        private readonly Services.AreaTrendService _trend;
        private readonly Services.GhgReportBuilder _reportBuilder;
        public EmissionController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment, UserManager<ApplicationUser> userManager, Services.CompanyOwnershipService ownership, Services.AreaTrendService trend, Services.GhgReportBuilder reportBuilder) : base(context)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _ownership = ownership;
            _trend = trend;
            _reportBuilder = reportBuilder;
        }

        // B2：歷年趨勢頁。用點進來的這個 Area 找出「同一個廠區」（CompanyId + FullAddress）
        // 歷年的排放量資料，畫成趨勢圖；只有 1 個年度時交給 View 自己顯示空狀態，不在這裡擋。
        public async Task<IActionResult> Trend(Guid id)
        {
            if (!await CanAccessAreaAsync(id))
            {
                return NotFound();
            }

            var area = await _context.Areas.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            if (area == null)
            {
                return NotFound();
            }

            ViewBag.AreaName = string.IsNullOrEmpty(area.Name) ? area.FullAddress : area.Name;
            var trend = await _trend.GetTrendAsync(area.CompanyId, area.FullAddress);
            return View(trend);
        }

        // 原本報表/圖表動作只用 Area Id 查詢，任何登入者拿到別家公司的 Area Id 就能取得對方完整盤查資料，
        // 會導致跨公司資料外洩。統一改為呼叫共用的 CompanyOwnershipService（與 AreasController／DevicesController 共用同一份邏輯）。
        private Task<bool> CanAccessAreaAsync(Guid? areaId) => _ownership.CanAccessAreaAsync(User, areaId);


        public async Task<IActionResult> IndexAsync(Guid? id)
        {
            // 原本沒有檢查 Area 是否屬於登入者的公司，會導致任何登入者看到別家公司的排放數據。
            if (!await CanAccessAreaAsync(id))
            {
                return NotFound();
            }
            TempData["yearId"] = id;
            bool success = await CountEmissionAsync(id);
            if (!success)
            {
                return NotFound();
            }

            var emissions = await _context.Areas
                .AsNoTracking()
                .Include(y => y.Company)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            return View(emissions);
        }
        public async Task<IActionResult> AR5ReportAsync(Guid? id)
        {
            // 跳轉到Area修改AR版本
            // 完後跳到MOEReportAync
            var areaId = TempData.Peek("areaId");
            return RedirectToAction("Index", "Devices", new { id = areaId });
        }
        public async Task<IActionResult> AR6ReportAsync(Guid? id)
        {
            // 跳轉到Area修改AR版本
            // 完後跳到MOEReportAync
            var areaId = TempData.Peek("areaId");
            return RedirectToAction("Index", "Devices", new { id = areaId });
        }
        public async Task<IActionResult> ChartAsync(Guid? id)
        {
            // 原本沒有檢查 Area 是否屬於登入者的公司，會導致任何登入者看到別家公司的排放圖表。
            if (!await CanAccessAreaAsync(id))
            {
                return NotFound();
            }
            TempData["yearId"] = id;
            bool success = await CountEmissionAsync(id);
            if (!success)
            {
                return NotFound();
            }

            var emissions = await _context.Areas.AsNoTracking().Include(y => y.Company).Where(x => x.Id == id).FirstOrDefaultAsync();
            return View(emissions);

        }
        // A8：原本這三個 action 各自約 450 行、彼此 73%~99% 重複，同一個 bug 必須在三個地方各修一次。
        // 現在共用流程集中在 Services/GhgReportBuilder，各標準的差異集中在 Services/GhgReportSpec 的三個描述物件。
        public Task<IActionResult> ISOReportAsync(Guid id) => BuildReportAsync(Services.GhgReportSpec.Iso, id);

        public Task<IActionResult> MOEReportAsync(Guid id) => BuildReportAsync(Services.GhgReportSpec.Moe, id);

        public Task<IActionResult> IISReportAsync(Guid id) => BuildReportAsync(Services.GhgReportSpec.Iis, id);

        private async Task<IActionResult> BuildReportAsync(Services.GhgReportSpec spec, Guid id)
        {
            // 原本沒有檢查 Area 是否屬於登入者的公司，會導致任何登入者下載別家公司的完整盤查報告書。
            if (!await CanAccessAreaAsync(id))
            {
                return NotFound();
            }

            // 這一步會重算並寫回 Area 上的彙總欄位，必須在 Builder 讀取資料之前跑完，
            // 否則報告書裡的數字會是上一次計算的舊值。
            bool success = await CountEmissionAsync(id);
            if (!success)
            {
                return NotFound();
            }

            var result = await _reportBuilder.BuildAsync(spec, id);
            switch (result.Status)
            {
                case Services.GhgReportStatus.Ok:
                    return File(result.FileBytes!, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", result.FileName);
                case Services.GhgReportStatus.Error:
                    // 原本圖片讀取失敗時是用未檢查的 (Guid)TempData.Peek("areaId") 取值再導頁，
                    // 那個鍵不一定存在，錯誤處理本身就可能再丟一次例外；改成直接用當下的 Area Id。
                    TempData["Error"] = result.ErrorMessage;
                    return RedirectToAction("Index", "Devices", new { id = id });
                default:
                    return NotFound();
            }
        }

    }
}