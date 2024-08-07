using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Carbon_inventory_platform.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class UserController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IUserEmailStore<ApplicationUser> _emailStore;
    private readonly ILogger<UserController> _logger;
    private readonly IEmailSender _emailSender;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public UserController(UserManager<ApplicationUser> userManager,
        IUserStore<ApplicationUser> userStore,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<UserController> logger,
        IEmailSender emailSender,
        ApplicationDbContext context)
    {
        _context = context;
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _signInManager = signInManager;
        _logger = logger;
        _emailSender = emailSender;
        _roleManager = roleManager;
    }

    public IActionResult Index()
    {
        List<ApplicationUser> users = _userManager.Users.ToList();
        List<CompanyUserViewModel> viewModel = new List<CompanyUserViewModel>();

        foreach (var user in users)
        {
            CompanyUserViewModel model = new CompanyUserViewModel
            {
                ApplicationUser = user,
                Company = _context.Companies.FirstOrDefault(x => x.UserId == user.Id)
            };
            viewModel.Add(model);
        }
        viewModel.Remove(viewModel.FirstOrDefault(x => x.ApplicationUser.Email == "Admin"));
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(CompanyUserViewModel model)
    {
        var user = CreateUser();
        await _userStore.SetUserNameAsync(user, model.ApplicationUser.UserName, CancellationToken.None);
        await _emailStore.SetEmailAsync(user, model.ApplicationUser.Email, CancellationToken.None);
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation("User created a new account with password.");
            var role = await _roleManager.FindByNameAsync("User");

            if (role != null)
            {
                await _userManager.AddToRoleAsync(user, role.Name);
            }
            user.UserLimitData = model.UserLimitData;
            await _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        // Return the Index view with a model that matches the expected type
        var users = _userManager.Users.ToList();
        var viewModel = new List<CompanyUserViewModel>();

        foreach (var single_user in users)
        {
            CompanyUserViewModel userModel = new CompanyUserViewModel
            {
                ApplicationUser = single_user,
                Company = _context.Companies.FirstOrDefault(x => x.UserId == single_user.Id)
            };
            viewModel.Add(userModel);
        }
        viewModel.Remove(viewModel.FirstOrDefault(x => x.ApplicationUser.Email == "Admin"));

        return View("Index", viewModel);
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CompanyUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(model.ApplicationUser.Id);
            if (user != null)
            {
                if (user.UserName != model.ApplicationUser.UserName)
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

                user.UserLimitData = model.ApplicationUser.UserLimitData;
                await _userManager.UpdateAsync(user);
                return RedirectToAction(nameof(Index));
            }
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

    private ApplicationUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<ApplicationUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor.");
        }
    }

    private IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<ApplicationUser>)_userStore;
    }
}
