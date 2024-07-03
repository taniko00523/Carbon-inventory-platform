using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Authorization;
using Carbon_inventory_platform.Filters;
using Microsoft.AspNetCore.Identity;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        // GET: Areas
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

        // GET: Areas/Details/5
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

        // GET: Areas/Create
        public IActionResult Create()
        {
            ViewBag.ARVersion = _context.GWPs.Select(x => x.ARCount).Distinct().ToList();
            return View();
        }

        // POST: Areas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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



        // GET: Areas/Edit/5
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


        // POST: Areas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,FullAddress,Year,BaseYear,Type,UniqueCode,FactorCode,OrganizationImage,MapImage,ShopDrawings,ARVersion")] Area area)
        {
            if (id != area.Id)
            {
                return NotFound();
            }
            var companyId = TempData.Peek("companyId") as Guid?;
            if (ModelState.IsValid)
            {
                try
                {
                    var baseyearData = await _context.Areas.Where(x => x.CompanyId == companyId && x.FullAddress == area.FullAddress && x.BaseYear).FirstOrDefaultAsync();
                    if (baseyearData != null)
                    {
                        baseyearData.BaseYear = false;
                        await _context.SaveChangesAsync();
                    }
                    var toUpdate = await _context.Areas.FindAsync(id);
                    if (toUpdate != null)
                    {
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
                            string OrganizationImagePath = await SaveImage(area.OrganizationImage, companyId.ToString(), area.Id.ToString());
                            toUpdate.OrganizationImagePath = OrganizationImagePath;
                        }
                        if (area.MapImage != null)
                        {
                            string MapImagePath = await SaveImage(area.MapImage, companyId.ToString(), area.Id.ToString());
                            toUpdate.MapImagePath = MapImagePath;
                        }
                        if (area.ShopDrawings != null)
                        {
                            string ShopDrawingsPath = await SaveImage(area.ShopDrawings, companyId.ToString(), area.Id.ToString());
                            toUpdate.ShopDrawingsPath = ShopDrawingsPath;
                        }
                        toUpdate.Year = area.Year;
                        toUpdate.BaseYear = area.BaseYear;
                        toUpdate.Type = area.Type;
                        toUpdate.ModifiedTime = DateTime.Now;
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
            }
            ViewBag.ARVersion = _context.GWPs.Select(x => x.ARCount).Distinct().ToList();
            return View(area);
        }

        // GET: Areas/Delete/5
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

        // POST: Areas/Delete/5
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

        static string GetCity(string input)
        {
            return input.Substring(0, 3);
        }
        static string GetDistrict(string input)
        {
            return input.Substring(3, 3);
        }
        static string GetAddress(string input)
        {
            return input.Substring(6);
        }

        private static async Task<string> SaveImage(IFormFile file, string companyId, string areaId)
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

    }
}
