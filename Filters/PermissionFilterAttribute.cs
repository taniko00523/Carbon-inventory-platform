using Carbon_inventory_platform.Models;
using Carbon_inventory_platform.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Carbon_inventory_platform.Filters
{
    /// <summary>
    /// 依 Function / FunctionAction / Permission / RolePermission 資料表決定是否放行。
    ///
    /// 修正的問題：原本用 <c>roles.Contains(rp.RoleId)</c> 比對，
    /// 但 <c>GetRolesAsync</c> 回傳的是「角色名稱」，而 <c>RolePermission.RoleId</c>
    /// 存的是角色的 GUID，兩者永遠不相等 —— 只要掛上這個 Filter，所有請求都會被拒絕。
    /// 現在改為先把角色名稱換成角色 Id 再比對，並把查詢集中到 RolePermissionService。
    ///
    /// 用法（尚未全面掛上，目前授權仍以 [Authorize(Roles = "...")] 為主）：
    /// <c>[ServiceFilter(typeof(PermissionFilterAttribute))]</c>
    /// </summary>
    public class PermissionFilterAttribute : ActionFilterAttribute
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly RolePermissionService _rolePermissionService;

        public PermissionFilterAttribute(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            RolePermissionService rolePermissionService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _rolePermissionService = rolePermissionService;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controllerName = context.ActionDescriptor.RouteValues["controller"];
            var actionName = context.ActionDescriptor.RouteValues["action"];

            if (string.IsNullOrEmpty(controllerName) || string.IsNullOrEmpty(actionName))
            {
                context.Result = new NotFoundResult();
                return;
            }

            // 確保用戶已登入
            var principal = context.HttpContext.User;
            if (principal?.Identity?.IsAuthenticated != true)
            {
                context.Result = new ChallengeResult();
                return;
            }

            var user = await _userManager.GetUserAsync(principal);
            if (user == null)
            {
                context.Result = new ChallengeResult();
                return;
            }

            // Admin 一律放行，避免權限表尚未設定時把管理員自己鎖在外面。
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await next();
                return;
            }

            // GetRolesAsync 回傳「名稱」，權限表存的是「Id」，這裡要先轉換。
            var roleNames = await _userManager.GetRolesAsync(user);
            var roleIds = new List<string>();
            foreach (var roleName in roleNames)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role?.Id != null)
                {
                    roleIds.Add(role.Id);
                }
            }

            if (!await _rolePermissionService.HasPermissionAsync(user.Id, roleIds, controllerName, actionName))
            {
                context.Result = new ForbidResult();
                return;
            }

            await next();
        }
    }
}
