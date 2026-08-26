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
    public class DeviceDatasController : Controller
    {
        private const int PageSize = 20; // 跟 Materials/Permissions 一致，避免筆數變多後一次全部渲染拖慢畫面。

        private readonly ApplicationDbContext _context;
        private readonly MaterialCatalogService _materialCatalog;

        public DeviceDatasController(ApplicationDbContext context, MaterialCatalogService materialCatalog)
        {
            _context = context;
            _materialCatalog = materialCatalog;
        }

        // GET: DeviceDatas
        public async Task<IActionResult> Index(string? keyword, int pageNumber = 1)
        {
            // 原本沒有 AsNoTracking，也沒有搜尋/分頁——筆數變多之後，找一筆排放源設定只能整頁往下翻。
            var query = _context.deviceDatas.AsNoTracking().OrderBy(d => d.Id).AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(d => d.Name.Contains(keyword) || d.Material.Contains(keyword));
            }

            var paged = await ViewModel.PagedResult<DeviceData>.CreateAsync(query, pageNumber, PageSize);
            ViewData["PageNumber"] = paged.PageNumber;
            ViewData["TotalPages"] = paged.TotalPages;
            ViewData["Keyword"] = keyword;
            ViewData["TotalCount"] = paged.TotalCount;
            return View(paged.Items);
        }

        // GET: DeviceDatas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deviceData = await _context.deviceDatas
                .AsNoTracking()
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
            PopulateCorrectionSelectLists();
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
                TempData["StatusMessage"] = $"已新增排放源設定「{deviceData.Name}」。";
                return RedirectToAction(nameof(Index));
            }
            await PopulateMaterialSelectListsAsync(deviceData.Material, deviceData.Scope, deviceData.EmissionPattern);
            PopulateCorrectionSelectLists();
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
            PopulateCorrectionSelectLists();
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
                TempData["StatusMessage"] = $"已更新排放源設定「{deviceData.Name}」。";
                return RedirectToAction(nameof(Index));
            }
            await PopulateMaterialSelectListsAsync(deviceData.Material, deviceData.Scope, deviceData.EmissionPattern);
            PopulateCorrectionSelectLists();
            return View(deviceData);
        }

        // POST: DeviceDatas/Delete/5
        // 原本刪除要先進一個確認頁（GET Delete → 按確認 → POST），改成跟 Devices/Areas 一樣，
        // 直接在清單頁用 data-cip-confirm 二次確認後送出，不需要多一次頁面切換。
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deviceData = await _context.deviceDatas.FindAsync(id);
            if (deviceData != null)
            {
                _context.deviceDatas.Remove(deviceData);
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = $"已刪除排放源設定「{deviceData.Name}」。";
            }

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
        private async Task ValidateMaterialAsync(DeviceData deviceData)
        {
            var result = await _materialCatalog.ValidateAsync(deviceData.Material, deviceData.Scope, deviceData.EmissionPattern);
            foreach (var (field, message) in result.Errors)
            {
                ModelState.AddModelError(field, message);
            }
        }

        /// <summary>
        /// 原本活動數據誤差等級/儀器校正等級是裸的數字輸入框，畫面上只看得到 1、2、3，
        /// 使用者要另外去查文件才知道代表什麼；用詞跟 DevicesController.AddActivityData
        /// 的活動數據登記表單一致（同一套 1~3 等級定義），改用下拉選單。
        /// </summary>
        private void PopulateCorrectionSelectLists()
        {
            var dataCorrections = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "連續監測" },
                new SelectListItem { Value = "2", Text = "定期/間歇量測" },
                new SelectListItem { Value = "3", Text = "自行/財務推估" },
            };
            ViewData["DataCorrectionList"] = new SelectList(dataCorrections, "Value", "Text");

            var deviceCorrections = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "有外部校正或多組數據佐證者" },
                new SelectListItem { Value = "2", Text = "有內部校正或經過會計簽證等證明者" },
                new SelectListItem { Value = "3", Text = "未進行儀器校正或未進行紀錄彙整者" },
            };
            ViewData["DeviceCorrectionList"] = new SelectList(deviceCorrections, "Value", "Text");
        }
    }
}
