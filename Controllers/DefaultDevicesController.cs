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
using Carbon_inventory_platform.Services;

namespace Carbon_inventory_platform.Controllers
{
    // 原本寫死 [Authorize(Roles = "Admin")]，改用權限表驅動：PermissionFilterAttribute
    // 內部已有「Admin 一律放行」的判斷，因此行為預設不變，但之後可透過角色權限總覽
    // 開放給其他角色（例如唯讀檢視）。
    [Authorize]
    [ServiceFilter(typeof(PermissionFilterAttribute))]
    public class DefaultDevicesController : Controller
    {
        private const int PageSize = 20; // 跟 Materials/Permissions 一致，避免筆數變多後一次全部渲染拖慢畫面。

        private readonly ApplicationDbContext _context;
        private readonly MaterialCatalogService _materialCatalog;

        public DefaultDevicesController(ApplicationDbContext context, MaterialCatalogService materialCatalog)
        {
            _context = context;
            _materialCatalog = materialCatalog;
        }

        // GET: DefaultDevices
        public async Task<IActionResult> Index(string? keyword, int pageNumber = 1)
        {
            // 原本沒有 AsNoTracking，也沒有搜尋/分頁——筆數變多之後，找一筆預設排放源只能整頁往下翻。
            var query = _context.defaultDevices.AsNoTracking().OrderBy(d => d.Id).AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(d => d.Name.Contains(keyword) || d.Material.Contains(keyword));
            }

            var paged = await ViewModel.PagedResult<DefaultDevices>.CreateAsync(query, pageNumber, PageSize);
            ViewData["PageNumber"] = paged.PageNumber;
            ViewData["TotalPages"] = paged.TotalPages;
            ViewData["Keyword"] = keyword;
            ViewData["TotalCount"] = paged.TotalCount;
            return View(paged.Items);
        }

        // GET: DefaultDevices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var defaultDevices = await _context.defaultDevices
                .AsNoTracking()
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
                TempData["StatusMessage"] = $"已新增預設排放源「{defaultDevices.Name}」。";
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
                TempData["StatusMessage"] = $"已更新預設排放源「{defaultDevices.Name}」。";
                return RedirectToAction(nameof(Index));
            }
            await PopulateMaterialSelectListsAsync(defaultDevices.Material, defaultDevices.Scope, defaultDevices.EmissionPattern);
            return View(defaultDevices);
        }

        // POST: DefaultDevices/Delete/5
        // 原本刪除要先進一個確認頁（GET Delete → 按確認 → POST），改成跟 Devices/Areas 一樣，
        // 直接在清單頁用 data-cip-confirm 二次確認後送出，不需要多一次頁面切換。
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var defaultDevices = await _context.defaultDevices.FindAsync(id);
            if (defaultDevices != null)
            {
                _context.defaultDevices.Remove(defaultDevices);
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = $"已刪除預設排放源「{defaultDevices.Name}」。";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DefaultDevicesExists(int id)
        {
            return _context.defaultDevices.Any(e => e.Id == id);
        }

        /// <summary>
        /// 原燃物料 / 類別 / 排放型式原本是純文字輸入框，打錯字不會有任何提示，
        /// 之後用這台預設裝置產生的排放資料就會靜靜地對不到係數。改為由現有資料產生下拉選單。
        /// </summary>
        private async Task PopulateMaterialSelectListsAsync(string? selectedMaterial, string? selectedScope, string? selectedEmissionPattern)
        {
            ViewData["MaterialList"] = new SelectList(await _materialCatalog.GetMaterialNamesAsync(), selectedMaterial);
            ViewData["ScopeList"] = new SelectList(await _materialCatalog.GetScopesAsync(), selectedScope);
            ViewData["EmissionPatternList"] = new SelectList(await _materialCatalog.GetEmissionPatternsAsync(), selectedEmissionPattern);
        }

        /// <summary>
        /// 伺服器端也要擋，避免有人繞過下拉選單直接送出不存在的原燃物料/類別/排放型式。
        /// 原本只驗證了 Material，Scope／EmissionPattern 雖然畫面上是下拉選單，
        /// 但直接組 POST 還是能送出任意字串。
        /// </summary>
        private async Task ValidateMaterialAsync(DefaultDevices defaultDevices)
        {
            var result = await _materialCatalog.ValidateAsync(defaultDevices.Material, defaultDevices.Scope, defaultDevices.EmissionPattern);
            foreach (var (field, message) in result.Errors)
            {
                ModelState.AddModelError(field, message);
            }
        }
    }
}
