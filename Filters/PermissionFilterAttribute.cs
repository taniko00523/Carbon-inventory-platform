using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class PermissionFilterAttribute : ActionFilterAttribute
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PermissionFilterAttribute(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        string controllerName = context.ActionDescriptor.RouteValues["controller"];
        string actionName = context.ActionDescriptor.RouteValues["action"];

        var user = context.HttpContext.User;

        // 確保用戶已登入
        if (user == null || !user.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // 獲取用戶的 Id
        string userId = _userManager.GetUserId(user);

        // 獲取用戶的角色
        var roles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(userId));
        if (roles == null || !roles.Any())
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // 使用 _context 進行資料庫操作
        var function = _context.Functions.FirstOrDefault(c => c.Name == controllerName);
        if (function == null)
        {
            context.Result = new NotFoundResult();
            return;
        }

        var action = _context.FunctionActions.FirstOrDefault(a => a.Name == actionName);
        if (action == null)
        {
            context.Result = new NotFoundResult();
            return;
        }

        var permission = _context.Permissions
            .FirstOrDefault(p => p.FunctionId == function.Id && p.FunctionActionId == action.Id);
        if (permission == null)
        {
            context.Result = new ForbidResult();
            return;
        }

        var hasPermission = _context.RolePermissions
            .Any(rp => roles.Contains(rp.RoleId) && rp.PermissionId == permission.Id);

        if (!hasPermission)
        {
            context.Result = new ForbidResult();
            return;
        }

        await next();
    }

    
}
