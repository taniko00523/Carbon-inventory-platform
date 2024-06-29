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
                ViewBag.SearchARcount = year;
                var gwps = await _context.GWPs.Where(x => x.ARCount == year).ToListAsync();
                return gwps != null ? View(gwps) : Problem("沒有找到資料表");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(GWP model, int searchARcount)
        {
            if (ModelState.IsValid)
            {
                model.ARCount = searchARcount;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { year = searchARcount });
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GWP model, int searchARcount)
        {
            if (ModelState.IsValid)
            {
                model.ARCount = searchARcount;
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { year = searchARcount });
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id, int searchARcount)
        {
            if (id == null)
            {
                return NotFound();
            }
            var gwp = await _context.GWPs.FindAsync(id);
            if (gwp != null)
            {
                _context.GWPs.Remove(gwp);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { year = searchARcount });
        }
    }
}
