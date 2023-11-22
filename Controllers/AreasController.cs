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
        public async Task<IActionResult> Index() //非同步方法
        {
            return _context.Areas != null ? //如果有抓到資料表Null
                          View(await _context.Areas
                          .Where(x=>x.isDeleted==0) //抓出資料表裡面沒被刪除的
                          .Include(x=> x.Company)
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
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name");
            return View();
        }

        // POST: Areas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CompanyId,Name,PostalCode,City,District,Address,FactorCode,UniqueCode,Year,Type")] Area area)
        {
            if (ModelState.IsValid)
            {
                var toCreate = new Area()
                {
                    Id = Guid.NewGuid(),
                    Name = area.Name,
                    PostalCode = area.PostalCode,
                    City = area.City,
                    District = area.District,
                    Address = area.Address,
                    FactorCode = area.FactorCode,
                    UniqueCode = area.UniqueCode,
                    Year = area.Year,
                    Type = area.Type,
                    isDeleted = 0,
                    CreateTime = DateTime.Now
                };
                _context.Add(area);
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
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,CompanyId,Name,PostalCode,City,District,FactorCode,UniqueCode,Address,Year,Type")] Area area)
        {
            if (id != area.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var toUpdate = await _context.Areas.FindAsync(id);

                    if (toUpdate != null)
                    {
                        toUpdate.CompanyId = area.CompanyId;
                        toUpdate.Name = area.Name;
                        toUpdate.PostalCode = area.PostalCode;
                        toUpdate.City = area.City;
                        toUpdate.District = area.District;
                        toUpdate.Address = area.Address;
                        toUpdate.FactorCode = area.FactorCode;
                        toUpdate.UniqueCode = area.UniqueCode;
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
            if (toDelete != null)
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

        public async Task<IActionResult> Default()
        {
            await _context.Areas.AddAsync(new Area()
            {
                Id = Guid.NewGuid(),
                CompanyId = _context.Companies.Where(c => c.Name == "Default").Select(c => c.Id).FirstOrDefault(),
                Name = "Default",
                PostalCode = 0,
                City = "Default",
                District = "Default",
                Address = "Default",
                Year = 111,
                Type = "Default",
                CreateTime = DateTime.Now
            });
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
