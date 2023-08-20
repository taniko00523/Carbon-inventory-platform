using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Carbon_inventory_platform.Controllers
{


    public class AreaController : Controller
    {
        private readonly ApplicationDbContext _context;
        //建構函式插入，在ASP.NET Core控制器中使用ApplicationDbContext
        public AreaController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Areas);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Owner,Email,Phone,isDeleted,CreateTime,ModifiedTime,DeleteTime")] Area company)
        {
            if (ModelState.IsValid)
            {
                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        public async Task<IActionResult> Edit(int? id) //非同步方法
        //Edit畫面顯示 Route:ID
        {
            if (id == null || _context.Areas == null)
            {
                return NotFound();
            }
            //檢查資料庫是否為空，或者是id為空

            var area = await _context.Areas.FindAsync(id);
            //抓取資料庫中的這筆資料

            if (area == null)
            {
                return NotFound();
            }
            //檢查資料庫是否有這筆資料
            return View(area);
        }

        public async Task<IActionResult> Delete(int? id) //非同步方法
        {
            if (id == null || _context.Areas == null)
            {
                return NotFound();
            }
            //檢查資料庫是否為空，或者是id為空

            var area = await _context.Areas
                .FirstOrDefaultAsync(m => m.Id == id);
            //抓取資料庫中的這筆資料

            if (area == null)
            {
                return NotFound();
            }
            //檢查資料庫是否有這筆資料
            return View(area);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,PostalCode,City,District,Address,Year,Type,ModifiedTime")] Area area)
        {
            if (id != area.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(area);
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
                return RedirectToAction(nameof(Index));
            }
            return View(area);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, [Bind("Id,IsDeleted,DeleteTime")] Area area)
        {
            if (id != area.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(area);
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
                return RedirectToAction(nameof(Index));
            }
            return View(area);
        }


        private bool AreaExists(int id)
        {
            return (_context.Areas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
