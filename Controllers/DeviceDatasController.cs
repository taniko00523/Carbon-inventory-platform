using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;

namespace Carbon_inventory_platform.Controllers
{
    [Authorize(Roles = "Admin")]
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
        public async Task<IActionResult> Create()
        {
            await PopulateMaterialSelectListsAsync(null, null, null);
            return View();
        }

        // POST: DeviceDatas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // 原本新增時也綁定了自動編號主鍵 Id，只要有人多送一個 Id 欄位，
        // INSERT 就會帶上明確的 Id 而觸發 IDENTITY_INSERT 錯誤（500）。
        public async Task<IActionResult> Create([Bind("Name,Scope,EmissionPattern,Material,Data_Correction,Device_Correction,unit")] DeviceData deviceData)
        {
            await ValidateMaterialAsync(deviceData);

            if (ModelState.IsValid)
            {
                _context.Add(deviceData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            await PopulateMaterialSelectListsAsync(deviceData.Material, deviceData.Scope, deviceData.EmissionPattern);
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
            await PopulateMaterialSelectListsAsync(deviceData.Material, deviceData.Scope, deviceData.EmissionPattern);
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

            await ValidateMaterialAsync(deviceData);

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
            await PopulateMaterialSelectListsAsync(deviceData.Material, deviceData.Scope, deviceData.EmissionPattern);
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

        /// <summary>
        /// 原燃物料 / 類別 / 排放型式原本是純文字輸入框，而後續計算是用字串比對
        /// Materials 與 GWPs 的名稱，打錯一個字（例如 R-410a）不會有任何提示，
        /// 該排放源之後算出來的排放量會靜靜地變成 0。改為由現有資料產生下拉選單。
        /// 冷媒類的原燃物料（R-410A、FM200…）只存在 GWPs，所以兩張表都要取。
        /// </summary>
        private async Task PopulateMaterialSelectListsAsync(string? selectedMaterial, string? selectedScope, string? selectedEmissionPattern)
        {
            ViewData["MaterialList"] = new SelectList(await GetMaterialNamesAsync(), selectedMaterial);

            var scopes = await _context.Materials
                .AsNoTracking()
                .Select(m => m.Scope)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();
            ViewData["ScopeList"] = new SelectList(scopes, selectedScope);

            var emissionPatterns = await _context.Materials
                .AsNoTracking()
                .Select(m => m.EmissionPattern)
                .Distinct()
                .OrderBy(e => e)
                .ToListAsync();
            ViewData["EmissionPatternList"] = new SelectList(emissionPatterns, selectedEmissionPattern);
        }

        private async Task<List<string>> GetMaterialNamesAsync()
        {
            var fromMaterials = await _context.Materials.AsNoTracking().Select(m => m.Name).Distinct().ToListAsync();
            var fromGwps = await _context.GWPs.AsNoTracking().Select(g => g.Name).Distinct().ToListAsync();
            return fromMaterials.Union(fromGwps).OrderBy(n => n).ToList();
        }

        /// <summary>伺服器端也要擋，避免有人繞過下拉選單直接送出不存在的原燃物料。</summary>
        private async Task ValidateMaterialAsync(DeviceData deviceData)
        {
            var names = await GetMaterialNamesAsync();
            if (!names.Contains(deviceData.Material))
            {
                ModelState.AddModelError(nameof(DeviceData.Material), "原燃物料必須是排放係數表或 GWP 表中已存在的名稱，請重新選擇。");
            }
        }
    }
}
