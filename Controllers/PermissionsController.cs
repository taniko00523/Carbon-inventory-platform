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
    public class PermissionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PermissionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Permissions
        public async Task<IActionResult> Index()
        {
            // 原本沒有過濾 IsDeleted，已軟刪除的權限還是會列出來。
            var applicationDbContext = _context.Permissions
                .Where(p => p.IsDeleted == 0)
                .Include(p => p.FunctionAction)
                .Include(p => p.Function);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Permissions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permission = await _context.Permissions
                .Include(p => p.FunctionAction)
                .Include(p => p.Function)
                .FirstOrDefaultAsync(m => m.Id == id && m.IsDeleted == 0);
            if (permission == null)
            {
                return NotFound();
            }

            return View(permission);
        }

        // GET: Permissions/Create
        public IActionResult Create()
        {
            PopulateSelectLists(null, null);
            return View();
        }

        // POST: Permissions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,FunctionId,FunctionActionId")] Permission permission)
        {
            // 原本 [Bind] 把外鍵寫成 "ActionId"（模型上真正的屬性是 FunctionActionId），
            // 白名單會把表單送來的 FunctionActionId 濾掉、永遠是 0，SaveChanges 直接違反外鍵而 500。
            // 另外原本連主鍵 Id 與 CreateTime/IsDeleted 等稽核欄位都由表單綁定，
            // 會造成 IDENTITY_INSERT 錯誤並讓稽核時間可被竄改，改為只綁必要欄位、時間在伺服器端戳記。
            await ValidateNotDuplicateAsync(permission, 0);

            if (ModelState.IsValid)
            {
                permission.CreateTime = DateTime.Now;
                permission.IsDeleted = 0;
                _context.Add(permission);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateSelectLists(permission.FunctionId, permission.FunctionActionId);
            return View(permission);
        }

        // GET: Permissions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permission = await _context.Permissions
                .FirstOrDefaultAsync(m => m.Id == id && m.IsDeleted == 0);
            if (permission == null)
            {
                return NotFound();
            }
            PopulateSelectLists(permission.FunctionId, permission.FunctionActionId);
            return View(permission);
        }

        // POST: Permissions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,FunctionId,FunctionActionId")] Permission permission)
        {
            if (id != permission.Id)
            {
                return NotFound();
            }

            await ValidateNotDuplicateAsync(permission, id);

            if (ModelState.IsValid)
            {
                // 原本直接 _context.Update(permission)，而表單並不會送出 CreateTime/IsDeleted，
                // 新建的實體會把 CreateTime 覆寫成當下時間、把軟刪除旗標一起蓋掉。
                // 改成讀出現有資料列後只更新可編輯的欄位。
                var permissionToUpdate = await _context.Permissions
                    .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == 0);
                if (permissionToUpdate == null)
                {
                    return NotFound();
                }

                permissionToUpdate.Name = permission.Name;
                permissionToUpdate.FunctionId = permission.FunctionId;
                permissionToUpdate.FunctionActionId = permission.FunctionActionId;
                permissionToUpdate.ModifyTime = DateTime.Now;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PermissionExists(permissionToUpdate.Id))
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
            PopulateSelectLists(permission.FunctionId, permission.FunctionActionId);
            return View(permission);
        }

        // GET: Permissions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permission = await _context.Permissions
                .Include(p => p.FunctionAction)
                .Include(p => p.Function)
                .FirstOrDefaultAsync(m => m.Id == id && m.IsDeleted == 0);
            if (permission == null)
            {
                return NotFound();
            }

            return View(permission);
        }

        // POST: Permissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // 原本是實體刪除 (_context.Permissions.Remove)，但模型本來就設計成軟刪除
            // (IsDeleted / DeleteTime)。實體刪除會讓 RolePermissions / UserPermissions
            // 一併被連帶刪除，而且沒有任何稽核紀錄可以追查。
            var permission = await _context.Permissions.FindAsync(id);
            if (permission != null && permission.IsDeleted == 0)
            {
                permission.IsDeleted = 1;
                permission.DeleteTime = DateTime.Now;
                _context.Permissions.Update(permission);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PermissionExists(int id)
        {
            return _context.Permissions.Any(e => e.Id == id);
        }

        /// <summary>
        /// 建立畫面 / 功能的下拉選單。原本 value 與顯示文字都用 "Id"，
        /// 畫面上只看到 1、2、3 這種無意義的整數，改用中文名稱 (CName)。
        /// </summary>
        private void PopulateSelectLists(int? selectedFunctionId, int? selectedFunctionActionId)
        {
            ViewData["FunctionId"] = new SelectList(
                _context.Functions.AsNoTracking().OrderBy(f => f.Sort).ThenBy(f => f.Name).ToList(),
                "Id", "CName", selectedFunctionId);
            ViewData["ActionId"] = new SelectList(
                _context.FunctionActions.AsNoTracking().OrderBy(a => a.Name).ToList(),
                "Id", "CName", selectedFunctionActionId);
        }

        /// <summary>
        /// (FunctionId, FunctionActionId) 在資料庫上是「未刪除才唯一」的索引，
        /// 重複新增原本會直接拋 DbUpdateException 變成 500，改成顯示錯誤訊息。
        /// </summary>
        private async Task ValidateNotDuplicateAsync(Permission permission, int excludeId)
        {
            var duplicated = await _context.Permissions.AnyAsync(p =>
                p.Id != excludeId
                && p.IsDeleted == 0
                && p.FunctionId == permission.FunctionId
                && p.FunctionActionId == permission.FunctionActionId);
            if (duplicated)
            {
                ModelState.AddModelError(string.Empty, "此畫面與功能的權限已經存在，請勿重複新增。");
            }
        }
    }
}
