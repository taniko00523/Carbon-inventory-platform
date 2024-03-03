using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.AspNetCore.Authorization;

namespace Carbon_inventory_platform.Controllers
{
    [Authorize]
    public class AreasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AreasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Areas
        public async Task<IActionResult> Index(Guid Id) //非同步方法
        {
            TempData["companyId"] = Id; //暫存進入畫面所查詢的CompanyId
            TempData["companyName"] = await _context.Companies // 暫存目前所在的公司名稱 顯示在畫面上方
           .Where(a => a.Id == Id)
           .Select(a => a.Name)
           .FirstOrDefaultAsync();
            return _context.Areas != null ? //如果有抓到資料表Null
                          View(await _context.Areas
                          .Include(x => x.Company)
                          .Where(x => x.isDeleted == 0 && x.CompanyId == Id) //抓出資料表裡面沒被刪除的
                          .OrderBy(x => x.CreateTime)
                          .ToListAsync()) : //非同步方法
                          Problem("沒有找到資料表"); //否則回報問題 Entity set 'ApplicationDbContext.Companies'  is null.
        }

        // GET: Areas/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Areas == null)
            {
                return NotFound();
            }

            var area = await _context.Areas
                .Include(a => a.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (area == null)
            {
                return NotFound();
            }

            return View(area);
        }

        // GET: Areas/Create
        public IActionResult Create(Guid Id)
        {
            return View();
        }

        // POST: Areas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,FullAddress,Year,Type,UniqueCode,FactorCode,OrganizationImage,MapImage,ShopDrawings")] Area area)
        {
            if (ModelState.IsValid)
            {
                var companyId = TempData.Peek("companyId") as Guid?;
                string OrganizationImagePath = await SaveImage(area.OrganizationImage, companyId.ToString(), area.Id.ToString(), "Organization.jpg");
                string MapImagePath = await SaveImage(area.MapImage, companyId.ToString(), area.Id.ToString(), "Map.jpg");
                string ShopDrawingsPath = await SaveImage(area.ShopDrawings, companyId.ToString(), area.Id.ToString(), "ShopDrawings.jpg");
                var toCreate = new Area();
                {
                    toCreate.CompanyId = companyId.Value;
                    toCreate.Id = Guid.NewGuid();
                    toCreate.Name = area.Name;
                    toCreate.FullAddress = area.FullAddress;
                    if (area.FullAddress.Length > 6)
                    {
                        toCreate.City = GetCity(area.FullAddress);
                        toCreate.District = GetDistrict(area.FullAddress);
                        toCreate.Address = GetAddress(area.FullAddress);
                    }
                    else
                    {
                        toCreate.Address = area.FullAddress;
                    }
                    toCreate.Year = area.Year;
                    toCreate.Type = area.Type;
                    toCreate.OrganizationImagePath = OrganizationImagePath;
                    toCreate.MapImagePath = MapImagePath;
                    toCreate.ShopDrawingsPath = ShopDrawingsPath;
                    toCreate.isDeleted = 0;
                    toCreate.CreateTime = DateTime.Now;
                }
                _context.Add(toCreate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { id = companyId });

            }
            return View(area);
        }



        // GET: Areas/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            var area = await _context.Areas
                .Include(x => x.Company)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (area == null)
            {
                return NotFound();
            }
            return View(area);
        }


        // POST: Areas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,FullAddress,Year,Type,UniqueCode,FactorCode,OrganizationImage,MapImage,ShopDrawings")] Area area)
        {
            if (id != area.Id)
            {
                return NotFound();
            }
            var companyId = TempData.Peek("companyId") as Guid?;
            if (ModelState.IsValid)
            {
                string OrganizationImagePath = await SaveImage(area.OrganizationImage, companyId.ToString(), area.Id.ToString(), "Organization.jpg");
                string MapImagePath = await SaveImage(area.MapImage, companyId.ToString(), area.Id.ToString(), "Map.jpg");
                string ShopDrawingsPath = await SaveImage(area.ShopDrawings, companyId.ToString(), area.Id.ToString(), "ShopDrawings.jpg");
                try
                {
                    var toUpdate = await _context.Areas.FindAsync(id);
                    if (toUpdate != null)
                    {
                        toUpdate.Name = area.Name;
                        toUpdate.FullAddress = area.FullAddress;
                        if (area.FullAddress.Length > 6)
                        {
                            toUpdate.City = GetCity(area.FullAddress);
                            toUpdate.District = GetDistrict(area.FullAddress);
                            toUpdate.Address = GetAddress(area.FullAddress);
                        }
                        else
                        {
                            toUpdate.Address = area.FullAddress;
                        }
                        toUpdate.UniqueCode = area.UniqueCode;
                        toUpdate.FactorCode = area.FactorCode;
                        toUpdate.Year = area.Year;
                        toUpdate.Type = area.Type;
                        toUpdate.ModifiedTime = DateTime.Now;
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AreaExists(area.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { id = companyId });
            }
            return View(area);
        }

        // GET: Areas/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Areas == null)
            {
                return NotFound();
            }

            var area = await _context.Areas
                .Include(a => a.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (area == null)
            {
                return NotFound();
            }

            return View(area);
        }

        // POST: Areas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Areas == null)
            {
                return Problem("沒有找到資料");
            }
            var toDelete = await _context.Areas.FindAsync(id);
            if (toDelete.ModifiedTime == null)
            {
                _context.Areas.Remove(toDelete);
            }
            else
            {
                toDelete.isDeleted = 1;
                toDelete.DeleteTime = DateTime.Now;
            }
            await _context.SaveChangesAsync();
            var companyId = TempData.Peek("companyId") as Guid?;
            return RedirectToAction(nameof(Index), new { id = companyId });
        }

        private bool AreaExists(Guid id)
        {
            return (_context.Areas?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        static string GetCity(string input)
        {
            return input.Substring(0, 3);
        }
        static string GetDistrict(string input)
        {
            return input.Substring(3, 3);
        }
        static string GetAddress(string input)
        {
            return input.Substring(6);
        }

        private async Task<string> SaveImage(IFormFile file, string companyId, string areaId, string fileName)
        {
            if (file != null && file.Length > 0)
            {
                //var fileName = Path.GetFileName(file.FileName);
                var FolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", companyId, areaId);
                var FilePath = Path.Combine(FolderPath, fileName);
                try
                {
                    // 檢查資料夾是否存在，如果不存在則創建資料夾，並新增圖片回傳檔案位置
                    if (!Directory.Exists(FolderPath))
                    {
                        Directory.CreateDirectory(FolderPath);
                        using (var stream = new FileStream(FilePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        return FilePath;
                    }
                    else
                    {
                        using (var stream = new FileStream(FilePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        return FilePath;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("發生錯誤：" + ex.Message);
                }
            }
            return null;
        }

    }
}
