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
using System.ComponentModel.Design;

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

            var del = TempData["companyId"];
            del = TempData["areaId"];
            del = TempData["areaName"];
            del = TempData["yearId"];
            del = TempData["year"];
            del = TempData["companyName"];
            var AllArea = await _context.Areas
    .Where(x => x.isDeleted == 0)
    .Include(x => x.Company)
    .OrderBy(x => x.CreateTime)
    .ToListAsync();

            var filteredAreas = AllArea.GroupBy(x => x.CompanyId) //篩選掉重複CompanyId的資料
                .Select(group => group.First())
                .ToList();
            var company = await _context.Companies.Where(x => x.isDeleted == 0).OrderByDescending(x => x.CreateTime).ToListAsync();

            return View(company);

        }

        public IActionResult Privacy()
        {
            return View();
        }



        public async Task<IActionResult> Create()
        {
            var Company_id = Guid.NewGuid();
            var Area_id = Guid.NewGuid();
            if (ModelState.IsValid)
            {
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
                    Year = DateTime.Now.Year - 1912, //減去1911取得民國年 在減1取去年當基準年
                    CreateTime = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Home");
        }



        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Companies == null)
            {
                return Problem("沒有找到資料");
            }
            //抓出要刪除的資料
            var delCompany = await _context.Companies.FindAsync(id);
            var hasCompany = _context.Companies //如果此公司有修改過
                .Where(c => c.Id == id)
                .Any(c => c.ModifiedTime != null);

            var hasArea = _context.Companies //如果此公司下有修改過的廠區
    .Where(c => c.Id == id)
    .SelectMany(c => c.Areas)
    .Any(area => area.ModifiedTime != null);

            var hasDevice = _context.Companies //如果此公司有修改過的排放源
    .Where(c => c.Id == id)
    .SelectMany(c => c.Areas)
    .SelectMany(a => a.Years)
    .SelectMany(y => y.Devices)
    .Any(device => device.ModifiedTime != null);


            if (hasCompany || hasArea || hasDevice) 
            {
                _context.Companies.Remove(delCompany);
            }
            else
            {
                delCompany.isDeleted = 1;
                delCompany.DeleteTime = DateTime.Now;
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