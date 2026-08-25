using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Carbon_inventory_platform.Filters;
using Carbon_inventory_platform.ViewModel;

namespace Carbon_inventory_platform.Controllers
{

    [CheckSubscriptionData]
    [Authorize(Roles = "Admin")]
    public class CompaniesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CompaniesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string searchTerm, int pageNumber = 1)
        {
            const int pageSize = 10;
            // 原本用 _userManager.Users.ToList() 撈出「全部」使用者，再對每一個使用者各自查一次 Companies，
            // 是典型的 N+1；而且完全沒有濾掉已軟刪除（isDeleted=1）的公司，全部撈進記憶體之後才做搜尋跟分頁。
            // 改成從 Companies 直接下手，一次查詢、交給資料庫做搜尋與分頁；isDeleted 現在由全域查詢過濾器處理。
            IQueryable<Company> query = _context.Companies
                .AsNoTracking()
                .Where(c => c.User == null || c.User.Email != "Admin")
                .Include(c => c.User);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c => c.Name.Contains(searchTerm));
            }

            var paged = await PagedResult<Company>.CreateAsync(query.OrderBy(c => c.CreateTime), pageNumber, pageSize);

            var viewModel = paged.Items.Select(c => new CompanyUserViewModel
            {
                Company = c,
                ApplicationUser = c.User
            }).ToList();

            // 將搜尋和分頁資訊加入ViewData
            ViewData["SearchTerm"] = searchTerm;
            ViewData["PageNumber"] = paged.PageNumber;
            ViewData["TotalPages"] = paged.TotalPages;

            return View(viewModel);
        }
        public async Task<IActionResult> Edit(Guid id)
        {
            // 原本用 FindAsync 直接找，沒有濾掉已軟刪除（isDeleted=1）的公司，
            // 站內找不到任何「復原已刪除公司」的功能，等於讓已刪除的公司還能被打開來改。
            // isDeleted 現在由全域查詢過濾器處理，這裡不需要重複寫。
            Company? company = await _context.Companies.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }
        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,EasyName,ContactName,Email,Phone,EnglishName,EasyEnglishName,UserId,CompanyInformation,AddressInformation,ReportingInformation,GHGInformation,Scope1Information,Scope2Information,ReportOpening,ReportingPurposes")] Company company)
        {
            string userId = _userManager.GetUserId(User);
            if (id != company.Id)
            {
                return NotFound();
            }
            ModelState.Remove("User");
            if (ModelState.IsValid)
            {
                try
                {
                    // 原本用 FindAsync 直接找，且不論找到的公司是否已被軟刪除都會把 isDeleted 蓋回 0，
                    // 等於讓 Edit 表單變成一個隱藏的「復原已刪除公司」功能；站內找不到對應的復原介面，
                    // 因此改成已刪除的公司一律回 NotFound，不再讓 Edit 動到它（isDeleted 由全域過濾器處理）。
                    var toUpdate = await _context.Companies.FirstOrDefaultAsync(x => x.Id == id);
                    if (toUpdate == null)
                    {
                        return NotFound();
                    }
                    toUpdate.Name = company.Name;
                    toUpdate.EasyName = RemoveSuffixes(company.Name);
                    toUpdate.EasyEnglishName = RemoveENSuffixes(company.EnglishName);
                    toUpdate.EnglishName = company.EnglishName;
                    toUpdate.ContactName = company.ContactName;
                    toUpdate.Email = company.Email;
                    toUpdate.Phone = company.Phone;
                    toUpdate.ReportingPurposes = company.ReportingPurposes;
                    toUpdate.ReportOpening = company.ReportOpening;
                    toUpdate.CompanyInformation = company.CompanyInformation;
                    toUpdate.AddressInformation = company.AddressInformation;
                    toUpdate.ReportingInformation = company.ReportingInformation;
                    toUpdate.GHGInformation = company.GHGInformation;
                    toUpdate.Scope1Information = company.Scope1Information;
                    toUpdate.Scope2Information = company.Scope2Information;
                    toUpdate.ModifiedTime = DateTime.Now;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
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
            return View(company);
        }
        private bool CompanyExists(Guid id)
        {
            return (_context.Companies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public string RemoveSuffixes(string input)
        {

            string[] suffixes = { "股份有限公司", "有限公司" };
            foreach (string suffix in suffixes)
            {
                if (input.EndsWith(suffix))
                {
                    return input.Substring(0, input.Length - suffix.Length);
                }
            }

            // 如果没有匹配的后缀，则返回原始输入
            return input;
        }
        public string RemoveENSuffixes(string input)
        {
            string[] suffixes = { "INDUSTRIAL CO., LTD", "PAPERWARE CO., LTD", "SCIENTIFIC CO., LTD", "B.T. CO.,LTD", "AUTOMOBILE DIE CO., LTD", "INTERNATIONAL CO., LTD", "MACHINE CO., LTD", "TECHNOLOGY CO., LTD", "Co., Ltd", "CORPORATION" };

            // 使用 Trim() 去除字符串两端的空格
            string trimmedInput = input.Trim();

            // 判断是否包含后缀
            foreach (string suffix in suffixes)
            {
                // 使用 StringComparison.OrdinalIgnoreCase 进行不区分大小写的比较
                if (trimmedInput.IndexOf(suffix, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    // 如果包含后缀，截取到后缀的位置
                    return trimmedInput.Substring(0, trimmedInput.IndexOf(suffix, StringComparison.OrdinalIgnoreCase));
                }
            }

            // 如果没有匹配的后缀，则返回原始输入
            return input;
        }
    }
}
