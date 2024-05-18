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

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if(user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
