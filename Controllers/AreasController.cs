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

namespace Carbon_inventory_platform.Controllers
{
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
            return _context.Areas != null ? //如果有抓到資料表Null
                          View(await _context.Areas
                          .Include(x => x.Company)
                          .Where(x=>x.isDeleted==0 && x.CompanyId==Id) //抓出資料表裡面沒被刪除的
                          .OrderBy(x=>x.CreateTime)
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
            TempData["comanyId"] = Id;
            return View();
        }

        // POST: Areas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,PostalCode,FactorCode,UniqueCode,FullAddress,Year,Type")] Area area)
        {
            if (ModelState.IsValid)
            {
                string City = area.FullAddress;
                string District = area.FullAddress;
                string Address = area.FullAddress;
                var companyId = TempData["comanyId"] as Guid?;
                var toCreate = new Area();
                {
                    if (companyId.HasValue)
                    {
                        toCreate.CompanyId = companyId.Value;
                    }
                    toCreate.Id = Guid.NewGuid();
                    toCreate.Name = "test";
                    //PostalCode = area.PostalCode,
                    toCreate.FullAddress = area.FullAddress;
                    toCreate.City = GetCity(City);
                    toCreate.District = GetDistrict(District);
                    toCreate.Address = GetAddress(Address);
                    //FactorCode = area.FactorCode,
                    //UniqueCode = area.UniqueCode,
                    toCreate.Year = area.Year;
                    toCreate.Type = area.Type;
                    toCreate.isDeleted = 0;
                    toCreate.CreateTime = DateTime.Now;
                }
                _context.Add(toCreate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", area.CompanyId);
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

            ViewBag.CompanyId = new SelectList(_context.Companies, "Id", "Name", area.CompanyId);
            return View(area);
        }


        // POST: Areas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,CompanyId,Name,PostalCode,FactorCode,UniqueCode,FullAddress,Year,Type")] Area area)
        {
            if (id != area.Id)
            {
                return NotFound();
            }

            string City = area.FullAddress;
            string District = area.FullAddress;
            string Address = area.FullAddress;
            if (ModelState.IsValid)
            {
                try
                {
                    var toUpdate = await _context.Areas.FindAsync(id);

                    if (toUpdate != null)
                    {
                        toUpdate.CompanyId = area.CompanyId;
                        toUpdate.Name = area.Name;
                        //toUpdate.PostalCode = area.PostalCode;
                        toUpdate.FullAddress = area.FullAddress;
                        toUpdate.City = GetCity(City);
                        toUpdate.District = GetDistrict(District);
                        toUpdate.Address = GetAddress(Address);
                        //toUpdate.FactorCode = area.FactorCode;
                        //toUpdate.UniqueCode = area.UniqueCode;
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
                return RedirectToAction("Index","Home");
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", area.CompanyId);
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
            if (toDelete.Address == "Default")
            {
                _context.Areas.Remove(toDelete);
            }else if (toDelete != null)
            {
                toDelete.isDeleted = 1;
                toDelete.DeleteTime = DateTime.Now;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AreaExists(Guid id)
        {
          return (_context.Areas?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        static string GetCity(string input)
        {
            // 使用Substring获取从指定位置开始到字符串末尾的子串
            return input.Substring(0,3);
        }
        static string GetDistrict(string input)
        {
            // 使用Substring获取从指定位置开始到字符串末尾的子串
            return input.Substring(3,3);
        }
        static string GetAddress(string input)
        {
            // 使用Substring获取从指定位置开始到字符串末尾的子串
            return input.Substring(6);
        }
    }
}
