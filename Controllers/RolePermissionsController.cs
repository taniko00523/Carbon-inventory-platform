using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Carbon_inventory_platform.Services;
using Carbon_inventory_platform.ViewModel;

namespace Carbon_inventory_platform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolePermissionsController : Controller
    {
        // Admin 一律放行所有權限檢查（見 PermissionFilterAttribute），在這裡設定完全沒有效果，
        // 列在矩陣的角色選單裡只會讓人誤以為勾選有作用，所以排除。
        private const string AdminRoleName = "Admin";

        private readonly ApplicationDbContext _context;
        private readonly PermissionMatrixService _matrixService;

        public RolePermissionsController(ApplicationDbContext context, PermissionMatrixService matrixService)
        {
            _context = context;
            _matrixService = matrixService;
        }

        // GET: RolePermissions/Matrix?roleId=xxx
        // B1 階段 3：一個角色一個畫面，列出每個畫面 × 動作勾選，取代下面一次一筆的 CRUD。
        public async Task<IActionResult> Matrix(string? roleId)
        {
            var roles = await _context.Roles
                .AsNoTracking()
                .Where(r => r.Name != AdminRoleName)
                .OrderBy(r => r.Name)
                .ToListAsync();

            if (roles.Count == 0)
            {
                ViewData["Roles"] = roles;
                return View(new Carbon_inventory_platform.ViewModel.PermissionMatrixViewModel());
            }

            var selectedRole = roles.FirstOrDefault(r => r.Id == roleId) ?? roles[0];
            ViewData["Roles"] = roles;
            ViewData["SelectedRoleId"] = selectedRole.Id;
            ViewData["SelectedRoleName"] = selectedRole.Name;

            var matrix = await _matrixService.BuildRoleMatrixAsync(selectedRole.Id);
            return View(matrix);
        }

        // POST: RolePermissions/Matrix
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Matrix(string roleId, List<int>? checkedPermissionIds, string? expectedBaseline)
        {
            var role = await _context.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == roleId);
            if (role == null || role.Name == AdminRoleName)
            {
                return NotFound();
            }

            var result = await _matrixService.SaveRoleMatrixAsync(
                roleId, checkedPermissionIds ?? new List<int>(), PermissionMatrixViewModel.DecodeExpectedBaseline(expectedBaseline));
            if (result == PermissionMatrixSaveResult.Conflict)
            {
                // 存檔前有別的管理員也改過這個角色的權限，直接套用這次的勾選會悄悄蓋掉對方的異動，
                // 所以拒絕存檔並請使用者重新整理後再試一次（重新整理會拿到最新狀態）。
                // 用 TempData["ErrorMessage"]（不是 TempData["Error"]）：前者由 _Layout.cshtml 全域顯示，
                // 後者目前只有 Views/Devices/Index.cshtml 自己會渲染，這個畫面沒有對應的顯示邏輯。
                TempData["ErrorMessage"] = $"「{role.Name}」的權限設定在您編輯的同時被其他人修改過，為了避免覆蓋對方的異動，這次的儲存已被取消。請重新整理後再試一次。";
                return RedirectToAction(nameof(Matrix), new { roleId });
            }

            TempData["StatusMessage"] = $"已更新「{role.Name}」角色的權限設定。";
            return RedirectToAction(nameof(Matrix), new { roleId });
        }

        // GET: RolePermissions
        public async Task<IActionResult> Index()
        {
            // 原本沒有 Include，畫面只能顯示角色 GUID 與權限整數，管理員無法辨識。
            return View(await _context.RolePermissions
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .ToListAsync());
        }

        // GET: RolePermissions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rolePermission = await _context.RolePermissions
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rolePermission == null)
            {
                return NotFound();
            }

            return View(rolePermission);
        }

        // GET: RolePermissions/Create
        public async Task<IActionResult> Create()
        {
            await PopulateSelectListsAsync(null, null);
            return View();
        }

        // POST: RolePermissions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoleId,PermissionId")] RolePermission rolePermission)
        {
            // 原本 [Bind] 寫成 "RolePermissionId"（模型上不存在的屬性），而且在新增時綁定
            // 主鍵 Id，會造成 IDENTITY_INSERT 錯誤。改成只綁定真正需要的兩個欄位。
            await ValidateNotDuplicateAsync(rolePermission, 0);

            if (ModelState.IsValid)
            {
                _context.Add(rolePermission);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            await PopulateSelectListsAsync(rolePermission.RoleId, rolePermission.PermissionId);
            return View(rolePermission);
        }

        // GET: RolePermissions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rolePermission = await _context.RolePermissions.FindAsync(id);
            if (rolePermission == null)
            {
                return NotFound();
            }
            await PopulateSelectListsAsync(rolePermission.RoleId, rolePermission.PermissionId);
            return View(rolePermission);
        }

        // POST: RolePermissions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RoleId,PermissionId")] RolePermission rolePermission)
        {
            // 原本 [Bind] 少了真正的主鍵 Id（寫成不存在的 RolePermissionId），
            // 導致 rolePermission.Id 永遠是 0，下面的判斷必定成立而回傳 404，
            // 任何一筆角色權限都無法修改。
            if (id != rolePermission.Id)
            {
                return NotFound();
            }

            await ValidateNotDuplicateAsync(rolePermission, id);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rolePermission);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RolePermissionExists(rolePermission.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            await PopulateSelectListsAsync(rolePermission.RoleId, rolePermission.PermissionId);
            return View(rolePermission);
        }

        // GET: RolePermissions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rolePermission = await _context.RolePermissions
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rolePermission == null)
            {
                return NotFound();
            }

            return View(rolePermission);
        }

        // POST: RolePermissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rolePermission = await _context.RolePermissions.FindAsync(id);
            if (rolePermission != null)
            {
                _context.RolePermissions.Remove(rolePermission);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RolePermissionExists(int id)
        {
            return _context.RolePermissions.Any(e => e.Id == id);
        }

        /// <summary>
        /// 建立角色與權限的下拉選單。原本畫面是純文字輸入框，管理員必須手打 Identity
        /// 角色 GUID 與權限整數，打錯不會有任何提示，只會產生一筆對不到任何權限的死資料。
        /// </summary>
        private async Task PopulateSelectListsAsync(string? selectedRoleId, int? selectedPermissionId)
        {
            var roles = await _context.Roles
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .Select(r => new { r.Id, r.Name })
                .ToListAsync();
            ViewData["RoleId"] = new SelectList(roles, "Id", "Name", selectedRoleId);

            var permissions = await _context.Permissions // IsDeleted 由全域查詢過濾器處理
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Select(p => new { p.Id, p.Name })
                .ToListAsync();
            ViewData["PermissionId"] = new SelectList(permissions, "Id", "Name", selectedPermissionId);
        }

        /// <summary>
        /// (RoleId, PermissionId) 在資料庫上是唯一索引，重複新增原本會直接拋
        /// DbUpdateException 變成 500，改成在畫面上顯示錯誤訊息。
        /// </summary>
        private async Task ValidateNotDuplicateAsync(RolePermission rolePermission, int excludeId)
        {
            var duplicated = await _context.RolePermissions.AnyAsync(rp =>
                rp.Id != excludeId
                && rp.RoleId == rolePermission.RoleId
                && rp.PermissionId == rolePermission.PermissionId);
            if (duplicated)
            {
                ModelState.AddModelError(string.Empty, "此角色已經擁有該權限，請勿重複設定。");
            }
        }
    }
}
