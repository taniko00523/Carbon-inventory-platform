using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Carbon_inventory_platform.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;

namespace Carbon_inventory_platform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CompanyUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 在这里处理模型数据，例如更新数据库
                var user = await _userManager.FindByIdAsync(model.ApplicationUser.Id);
                if (user != null)
                {
                    var userName = await _userManager.GetUserNameAsync(user);
                    

                    user.UserLimitData = model.ApplicationUser.UserLimitData;
                    if (userName != model.ApplicationUser.UserName)
                    {
                        await _userManager.SetUserNameAsync(user, model.ApplicationUser.UserName);
                    }
                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var result = await _userManager.ResetPasswordAsync(user, token, model.Password);
                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return View(model);
                        }
                    }
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }


        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
