using Carbon_inventory_platform.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Carbon_inventory_platform.Controllers
{
    public class EmissionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmissionController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> IndexAsync()
        {
            float sum_hardlymove = 0;
            float sum_move = 0;
            float sum_escape = 0;
            float sum_process = 0;
            float sum_electricity = 0;

            foreach (var item in await _context.Devices.ToListAsync())
            {
                if(item.EmissionPattern == "固定")
                {
                    sum_hardlymove = sum_hardlymove + item.Emissions;
                } else if (item.EmissionPattern == "移動")
                {
                    sum_move = sum_move + item.Emissions;
                }
                else if (item.EmissionPattern == "逸散")
                {
                    sum_escape = sum_escape + item.Emissions;
                }
                else if (item.EmissionPattern == "移動")
                {
                    sum_process = sum_process + item.Emissions;
                }
                else if (item.EmissionPattern == "外購電力")
                {
                    sum_electricity = sum_electricity + item.Emissions;
                }
            }
            ViewData["sum_hardlymove"] = sum_hardlymove;
            ViewData["sum_move"] = sum_move;
            ViewData["sum_escape"] = sum_escape;
            ViewData["sum_process"] = sum_process;
            ViewData["sum_electricity"] = sum_electricity;

            return _context.Devices != null ? //如果有抓到資料表Null
                          View(await _context.Devices
                          .Where(x => x.isDeleted == 0) //抓出資料表裡面沒被刪除的
                          .OrderBy(x => x.CreateTime)
                          .Include(x => x.Areas)
                          .ToListAsync()) : //非同步方法
                          Problem("沒有找到資料表"); //否則回報問題 Entity set 'ApplicationDbContext.Companies'  is null.
        }
    }
}
