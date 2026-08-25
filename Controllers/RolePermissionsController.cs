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

namespace Carbon_inventory_platform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolePermissionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RolePermissionsController(ApplicationDbContext context)
        {
            _context = context;
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
