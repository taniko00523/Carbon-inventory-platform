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
using NuGet.ContentModel;

namespace Carbon_inventory_platform.Controllers
{
    public class DevicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DevicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Devices
        public async Task<IActionResult> Index()
        {
            return _context.Devices != null ? //如果有抓到資料表Null
                          View(await _context.Devices
                          .Where(x => x.isDeleted == 0) //抓出資料表裡面沒被刪除的
                          .Include(x => x.Areas)
                          .Include (x => x.Material)
                          .Include (x => x.ActivityData)
                          .ToListAsync()) : //非同步方法
                          Problem("沒有找到資料表"); //否則回報問題 Entity set 'ApplicationDbContext.Companies'  is null.
        }

        // GET: Devices/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Devices == null)
            {
                return NotFound();
            }

            var device = await _context.Devices
                .Include(d => d.Areas)
                .Include(d => d.ActivityData)
                .Include(d => d.Material)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (device == null)
            {
                return NotFound();
            }
            return View(device);
        }

        // GET: Devices/Create
        public IActionResult Create()
        {
            ViewData["AreasId"] = new SelectList(_context.Areas.Where(x => x.isDeleted == 0), "Id", "Name");
            ViewData["ActivityDataId"] = new SelectList(_context.ActivityDatas, "Id", "Name");
            ViewData["MaterialId"] = new SelectList(_context.Materials, "Id", "Name");
            return View();
        }

        // POST: Devices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AreaId,AssetNo,Name,Provess,MaterialId")] Device device)
        {
            if (ModelState.IsValid)
            {
                var toCreate = new Device();
                if(toCreate != null)
                {
                    toCreate.Id = Guid.NewGuid();
                    toCreate.AreaId = device.AreaId;
                    toCreate.AssetNo = device.AssetNo;
                    toCreate.Name = device.Name;
                    toCreate.Provess = device.Provess;
                    toCreate.MaterialId = device.MaterialId;
                    toCreate.isDeleted = 0;
                    toCreate.CreateTime = DateTime.Now;
                    //不需要一創建就需要填寫排放氣體 否則客戶會無法填寫
                    //排放氣體應該留到Edit讓盤查員填寫
                }
                
                if (_context.Materials
                    .Where(x=>x.Id==device.MaterialId)
                    .Select(x=>x.CO2Id) != null)//如果物料的CO2有排放係數(CEF) 則自動帶出 //修正資料庫如果沒有排放係數即外來鍵Null 
                {
                    toCreate.CO2 = true;
                }
                if (_context.Materials
                    .Where(x => x.Id == device.MaterialId)
                    .Select(x => x.CH4Id) != null)//如果物料的CH4有排放係數(CEF) 則自動帶出 //修正資料庫如果沒有排放係數即外來鍵Null 
                {
                    toCreate.CH4 = true;
                }
                if (_context.Materials
                    .Where(x => x.Id == device.MaterialId)
                    .Select(x => x.N2OId) != null)//如果物料的N2O有排放係數(CEF) 則自動帶出 //修正資料庫如果沒有排放係數即外來鍵Null 
                {
                    toCreate.N2O = true;
                }
                _context.Add(toCreate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AreasId"] = new SelectList(_context.Areas.Where(x => x.isDeleted == 0), "Id", "Name");
            ViewData["ActivityDataId"] = new SelectList(_context.ActivityDatas, "Id", "Name", device.ActivityDataId);
            ViewData["MaterialId"] = new SelectList(_context.Materials, "Id", "Name", device.MaterialId);
            return View(device);
        }

        // GET: Devices/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Devices == null)
            {
                return NotFound();
            }

            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }
            ViewData["AreasId"] = new SelectList(_context.Areas.Where(x => x.isDeleted == 0), "Id", "Name");
            ViewData["ActivityDataId"] = new SelectList(_context.ActivityDatas, "Id", "Name", device.ActivityDataId);
            ViewData["MaterialId"] = new SelectList(_context.Materials, "Id", "Name", device.MaterialId);
            return View(device);
        }

        // POST: Devices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,AreaId,AssetNo,Name,Provess,ActivityDataId,MaterialId,CO2,CH4,N2O,HFCS,PFCS,SF6,NF3")] Device device)
        {
            if (id != device.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var toUpdate = await _context.Devices.FindAsync(id);

                    if (toUpdate != null)
                    {
                        toUpdate.AreaId = device.AreaId;
                        toUpdate.AssetNo = device.AssetNo;
                        toUpdate.Name = device.Name;
                        toUpdate.Provess = device.Provess;
                        toUpdate.ActivityDataId = device.ActivityDataId;
                        toUpdate.MaterialId = device.MaterialId;
                        toUpdate.CO2 = device.CO2;
                        toUpdate.CH4 = device.CH4;
                        toUpdate.N2O = device.N2O;
                        toUpdate.HFCS = device.HFCS;
                        toUpdate.PFCS = device.PFCS;
                        toUpdate.SF6 = device.SF6;
                        toUpdate.NF3 = device.NF3;
                        toUpdate.ModifiedTime = DateTime.Now;
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeviceExists(device.Id))
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
            ViewData["AreasId"] = new SelectList(_context.Areas.Where(x => x.isDeleted == 0), "Id", "Name");
            ViewData["ActivityDataId"] = new SelectList(_context.ActivityDatas, "Id", "Name", device.ActivityDataId);
            ViewData["MaterialId"] = new SelectList(_context.Materials, "Id", "Name", device.MaterialId);
            return View(device);
        }

        // GET: Devices/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Devices == null)
            {
                return NotFound();
            }

            var device = await _context.Devices
                .Include(d => d.ActivityData)
                .Include(d => d.Material)
                .Include(d => d.Areas)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        // POST: Devices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Devices == null)
            {
                return Problem("沒有找到資料");
            }
            var toDelete = await _context.Devices.FindAsync(id);
            if (toDelete != null)
            {
                toDelete.isDeleted = 1;
                toDelete.DeleteTime = DateTime.Now;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeviceExists(Guid id)
        {
            return (_context.Devices?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public IActionResult AddActivityData()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddActivityData([Bind("Id,Num,Unit,Source,Dept,Level,Correction")] ActivityData activityData)
        {
            if (ModelState.IsValid)
            {
                var toCreate = new ActivityData()
                {
                    Id = Guid.NewGuid(),
                    Num = activityData.Num,
                    Unit = activityData.Unit,
                    Source = activityData.Source,
                    Dept = activityData.Dept,
                    Level = activityData.Level,
                    Correction = activityData.Correction,
                    isDeleted = 0,
                    CreateTime = DateTime.Now
                };
                _context.Add(activityData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(activityData);
        }
    }
}
