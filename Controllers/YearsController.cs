using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.AspNetCore.Authorization;

namespace Carbon_inventory_platform.Controllers
{
    [Authorize]
    public class YearsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public YearsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Years
        public async Task<IActionResult> Index(Guid Id)
        {
            
            if (TempData.ContainsKey("ErrorMessage"))
            {
                ModelState.AddModelError(string.Empty, TempData["ErrorMessage"].ToString());
            }
            TempData["areaId"] = Id; //暫存進入畫面所查詢的areaId
            TempData["areaName"] = await _context.Areas // 暫存目前所在的公司名稱 顯示在畫面上方
           .Where(a => a.Id == Id)
           .Select(a => a.Name)
           .FirstOrDefaultAsync();
            return _context.Years != null ? //如果有抓到資料表Null
                          View(await _context.Years
                          .Include(d => d.Area.Company)
                          .Where(x => x.isDeleted == 0 && x.AreaId == Id)//抓出資料表裡面沒被刪除的
                          .OrderBy(x => x.CreateTime)
                          .ToListAsync()) : //非同步方法
                          Problem("沒有找到資料表"); //否則回報問題 Entity set 'ApplicationDbContext.Companies'  is null.
            
        }

        // GET: Years/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Years == null)
            {
                return NotFound();
            }

            var year = await _context.Years
                .Include(y => y.Area)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (year == null)
            {
                return NotFound();
            }

            return View(year);
        }

        // GET: Years/Create
        public IActionResult Create()
        {
            ViewData["AreaId"] = new SelectList(_context.Areas, "Id", "Id");
            return View();
        }

        // POST: Years/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int Year)
        {
            var areaId = TempData.Peek("areaId") as Guid?;
            var year = await _context.Years.Where(x => x.AreaId == areaId && x.Num == Year).ToListAsync();
            if (ModelState.IsValid)
            {
                if (Year < 1)
                {
                    TempData["ErrorMessage"] = "請填寫正確的年份";
                }
                else
                {
                    if (year.Count() == 0)
                    {

                        var toCreate = new Year();
                        toCreate.Id = Guid.NewGuid();
                        toCreate.Num = Year;
                        toCreate.AreaId = areaId.Value;
                        toCreate.CreateTime = DateTime.Now;
                        _context.Add(toCreate);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "已存在相同的年份";
                    }

                }

            }
            return RedirectToAction(nameof(Index), new { id = areaId });
        }

        // GET: Years/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Years == null)
            {
                return NotFound();
            }

            var year = await _context.Years.FindAsync(id);
            if (year == null)
            {
                return NotFound();
            }
            ViewData["AreaId"] = new SelectList(_context.Areas, "Id", "Id", year.AreaId);
            return View(year);
        }

        // POST: Years/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Num,Company")] Year year)
        {
            if (id != year.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var areaID = TempData.Peek("areaId") as Guid?;
                try
                {
                    var toUpdate = await _context.Years.FindAsync(id);

                    var hasYear = await _context.Areas.Where(x => x.Id == areaID).SelectMany(a => a.Years).AnyAsync(y => y.Num == year.Num);

                    if (toUpdate != null)
                    {
                        if (hasYear) 
                        {
                            ModelState.AddModelError(string.Empty, "已存在相同的年份。");
                            return View();   
                        }
                        else
                        {
                            toUpdate.Num = year.Num;
                            toUpdate.ModifiedTime = DateTime.Now;
                            _context.Update(toUpdate);
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!YearExists(year.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { id = areaID });
            }
            ViewData["AreaId"] = new SelectList(_context.Areas, "Id", "Id", year.AreaId);
            return View(year);
        }

        // GET: Years/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Years == null)
            {
                return NotFound();
            }

            var year = await _context.Years
                .Include(y => y.Area)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (year == null)
            {
                return NotFound();
            }

            return View(year);
        }

        // POST: Years/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var areaId = TempData.Peek("areaId") as Guid?;
            if (_context.Years == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Years'  is null.");
            }
            var year = await _context.Years.FindAsync(id);
            if (year != null)
            {
                _context.Years.Remove(year);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = areaId });
        }

        private bool YearExists(Guid id)
        {
            return (_context.Years?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
