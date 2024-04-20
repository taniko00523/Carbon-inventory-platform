using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Carbon_inventory_platform.Filters;

namespace Carbon_inventory_platform.Controllers
{
    [CheckSubscriptionData]
    [Authorize]
    public class CompaniesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CompaniesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //[Authorize]
        // GET: Companies
        public async Task<IActionResult> Index()
        {
            string userId = _userManager.GetUserId(User);
            if(userId != null)
            {
                var company = await _context.Companies.Where(x => x.isDeleted == 0 && x.UserId == userId).OrderByDescending(x => x.CreateTime).ToListAsync();
                return View(company);
            }
            else
            {
                return View();
            }
            
        }


        public async Task<IActionResult> Create()
        {
            string userId = _userManager.GetUserId(User);
            var Company_id = Guid.NewGuid();
            var Area_id = Guid.NewGuid();
            if (ModelState.IsValid)
            {
                await _context.Companies.AddAsync(new Company()
                {
                    Id = Company_id,
                    UserId = userId,
                    CreateTime = DateTime.Now
                });
                await _context.SaveChangesAsync();
                    await _context.Areas.AddAsync(new Area()
                {
                    Id = Area_id,
                    CompanyId = Company_id,
                    Year = DateTime.Now.Year - 1912, //減去1911取得民國年 再減去1盤去年
                    BaseYear = true,
                    CreateTime = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }



        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Companies == null)
            {
                return Problem("沒有找到資料");
            }
            //抓出要刪除的資料
            var delCompany = await _context.Companies.FindAsync(id);
            var hasCompany = _context.Companies //如果此公司有修改過
                .Where(c => c.Id == id)
                .Any(c => c.ModifiedTime != null);

            var hasArea = _context.Companies //如果此公司下有修改過的廠區
    .Where(c => c.Id == id)
    .SelectMany(c => c.Areas)
    .Any(area => area.ModifiedTime != null);

            var hasDevice = _context.Companies //如果此公司有修改過的排放源
    .Where(c => c.Id == id)
    .SelectMany(c => c.Areas)
    .SelectMany(y => y.Devices)
    .Any(device => device.ModifiedTime != null);


            if (hasCompany || hasArea || hasDevice)
            {
                delCompany.isDeleted = 1;
                delCompany.DeleteTime = DateTime.Now;
            }
            else
            {
                _context.Companies.Remove(delCompany);
            }




            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.FindAsync(id);
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
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,EasyName,ContactName,Email,Phone,EnglishName,EasyEnglishName,UserId,CompanyInformation,AddressInformation,ReportingInformation,GHGInformation,Scope1Information,Scope2Information")] Company company)
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
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        

        private bool CompanyExists(Guid id)
        {
          return (_context.Companies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        static string RemoveSuffixes(string input)
        {

            string[] suffixes = {  "股份有限公司","有限公司" };
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
