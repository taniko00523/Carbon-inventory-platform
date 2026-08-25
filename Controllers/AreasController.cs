using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Authorization;
using Carbon_inventory_platform.Filters;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.Design;

namespace Carbon_inventory_platform.Controllers
{
    [CheckSubscriptionData]
    [Authorize]
    public class AreasController : CountController
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        public AreasController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment, UserManager<ApplicationUser> userManager) : base(context)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;

        }

        #region 權限與驗證共用方法
        // 原本每個 Action 都只用網址上的 Guid 找資料，完全沒有比對資料屬於哪一個登入者，
        // 會導致任何登入者只要拿到別家公司的 Guid，就能讀取、修改、複製、刪除別人的邊界資料。
        [NonAction]
        private async Task<Guid?> GetCallerCompanyIdAsync() //取得登入者自己的公司
        {
            string? userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            // isDeleted == 0 已由 ApplicationDbContext 的全域查詢過濾器處理，不需要重複寫。
            return await _context.Companies
                         .Where(x => x.UserId == userId)
                         .Select(x => (Guid?)x.Id)
                         .FirstOrDefaultAsync();
        }

        [NonAction]
        private async Task<bool> CanAccessCompanyAsync(Guid? companyId) //管理員可以看全部，其他人只能看自己的公司
        {
            if (companyId == null || companyId == Guid.Empty)
            {
                return false;
            }
            if (User.IsInRole("Admin"))
            {
                return true;
            }
            var ownCompanyId = await GetCallerCompanyIdAsync();
            return ownCompanyId != null && ownCompanyId == companyId;
        }

        [NonAction]
        private async Task<Area?> FindOwnedAreaAsync(Guid id, bool includeCompany = false) //取廠區並確認擁有權
        {
            if (id == Guid.Empty)
            {
                return null;
            }
            IQueryable<Area> query = _context.Areas;
            if (includeCompany)
            {
                query = query.Include(x => x.Company);
            }
            var area = await query.FirstOrDefaultAsync(x => x.Id == id);
            if (area == null)
            {
                return null;
            }
            return await CanAccessCompanyAsync(area.CompanyId) ? area : null;
        }

        // 原本 City / District / Address / all_Grade 是不可為 null 的字串，專案開了 <Nullable>enable</Nullable>，
        // MVC 會自動補上隱含的 [Required]；表單送出的是空字串（all_Grade 根本沒送），
        // 會導致 ModelState 永遠驗證失敗，而畫面上又看不到任何錯誤訊息（按了確認完全沒反應）。
        [NonAction]
        private void RemoveImplicitRequiredErrors(params string[] keys)
        {
            foreach (var key in keys)
            {
                ModelState.Remove(key);
            }
        }
        #endregion

        public async Task<IActionResult> Index(Guid? Id) //非同步方法
        {
            if (Id == null) //使用者不用給AreaId
            {
                var compnay = await _context.Companies // 暫存目前所在的公司名稱 顯示在畫面上方
                                   .Where(a => a.UserId == _userManager.GetUserId(User))
                                   .FirstOrDefaultAsync();

                // 原本直接取 compnay.Id，帳號還沒有公司資料時（例如管理員帳號、公司被刪除的帳號）
                // 會丟 NullReferenceException，會導致整個邊界總覽變成 HTTP 500 完全打不開。
                if (compnay == null)
                {
                    TempData.Remove("companyId");
                    TempData["companyName"] = "";
                    ViewData["NoCompanyMessage"] = "目前的帳號尚未建立公司資料，請先由管理員建立公司資料後再設定邊界。";
                    return View(new List<Area>());
                }

                Id = compnay.Id;
                TempData["companyId"] = Id; //暫存進入畫面所查詢的CompanyId
                TempData["companyName"] = compnay.Name;
            }
            else //管理員要給AreaId
            {
                // 原本任何登入者都可以用 ?Id=<別家公司Guid> 列出別人的廠區資料。
                if (!await CanAccessCompanyAsync(Id))
                {
                    return Forbid();
                }
                TempData["companyId"] = Id; //暫存進入畫面所查詢的CompanyId
                TempData["companyName"] = await _context.Companies // 暫存目前所在的公司名稱 顯示在畫面上方
                                                       .Where(a => a.Id == Id)
                                                       .Select(a => a.Name)
                                                       .FirstOrDefaultAsync();
            }

            var area = await _context.Areas
                          .Include(x => x.Company)
                          .Where(x => x.CompanyId == Id) // isDeleted 已由全域查詢過濾器處理
                          .OrderBy(x => x.CreateTime)
                          .Include(x => x.Analysis)
                          .ToListAsync();
            foreach (var item in area)
            {
                item.MapImagePath = !string.IsNullOrEmpty(item.MapImagePath) ? (string.Format("/{0}/{1}/{2}/{3}", "images", item.CompanyId.ToString(), item.Id.ToString(), item.MapImagePath)) : "";
                item.OrganizationImagePath = !string.IsNullOrEmpty(item.OrganizationImagePath) ? (string.Format("/{0}/{1}/{2}/{3}", "images", item.CompanyId.ToString(), item.Id.ToString(), item.OrganizationImagePath)) : "";
                item.ShopDrawingsPath = !string.IsNullOrEmpty(item.ShopDrawingsPath) ? (string.Format("/{0}/{1}/{2}/{3}", "images", item.CompanyId.ToString(), item.Id.ToString(), item.ShopDrawingsPath)) : "";
            }
            //if (area[0].Analysis == null)
            //{
            //    var Analysis_id = Guid.NewGuid();
            //    Guid Area_id = area[0].Id;
            //    await _context.Analyses.AddAsync(new Analysis()
            //    {
            //        Id = Analysis_id,
            //        AreaId = Area_id,
            //        CreateTime = DateTime.Now
            //    });
            //    await _context.SaveChangesAsync();
            //    area = await _context.Areas
            //              .Include(x => x.Company)
            //              .Where(x => x.isDeleted == 0 && x.CompanyId == Id) //抓出資料表裡面沒被刪除的
            //              .OrderBy(x => x.CreateTime)
            //              .Include(x => x.Analysis)
            //              .ToListAsync();
            //}
            return View(area);
        }
        // 原本這裡有 Image / Details 兩個 Action 及專屬的 GetRelativePath 輔助方法，
        // 但整個 Views/Areas 資料夾沒有對應的 .cshtml，站內也沒有任何連結會呼叫到它們（皆已用 grep 確認），
        // 屬於沒人使用又缺頁面、一叫就 500 的死程式碼，且原本也完全沒有做擁有權檢查，故直接移除。

        public IActionResult Create()
        {
            ViewBag.ARVersion = _context.GWPs.Select(x => x.ARVersion).Distinct().ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        // 原本 [Bind] 雖然有列出 UniqueCode/FactorCode，卻沒有 ARVersion，且下面組 toCreate 時完全沒有把
        // area.UniqueCode / area.FactorCode / area.ARVersion 複製過去，導致使用者填的值會被靜靜丟掉。
        public async Task<IActionResult> Create([Bind("Name,FullAddress,Year,BaseYear,Type,UniqueCode,FactorCode,ARVersion,OrganizationImage,MapImage,ShopDrawings")] Area area)
        {
            var companyId = TempData.Peek("companyId") as Guid?;
            // 原本沒有比對這個 companyId 是否屬於登入者，任何登入者都可以偽造 TempData 或直接呼叫此 Action 幫別家公司新增廠區。
            if (!await CanAccessCompanyAsync(companyId))
            {
                return Forbid();
            }
            // City / District / Address / all_Grade 是不可為 null 的字串欄位，但這個表單並沒有蒐集它們，
            // 在 <Nullable>enable</Nullable> 下會被 MVC 視為隱含必填，導致 ModelState 永遠失敗且畫面無任何提示。
            RemoveImplicitRequiredErrors(nameof(Area.City), nameof(Area.District), nameof(Area.Address), nameof(Area.all_Grade));
            if (ModelState.IsValid)
            {
                // 原本不論送出的這筆廠區是否要設為基準年，只要地址相同就會把同公司既有的基準年廠區關掉；
                // 改成只有在「這筆本身要被設為基準年」時，才去解除舊的基準年標記。
                if (area.BaseYear)
                {
                    var baseyearData = await _context.Areas.Where(x => x.CompanyId == companyId && x.FullAddress == area.FullAddress && x.BaseYear).FirstOrDefaultAsync();
                    if (baseyearData != null)
                    {
                        baseyearData.BaseYear = false;
                    }
                }

                // 原本用尚未賦值、永遠是 Guid.Empty 的 area.Id 當圖片資料夾名稱，
                // 跟實際新建立的廠區 Id 對不起來，導致圖片存了卻讀不到；改成先產生新 Id 再一路沿用。
                var newAreaId = Guid.NewGuid();
                var toCreate = new Area
                {
                    Id = newAreaId,
                    CompanyId = companyId.Value,
                    Name = area.Name,
                    FullAddress = area.FullAddress,
                    Year = area.Year,
                    BaseYear = area.BaseYear,
                    Type = area.Type,
                    UniqueCode = area.UniqueCode,
                    FactorCode = area.FactorCode,
                    ARVersion = area.ARVersion,
                    isDeleted = 0,
                    CreateTime = DateTime.Now
                };
                if (area.FullAddress.Length > 6)
                {
                    toCreate.City = GetCity(area.FullAddress);
                    toCreate.District = GetDistrict(area.FullAddress);
                    toCreate.Address = GetAddress(area.FullAddress);
                }
                else
                {
                    toCreate.Address = area.FullAddress;
                }
                if (area.OrganizationImage != null)
                {
                    toCreate.OrganizationImagePath = await SaveImage(area.OrganizationImage, companyId.ToString(), newAreaId.ToString());
                }
                if (area.MapImage != null)
                {
                    toCreate.MapImagePath = await SaveImage(area.MapImage, companyId.ToString(), newAreaId.ToString());
                }
                if (area.ShopDrawings != null)
                {
                    toCreate.ShopDrawingsPath = await SaveImage(area.ShopDrawings, companyId.ToString(), newAreaId.ToString());
                }
                _context.Add(toCreate);
                // Area 與 Analysis 是必要的 1:1 關聯，原本只有註冊時建立的第一個廠區才有對應的 Analysis，
                // 之後每新增一個廠區都沒有建立 Analysis，導致該廠區的「重大性評估」一定 404。
                _context.Analyses.Add(new Analysis
                {
                    Id = Guid.NewGuid(),
                    AreaId = newAreaId,
                    CreateTime = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { id = companyId });
            }
            ViewBag.ARVersion = _context.GWPs.Select(x => x.ARVersion).Distinct().ToList();
            return View(area);
        }
        public async Task<IActionResult> Edit(Guid? id)
        {
            ViewBag.ARVersion = _context.GWPs.Select(x => x.ARVersion).Distinct().ToList();
            if (id == null)
            {
                return NotFound();
            }
            // 原本沒有比對這個廠區屬於哪個公司，任何登入者只要猜到別人的廠區 Guid 就能開啟修改畫面。
            var area = await FindOwnedAreaAsync(id.Value, includeCompany: true);

            if (area == null)
            {
                return NotFound();
            }
            return View(area);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,FullAddress,Year,BaseYear,Type,UniqueCode,FactorCode,OrganizationImage,MapImage,ShopDrawings,ARVersion")] Area area)
        {
            if (id != area.Id)
            {
                return NotFound();
            }
            // 原本沒有比對這個廠區屬於哪個公司，任何登入者只要猜到別人的廠區 Guid 就能送出修改。
            if (await FindOwnedAreaAsync(id) == null)
            {
                return NotFound();
            }

            var companyId = TempData.Peek("companyId") as Guid?;
            // City / District / Address / all_Grade 是不可為 null 的字串欄位，但這個表單並沒有蒐集它們，
            // 在 <Nullable>enable</Nullable> 下會被 MVC 視為隱含必填，導致 ModelState 永遠失敗且畫面無任何提示。
            RemoveImplicitRequiredErrors(nameof(Area.City), nameof(Area.District), nameof(Area.Address), nameof(Area.all_Grade));
            if (!ModelState.IsValid)
            {
                ViewBag.ARVersion = _context.GWPs.Select(x => x.ARVersion).Distinct().ToList();
                return View(area);
            }

            try
            {
                // 原本不論送出的這筆廠區是否要設為基準年，只要地址相同就會把同公司既有的基準年廠區關掉；
                // 改成只有在「這筆本身要被設為基準年」時，才去解除舊的基準年標記。
                if (area.BaseYear)
                {
                    var baseyearData = await _context.Areas
                        .Where(x => x.CompanyId == companyId && x.FullAddress == area.FullAddress && x.BaseYear)
                        .FirstOrDefaultAsync();

                    if (baseyearData != null)
                    {
                        baseyearData.BaseYear = false;
                        await _context.SaveChangesAsync();
                    }
                }

                var toUpdate = await _context.Areas.FindAsync(id);
                if (toUpdate == null)
                {
                    return NotFound();
                }


                toUpdate.Name = area.Name;
                toUpdate.FullAddress = area.FullAddress;
                if (area.FullAddress.Length > 6)
                {
                    toUpdate.City = GetCity(area.FullAddress);
                    toUpdate.District = GetDistrict(area.FullAddress);
                    toUpdate.Address = GetAddress(area.FullAddress);
                }
                else
                {
                    toUpdate.Address = area.FullAddress;
                }
                toUpdate.UniqueCode = area.UniqueCode;
                toUpdate.FactorCode = area.FactorCode;

                // 原本上傳失敗（超過大小限制、格式不符）時 SaveImage 會回傳 null，直接蓋掉既有已存在的圖片路徑，
                // 造成使用者一次失敗的上傳就把原本好好的圖也弄丟；改成失敗時保留原本的路徑。
                if (area.OrganizationImage != null)
                {
                    toUpdate.OrganizationImagePath = await SaveImage(area.OrganizationImage, companyId.ToString(), area.Id.ToString()) ?? toUpdate.OrganizationImagePath;
                }
                if (area.MapImage != null)
                {
                    toUpdate.MapImagePath = await SaveImage(area.MapImage, companyId.ToString(), area.Id.ToString()) ?? toUpdate.MapImagePath;
                }
                if (area.ShopDrawings != null)
                {
                    toUpdate.ShopDrawingsPath = await SaveImage(area.ShopDrawings, companyId.ToString(), area.Id.ToString()) ?? toUpdate.ShopDrawingsPath;
                }

                toUpdate.Year = area.Year;
                toUpdate.BaseYear = area.BaseYear;
                toUpdate.Type = area.Type;
                toUpdate.ModifiedTime = DateTime.Now;
                #region 是否改變GWP
                var changeGWP = false;
                if (toUpdate.ARVersion != area.ARVersion)
                {
                    changeGWP = true;
                }
                #endregion

                toUpdate.ARVersion = area.ARVersion;
                await _context.SaveChangesAsync();
                if (changeGWP)
                {
                    var AreaDevices = await _context.Devices.Where(x => x.AreaId == id).ToListAsync();
                    foreach (var item in AreaDevices)
                    {
                        await ResetGHG(item.Id);
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AreaExists(area.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index), new { id = companyId });
        }
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // 原本沒有比對這個廠區屬於哪個公司，任何登入者只要猜到別人的廠區 Guid 就能開啟刪除確認畫面。
            var area = await FindOwnedAreaAsync(id.Value, includeCompany: true);
            if (area == null)
            {
                return NotFound();
            }

            return View(area);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            // 原本沒有比對這個廠區屬於哪個公司，任何登入者只要猜到別人的廠區 Guid 就能刪除。
            var toDelete = await FindOwnedAreaAsync(id);
            if (toDelete == null)
            {
                return NotFound();
            }
            // 原本依 ModifiedTime 是否為 null 決定「硬刪除」或「軟刪除」，沒改過的廠區會被硬刪除，
            // 而 Devices 對 Area 的外鍵是 Cascade，等於連帶把底下所有排放源資料一起刪光；
            // 統一改成軟刪除（isDeleted=1 + DeleteTime），不再有任何情況會真的刪掉資料列。
            toDelete.isDeleted = 1;
            toDelete.DeleteTime = DateTime.Now;
            await _context.SaveChangesAsync();
            var companyId = TempData.Peek("companyId") as Guid?;
            return RedirectToAction(nameof(Index), new { id = companyId });
        }
        private bool AreaExists(Guid id)
        {
            return (_context.Areas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public string GetCity(string input)
        {
            return input.Substring(0, 3);
        }
        public string GetDistrict(string input)
        {
            return input.Substring(3, 3);
        }
        public string GetAddress(string input)
        {
            return input.Substring(6);
        }
        // 原本資料夾第一次建立時回傳伺服器實體路徑（FilePath），之後才回傳純檔名（fileName），
        // 兩種回傳格式不一致，Index()/Image 讀圖時是用「/images/{companyId}/{areaId}/{儲存值}」去拼網址，
        // 存到實體路徑的那一次一定連不到圖，造成第一張上傳的圖片永遠顯示不出來。
        // 這裡統一只回傳純檔名，並加上大小限制（5MB）與副檔類型白名單（僅允許 jpeg/png），失敗時回傳 null。
        private async Task<string?> SaveImage(IFormFile file, string companyId, string areaId)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }
            const long MaxImageBytes = 5 * 1024 * 1024;
            var allowedContentTypes = new[] { "image/jpeg", "image/png" };
            if (file.Length > MaxImageBytes || !allowedContentTypes.Contains(file.ContentType))
            {
                return null;
            }

            string extension = string.Equals(file.ContentType, "image/png", StringComparison.OrdinalIgnoreCase) ? ".png" : ".jpg";
            string fileName = Guid.NewGuid().ToString() + extension;
            var FolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "images", companyId, areaId);
            var FilePath = Path.Combine(FolderPath, fileName);
            try
            {
                if (!Directory.Exists(FolderPath))
                {
                    Directory.CreateDirectory(FolderPath);
                }
                using (var stream = new FileStream(FilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                return fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine("發生錯誤：" + ex.Message);
                return null;
            }
        }
        // 原本這裡限定 [Authorize(Roles = "Admin")]，但 Views/Areas/Index.cshtml 的「設定」按鈕
        // 對一般使用者也會顯示，點下去只會拿到 AccessDenied——重大性評估依 ISO 14064-1 慣例
        // 是由填報單位自行判斷、留給查證單位覆核，因此改為開放給任何擁有該廠區的登入者，
        // 不再限定角色；資料範圍改由既有的 FindOwnedAreaAsync 擁有權檢查把關。
        public async Task<IActionResult> Analyses(Guid Id) //非同步方法
        {
            if (await FindOwnedAreaAsync(Id) == null)
            {
                return NotFound();
            }
            var analyses = await _context.Analyses
                          .Where(x => x.AreaId == Id) // isDeleted 已由全域查詢過濾器處理
                          .FirstOrDefaultAsync();
            if (analyses == null)
            {
                return NotFound();
            }

            return View(analyses);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        // 同 GET：不再限定 Admin 角色，開放給擁有該廠區的登入者填寫。
        // 資料保護仍然靠下面的擁有權檢查——原本完全沒有比對 Analysis 屬於哪個廠區/公司，
        // 任何登入者只要知道別人的 Analysis Guid 就能竄改該公司的重大性評估資料。
        public async Task<IActionResult> Analyses(Guid id, Analysis analysis)
        {
            if (id != analysis.Id)
            {
                return NotFound();
            }
            var existing = await _context.Analyses.Where(x => x.Id == id).Select(x => new { x.AreaId }).FirstOrDefaultAsync();
            if (existing == null || await FindOwnedAreaAsync(existing.AreaId) == null)
            {
                return NotFound();
            }
            // 24 個 Remark 欄位是不可為 null 的字串，但屬於選填備註，在 <Nullable>enable</Nullable> 下
            // 會被 MVC 視為隱含必填，只要使用者清空備註就會導致 ModelState 永遠失敗、儲存完全沒反應。
            RemoveImplicitRequiredErrors(
                nameof(Analysis._21Remark), nameof(Analysis._22Remark), nameof(Analysis._31Remark), nameof(Analysis._32Remark),
                nameof(Analysis._33Remark), nameof(Analysis._34Remark), nameof(Analysis._35Remark), nameof(Analysis._41Remark),
                nameof(Analysis._42Remark), nameof(Analysis._43Remark), nameof(Analysis._44Remark), nameof(Analysis._45Remark),
                nameof(Analysis._46Remark), nameof(Analysis._47Remark), nameof(Analysis._48Remark), nameof(Analysis._49Remark),
                nameof(Analysis._410Remark), nameof(Analysis._411Remark), nameof(Analysis._51Remark), nameof(Analysis._52Remark),
                nameof(Analysis._53Remark), nameof(Analysis._54Remark), nameof(Analysis._55Remark), nameof(Analysis._61Remark));
            if (ModelState.IsValid)
            {
                var companyId = TempData.Peek("companyId") as Guid?;
                try
                {
                    var toUpdate = await _context.Analyses.FindAsync(id);
                    if (toUpdate != null)
                    {
                        toUpdate._21A = analysis._21A;
                        toUpdate._22A = analysis._22A;
                        toUpdate._31A = analysis._31A;
                        toUpdate._32A = analysis._32A;
                        toUpdate._33A = analysis._33A;
                        toUpdate._34A = analysis._34A;
                        toUpdate._35A = analysis._35A;
                        toUpdate._41A = analysis._41A;
                        toUpdate._42A = analysis._42A;
                        toUpdate._43A = analysis._43A;
                        toUpdate._44A = analysis._44A;
                        toUpdate._45A = analysis._45A;
                        toUpdate._46A = analysis._46A;
                        toUpdate._47A = analysis._47A;
                        toUpdate._48A = analysis._48A;
                        toUpdate._49A = analysis._49A;
                        toUpdate._410A = analysis._410A;
                        toUpdate._411A = analysis._411A;
                        toUpdate._51A = analysis._51A;
                        toUpdate._52A = analysis._52A;
                        toUpdate._53A = analysis._53A;
                        toUpdate._54A = analysis._54A;
                        toUpdate._55A = analysis._55A;
                        toUpdate._61A = analysis._61A;

                        toUpdate._21B = analysis._21B;
                        toUpdate._22B = analysis._22B;
                        toUpdate._31B = analysis._31B;
                        toUpdate._32B = analysis._32B;
                        toUpdate._33B = analysis._33B;
                        toUpdate._34B = analysis._34B;
                        toUpdate._35B = analysis._35B;
                        toUpdate._41B = analysis._41B;
                        toUpdate._42B = analysis._42B;
                        toUpdate._43B = analysis._43B;
                        toUpdate._44B = analysis._44B;
                        toUpdate._45B = analysis._45B;
                        toUpdate._46B = analysis._46B;
                        toUpdate._47B = analysis._47B;
                        toUpdate._48B = analysis._48B;
                        toUpdate._49B = analysis._49B;
                        toUpdate._410B = analysis._410B;
                        toUpdate._411B = analysis._411B;
                        toUpdate._51B = analysis._51B;
                        toUpdate._52B = analysis._52B;
                        toUpdate._53B = analysis._53B;
                        toUpdate._54B = analysis._54B;
                        toUpdate._55B = analysis._55B;
                        toUpdate._61B = analysis._61B;

                        toUpdate._21C = analysis._21C;
                        toUpdate._22C = analysis._22C;
                        toUpdate._31C = analysis._31C;
                        toUpdate._32C = analysis._32C;
                        toUpdate._33C = analysis._33C;
                        toUpdate._34C = analysis._34C;
                        toUpdate._35C = analysis._35C;
                        toUpdate._41C = analysis._41C;
                        toUpdate._42C = analysis._42C;
                        toUpdate._43C = analysis._43C;
                        toUpdate._44C = analysis._44C;
                        toUpdate._45C = analysis._45C;
                        toUpdate._46C = analysis._46C;
                        toUpdate._47C = analysis._47C;
                        toUpdate._48C = analysis._48C;
                        toUpdate._49C = analysis._49C;
                        toUpdate._410C = analysis._410C;
                        toUpdate._411C = analysis._411C;
                        toUpdate._51C = analysis._51C;
                        toUpdate._52C = analysis._52C;
                        toUpdate._53C = analysis._53C;
                        toUpdate._54C = analysis._54C;
                        toUpdate._55C = analysis._55C;
                        toUpdate._61C = analysis._61C;

                        toUpdate._21D = analysis._21D;
                        toUpdate._22D = analysis._22D;
                        toUpdate._31D = analysis._31D;
                        toUpdate._32D = analysis._32D;
                        toUpdate._33D = analysis._33D;
                        toUpdate._34D = analysis._34D;
                        toUpdate._35D = analysis._35D;
                        toUpdate._41D = analysis._41D;
                        toUpdate._42D = analysis._42D;
                        toUpdate._43D = analysis._43D;
                        toUpdate._44D = analysis._44D;
                        toUpdate._45D = analysis._45D;
                        toUpdate._46D = analysis._46D;
                        toUpdate._47D = analysis._47D;
                        toUpdate._48D = analysis._48D;
                        toUpdate._49D = analysis._49D;
                        toUpdate._410D = analysis._410D;
                        toUpdate._411D = analysis._411D;
                        toUpdate._51D = analysis._51D;
                        toUpdate._52D = analysis._52D;
                        toUpdate._53D = analysis._53D;
                        toUpdate._54D = analysis._54D;
                        toUpdate._55D = analysis._55D;
                        toUpdate._61D = analysis._61D;

                        toUpdate._21 = analysis._21;
                        toUpdate._22 = analysis._22;
                        toUpdate._31 = analysis._31;
                        toUpdate._32 = analysis._32;
                        toUpdate._33 = analysis._33;
                        toUpdate._34 = analysis._34;
                        toUpdate._35 = analysis._35;
                        toUpdate._41 = analysis._41;
                        toUpdate._42 = analysis._42;
                        toUpdate._43 = analysis._43;
                        toUpdate._44 = analysis._44;
                        toUpdate._45 = analysis._45;
                        toUpdate._46 = analysis._46;
                        toUpdate._47 = analysis._47;
                        toUpdate._48 = analysis._48;
                        toUpdate._49 = analysis._49;
                        toUpdate._410 = analysis._410;
                        toUpdate._411 = analysis._411;
                        toUpdate._51 = analysis._51;
                        toUpdate._52 = analysis._52;
                        toUpdate._53 = analysis._53;
                        toUpdate._54 = analysis._54;
                        toUpdate._55 = analysis._55;
                        toUpdate._61 = analysis._61;

                        toUpdate._21isCal = analysis._21isCal;
                        toUpdate._22isCal = analysis._22isCal;
                        toUpdate._31isCal = analysis._31isCal;
                        toUpdate._32isCal = analysis._32isCal;
                        toUpdate._33isCal = analysis._33isCal;
                        toUpdate._34isCal = analysis._34isCal;
                        toUpdate._35isCal = analysis._35isCal;
                        toUpdate._41isCal = analysis._41isCal;
                        toUpdate._42isCal = analysis._42isCal;
                        toUpdate._43isCal = analysis._43isCal;
                        toUpdate._44isCal = analysis._44isCal;
                        toUpdate._45isCal = analysis._45isCal;
                        toUpdate._46isCal = analysis._46isCal;
                        toUpdate._47isCal = analysis._47isCal;
                        toUpdate._48isCal = analysis._48isCal;
                        toUpdate._49isCal = analysis._49isCal;
                        toUpdate._410isCal = analysis._410isCal;
                        toUpdate._411isCal = analysis._411isCal;
                        toUpdate._51isCal = analysis._51isCal;
                        toUpdate._52isCal = analysis._52isCal;
                        toUpdate._53isCal = analysis._53isCal;
                        toUpdate._54isCal = analysis._54isCal;
                        toUpdate._55isCal = analysis._55isCal;
                        toUpdate._61isCal = analysis._61isCal;

                        toUpdate._21Remark = analysis._21Remark;
                        toUpdate._22Remark = analysis._22Remark;
                        toUpdate._31Remark = analysis._31Remark;
                        toUpdate._32Remark = analysis._32Remark;
                        toUpdate._33Remark = analysis._33Remark;
                        toUpdate._34Remark = analysis._34Remark;
                        toUpdate._35Remark = analysis._35Remark;
                        toUpdate._41Remark = analysis._41Remark;
                        toUpdate._42Remark = analysis._42Remark;
                        toUpdate._43Remark = analysis._43Remark;
                        toUpdate._44Remark = analysis._44Remark;
                        toUpdate._45Remark = analysis._45Remark;
                        toUpdate._46Remark = analysis._46Remark;
                        toUpdate._47Remark = analysis._47Remark;
                        toUpdate._48Remark = analysis._48Remark;
                        toUpdate._49Remark = analysis._49Remark;
                        toUpdate._410Remark = analysis._410Remark;
                        toUpdate._411Remark = analysis._411Remark;
                        toUpdate._51Remark = analysis._51Remark;
                        toUpdate._52Remark = analysis._52Remark;
                        toUpdate._53Remark = analysis._53Remark;
                        toUpdate._54Remark = analysis._54Remark;
                        toUpdate._55Remark = analysis._55Remark;
                        toUpdate._61Remark = analysis._61Remark;

                        toUpdate.ModifiedTime = DateTime.Now;
                    }
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AreaExists(analysis.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { id = companyId });

            }
            return View(analysis);
        }
        #region 重置排放源
        public async Task ResetGHG(Guid Id)
        {
            Device? device = await _context.Devices.FindAsync(Id);
            List<GHG> ghgs = await _context.GHGs.Where(x => x.DeviceId == Id).ToListAsync();
            decimal CO2CEF = ghgs.Where(x => x.Name == "CO2").Select(x => x.CEF).FirstOrDefault();
            decimal CH4CEF = ghgs.Where(x => x.Name == "CH4").Select(x => x.CEF).FirstOrDefault();
            decimal N2OCEF = ghgs.Where(x => x.Name == "N2O").Select(x => x.CEF).FirstOrDefault();
            decimal HFCSCEF = ghgs.Where(x => x.Name == "HFCS").Select(x => x.CEF).FirstOrDefault();
            decimal PFCSCEF = ghgs.Where(x => x.Name == "PFCS").Select(x => x.CEF).FirstOrDefault();
            decimal SF6CEF = ghgs.Where(x => x.Name == "SF6").Select(x => x.CEF).FirstOrDefault();
            decimal NF3CEF = ghgs.Where(x => x.Name == "NF3").Select(x => x.CEF).FirstOrDefault();
            await SetGHG(device, CO2CEF, CH4CEF, N2OCEF, HFCSCEF, PFCSCEF, NF3CEF, SF6CEF);
        }
        public async Task SetGHG(Device device, decimal CO2CEF = 0, decimal CH4CEF = 0, decimal N2OCEF = 0, decimal HFCSCEF = 0, decimal PFCSCEF = 0, decimal NF3CEF = 0, decimal SF6CEF = 0)
        {
            bool Default = true;
            if (device.Id != Guid.Empty)
            {
                var GHGs = await _context.GHGs.Where(x => x.DeviceId == device.Id).ToListAsync(); //抓取所有溫室氣體設定
                if (GHGs != null)
                {
                    foreach (var item in GHGs)
                    {
                        _context.GHGs.Remove(item);
                    }
                }
            }
            if (device.Customize == true)
            {
                if (CO2CEF != 0)
                {
                    await CEFAddAsync(device, "CO2", CO2CEF);
                    Default = false;
                }
                if (CH4CEF != 0)
                {
                    await CEFAddAsync(device, "CH4", CH4CEF);
                    Default = false;
                }
                if (N2OCEF != 0)
                {
                    await CEFAddAsync(device, "N2O", N2OCEF);
                    Default = false;

                }
                if (HFCSCEF != 0)
                {
                    await CEFAddAsync(device, "HFCS", HFCSCEF);
                    Default = false;

                }
                if (PFCSCEF != 0)
                {
                    await CEFAddAsync(device, "PFCS", PFCSCEF);
                    Default = false;

                }
                if (NF3CEF != 0)
                {
                    await CEFAddAsync(device, "NF3", NF3CEF);
                    Default = false;

                }
                if (SF6CEF != 0)
                {
                    await CEFAddAsync(device, "SF6", SF6CEF);
                    Default = false;

                }
            }
            else
            {
                var area = _context.Areas.FirstOrDefault(x => x.Id == device.AreaId);
                await GHGCheckAsync(device, device.Name, device.Material, device.Scope, device.EmissionPattern, area.Year, area.ARVersion);
            }

            if (device.Id != Guid.Empty)
            {
                if (_context.ActivityDatas.Where(x => x.DeviceId == device.Id) != null) //計算排放量
                {
                    await CountEmissionData(device.Id);
                }
            }
        }

        #endregion
        // 原本是 GET，卻會寫入資料庫（違反 GET 不應有副作用的原則，也讓瀏覽器預先讀取/爬蟲都可能誤觸發複製），
        // 改成 POST + 驗證登入者是否擁有這個廠區，並在 Index 頁面改用表單送出。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CopyArea(Guid id)
        {
            var area = await FindOwnedAreaAsync(id);
            if (area == null)
            {
                return NotFound();
            }
            Guid newAreaId = Guid.NewGuid();
            var newArea = new Area
            {
                Id = newAreaId,
                Year = area.Year,
                CompanyId = area.CompanyId,
                ARVersion = area.ARVersion,
                Name = area.Name,
                // 原本這幾個 NOT NULL 欄位整段被註解掉沒有複製，加上下面誤用 [NotMapped] 的 IFormFile 屬性
                // （複製既有廠區時一定是 null）取代實際存放路徑的字串屬性，導致複製出來的廠區地址是空的、圖片也不見。
                PostalCode = area.PostalCode,
                UniqueCode = area.UniqueCode,
                FactorCode = area.FactorCode,
                City = area.City,
                District = area.District,
                Address = area.Address,
                FullAddress = area.FullAddress,
                BaseYear = area.BaseYear,
                Type = area.Type,
                OrganizationImagePath = area.OrganizationImagePath,
                MapImagePath = area.MapImagePath,
                ShopDrawingsPath = area.ShopDrawingsPath,
                Scope1_CO2 = area.Scope1_CO2,
                Scope1_CH4 = area.Scope1_CH4,
                Scope1_N2O = area.Scope1_N2O,
                Scope1_HFCS = area.Scope1_HFCS,
                Scope1_PFCS = area.Scope1_PFCS,
                Scope1_NF3 = area.Scope1_NF3,
                Scope1_SF6 = area.Scope1_SF6,

                Scope2_CO2 = area.Scope2_CO2,
                Scope2_CH4 = area.Scope2_CH4,
                Scope2_N2O = area.Scope2_N2O,
                Scope2_HFCS = area.Scope2_HFCS,
                Scope2_PFCS = area.Scope2_PFCS,
                Scope2_NF3 = area.Scope2_NF3,
                Scope2_SF6 = area.Scope2_SF6,

                CO2 = area.CO2,
                CH4 = area.CH4,
                N2O = area.N2O,
                HFCS = area.HFCS,
                PFCS = area.PFCS,
                NF3 = area.NF3,
                SF6 = area.SF6,

                percentage1_CO2 = area.percentage1_CO2,
                percentage1_CH4 = area.percentage1_CH4,
                percentage1_N2O = area.percentage1_N2O,
                percentage1_HFCS = area.percentage1_HFCS,
                percentage1_PFCS = area.percentage1_PFCS,
                percentage1_NF3 = area.percentage1_NF3,
                percentage1_SF6 = area.percentage1_SF6,

                percentage2_CO2 = area.percentage2_CO2,
                percentage2_CH4 = area.percentage2_CH4,
                percentage2_N2O = area.percentage2_N2O,
                percentage2_HFCS = area.percentage2_HFCS,
                percentage2_PFCS = area.percentage2_PFCS,
                percentage2_NF3 = area.percentage2_NF3,
                percentage2_SF6 = area.percentage2_SF6,

                non_move = area.non_move,
                move = area.move,
                process = area.process,
                escape = area.escape,
                percentage_nonMove = area.percentage_nonMove,
                percentage_Move = area.percentage_Move,
                percentage_Process = area.percentage_Process,
                percentage_Escape = area.percentage_Escape,

                percentage_Scope1 = area.percentage_Scope1,
                percentage_Scope2 = area.percentage_Scope2,
                Scope1 = area.Scope1,
                Scope2 = area.Scope2,
                All = area.All,

                cal_all = area.cal_all,
                percentage_CalAll = area.percentage_CalAll,

                no1_Grade = area.no1_Grade,
                no2_Grade = area.no2_Grade,
                no3_Grade = area.no3_Grade,
                avg_Grade = area.avg_Grade,
                all_Grade = area.all_Grade,
                ULL = area.ULL,
                UUL = area.UUL,

                isDeleted = 0,
                CreateTime = DateTime.Now
            };
            _context.Areas.Add(newArea);
            // Area 與 Analysis 是必要的 1:1 關聯，複製廠區時同樣要建立一筆新的 Analysis，否則複製出來的
            // 廠區一樣會在「重大性評估」頁面 404。
            _context.Analyses.Add(new Analysis
            {
                Id = Guid.NewGuid(),
                AreaId = newAreaId,
                CreateTime = DateTime.Now
            });

            // isDeleted 已由全域查詢過濾器處理，這裡不需要重複寫。
            var devices = await _context.Devices.Where(x => x.AreaId == area.Id).ToListAsync();
            if (devices != null)
            {
                foreach (var device in devices)
                {
                    Guid newDeviceId = Guid.NewGuid();
                    Device newDevice = CopyDevice(device, newAreaId, newDeviceId);
                    _context.Devices.Add(newDevice);
                    var GHGs = await _context.GHGs.Where(x => x.DeviceId == device.Id).ToListAsync();
                    if (GHGs != null)
                    {
                        foreach (var ghg in GHGs)
                        {
                            var newGHG = CopyGHG(ghg, newDeviceId);
                            _context.GHGs.Add(newGHG);
                        }
                    }
                    var activityDatas = await _context.ActivityDatas.Where(x => x.DeviceId == device.Id).ToListAsync();
                    if (activityDatas != null)
                    {
                        foreach (var activityData in activityDatas)
                        {
                            var newActivityData = CopyActivityData(activityData, newDeviceId);
                            _context.ActivityDatas.Add(newActivityData);
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }
            var companyId = area.CompanyId;
            return RedirectToAction(nameof(Index), new { id = companyId });
        }

    }

}
