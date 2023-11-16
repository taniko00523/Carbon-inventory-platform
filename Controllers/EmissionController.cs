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
            float sum1_CO2 = 0;
            float sum2_CO2 = 0;
            float sum_CH4 = 0;
            float sum_N2O = 0;
            float sum_HFCS = 0;
            float sum_PFCS = 0;
            float sum_SF6 = 0;
            float sum_NF3 = 0;
            float sum_Scope1 = 0;
            float sum_Scope2 = 0;
            float sum_all = 0;
            foreach (var item in await _context.Devices.ToListAsync())
            {
                if (item.EmissionPattern == "固定")
                {
                    sum_hardlymove = sum_hardlymove + item.Emissions;
                }
                else if (item.EmissionPattern == "移動")
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
            foreach (var item in await _context.Devices.ToListAsync())
            {
                if(item.Scope == "類別一")
                {
                    sum1_CO2 += item.CO2;
                    sum_CH4 += item.CH4;
                    sum_N2O += item.N2O;
                    sum_HFCS += item.HFCS;
                    sum_PFCS += item.PFCS;
                    sum_SF6 += item.SF6;
                    sum_NF3 += item.NF3;
                }
                else if (item.Scope == "類別二")
                {
                    sum2_CO2 += item.CO2; //可能會重複加總
                }
            }
            sum_Scope1 = sum1_CO2 + sum_CH4 + sum_N2O + sum_HFCS + sum_PFCS + sum_NF3 + sum_SF6;
            sum_Scope2 = sum2_CO2;
            sum_all = sum_Scope1 + sum_Scope2;

            ViewData["sum_hardlymove"] = sum_hardlymove.ToString("F4");
            ViewData["sum_move"] = sum_move.ToString("F4");
            ViewData["sum_escape"] = sum_escape.ToString("F4");
            ViewData["sum_process"] = sum_process.ToString("F4");
            ViewData["sum_electricity"] = sum_electricity.ToString("F4");
            ViewData["sum1_CO2"] = sum1_CO2.ToString("F4");
            ViewData["sum2_CO2"] = (sum1_CO2 + sum2_CO2).ToString("F4");
            ViewData["sum_CH4"] = sum_CH4.ToString("F4");
            ViewData["sum_N2O"] = sum_N2O.ToString("F4");
            ViewData["sum_HFCS"] = sum_HFCS.ToString("F4");
            ViewData["sum_PFCS"] = sum_PFCS.ToString("F4");
            ViewData["sum_SF6"] = sum_SF6.ToString("F4");
            ViewData["sum_NF3"] = sum_NF3.ToString("F4");
            ViewData["sum_Scope1"] = sum_Scope1.ToString("F4");
            ViewData["sum_Scope2"] = sum_Scope2.ToString("F4");
            ViewData["sum_All"] = sum_all.ToString("F4");


            ViewData["percentage1_CO2"] = (sum1_CO2 / sum_Scope1 * 100).ToString("F2") + "%";
            ViewData["percentage1_CH4"] = (sum_CH4 / sum_Scope1 * 100).ToString("F2") + "%";
            ViewData["percentage1_N2O"] = (sum_N2O / sum_Scope1 * 100).ToString("F2") + "%";
            ViewData["percentage1_HFCS"] = (sum_HFCS / sum_Scope1 * 100).ToString("F2") + "%";
            ViewData["percentage1_PFCS"] = (sum_PFCS / sum_Scope1 * 100).ToString("F2") + "%";
            ViewData["percentage1_SF6"] = (sum_SF6 / sum_Scope1 * 100).ToString("F2") + "%";
            ViewData["percentage1_NF3"] = (sum_NF3 / sum_Scope1 * 100).ToString("F2") + "%";


            ViewData["percentage2_CO2"] = ((sum1_CO2 + sum2_CO2) / sum_all * 100).ToString("F2") + "%";
            ViewData["percentage2_CH4"] = (sum_CH4 / sum_all * 100).ToString("F2") + "%";
            ViewData["percentage2_N2O"] = (sum_N2O / sum_all * 100).ToString("F2") + "%";
            ViewData["percentage2_HFCS"] = (sum_HFCS / sum_all * 100).ToString("F2") + "%";
            ViewData["percentage2_PFCS"] = (sum_PFCS / sum_all * 100).ToString("F2") + "%";
            ViewData["percentage2_SF6"] = (sum_SF6 / sum_all * 100).ToString("F2") + "%";
            ViewData["percentage2_NF3"] = (sum_NF3 / sum_all * 100).ToString("F2") + "%";




            ViewData["percentage_hardlymove"] = (sum_hardlymove / sum_all * 100).ToString("F2") + "%";
            ViewData["percentage_move"] = (sum_move / sum_all * 100).ToString("F2") + "%";
            ViewData["percentage_escape"] = (sum_escape / sum_all * 100).ToString("F2") + "%";
            ViewData["percentage_process"] = (sum_process / sum_all * 100).ToString("F2") + "%";
            ViewData["percentage_Scope1"] = (sum_Scope1 / sum_all * 100).ToString("F2") + "%";

            ViewData["percentage_Scope2"] = (sum_Scope2 / sum_all * 100).ToString("F2") + "%";

            //ViewData[""]
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
