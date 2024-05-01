using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Carbon_inventory_platform.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Carbon_inventory_platform.Controllers
{
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

            return View(viewModel);
        }
    }
}
