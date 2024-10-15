using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;

namespace Carbon_inventory_platform.Controllers
{
    public class DefaultDevicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DefaultDevicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DefaultDevices
        public async Task<IActionResult> Index()
        {
            return View(await _context.defaultDevices.ToListAsync());
        }

        // GET: DefaultDevices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var defaultDevices = await _context.defaultDevices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (defaultDevices == null)
            {
                return NotFound();
            }

            return View(defaultDevices);
        }

        // GET: DefaultDevices/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DefaultDevices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Material,Scope,EmissionPattern,Type")] DefaultDevices defaultDevices)
        {
            if (ModelState.IsValid)
            {
                _context.Add(defaultDevices);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(defaultDevices);
        }

        // GET: DefaultDevices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var defaultDevices = await _context.defaultDevices.FindAsync(id);
            if (defaultDevices == null)
            {
                return NotFound();
            }
            return View(defaultDevices);
        }

        // POST: DefaultDevices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Material,Scope,EmissionPattern,Type")] DefaultDevices defaultDevices)
        {
            if (id != defaultDevices.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(defaultDevices);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DefaultDevicesExists(defaultDevices.Id))
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
            return View(defaultDevices);
        }

        // GET: DefaultDevices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var defaultDevices = await _context.defaultDevices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (defaultDevices == null)
            {
                return NotFound();
            }

            return View(defaultDevices);
        }

        // POST: DefaultDevices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var defaultDevices = await _context.defaultDevices.FindAsync(id);
            if (defaultDevices != null)
            {
                _context.defaultDevices.Remove(defaultDevices);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DefaultDevicesExists(int id)
        {
            return _context.defaultDevices.Any(e => e.Id == id);
        }
    }
}
