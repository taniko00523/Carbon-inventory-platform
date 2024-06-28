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

        public async Task<IActionResult> AdminIndex()
        {
            List<ApplicationUser> users = _userManager.Users.ToList();
            List<CompanyUserViewModel> viewModel = new List<CompanyUserViewModel>();

            foreach (var user in users)
            {
                CompanyUserViewModel model = new CompanyUserViewModel();
                model.ApplicationUser = user;
                model.Company = _context.Companies.Where(x => x.UserId == user.Id).FirstOrDefault();
                viewModel.Add(model);
            }
            viewModel.Remove(viewModel.FirstOrDefault(x => x.ApplicationUser.Email == "Admin"));
            return View(viewModel);
        }





        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {

            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            Company company = await _context.Companies.FindAsync(id);

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
                    var toUpdate = await _context.Companies.FindAsync(id);
                    if (toUpdate != null)
                    {
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
                        toUpdate.isDeleted = 0;
                        toUpdate.ModifiedTime = DateTime.Now;
                    }
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
                return RedirectToAction(nameof(AdminIndex));
            }
            return View(company);
        }

        private bool CompanyExists(Guid id)
        {
            return (_context.Companies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        static string RemoveSuffixes(string input)
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
        static string RemoveENSuffixes(string input)
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
