using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Carbon_inventory_platform.Services;
using Carbon_inventory_platform.ViewModel;

namespace Carbon_inventory_platform.Controllers
{
    /// <summary>
    /// B1 階段 4：UserPermission 的維護介面（原本這張表存在但完全沒有畫面能編輯，資料表永遠是空的）。
    /// 一個使用者一個畫面，勾選「在角色之外額外授予」的權限。角色本身已經授予的權限會顯示成
    /// 勾選並鎖住——目前的資料結構無法表達「個別收回角色已授予的權限」，要調整角色權限請到
    /// 角色權限設定；這裡只負責新增額外授權。
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class UserPermissionsController : Controller
    {
        // Admin 帳號不受權限表限制（PermissionFilterAttribute 一律放行），在這裡設定完全沒有效果。
        private const string AdminRoleName = "Admin";

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly PermissionMatrixService _matrixService;

        public UserPermissionsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            PermissionMatrixService matrixService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _matrixService = matrixService;
        }

        // GET: UserPermissions/Matrix?userId=xxx
        public async Task<IActionResult> Matrix(string? userId)
        {
            var users = await BuildSelectableUsersAsync();
            ViewData["Users"] = users;

            var selectedUser = users.FirstOrDefault(u => u.Id == userId);
            if (selectedUser == null)
            {
                // 還沒選使用者，或選到的使用者已經不存在／是管理員：顯示空狀態讓管理員先挑一個。
                ViewData["SelectedUserId"] = null;
                return View(new PermissionMatrixViewModel());
            }

            ViewData["SelectedUserId"] = selectedUser.Id;
            ViewData["SelectedUserName"] = selectedUser.UserName;
            var roleNames = await _userManager.GetRolesAsync(selectedUser);
            ViewData["SelectedUserRoles"] = roleNames;

            var roleIds = await ResolveRoleIdsAsync(roleNames);
            var matrix = await _matrixService.BuildUserMatrixAsync(selectedUser.Id, roleIds);
            return View(matrix);
        }

        // POST: UserPermissions/Matrix
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Matrix(string userId, List<int>? checkedPermissionIds, string? expectedBaseline)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            if (await _userManager.IsInRoleAsync(user, AdminRoleName))
            {
                // 原本畫面上就不會出現管理員可選，這裡再擋一次，避免直接組 POST 繞過畫面限制。
                return Forbid();
            }

            var roleNames = await _userManager.GetRolesAsync(user);
            var roleIds = await ResolveRoleIdsAsync(roleNames);

            var result = await _matrixService.SaveUserMatrixAsync(
                userId, roleIds, checkedPermissionIds ?? new List<int>(), PermissionMatrixViewModel.DecodeExpectedBaseline(expectedBaseline));
            if (result == PermissionMatrixSaveResult.Conflict)
            {
                // 存檔前有別的管理員也改過這個使用者的個別權限，直接套用會悄悄蓋掉對方的異動，
                // 拒絕存檔並請使用者重新整理後再試一次。
                TempData["ErrorMessage"] = $"「{user.UserName}」的個別權限設定在您編輯的同時被其他人修改過，為了避免覆蓋對方的異動，這次的儲存已被取消。請重新整理後再試一次。";
                return RedirectToAction(nameof(Matrix), new { userId });
            }

            TempData["StatusMessage"] = $"已更新「{user.UserName}」的個別權限設定。";
            return RedirectToAction(nameof(Matrix), new { userId });
        }

        /// <summary>可設定個別權限的使用者清單：排除管理員（不受權限表限制，設定了也沒有效果）。</summary>
        private async Task<List<ApplicationUser>> BuildSelectableUsersAsync()
        {
            var admins = await _userManager.GetUsersInRoleAsync(AdminRoleName);
            var adminIds = admins.Select(u => u.Id).ToHashSet();

            return await _context.Users
                .AsNoTracking()
                .Where(u => !adminIds.Contains(u.Id))
                .OrderBy(u => u.UserName)
                .ToListAsync();
        }

        /// <summary>
        /// UserManager.GetRolesAsync 回傳的是角色「名稱」，但 RolePermission.RoleId 存的是角色 GUID，
        /// 兩者不能直接比對（PermissionFilterAttribute 也有同樣的轉換，這裡是另一個呼叫端）。
        /// </summary>
        private async Task<List<string>> ResolveRoleIdsAsync(IEnumerable<string> roleNames)
        {
            var roleIds = new List<string>();
            foreach (var roleName in roleNames)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role?.Id != null)
                {
                    roleIds.Add(role.Id);
                }
            }
            return roleIds;
        }
    }
}
