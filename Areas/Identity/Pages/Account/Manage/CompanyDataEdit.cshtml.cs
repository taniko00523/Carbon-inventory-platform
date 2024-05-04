// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Carbon_inventory_platform.Areas.Identity.Pages.Account.Manage
{
    public class CompanyDataModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public CompanyDataModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [Display(Name = "用戶名稱")]
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "ID")]
            public Guid Id { get; set; }

            public string UserId { get; set; } // 外來鍵屬性

            [MaxLength(50)]
            [Display(Name = "公司名稱")]
            [Required(ErrorMessage = "請填寫公司名稱")]
            public string Name { get; set; } = "";

            [MaxLength(10)]
            [Display(Name = "公司簡稱")]
            public string? EasyName { get; set; } = "";

            [MaxLength(100)]
            [Display(Name = "英文公司名稱")]
            [Required(ErrorMessage = "請填寫英文公司名稱")]
            public string EnglishName { get; set; } = "";

            [MaxLength(100)]
            [Display(Name = "英文公司簡稱")]
            public string? EasyEnglishName { get; set; } = "";

            [MaxLength(20)]
            [Display(Name = "聯絡人")]
            [Required(ErrorMessage = "請填寫聯絡人")]
            public string ContactName { get; set; } = "";

            [EmailAddress(ErrorMessage = "電子信箱格式錯誤")]
            [MaxLength(50)]
            [Display(Name = "電子信箱")]
            [Required(ErrorMessage = "請填寫電子信箱")]
            public string Email { get; set; } = "";

            [MaxLength(20)]
            [Display(Name = "電話號碼")]
            [Required(ErrorMessage = "請填寫手機號碼")]
            public string Phone { get; set; } = "";
        }

        private async Task LoadAsync(string userId)
        {
            Company company = await _context.Companies.Where(x => x.UserId == userId).FirstOrDefaultAsync();


            Input = new InputModel
            {
                Id = company.Id,
                UserId = company.UserId,
                Name = company.Name,
                EnglishName = company.EnglishName,
                ContactName = company.ContactName,
                Email = company.Email,
                Phone = company.Phone
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            string userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return NotFound($"找不到用戶 '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(userId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return NotFound($"找不到用戶 '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(userId);
                return Page();
            }

            var toUpdate = await _context.Companies.FindAsync(Input.Id);
            if (toUpdate != null)
            {
                toUpdate.Name = Input.Name;
                toUpdate.EasyName = RemoveSuffixes(Input.Name);
                toUpdate.EasyEnglishName = RemoveENSuffixes(Input.EnglishName);
                toUpdate.EnglishName = Input.EnglishName;
                toUpdate.ContactName = Input.ContactName;
                toUpdate.Email = Input.Email;
                toUpdate.Phone = Input.Phone;
                toUpdate.ModifiedTime = DateTime.Now;
            }
            await _context.SaveChangesAsync();
            StatusMessage = "公司資訊已更新";
            return RedirectToPage();
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

            return input;
        }
        static string RemoveENSuffixes(string input)
        {
            string[] suffixes = { "INDUSTRIAL CO., LTD", "PAPERWARE CO., LTD", "SCIENTIFIC CO., LTD", "B.T. CO.,LTD", "AUTOMOBILE DIE CO., LTD", "INTERNATIONAL CO., LTD", "MACHINE CO., LTD", "TECHNOLOGY CO., LTD", "Co., Ltd", "CORPORATION" };

            string trimmedInput = input.Trim();

            foreach (string suffix in suffixes)
            {
                if (trimmedInput.IndexOf(suffix, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    return trimmedInput.Substring(0, trimmedInput.IndexOf(suffix, StringComparison.OrdinalIgnoreCase));
                }
            }

            return input;
        }
    }
}
