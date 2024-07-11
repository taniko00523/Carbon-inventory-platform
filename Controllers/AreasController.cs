using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Authorization;
using Carbon_inventory_platform.Filters;
using Microsoft.AspNetCore.Identity;

namespace Carbon_inventory_platform.Controllers
{
    [CheckSubscriptionData]
    [Authorize]
    public class AreasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        public AreasController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;

        }
        public async Task<IActionResult> Index() //非同步方法
        {
            string userId = _userManager.GetUserId(User);

            var compnay = await _context.Companies // 暫存目前所在的公司名稱 顯示在畫面上方
                               .Where(a => a.UserId == userId)
                               .FirstOrDefaultAsync();

            Guid Id = compnay.Id;
            TempData["companyId"] = Id; //暫存進入畫面所查詢的CompanyId
            TempData["companyName"] = compnay.Name;
            var area = await _context.Areas
                       .Include(x => x.Company)
                       .Include(x => x.Analysis)
                       .Where(x => x.isDeleted == 0 && x.CompanyId == Id) //抓出資料表裡面沒被刪除的
                       .OrderBy(x => x.CreateTime)
                       .ToListAsync();
            foreach (var item in area)
            {
                item.MapImagePath = !string.IsNullOrEmpty(item.MapImagePath) ? (string.Format("/{0}/{1}/{2}/{3}", "images", item.CompanyId.ToString(), item.Id.ToString(), item.MapImagePath)) : "";
                item.OrganizationImagePath = !string.IsNullOrEmpty(item.OrganizationImagePath) ? (string.Format("/{0}/{1}/{2}/{3}", "images", item.CompanyId.ToString(), item.Id.ToString(), item.OrganizationImagePath)) : "";
                item.ShopDrawingsPath = !string.IsNullOrEmpty(item.ShopDrawingsPath) ? (string.Format("/{0}/{1}/{2}/{3}", "images", item.CompanyId.ToString(), item.Id.ToString(), item.ShopDrawingsPath)) : "";
            }
            if (area[0].Analysis == null)
            {
                var Analysis_id = Guid.NewGuid();
                Guid Area_id = area[0].Id;
                await _context.Analyses.AddAsync(new Analysis()
                {
                    Id = Analysis_id,
                    AreaId = Area_id,
                    CreateTime = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }
            return View(area);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminIndex(Guid Id) //非同步方法
        {
            TempData["companyId"] = Id; //暫存進入畫面所查詢的CompanyId
            TempData["companyName"] = await _context.Companies // 暫存目前所在的公司名稱 顯示在畫面上方
           .Where(a => a.Id == Id)
           .Select(a => a.Name)
           .FirstOrDefaultAsync();
            var area = await _context.Areas
                          .Include(x => x.Company)
                          .Where(x => x.isDeleted == 0 && x.CompanyId == Id) //抓出資料表裡面沒被刪除的
                          .OrderBy(x => x.CreateTime)
                          .Include(x => x.Analysis)
                          .ToListAsync();
            foreach (var item in area)
            {
                item.MapImagePath = !string.IsNullOrEmpty(item.MapImagePath) ? (string.Format("/{0}/{1}/{2}/{3}", "images", item.CompanyId.ToString(), item.Id.ToString(), item.MapImagePath)) : "";
                item.OrganizationImagePath = !string.IsNullOrEmpty(item.OrganizationImagePath) ? (string.Format("/{0}/{1}/{2}/{3}", "images", item.CompanyId.ToString(), item.Id.ToString(), item.OrganizationImagePath)) : "";
                item.ShopDrawingsPath = !string.IsNullOrEmpty(item.ShopDrawingsPath) ? (string.Format("/{0}/{1}/{2}/{3}", "images", item.CompanyId.ToString(), item.Id.ToString(), item.ShopDrawingsPath)) : "";
            }
            if (area[0].Analysis == null)
            {
                var Analysis_id = Guid.NewGuid();
                Guid Area_id = area[0].Id;
                await _context.Analyses.AddAsync(new Analysis()
                {
                    Id = Analysis_id,
                    AreaId = Area_id,
                    CreateTime = DateTime.Now
                });
                await _context.SaveChangesAsync();
                area = await _context.Areas
                          .Include(x => x.Company)
                          .Where(x => x.isDeleted == 0 && x.CompanyId == Id) //抓出資料表裡面沒被刪除的
                          .OrderBy(x => x.CreateTime)
                          .Include(x => x.Analysis)
                          .ToListAsync();
            }
            return View(area);
        }
        [HttpPost]
        public async Task<IActionResult> Image(Guid Id, string item) //非同步方法
        {
            var area = await _context.Areas
                         .Where(x => x.isDeleted == 0 && x.Id == Id) //抓出資料表裡面沒被刪除的
                         .FirstOrDefaultAsync();
            if (area != null)
            {
                string BaseURL = string.Format("{0}://{1}{2}", Request.Scheme, Request.Host, Request.PathBase);//http://localhost:5000
                if (item == "Map")
                {
                    ViewBag.Image = !string.IsNullOrEmpty(area.MapImagePath) ? (string.Format("/{0}/{1}/{2}/{3}", "images", area.CompanyId.ToString(), area.Id.ToString(), area.MapImagePath )): "";
                }
                else if (item == "Organization")
                {
                    ViewBag.Image = !string.IsNullOrEmpty(area.OrganizationImagePath) ? (string.Format("/{0}/{1}/{2}/{3}", "images", area.CompanyId.ToString(), area.Id.ToString(), area.OrganizationImagePath)) : "";
                }
                else if (item == "ShopDrawings")
                {
                    ViewBag.Image = !string.IsNullOrEmpty(area.ShopDrawingsPath) ? (string.Format("/{0}/{1}/{2}/{3}" , "images", area.CompanyId.ToString(), area.Id.ToString(), area.ShopDrawingsPath)) : "";
                }
            }

            return View();
        }
        private string GetRelativePath(string absolutePath) //抓取圖片資料夾的相對位置
        {
            string basePath = _hostingEnvironment.WebRootPath + "\\images";
            Uri baseUri = new(basePath);
            Uri absoluteUri = new(absolutePath);
            Uri relativeUri = baseUri.MakeRelativeUri(absoluteUri);
            string relativePath = relativeUri.ToString();

            relativePath = "/" + relativePath;
            return relativePath;
        }
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Areas == null)
            {
                return NotFound();
            }

            var area = await _context.Areas
                .Include(a => a.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (area == null)
            {
                return NotFound();
            }

            return View(area);
        }
        public IActionResult Create()
        {
            ViewBag.ARVersion = _context.GWPs.Select(x => x.ARCount).Distinct().ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,FullAddress,Year,BaseYear,Type,UniqueCode,FactorCode,OrganizationImage,MapImage,ShopDrawings")] Area area)
        {
            if (ModelState.IsValid)
            {
                var companyId = TempData.Peek("companyId") as Guid?;
                var baseyearData = await _context.Areas.Where(x => x.CompanyId == companyId && x.FullAddress == area.FullAddress && x.BaseYear).FirstOrDefaultAsync();
                if (baseyearData != null)
                {
                    baseyearData.BaseYear = false;
                    await _context.SaveChangesAsync();
                }


                var toCreate = new Area();
                {
                    toCreate.CompanyId = companyId.Value;
                    toCreate.Id = Guid.NewGuid();
                    toCreate.Name = area.Name;
                    toCreate.FullAddress = area.FullAddress;
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
                    toCreate.Year = area.Year;
                    toCreate.BaseYear = area.BaseYear;
                    toCreate.Type = area.Type;
                    if (area.OrganizationImage != null)
                    {
                        string OrganizationImagePath = await SaveImage(area.OrganizationImage, companyId.ToString(), area.Id.ToString());
                        toCreate.OrganizationImagePath = OrganizationImagePath;
                    }
                    if (area.MapImage != null)
                    {
                        string MapImagePath = await SaveImage(area.MapImage, companyId.ToString(), area.Id.ToString());
                        toCreate.MapImagePath = MapImagePath;
                    }
                    if (area.ShopDrawings != null)
                    {
                        string ShopDrawingsPath = await SaveImage(area.ShopDrawings, companyId.ToString(), area.Id.ToString());
                        toCreate.ShopDrawingsPath = ShopDrawingsPath;
                    }
                    toCreate.isDeleted = 0;
                    toCreate.CreateTime = DateTime.Now;
                }
                _context.Add(toCreate);
                await _context.SaveChangesAsync();
                if (User.IsInRole("User"))
                {
                    return RedirectToAction(nameof(Index), new { id = companyId });
                }
                else if (User.IsInRole("Admin"))
                {
                    return RedirectToAction(nameof(AdminIndex), new { id = companyId });
                }

            }
            ViewBag.ARVersion = _context.GWPs.Select(x => x.ARCount).Distinct().ToList();
            return View(area);
        }
        public async Task<IActionResult> Edit(Guid? id)
        {
            ViewBag.ARVersion = _context.GWPs.Select(x => x.ARCount).Distinct().ToList();
            var area = await _context.Areas
                .Include(x => x.Company)
                .FirstOrDefaultAsync(m => m.Id == id);

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

            var companyId = TempData.Peek("companyId") as Guid?;
            if (!ModelState.IsValid)
            {
                ViewBag.ARVersion = _context.GWPs.Select(x => x.ARCount).Distinct().ToList();
                return View(area);
            }

            try
            {
                var baseyearData = await _context.Areas
                    .Where(x => x.CompanyId == companyId && x.FullAddress == area.FullAddress && x.BaseYear)
                    .FirstOrDefaultAsync();

                if (baseyearData != null)
                {
                    baseyearData.BaseYear = false;
                    await _context.SaveChangesAsync();
                }

                var toUpdate = await _context.Areas.FindAsync(id);
                if (toUpdate == null)
                {
                    return NotFound();
                }

                toUpdate.ARVersion = area.ARVersion;
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

                if (area.OrganizationImage != null)
                {
                    toUpdate.OrganizationImagePath = await SaveImage(area.OrganizationImage, companyId.ToString(), area.Id.ToString());
                }
                if (area.MapImage != null)
                {
                    toUpdate.MapImagePath = await SaveImage(area.MapImage, companyId.ToString(), area.Id.ToString());
                }
                if (area.ShopDrawings != null)
                {
                    toUpdate.ShopDrawingsPath = await SaveImage(area.ShopDrawings, companyId.ToString(), area.Id.ToString());
                }

                toUpdate.Year = area.Year;
                toUpdate.BaseYear = area.BaseYear;
                toUpdate.Type = area.Type;
                toUpdate.ModifiedTime = DateTime.Now;

                var AreaDevices = await _context.Devices.Where(x => x.AreaId == id).ToListAsync();
                foreach (var item in AreaDevices)
                {
                    await ResetGHG(item.Id);
                }

                await _context.SaveChangesAsync();
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

            if (User.IsInRole("User"))
            {
                return RedirectToAction(nameof(Index), new { id = companyId });
            }
            else if (User.IsInRole("Admin"))
            {
                return RedirectToAction(nameof(AdminIndex), new { id = companyId });
            }

            ViewBag.ARVersion = _context.GWPs.Select(x => x.ARCount).Distinct().ToList();
            return View(area);
        }
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Areas == null)
            {
                return NotFound();
            }

            var area = await _context.Areas
                .Include(a => a.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            if (_context.Areas == null)
            {
                return Problem("沒有找到資料");
            }
            var toDelete = await _context.Areas.FindAsync(id);
            if (toDelete != null)
            {
                if (toDelete.ModifiedTime == null)
                {
                    _context.Areas.Remove(toDelete);
                }
                else
                {
                    toDelete.isDeleted = 1;
                    toDelete.DeleteTime = DateTime.Now;
                }
                await _context.SaveChangesAsync();
            }
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
        private async Task<string> SaveImage(IFormFile file, string companyId, string areaId)
        {
            string fileName = Guid.NewGuid().ToString() + ".jpg";
            if (file != null && file.Length > 0)
            {
                //var fileName = Path.GetFileName(file.FileName);
                var FolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", companyId, areaId);
                var FilePath = Path.Combine(FolderPath, fileName);
                try
                {
                    // 檢查資料夾是否存在，如果不存在則創建資料夾，並新增圖片回傳檔案位置
                    if (!Directory.Exists(FolderPath))
                    {
                        Directory.CreateDirectory(FolderPath);
                        using (var stream = new FileStream(FilePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        return FilePath;
                    }
                    else
                    {
                        using (var stream = new FileStream(FilePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        return fileName;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("發生錯誤：" + ex.Message);
                }
            }
            return null;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Analyses(Guid Id) //非同步方法
        {

            var analyses = await _context.Analyses
                          .Where(x => x.isDeleted == 0 && x.AreaId == Id) //抓出資料表裡面沒被刪除的
                          .FirstOrDefaultAsync();
            if (analyses == null)
            {
                return NotFound();
            }

            return View(analyses);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Analyses(Guid id, Analysis analysis)
        {
            if (id != analysis.Id)
            {
                return NotFound();
            }
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
                if (User.IsInRole("User"))
                {
                    return RedirectToAction(nameof(Index), new { id = companyId });
                }
                else if (User.IsInRole("Admin"))
                {
                    return RedirectToAction(nameof(AdminIndex), new { id = companyId });
                }
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

            if (Default) //預設排放係數
            {
                await GHGCheckAsync(device.Id, device.Name, device.Material, device.Scope, device.EmissionPattern, _context.Areas.FirstOrDefault(x => x.Id == device.AreaId).Year);
            }

            if (device.Id != Guid.Empty)
            {
                if (_context.ActivityDatas.Where(x => x.DeviceId == device.Id) != null) //計算排放量
                {
                    await CountEmissionData(device.Id);
                }
            }
        }
        public async Task CEFAddAsync(Device device, string GHG, decimal? CEF)
        {
            int ARVersion = await _context.Areas.Where(x => x.Id == device.AreaId).Select(x => x.ARVersion).FirstOrDefaultAsync();
            var GWP = await _context.GWPs.Where(x => x.ARCount == ARVersion).ToListAsync();

            if (ModelState.IsValid)
            {

                var toCreate = new GHG();
                toCreate.Id = Guid.NewGuid();
                toCreate.Name = GHG;
                toCreate.DeviceId = device.Id;
                toCreate.CEF = (decimal)CEF;
                if (GHG == "HFCS")
                {
                    toCreate.GWP = GWP.Where(x => x.Name == device.Material).Select(x => x.Num).FirstOrDefault();
                }
                else
                {
                    toCreate.GWP = GWP.Where(x => x.Name == GHG).Select(x => x.Num).FirstOrDefault();
                }
                toCreate.CreateTime = DateTime.Now;
                device.CEF_Correction = 1;//輸入?
                _context.Add(toCreate);
                await _context.SaveChangesAsync();

            }
        }
        public async Task<GHG?> GHGCheckAsync(Guid id, string name, string material, string scope, string emisspatern, int year) //排放源Id, 排放源名稱, 物料名稱, 類別, 排放型式
        {
            var Device = await _context.Devices.Include(x => x.Area).Where(x => x.Id == id).FirstOrDefaultAsync();
            var GWP = await _context.GWPs.Where(x => x.ARCount == Device.Area.ARVersion).ToListAsync();
            var MaterialList = await _context.Materials
            .Where(x => x.Name == material && x.Scope == scope && x.EmissionPattern == emisspatern && x.Year <= year).ToListAsync();
            var Material = MaterialList.OrderByDescending(x => x.Year).FirstOrDefault();

            var otherMaterial = await _context.Materials.Where(x => x.Name == name && x.Scope == scope && x.EmissionPattern == emisspatern).FirstOrDefaultAsync(); //目前只有冷媒設備，但我包含了PFCS以防萬一
            if (ModelState.IsValid)
            {
                if (Material != null)
                {
                    if (Material.CO2CEF != 0)
                    {
                        var toCreateCO2 = new GHG();
                        toCreateCO2.Id = Guid.NewGuid();
                        toCreateCO2.Name = "CO2";
                        toCreateCO2.DeviceId = id;
                        toCreateCO2.CEF = Material.CO2CEF;
                        toCreateCO2.CEF_UUL = Material.CO2UUL;
                        toCreateCO2.CEF_ULL = Material.CO2ULL;
                        toCreateCO2.GWP = GWP.Where(x => x.Name == "CO2").Select(x => x.Num).FirstOrDefault();
                        toCreateCO2.all_UUL = CalculateRoundDistance(Material.CO2UUL, Material.DataUUL);
                        toCreateCO2.all_ULL = CalculateRoundDistance(Material.CO2ULL, Material.DataULL);
                        toCreateCO2.CreateTime = DateTime.Now;
                        _context.AddRange(toCreateCO2);
                        Device.CEF_Correction = Material.CEF_Correction;
                        Device.data_UUL = Material.DataUUL;
                        Device.data_ULL = Material.DataULL;

                    }
                    if (Material.CH4CEF != 0)
                    {
                        var toCreateCH4 = new GHG();
                        toCreateCH4.Id = Guid.NewGuid();
                        toCreateCH4.Name = "CH4";
                        toCreateCH4.DeviceId = id;
                        toCreateCH4.CEF = Material.CH4CEF;
                        toCreateCH4.CEF_UUL = Material.CH4UUL;
                        toCreateCH4.CEF_ULL = Material.CH4ULL;
                        toCreateCH4.GWP = GWP.Where(x => x.Name == "CH4").Select(x => x.Num).FirstOrDefault();
                        toCreateCH4.all_UUL = CalculateRoundDistance(Material.CH4UUL, Material.DataUUL);
                        toCreateCH4.all_ULL = CalculateRoundDistance(Material.CH4ULL, Material.DataULL);
                        toCreateCH4.CreateTime = DateTime.Now;
                        _context.Add(toCreateCH4);
                        Device.CEF_Correction = Material.CEF_Correction;
                        Device.data_UUL = Material.DataUUL;
                        Device.data_ULL = Material.DataULL;
                    }
                    if (Material.N2OCEF != 0)
                    {
                        var toCreateN2O = new GHG();
                        toCreateN2O.Id = Guid.NewGuid();
                        toCreateN2O.Name = "N2O";
                        toCreateN2O.DeviceId = id;
                        toCreateN2O.CEF = Material.N2OCEF;
                        toCreateN2O.CEF_UUL = Material.N2OUUL;
                        toCreateN2O.CEF_ULL = Material.N2OULL;
                        toCreateN2O.GWP = GWP.Where(x => x.Name == "N2O").Select(x => x.Num).FirstOrDefault();
                        toCreateN2O.all_UUL = CalculateRoundDistance(Material.N2OUUL, Material.DataUUL);
                        toCreateN2O.all_ULL = CalculateRoundDistance(Material.N2OULL, Material.DataULL);
                        toCreateN2O.CreateTime = DateTime.Now;
                        _context.Add(toCreateN2O);
                        Device.CEF_Correction = Material.CEF_Correction;
                        Device.data_UUL = Material.DataUUL;
                        Device.data_ULL = Material.DataULL;
                    }
                    if (Material.HFCSCEF != 0)
                    {
                        var toCreateHFCS = new GHG();
                        toCreateHFCS.Id = Guid.NewGuid();
                        toCreateHFCS.Name = "HFCS";
                        toCreateHFCS.DeviceId = id;
                        toCreateHFCS.CEF = Material.HFCSCEF;
                        toCreateHFCS.CEF_UUL = Material.HFCSUUL;
                        toCreateHFCS.CEF_ULL = Material.HFCSULL;
                        toCreateHFCS.GWP = GWP.Where(x => x.Name == material).Select(x => x.Num).FirstOrDefault();
                        toCreateHFCS.all_UUL = CalculateRoundDistance(Material.HFCSUUL, Material.DataUUL);
                        toCreateHFCS.all_ULL = CalculateRoundDistance(Material.HFCSULL, Material.DataULL);
                        toCreateHFCS.CreateTime = DateTime.Now;
                        _context.Add(toCreateHFCS);
                        Device.CEF_Correction = Material.CEF_Correction;
                        Device.data_UUL = Material.DataUUL;
                        Device.data_ULL = Material.DataULL;
                    }
                    if (Material.PFCSCEF != 0)
                    {
                        var toCreatePFCS = new GHG();
                        toCreatePFCS.Id = Guid.NewGuid();
                        toCreatePFCS.Name = "PFCS";
                        toCreatePFCS.DeviceId = id;
                        toCreatePFCS.CEF = Material.PFCSCEF;
                        toCreatePFCS.CEF_UUL = Material.PFCSUUL;
                        toCreatePFCS.CEF_ULL = Material.PFCSULL;
                        toCreatePFCS.GWP = GWP.Where(x => x.Name == material).Select(x => x.Num).FirstOrDefault();
                        toCreatePFCS.all_UUL = CalculateRoundDistance(Material.PFCSUUL, Material.DataUUL);
                        toCreatePFCS.all_ULL = CalculateRoundDistance(Material.PFCSULL, Material.DataULL);
                        toCreatePFCS.CreateTime = DateTime.Now;
                        _context.Add(toCreatePFCS);
                        Device.CEF_Correction = Material.CEF_Correction;
                        Device.data_UUL = Material.DataUUL;
                        Device.data_ULL = Material.DataULL;
                    }
                    if (Material.SF6CEF != 0)
                    {
                        var toCreateSF6 = new GHG();
                        toCreateSF6.Id = Guid.NewGuid();
                        toCreateSF6.Name = "SF6";
                        toCreateSF6.DeviceId = id;
                        toCreateSF6.CEF = Material.SF6CEF;
                        toCreateSF6.CEF_UUL = Material.SF6UUL;
                        toCreateSF6.CEF_ULL = Material.SF6ULL;
                        toCreateSF6.GWP = GWP.Where(x => x.Name == "SF6").Select(x => x.Num).FirstOrDefault();
                        toCreateSF6.all_UUL = CalculateRoundDistance(Material.SF6UUL, Material.DataUUL);
                        toCreateSF6.all_ULL = CalculateRoundDistance(Material.SF6ULL, Material.DataULL);
                        toCreateSF6.CreateTime = DateTime.Now;
                        _context.Add(toCreateSF6);
                        Device.CEF_Correction = Material.CEF_Correction;
                        Device.data_UUL = Material.DataUUL;
                        Device.data_ULL = Material.DataULL;
                    }
                    if (Material.NF3CEF != 0)
                    {
                        var toCreateNF3 = new GHG();
                        toCreateNF3.Id = Guid.NewGuid();
                        toCreateNF3.Name = "NF3";
                        toCreateNF3.DeviceId = id;
                        toCreateNF3.CEF = Material.NF3CEF;
                        toCreateNF3.CEF_UUL = Material.NF3UUL;
                        toCreateNF3.CEF_ULL = Material.NF3ULL;
                        toCreateNF3.GWP = GWP.Where(x => x.Name == "NF3").Select(x => x.Num).FirstOrDefault();
                        toCreateNF3.all_UUL = CalculateRoundDistance(Material.NF3UUL, Material.DataUUL);
                        toCreateNF3.all_ULL = CalculateRoundDistance(Material.NF3ULL, Material.DataULL);
                        toCreateNF3.CreateTime = DateTime.Now;
                        _context.Add(toCreateNF3);
                        Device.CEF_Correction = Material.CEF_Correction;
                        Device.data_UUL = Material.DataUUL;
                        Device.data_ULL = Material.DataULL;
                    }
                }
                else if (otherMaterial != null) //目前只有冷媒設備，但我包含了PFCS以防萬一
                {
                    if (otherMaterial.HFCSCEF != 0)
                    {
                        var toCreateHFCS = new GHG();
                        toCreateHFCS.Id = Guid.NewGuid();
                        toCreateHFCS.Name = "HFCS";
                        toCreateHFCS.DeviceId = id;
                        toCreateHFCS.CEF = otherMaterial.HFCSCEF;
                        toCreateHFCS.CEF_UUL = otherMaterial.HFCSUUL;
                        toCreateHFCS.CEF_ULL = otherMaterial.HFCSULL;
                        toCreateHFCS.GWP = GWP.Where(x => x.Name == material).Select(x => x.Num).FirstOrDefault();
                        toCreateHFCS.all_UUL = CalculateRoundDistance(otherMaterial.HFCSUUL, otherMaterial.DataUUL);
                        toCreateHFCS.all_ULL = CalculateRoundDistance(otherMaterial.HFCSULL, otherMaterial.DataULL);
                        toCreateHFCS.CreateTime = DateTime.Now;
                        _context.Add(toCreateHFCS);
                        Device.CEF_Correction = otherMaterial.CEF_Correction;
                        Device.data_UUL = otherMaterial.DataUUL;
                        Device.data_ULL = otherMaterial.DataULL;
                    }
                    if (otherMaterial.PFCSCEF != 0)
                    {
                        var toCreatePFCS = new GHG();
                        toCreatePFCS.Id = Guid.NewGuid();
                        toCreatePFCS.Name = "PFCS";
                        toCreatePFCS.DeviceId = id;
                        toCreatePFCS.CEF = otherMaterial.PFCSCEF;
                        toCreatePFCS.CEF_UUL = otherMaterial.PFCSUUL;
                        toCreatePFCS.CEF_ULL = otherMaterial.PFCSULL;
                        toCreatePFCS.GWP = GWP.Where(x => x.Name == material).Select(x => x.Num).FirstOrDefault();
                        toCreatePFCS.all_UUL = CalculateRoundDistance(otherMaterial.PFCSUUL, otherMaterial.DataUUL);
                        toCreatePFCS.all_ULL = CalculateRoundDistance(otherMaterial.PFCSULL, otherMaterial.DataULL);
                        toCreatePFCS.CreateTime = DateTime.Now;
                        _context.Add(toCreatePFCS);
                        Device.CEF_Correction = otherMaterial.CEF_Correction;
                        Device.data_UUL = otherMaterial.DataUUL;
                        Device.data_ULL = otherMaterial.DataULL;

                    }

                }
                await _context.SaveChangesAsync();

            }

            return null;
        }
        public async Task CountEmissionData(Guid? id)
        {
            var activityDatas = await _context.ActivityDatas.Where(x => x.DeviceId == id).ToListAsync();
            decimal Num = activityDatas.Sum(x => x.Num);


            var Device = await _context.Devices.Where(x => x.Id == id).Include(x => x.Area).FirstOrDefaultAsync();
            var GHG = await _context.GHGs.Where(x => x.DeviceId == id).ToListAsync(); //抓出需要算排放量的排放源中的溫室氣體
            var emission = await _context.Areas.FindAsync(Device.AreaId);
            decimal all_Emission = 0,
                GHG1 = 0, GHG2 = 0, GHG3 = 0,
                GHG1ULL = 0, GHG1UUL = 0,
                GHG2ULL = 0, GHG2UUL = 0,
                GHG3ULL = 0, GHG3UUL = 0;
            if (GHG != null && Device != null)
            {
                if (ModelState.IsValid)
                {
                    int i = 1;
                    foreach (var item in GHG)
                    {
                        if (item.Device.Material == "廢水處理")
                        {
                            item.Emission = item.CEF * Num * item.GWP;
                        }
                        else
                        {
                            item.Emission = item.CEF * Num / 1000 * item.GWP;
                        }

                        item.ModifiedTime = DateTime.Now;
                        if (i == 1)
                        {
                            GHG1 += item.Emission;
                            all_Emission += item.Emission;
                            GHG1ULL += item.all_ULL * 100;
                            GHG1UUL += item.all_UUL * 100;
                        }
                        if (i == 2)
                        {
                            GHG2 += item.Emission;
                            all_Emission += item.Emission;
                            GHG2ULL += item.all_ULL * 100;
                            GHG2UUL += item.all_UUL * 100;
                        }
                        if (i == 3)
                        {
                            GHG3 += item.Emission;
                            all_Emission += item.Emission;
                            GHG3ULL += item.all_ULL * 100;
                            GHG3UUL += item.all_UUL * 100;
                        }
                        i++;
                    }
                    decimal Device_allUUL = Calculate95U(GHG1, GHG2, GHG3, GHG1UUL, GHG2UUL, GHG3UUL);
                    decimal Device_allULL = Calculate95U(GHG1, GHG2, GHG3, GHG1ULL, GHG2ULL, GHG3ULL);
                    Device.Emissions = all_Emission;
                    emission.All += all_Emission;
                    Device.all_UUL = Device_allUUL;
                    Device.all_ULL = Device_allULL;
                    Device.ModifiedTime = DateTime.Now;
                    Device.count_UUL = Device_allUUL * all_Emission * Device_allUUL * all_Emission;
                    Device.count_ULL = Device_allULL * all_Emission * Device_allULL * all_Emission;

                    await _context.SaveChangesAsync();
                }
            }
        }
        public decimal Calculate95U(decimal GHG1, decimal GHG2, decimal GHG3, decimal GHG1UUL, decimal GHG2UUL, decimal GHG3UUL) //計算95%信賴區間 
        {
            decimal count;
            decimal allGHG;

            if (GHG1 != 0 && GHG1UUL != 0)
            {
                if (GHG2 != 0 && GHG2UUL != 0)
                {
                    if (GHG3 != 0 && GHG3UUL != 0)
                    {
                        count = DecimalSqrt((GHG1 * GHG1UUL * GHG1 * GHG1UUL) + (GHG2 * GHG2UUL * GHG2 * GHG2UUL) + (GHG3 * GHG3UUL * GHG3 * GHG3UUL));
                        allGHG = GHG1 + GHG2 + GHG3;

                        if (allGHG != 0)
                        {
                            return count / allGHG;
                        }

                    }
                    count = DecimalSqrt((GHG1 * GHG1UUL * GHG1 * GHG1UUL) + (GHG2 * GHG2UUL * GHG2 * GHG2UUL));
                    allGHG = GHG1 + GHG2;

                    if (allGHG != 0)
                    {
                        return count / allGHG;
                    }
                }
                else if (GHG3 != 0 && GHG3UUL != 0)
                {
                    if (GHG2 != 0 && GHG2UUL != 0)
                    {
                        count = DecimalSqrt((GHG1 * GHG1UUL * GHG1 * GHG1UUL) + (GHG2 * GHG2UUL * GHG2 * GHG2UUL) + (GHG3 * GHG3UUL * GHG3 * GHG3UUL));
                        allGHG = GHG1 + GHG2 + GHG3;

                        if (allGHG != 0)
                        {
                            return count / allGHG;
                        }

                    }
                    count = DecimalSqrt((GHG1 * GHG1UUL * GHG1 * GHG1UUL) + (GHG3 * GHG3UUL * GHG3 * GHG3UUL));
                    allGHG = GHG1 + GHG3;

                    if (allGHG != 0)
                    {
                        return count / allGHG;
                    }
                }

                return GHG1UUL;
            }

            return 0;
        }
        public decimal DecimalSqrt(decimal value, int iterations = 20) //用牛頓法逼近Decimal的平方根
        {
            if (value < 0)
            {
                throw new ArgumentException("不能計算負數的平方根");
            }

            decimal guess = value / 2;
            for (int i = 0; i < iterations; i++)
            {
                guess = 0.5m * (guess + value / guess);
            }

            return guess;
        }
        public decimal CalculateRoundDistance(decimal num1, decimal num2) // 計算兩數平方和的平方根，並四捨五入到小數點後5位
        {
            if (num1 != 0 && num2 != 0)
            {
                decimal distance = DecimalSqrt(num1 * num1 + num2 * num2);
                return Math.Round(distance, 5);
            }
            return 0;
        }
        #endregion
    }
}
