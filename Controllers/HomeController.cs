using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

using Xceed.Words.NET;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Xceed.Document.NET;


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
                        .OrderBy(x => x.CreateTime)
                        .ToListAsync()) :
                        Problem("沒有找到資料"); //否則回報問題 Entity set 'ApplicationDbContext.Companies'  is null.
        }

        public IActionResult Privacy()
        {
            return View();
        }

        

        public async Task<IActionResult> Default()
        {
            var Company_id = Guid.NewGuid();
            var Area_id = Guid.NewGuid();
            await _context.Companies.AddAsync(new Company()
            {
                Id = Company_id,
                Name = "Default",
                Owner = "Default",
                Email = "Default",
                Phone = "Default",
                CreateTime = DateTime.Now
            });
            await _context.SaveChangesAsync();
            await _context.Areas.AddAsync(new Area()
            {
                Id = Area_id,
                CompanyId = Company_id,
                Year = 111,
                CreateTime = DateTime.Now
            });
            await _context.SaveChangesAsync();
            await _context.Devices.AddAsync(new Device()
            {
                Id = Guid.NewGuid(),
                AreaId = Area_id,
                Name = "緊急發電機",
                Material = "柴油",
                Scope = "類別一",

                EmissionPattern = "固定",
                CO2_Emission = true,
                CH4_Emission = true,
                N2O_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.Devices.AddAsync(new Device()
            {
                Id = new Guid(),
                AreaId = Area_id,
                Name = "公務車",
                Material = "柴油",
                Scope = "類別一",
                EmissionPattern = "移動",
                CO2_Emission = true,
                CH4_Emission = true,
                N2O_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.Devices.AddAsync(new Device()
            {
                Id = new Guid(),
                AreaId = Area_id,
                Name = "公務車",
                Material = "車用汽油",
                Scope = "類別一",
                EmissionPattern = "移動",
                CO2_Emission = true,
                CH4_Emission = true,
                N2O_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.Devices.AddAsync(new Device()
            {
                Id = new Guid(),
                AreaId = Area_id,
                Name = "冷氣機",
                Material = "R-410A",
                Scope = "類別一",
                EmissionPattern = "逸散",
                HFCS_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.Devices.AddAsync(new Device()
            {
                Id = new Guid(),
                AreaId = Area_id,
                Name = "飲水機",
                Material = "R-134A",
                Scope = "類別一",
                EmissionPattern = "逸散",
                HFCS_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.Devices.AddAsync(new Device()
            {
                Id = new Guid(),
                AreaId = Area_id,
                Name = "乾燥機",
                Material = "R-134A",
                Scope = "類別一",
                EmissionPattern = "逸散",
                HFCS_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.Devices.AddAsync(new Device()
            {
                Id = new Guid(),
                AreaId = Area_id,
                Name = "冰水主機",
                Material = "R-134A",
                Scope = "類別一",
                EmissionPattern = "逸散",
                HFCS_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.Devices.AddAsync(new Device()
            {
                Id = new Guid(),
                AreaId = Area_id,
                Name = "車用空調",
                Material = "R-134A",
                Scope = "類別一",
                EmissionPattern = "逸散",
                HFCS_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.Devices.AddAsync(new Device()
            {
                Id = new Guid(),
                AreaId = Area_id,
                Name = "化糞池",
                Material = "廢水處理",
                Scope = "類別一",
                EmissionPattern = "逸散",
                CH4_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.Devices.AddAsync(new Device()
            {
                Id = new Guid(),
                AreaId = Area_id,
                Name = "電力",
                Material = "外購電力",
                Scope = "類別二",
                EmissionPattern = "外購電力",
                CO2_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
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