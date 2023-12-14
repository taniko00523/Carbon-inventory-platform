using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;

namespace Carbon_inventory_platform.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompaniesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Companies
        public async Task<IActionResult> Index()
        {
              return _context.Companies != null ? //如果有抓到資料表Null
                          View(await _context.Companies
                          .Where(x=>x.isDeleted==0) //抓出資料表裡面沒被刪除的
                            .OrderBy(x => x.CreateTime)

                          .ToListAsync()) :
                          Problem("沒有找到資料"); //否則回報問題 Entity set 'ApplicationDbContext.Companies'  is null.
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Owner,Email,Phone,CreateTime")] Company company)
        {
            if (ModelState.IsValid)
            {
                company.Id = Guid.NewGuid();
                _context.Companies.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }
        public async Task<IActionResult> Default()
        {
           await _context.Companies.AddAsync(new Company()
           {
               Id = Guid.NewGuid(),
               Name = "Default",
               Owner = "Default",
               Email = "Default",
               Phone = "Default",
               CreateTime = DateTime.Now
           });
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,EasyName,Owner,Email,Phone,Information,EnglishName,EasyEnglishName")] Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var toUpdate = await _context.Companies.FindAsync(id);
                    if (toUpdate != null)
                    {
                        toUpdate.Name = company.Name;
                        toUpdate.EasyName = company.EasyName;
                        toUpdate.EasyEnglishName = company.EasyEnglishName;
                        toUpdate.EnglishName = company.EnglishName;
                        toUpdate.Owner = company.Owner;
                        toUpdate.Email = company.Email;
                        toUpdate.Phone = company.Phone;
                        toUpdate.Information = company.Information;
                        toUpdate.isDeleted = 0;
                        toUpdate.ModifiedTime = DateTime.Now;
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index","Home");
            }
            return View(company);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Companies == null)
            {
                return Problem("沒有找到資料");
            }
            var toDelete = await _context.Companies.FindAsync(id);
            if (toDelete.Name == "Default")
            {
                _context.Companies.Remove(toDelete);
            }else if (toDelete != null)
            {
                toDelete.isDeleted = 1;
                toDelete.DeleteTime = DateTime.Now;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(Guid id)
        {
          return (_context.Companies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
