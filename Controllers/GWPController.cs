using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Filters;
using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Carbon_inventory_platform.Controllers
{
    // 原本寫死 [Authorize(Roles = "Admin")]，改用權限表驅動：PermissionFilterAttribute
    // 內部已有「Admin 一律放行」的判斷，因此行為預設不變，但之後可透過角色權限總覽
    // 開放給其他角色。新增動作叫 Add（不是標準的 Create），對應的權限已在 DbSeeder 額外補上。
    [Authorize]
    [ServiceFilter(typeof(PermissionFilterAttribute))]
    public class GWPController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public GWPController(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: GWP?year=6
        // 原本一定要先手動輸入 AR 版本才會顯示任何資料，剛進來的畫面永遠是空的，
        // 也沒有列出目前到底有哪些版本可以選。改成預設選最新（數字最大）的既有版本，
        // 並把所有既有版本列成快速切換的頁籤。
        public async Task<IActionResult> Index(int? year)
        {
            var availableVersions = await _context.GWPs
                .AsNoTracking()
                .Select(g => g.ARVersion)
                .Distinct()
                .OrderByDescending(v => v)
                .ToListAsync();

            int? selectedVersion = year ?? availableVersions.FirstOrDefault();
            if (selectedVersion == 0 && availableVersions.Count == 0)
            {
                selectedVersion = null; // 完全沒有任何 GWP 資料，不預設任何版本。
            }

            ViewBag.AvailableVersions = availableVersions;
            ViewBag.SearchARVersion = selectedVersion;

            if (selectedVersion == null)
            {
                return View(new List<GWP>());
            }

            var gwps = await _context.GWPs.AsNoTracking().Where(x => x.ARVersion == selectedVersion).OrderBy(x => x.Name).ToListAsync();
            return View(gwps);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(GWP model, int searchARVersion)
        {
            model.ARVersion = searchARVersion;
            await ValidateNotDuplicateAsync(model, excludeId: 0);

            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                _cache.Remove(CountController.GwpCacheKey); // A3：GWP 改了要讓計算引擎的快取立即失效。
                TempData["StatusMessage"] = $"已新增 GWP「{model.Name}」。";
                return RedirectToAction(nameof(Index), new { year = searchARVersion });
            }
            // 原本驗證失敗時 return View(model)，但 Views/GWP 底下只有 Index.cshtml，
            // 會拋 InvalidOperationException（找不到 Add 檢視）而變成 500。
            // 新增/編輯是 Index 頁裡的模態框，所以改為帶著錯誤訊息回到 Index。
            TempData["GWPError"] = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return RedirectToAction(nameof(Index), new { year = searchARVersion });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GWP model, int searchARVersion)
        {
            model.ARVersion = searchARVersion;
            await ValidateNotDuplicateAsync(model, excludeId: model.Id);

            if (ModelState.IsValid)
            {
                // 原本直接 _context.Update(model)，但模態框只送出 Id/Name/Num，
                // 會把沒送出的 Source 欄位一併覆寫成預設值 "MOE"。改為只更新可編輯欄位。
                var gwpToUpdate = await _context.GWPs.FindAsync(model.Id);
                if (gwpToUpdate == null)
                {
                    return NotFound();
                }

                gwpToUpdate.Name = model.Name;
                gwpToUpdate.Num = model.Num;
                gwpToUpdate.ARVersion = searchARVersion;
                await _context.SaveChangesAsync();
                _cache.Remove(CountController.GwpCacheKey); // A3：GWP 改了要讓計算引擎的快取立即失效。
                TempData["StatusMessage"] = $"已更新 GWP「{model.Name}」。";
                return RedirectToAction(nameof(Index), new { year = searchARVersion });
            }
            // 同 Add：Views/GWP 沒有 Edit.cshtml，原本的 return View(model) 是保證的 500。
            TempData["GWPError"] = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return RedirectToAction(nameof(Index), new { year = searchARVersion });
        }

        // 原本 Delete 沒有 [HttpPost]，GET 也能刪除資料，
        // 一個 <img src="/GWP/Delete/21"> 或瀏覽器預先載入就會刪掉 GWP 係數。
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int searchARVersion)
        {
            var gwp = await _context.GWPs.FindAsync(id);
            if (gwp != null)
            {
                _context.GWPs.Remove(gwp);
                await _context.SaveChangesAsync();
                _cache.Remove(CountController.GwpCacheKey); // A3：GWP 改了要讓計算引擎的快取立即失效。
                TempData["StatusMessage"] = $"已刪除 GWP「{gwp.Name}」。";
            }

            return RedirectToAction(nameof(Index), new { year = searchARVersion });
        }

        /// <summary>
        /// (Name, ARVersion) 原本只有非唯一索引（純粹為了查詢效能），沒有唯一性約束。
        /// CountController.CEFAddAsync 用同一組條件查 HFCS 的 GWP 值時，重複資料一樣會讓
        /// .FirstOrDefault() 取到哪一筆變成不確定，改為新增/編輯時就擋下重複組合。
        /// </summary>
        private async Task ValidateNotDuplicateAsync(GWP model, int excludeId)
        {
            var duplicated = await _context.GWPs.AnyAsync(g =>
                g.Id != excludeId
                && g.Name == model.Name
                && g.ARVersion == model.ARVersion);
            if (duplicated)
            {
                ModelState.AddModelError(string.Empty, $"AR{model.ARVersion} 已經有名稱「{model.Name}」的 GWP 資料，請確認是否要編輯既有資料，而不是新增重複的一筆。");
            }
        }
    }
}
