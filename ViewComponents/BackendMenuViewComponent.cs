using Carbon_inventory_platform.Models;
using Carbon_inventory_platform.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Carbon_inventory_platform.ViewComponents
{
    /// <summary>
    /// B1 階段 2：一般使用者（非 Admin/SuperAdmin）的選單裡，依權限表動態列出
    /// 他被授權的後台畫面（例如某個角色被開放檢視「排放係數設定」）。
    /// 原本這類使用者的選單完全寫死只有一個「邊界總覽」連結，即使後台管理員在
    /// 角色權限總覽幫他的角色開了某個後台畫面的權限，畫面上也沒有任何連結能點進去。
    /// </summary>
    public class BackendMenuViewComponent : ViewComponent
    {
        // 目前只有這些畫面的 Controller 已經掛上 PermissionFilterAttribute（見 plan.md B1 階段 1）。
        // 選單只能顯示「使用者有權限、而且畫面本身真的會依權限表檢查」的項目，否則會出現
        // 按下去一定被擋的死連結（例如 Companies／User 目前仍是寫死的 [Authorize(Roles="Admin")]）。
        // 之後每掛上一個 Controller，記得把它的 Function.Name 加進這個清單。
        private static readonly HashSet<string> PermissionEnforcedFunctions = new()
        {
            "GWP", "Materials", "DefaultDevices", "DeviceDatas",
        };

        private readonly RolePermissionService _rolePermissionService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public BackendMenuViewComponent(
            RolePermissionService rolePermissionService,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _rolePermissionService = rolePermissionService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Function> visible;
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    return View(new List<Function>());
                }

                // GetRolesAsync 回傳角色名稱，權限表存的是角色 Id，這裡要先轉換
                // （與 PermissionFilterAttribute 的作法一致）。
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

                var functions = await _rolePermissionService.GetVisibleFunctionsAsync(user.Id, roleIds);
                visible = functions.Where(f => PermissionEnforcedFunctions.Contains(f.Name)).ToList();
            }
            catch
            {
                // 這裡是版面配置的一部分，登入頁／錯誤頁都共用同一份版面，
                // 資料庫查詢失敗時選單只是少幾個項目，不能讓整頁因此掛掉。
                visible = new List<Function>();
            }

            return View(visible);
        }
    }
}
