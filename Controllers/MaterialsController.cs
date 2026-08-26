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
    public class MaterialsController : Controller
    {
        private const int PageSize = 20; // A5：81 筆種子資料一次全部渲染會拖慢畫面，分頁顯示。

        private readonly ApplicationDbContext _context;
        private readonly MaterialCatalogService _materialCatalog;

        public MaterialsController(ApplicationDbContext context, MaterialCatalogService materialCatalog)
        {
            _context = context;
            _materialCatalog = materialCatalog;
        }

        // GET: Materials
        // 原本只能靠翻頁找排放係數，81 筆分 5 頁，想找特定物料/類別只能一頁一頁找。
        public async Task<IActionResult> Index(string? keyword, string? scope, string? emissionPattern, int pageNumber = 1)
        {
            var query = _context.Materials.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(m => m.Name.Contains(keyword));
            }
            if (!string.IsNullOrWhiteSpace(scope))
            {
                query = query.Where(m => m.Scope == scope);
            }
            if (!string.IsNullOrWhiteSpace(emissionPattern))
            {
                query = query.Where(m => m.EmissionPattern == emissionPattern);
            }
            query = query.OrderBy(m => m.Id);

            var paged = await ViewModel.PagedResult<Material>.CreateAsync(query, pageNumber, PageSize);
            ViewData["PageNumber"] = paged.PageNumber;
            ViewData["TotalPages"] = paged.TotalPages;
            ViewData["TotalCount"] = paged.TotalCount;
            ViewData["Keyword"] = keyword;
            ViewData["Scope"] = scope;
            ViewData["EmissionPattern"] = emissionPattern;
            ViewData["ScopeList"] = new SelectList(await _materialCatalog.GetScopesAsync(), scope);
            ViewData["EmissionPatternList"] = new SelectList(await _materialCatalog.GetEmissionPatternsAsync(), emissionPattern);
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
        public async Task<IActionResult> Create()
        {
            await PopulateScopeAndPatternListsAsync();
            // 原本這裡是 return View()（沒有帶 model），畫面上 21 個排放係數欄位會全部顯示空白，
            // 而不是 Material 類別本身宣告的預設值 0。空字串套用到 decimal 這種不可為 null
            // 的型別時，ASP.NET Core 的模型繫結會直接判定「這個值無效」而擋下整筆送出——
            // 只想填 CO2 的人，會被其餘 20 個「沒有動過」的欄位擋下來，且完全看不出是哪裡出錯
            // （這幾個欄位原本就沒有 asp-validation-for，錯誤訊息無處顯示）。改成帶一個全部
            // 欄位都是 0 的空白 Material，讓每個欄位一開始就有合法的預設值可以送出。
            return View(new Material());
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
            await ValidateNotDuplicateAsync(material, excludeId: 0);

            if (ModelState.IsValid)
            {
                _context.Add(material);
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = $"已新增排放係數「{material.Name}」。";
                return RedirectToAction(nameof(Index));
            }
            await PopulateScopeAndPatternListsAsync();
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
            await PopulateScopeAndPatternListsAsync();
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

            await ValidateNotDuplicateAsync(material, excludeId: id);

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
                TempData["StatusMessage"] = $"已更新排放係數「{material.Name}」。";
                return RedirectToAction(nameof(Index));
            }
            await PopulateScopeAndPatternListsAsync();
            return View(material);
        }

        /// <summary>
        /// Create/Edit 的「類別」「排放型式」用 &lt;datalist&gt; 而不是強制下拉：Materials 本身就是
        /// 這兩個欄位的權威來源（DefaultDevices／DeviceDatas 才是反過來要對照 Materials 的下拉選單），
        /// 所以這裡只提供既有值的建議清單，仍然允許輸入新的類別/型式。
        /// </summary>
        private async Task PopulateScopeAndPatternListsAsync()
        {
            ViewBag.ScopeList = new SelectList(await _materialCatalog.GetScopesAsync());
            ViewBag.EmissionPatternList = new SelectList(await _materialCatalog.GetEmissionPatternsAsync());
        }

        // POST: Materials/Delete/5
        // 原本刪除要先進一個確認頁（GET Delete → 按確認 → POST），改成跟 Devices/Areas 一樣，
        // 直接在清單頁用 data-cip-confirm 二次確認後送出，不需要多一次頁面切換。
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material != null)
            {
                _context.Materials.Remove(material);
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = $"已刪除排放係數「{material.Name}」。";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MaterialExists(int id)
        {
            return _context.Materials.Any(e => e.Id == id);
        }

        /// <summary>
        /// (Name, Scope, EmissionPattern, Year) 在資料庫上只有非唯一索引（純粹為了查詢效能），
        /// 沒有唯一性約束，所以原本可以毫無提示地新增出兩筆一模一樣的排放係數。
        /// CountController.GHGCheckAsync 之後用同一組條件查詢時，重複資料會讓
        /// .OrderByDescending(Year).FirstOrDefault() 取到哪一筆變成不確定——排放量算出來
        /// 對不對全看資料庫剛好回傳哪一筆，改為新增/編輯時就擋下重複組合。
        /// </summary>
        private async Task ValidateNotDuplicateAsync(Material material, int excludeId)
        {
            var duplicated = await _context.Materials.AnyAsync(m =>
                m.Id != excludeId
                && m.Name == material.Name
                && m.Scope == material.Scope
                && m.EmissionPattern == material.EmissionPattern
                && m.Year == material.Year);
            if (duplicated)
            {
                ModelState.AddModelError(string.Empty, "已經有相同「原燃物料／類別／排放型式／年份」組合的排放係數，請確認是否要編輯既有資料，而不是新增重複的一筆。");
            }
        }
    }
}
