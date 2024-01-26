using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

using Xceed.Words.NET;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Xceed.Document.NET;
using System.Net;

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

            var del = TempData["SelectedAreaId"];
            del = TempData["yearSelected"];
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

        

        public async Task<IActionResult> Create()
        {
            var Company_id = Guid.NewGuid();
            var Area_id = Guid.NewGuid();
            await _context.Companies.AddAsync(new Company()
            {
                Id = Company_id,
                Name = "新增公司",
                Phone = "-",
                CreateTime = DateTime.Now
            });
            await _context.SaveChangesAsync();
            await _context.Areas.AddAsync(new Area()
            {
                Id = Area_id,
                CompanyId = Company_id,
                Year = DateTime.Now.Year-1912, //減去1911取得民國年 在減1取去年當基準年
                CreateTime = DateTime.Now
            });
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

       

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid companyId, Guid areaId)
        {
            if (_context.Companies == null)
            {
                return Problem("沒有找到資料");
            }
            var delComapany = await _context.Companies.FindAsync(companyId); //查詢Companies主鑑符合companyId的資料
            var delArea = await _context.Areas.Where(x => x.CompanyId == companyId).ToListAsync(); //查詢Areas中外來鑑CompanyId符合companyId的資料集合
            var delDevice = await _context.Devices.Where(x => x.AreaId == areaId).ToListAsync(); //查詢Devices中外來鑑AreaId中符合areaId的資料集合
            if (delComapany.ModifiedTime == null) 
            {
                _context.Companies.Remove(delComapany);
            }
            else
            {
                delComapany.isDeleted = 1;
                delComapany.DeleteTime = DateTime.Now;
            }

            if(delArea.Count != 0) //是否有Area資料
            {
                foreach (var area in delArea) //有則遍尋delArea裡的資料，沒修改過的直接刪除，有修改過的則隱藏
                {
                    if (area.ModifiedTime == null)
                    {
                        _context.Areas.Remove(area);
                    }
                    else
                    {
                        area.isDeleted = 1;
                        area.DeleteTime = DateTime.Now;
                    }
                }
            }
            if(delDevice.Count() != 0) //是否有Device資料
            {
                foreach (var device in delDevice) //有則遍尋delDevice 裡的資料，沒修改過的直接刪除，有修改過的則隱藏
                {
                    if (device.ModifiedTime == null) 
                    {
                        _context.Devices.Remove(device);
                    }
                    else
                    {
                        device.isDeleted = 1;
                        device.DeleteTime = DateTime.Now;
                    }
                }
            }
            
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}