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
    public class DeviceDatasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DeviceDatasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DeviceDatas
        public async Task<IActionResult> Index()
        {
            return View(await _context.deviceDatas.ToListAsync());
        }

        // GET: DeviceDatas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deviceData = await _context.deviceDatas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deviceData == null)
            {
                return NotFound();
            }

            return View(deviceData);
        }

        // GET: DeviceDatas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DeviceDatas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Scope,EmissionPattern,Material,Data_Correction,Device_Correction,unit")] DeviceData deviceData)
        {
            if (ModelState.IsValid)
            {
                _context.Add(deviceData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(deviceData);
        }

        // GET: DeviceDatas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deviceData = await _context.deviceDatas.FindAsync(id);
            if (deviceData == null)
            {
                return NotFound();
            }
            return View(deviceData);
        }

        // POST: DeviceDatas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Scope,EmissionPattern,Material,Data_Correction,Device_Correction,unit")] DeviceData deviceData)
        {
            if (id != deviceData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(deviceData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeviceDataExists(deviceData.Id))
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
            return View(deviceData);
        }

        // GET: DeviceDatas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deviceData = await _context.deviceDatas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deviceData == null)
            {
                return NotFound();
            }

            return View(deviceData);
        }

        // POST: DeviceDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deviceData = await _context.deviceDatas.FindAsync(id);
            if (deviceData != null)
            {
                _context.deviceDatas.Remove(deviceData);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeviceDataExists(int id)
        {
            return _context.deviceDatas.Any(e => e.Id == id);
        }
    }
}
