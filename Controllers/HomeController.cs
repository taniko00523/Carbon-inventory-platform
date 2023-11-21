using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Carbon_inventory_platform.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Companies
        public async Task<IActionResult> Index()
        {
            return _context.Areas != null ? //如果有抓到資料表Null
                        View(await _context.Areas
                        .Where(x => x.isDeleted == 0) //抓出資料表裡面沒被刪除的
                        .Include(x => x.Company)
                        .ToListAsync()) :
                        Problem("沒有找到資料"); //否則回報問題 Entity set 'ApplicationDbContext.Companies'  is null.
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //public IActionResult OtherAction(int id)
        //{
        //    // 從 TempData 中檢索 SelectedItemId
        //    TempData["SelectedItemId"] = id;

        //    // 重定向到 Devices 控制器的 Index 動作，並將 itemId 作為路由值傳遞
        //    return RedirectToAction("Index", "Devices", id);
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}