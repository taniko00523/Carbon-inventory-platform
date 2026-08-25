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
    public class FunctionActionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FunctionActionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FunctionActions
        public async Task<IActionResult> Index()
        {
            return View(await _context.FunctionActions.ToListAsync());
        }

        // GET: FunctionActions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var functionAction = await _context.FunctionActions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (functionAction == null)
            {
                return NotFound();
            }

            return View(functionAction);
        }

        // GET: FunctionActions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FunctionActions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // 原本新增時也綁定了自動編號主鍵 Id，只要有人多送一個 Id 欄位，
        // INSERT 就會帶上明確的 Id 而觸發 IDENTITY_INSERT 錯誤（500）。
        public async Task<IActionResult> Create([Bind("Name,CName,IsDefault")] FunctionAction functionAction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(functionAction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(functionAction);
        }

        // GET: FunctionActions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var functionAction = await _context.FunctionActions.FindAsync(id);
            if (functionAction == null)
            {
                return NotFound();
            }
            return View(functionAction);
        }

        // POST: FunctionActions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CName,IsDefault")] FunctionAction functionAction)
        {
            if (id != functionAction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(functionAction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FunctionActionExists(functionAction.Id))
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
            return View(functionAction);
        }

        // GET: FunctionActions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var functionAction = await _context.FunctionActions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (functionAction == null)
            {
                return NotFound();
            }

            return View(functionAction);
        }

        // POST: FunctionActions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var functionAction = await _context.FunctionActions.FindAsync(id);
            if (functionAction != null)
            {
                _context.FunctionActions.Remove(functionAction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FunctionActionExists(int id)
        {
            return _context.FunctionActions.Any(e => e.Id == id);
        }
    }
}
