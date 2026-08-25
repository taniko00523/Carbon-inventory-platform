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
    public class MaterialsController : Controller
    {
        private const int PageSize = 20; // A5：81 筆種子資料一次全部渲染會拖慢畫面，分頁顯示。

        private readonly ApplicationDbContext _context;

        public MaterialsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Materials
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            var query = _context.Materials.AsNoTracking().OrderBy(m => m.Id);
            var paged = await ViewModel.PagedResult<Material>.CreateAsync(query, pageNumber, PageSize);
            ViewData["PageNumber"] = paged.PageNumber;
            ViewData["TotalPages"] = paged.TotalPages;
            return View(paged.Items);
        }

        // GET: Materials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var material = await _context.Materials.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (material == null)
            {
                return NotFound();
            }

            return View(material);
        }

        // GET: Materials/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Materials/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // 原本新增時也綁定了自動編號主鍵 Id，只要有人多送一個 Id 欄位，
        // INSERT 就會帶上明確的 Id 而觸發 IDENTITY_INSERT 錯誤（500）。
        public async Task<IActionResult> Create([Bind("Name,Scope,EmissionPattern,CO2CEF,CO2ULL,CO2UUL,CH4CEF,CH4ULL,CH4UUL,N2OCEF,N2OULL,N2OUUL,HFCSCEF,HFCSULL,HFCSUUL,PFCSCEF,PFCSULL,PFCSUUL,SF6CEF,SF6ULL,NF3UUL,NF3CEF,NF3ULL,SF6UUL,CEF_Correction,DataUUL,DataULL,Year,Unit")] Material material)
        {
            if (ModelState.IsValid)
            {
                _context.Add(material);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(material);
        }

        // GET: Materials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var material = await _context.Materials.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (material == null)
            {
                return NotFound();
            }
            return View(material);
        }

        // POST: Materials/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Scope,EmissionPattern,CO2CEF,CO2ULL,CO2UUL,CH4CEF,CH4ULL,CH4UUL,N2OCEF,N2OULL,N2OUUL,HFCSCEF,HFCSULL,HFCSUUL,PFCSCEF,PFCSULL,PFCSUUL,SF6CEF,SF6ULL,NF3UUL,NF3CEF,NF3ULL,SF6UUL,CEF_Correction,DataUUL,DataULL,Year,Unit")] Material material)
        {
            if (id != material.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(material);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaterialExists(material.Id))
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
            return View(material);
        }

        // GET: Materials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var material = await _context.Materials.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (material == null)
            {
                return NotFound();
            }

            return View(material);
        }

        // POST: Materials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material != null)
            {
                _context.Materials.Remove(material);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MaterialExists(int id)
        {
            return _context.Materials.Any(e => e.Id == id);
        }
    }
}
