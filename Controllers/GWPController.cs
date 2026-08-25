using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Carbon_inventory_platform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GWPController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GWPController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int year)
        {
            if (year != 0)
            {
                ViewBag.SearchARVersion = year;
                var gwps = await _context.GWPs.Where(x => x.ARVersion == year).ToListAsync();
                return gwps != null ? View(gwps) : Problem("沒有找到資料表");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(GWP model, int searchARVersion)
        {
            if (ModelState.IsValid)
            {
                model.ARVersion = searchARVersion;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { year = searchARVersion });
            }
            // 原本驗證失敗時 return View(model)，但 Views/GWP 底下只有 Index.cshtml，
            // 會拋 InvalidOperationException（找不到 Add 檢視）而變成 500。
            // 新增/編輯是 Index 頁裡的模態框，所以改為帶著錯誤訊息回到 Index。
            TempData["GWPError"] = "資料格式錯誤，GWP值請輸入數字。";
            return RedirectToAction(nameof(Index), new { year = searchARVersion });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GWP model, int searchARVersion)
        {
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
                return RedirectToAction(nameof(Index), new { year = searchARVersion });
            }
            // 同 Add：Views/GWP 沒有 Edit.cshtml，原本的 return View(model) 是保證的 500。
            TempData["GWPError"] = "資料格式錯誤，GWP值請輸入數字。";
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
            }

            return RedirectToAction(nameof(Index), new { year = searchARVersion });
        }
    }
}
