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
    public class ReportDataModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public ReportDataModel(
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

            [Display(Name = "公司簡介")]
            public string? CompanyInformation { get; set; }

            [Display(Name = "組織邊界說明")]
            public string? AddressInformation { get; set; }

            [Display(Name = "報告邊界說明")]
            public string? ReportingInformation { get; set; }

            [Display(Name = "溫室氣體排放類型與排放量說明")]
            public string? GHGInformation { get; set; }

            [Display(Name = "直接溫室氣體排放說明")]
            public string? Scope1Information { get; set; }

            [Display(Name = "間接溫室氣體排放​說明")]
            public string? Scope2Information { get; set; }
            [Display(Name = "前言")]
            public string? ReportOpening { get; set; }

            [Display(Name = "報告用途")]
            public string? ReportingPurposes { get; set; }
        }

        private async Task LoadAsync(string userId)
        {
            Company company = await _context.Companies.Where(x => x.UserId == userId).FirstOrDefaultAsync();


            Input = new InputModel
            {
                Id = company.Id,
                UserId = company.UserId,
                CompanyInformation = company.CompanyInformation,
                
                AddressInformation = company.AddressInformation,
                ReportingPurposes = company.ReportingPurposes,
                GHGInformation = company.GHGInformation,
                Scope1Information = company.Scope1Information,
                Scope2Information = company.Scope2Information,
                ReportingInformation = company.ReportingInformation,
                ReportOpening = company.ReportOpening
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
                toUpdate.ReportOpening = Input.ReportOpening;
                toUpdate.ReportingPurposes = Input.ReportingPurposes;
                toUpdate.CompanyInformation = Input.CompanyInformation;
                toUpdate.AddressInformation = Input.AddressInformation;
                toUpdate.ReportingInformation = Input.ReportingInformation;
                toUpdate.GHGInformation = Input.GHGInformation;
                toUpdate.Scope1Information = Input.Scope1Information;
                toUpdate.Scope2Information = Input.Scope2Information;
                toUpdate.ModifiedTime = DateTime.Now;
            }
            await _context.SaveChangesAsync();
            StatusMessage = "報告資料已上傳";
            return RedirectToPage();
        }

       
    }
}
