using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Filters;
using Carbon_inventory_platform.Models;

namespace Carbon_inventory_platform.Controllers
{
    // 原本寫死 [Authorize(Roles = "Admin")]，改用權限表驅動：PermissionFilterAttribute
    // 內部已有「Admin 一律放行」的判斷，因此行為預設不變，但之後可透過角色權限總覽
    // 開放給其他角色（例如唯讀檢視）。
    [Authorize]
    [ServiceFilter(typeof(PermissionFilterAttribute))]
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
        public async Task<IActionResult> Create()
        {
            await PopulateMaterialSelectListsAsync(null, null, null);
            return View();
        }

        // POST: DefaultDevices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // 原本新增時也綁定了自動編號主鍵 Id，只要有人多送一個 Id 欄位，
        // INSERT 就會帶上明確的 Id 而觸發 IDENTITY_INSERT 錯誤（500）。
        public async Task<IActionResult> Create([Bind("Name,Material,Scope,EmissionPattern,Type")] DefaultDevices defaultDevices)
        {
            await ValidateMaterialAsync(defaultDevices);

            if (ModelState.IsValid)
            {
                _context.Add(defaultDevices);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            await PopulateMaterialSelectListsAsync(defaultDevices.Material, defaultDevices.Scope, defaultDevices.EmissionPattern);
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
            await PopulateMaterialSelectListsAsync(defaultDevices.Material, defaultDevices.Scope, defaultDevices.EmissionPattern);
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

            await ValidateMaterialAsync(defaultDevices);

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
            await PopulateMaterialSelectListsAsync(defaultDevices.Material, defaultDevices.Scope, defaultDevices.EmissionPattern);
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

        /// <summary>
        /// 原燃物料 / 類別 / 排放型式原本是純文字輸入框，打錯字不會有任何提示，
        /// 之後用這台預設裝置產生的排放資料就會靜靜地對不到係數。改為由現有資料產生下拉選單，
        /// 作法與 DeviceDatasController 的 PopulateMaterialSelectListsAsync 相同。
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
        private async Task ValidateMaterialAsync(DefaultDevices defaultDevices)
        {
            var names = await GetMaterialNamesAsync();
            if (!names.Contains(defaultDevices.Material))
            {
                ModelState.AddModelError(nameof(DefaultDevices.Material), "原燃物料必須是排放係數表或 GWP 表中已存在的名稱，請重新選擇。");
            }
        }
    }
}
