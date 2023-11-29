using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
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
        public async Task<Emission> CountEmissionAsync(Guid? id)
        {

            var devices = await _context.Devices
                .Where(x => x.isDeleted == 0)
                .Where(x => x.AreaId == id)
                .Include(x => x.Areas)
                .ToListAsync();
            var toCreate = new Emission();
            float sum_hardlymove = 0, sum_move = 0, sum_escape = 0, sum_process = 0, sum_electricity = 0,
                sum1_CO2 = 0, sum2_CO2 = 0, sum_CH4 = 0, sum_N2O = 0, sum_HFCS = 0, sum_PFCS = 0,
                sum_SF6 = 0, sum_NF3 = 0, sum_Scope1 = 0, sum_Scope2 = 0, sum_all = 0;
            int no1_Grade = 0, no2_Grade = 0, no3_Grade = 0;
            float avg_Grade = 0;
            float all_UUL = 0, all_ULL = 0, all_countUUL = 0, all_countULL = 0, sum_Uncertainty = 0;

            foreach (var item in devices)
            {
                switch (item.EmissionPattern)
                {
                    case "固定": sum_hardlymove += item.Emissions; break;
                    case "移動": sum_move += item.Emissions; break;
                    case "逸散": sum_escape += item.Emissions; break;
                    case "處理": sum_process += item.Emissions; break;
                    case "外購電力": sum_electricity += item.Emissions; break;
                }

                if (item.Scope == "類別一")
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
                    sum2_CO2 += item.CO2;
                }

                if (item.Grade < 10 && item.Grade > 0) no1_Grade++;
                else if (item.Grade < 19) no2_Grade++;
                else no3_Grade++;


                all_countULL += item.count_ULL;
                all_countUUL += item.count_UUL;
                if (item.UUL != 0)
                {
                    sum_Uncertainty += item.Emissions;
                }
            }
            sum_Scope1 = sum1_CO2 + sum_CH4 + sum_N2O + sum_HFCS + sum_PFCS + sum_NF3 + sum_SF6;
            sum_Scope2 = sum2_CO2;
            sum_all = sum_Scope1 + sum_Scope2;
            foreach (var item in devices)
            {
                avg_Grade += (float)Math.Round((float)(Math.Round(item.Emissions / sum_all, 4) * item.Grade), 2);
            }

            all_ULL = (float)(Math.Pow(all_countULL, 0.5) / sum_Uncertainty);
            all_UUL = (float)(Math.Pow(all_countUUL, 0.5) / sum_Uncertainty);
            if (_context.emissions.Where(x => x.AreaId == id).ToList().Count != 0)
            {
                toCreate = await _context.emissions.FindAsync(id);
                toCreate.Scope1_CO2 = sum1_CO2.ToString("F4");
                toCreate.CO2 = (sum1_CO2 + sum2_CO2).ToString("F4");
                toCreate.CH4 = sum_CH4.ToString("F4");
                toCreate.N2O = sum_N2O.ToString("F4");
                toCreate.HFCS = sum_HFCS.ToString("F4");
                toCreate.PFCS = sum_PFCS.ToString("F4");
                toCreate.SF6 = sum_SF6.ToString("F4");
                toCreate.NF3 = sum_NF3.ToString("F4");
                toCreate.Scope1 = sum_Scope1.ToString("F4");
                toCreate.Scope2 = sum_Scope2.ToString("F4");
                toCreate.All = sum_all.ToString("F3");
                toCreate.non_move = sum_hardlymove.ToString("F4");
                toCreate.move = sum_move.ToString("F4");
                toCreate.escape = sum_escape.ToString("F4");
                toCreate.process = sum_process.ToString("F4");
                toCreate.percentage1_CO2 = (sum1_CO2 / sum_Scope1 * 100).ToString("F2") + "%";
                toCreate.percentage1_CH4 = (sum_CH4 / sum_Scope1 * 100).ToString("F2") + "%";
                toCreate.percentage1_N2O = (sum_N2O / sum_Scope1 * 100).ToString("F2") + "%";
                toCreate.percentage1_HFCS = (sum_HFCS / sum_Scope1 * 100).ToString("F2") + "%";
                toCreate.percentage1_PFCS = (sum_PFCS / sum_Scope1 * 100).ToString("F2") + "%";
                toCreate.percentage1_SF6 = (sum_SF6 / sum_Scope1 * 100).ToString("F2") + "%";
                toCreate.percentage1_NF3 = (sum_NF3 / sum_Scope1 * 100).ToString("F2") + "%";
                toCreate.percentage2_CO2 = ((sum1_CO2 + sum2_CO2) / sum_all * 100).ToString("F2") + "%";
                toCreate.percentage2_CH4 = (sum_CH4 / sum_all * 100).ToString("F2") + "%";
                toCreate.percentage2_N2O = (sum_N2O / sum_all * 100).ToString("F2") + "%";
                toCreate.percentage2_HFCS = (sum_HFCS / sum_all * 100).ToString("F2") + "%";
                toCreate.percentage2_PFCS = (sum_PFCS / sum_all * 100).ToString("F2") + "%";
                toCreate.percentage2_SF6 = (sum_SF6 / sum_all * 100).ToString("F2") + "%";
                toCreate.percentage2_NF3 = (sum_NF3 / sum_all * 100).ToString("F2") + "%";
                toCreate.percentage_nonMove = (sum_hardlymove / sum_all * 100).ToString("F2") + "%";
                toCreate.percentage_Move = (sum_move / sum_all * 100).ToString("F2") + "%";
                toCreate.percentage_Escape = (sum_escape / sum_all * 100).ToString("F2") + "%";
                toCreate.percentage_Process = (sum_process / sum_all * 100).ToString("F2") + "%";
                toCreate.percentage_Scope1 = (sum_Scope1 / sum_all * 100).ToString("F2") + "%";
                toCreate.percentage_Scope2 = (sum_Scope2 / sum_all * 100).ToString("F2") + "%";
                toCreate.cal_all = sum_Uncertainty.ToString("F4");
                toCreate.no1_Grade = no1_Grade.ToString();
                toCreate.no2_Grade = no2_Grade.ToString();
                toCreate.no3_Grade = no3_Grade.ToString();
                toCreate.avg_Grade = avg_Grade.ToString("F2");
                toCreate.all_Grade = avg_Grade < 10 ? "第一級" : (avg_Grade < 19 ? "第二級" : "第三級");
                toCreate.percentage_CalAll = (sum_Uncertainty / sum_all * 100).ToString("F2") + "%";
                toCreate.ULL = "-" + all_ULL.ToString("P2");
                toCreate.UUL = "+" + all_UUL.ToString("P2");
            }
            else
            {
                toCreate.AreaId = (Guid)id;
                toCreate.Scope1_CO2 = sum1_CO2.ToString("F4");
                toCreate.CO2 = (sum1_CO2 + sum2_CO2).ToString("F4");
                toCreate.CH4 = sum_CH4.ToString("F4");
                toCreate.N2O = sum_N2O.ToString("F4");
                toCreate.HFCS = sum_HFCS.ToString("F4");
                toCreate.PFCS = sum_PFCS.ToString("F4");
                toCreate.SF6 = sum_SF6.ToString("F4");
                toCreate.NF3 = sum_NF3.ToString("F4");
                toCreate.Scope1 = sum_Scope1.ToString("F4");
                toCreate.Scope2 = sum_Scope2.ToString("F4");
                toCreate.All = sum_all.ToString("F3");
                toCreate.non_move = sum_hardlymove.ToString("F4");
                toCreate.move = sum_move.ToString("F4");
                toCreate.escape = sum_escape.ToString("F4");
                toCreate.process = sum_process.ToString("F4");
                toCreate.percentage1_CO2 = (sum1_CO2 / sum_Scope1 * 100).ToString("F2") + "%";
                toCreate.percentage1_CH4 = (sum_CH4 / sum_Scope1 * 100).ToString("F2") + "%";
                toCreate.percentage1_N2O = (sum_N2O / sum_Scope1 * 100).ToString("F2") + "%";
                toCreate.percentage1_HFCS = (sum_HFCS / sum_Scope1 * 100).ToString("F2") + "%";
                toCreate.percentage1_PFCS = (sum_PFCS / sum_Scope1 * 100).ToString("F2") + "%";
                toCreate.percentage1_SF6 = (sum_SF6 / sum_Scope1 * 100).ToString("F2") + "%";
                toCreate.percentage1_NF3 = (sum_NF3 / sum_Scope1 * 100).ToString("F2") + "%";
                toCreate.percentage2_CO2 = ((sum1_CO2 + sum2_CO2) / sum_all * 100).ToString("F2") + "%";
                toCreate.percentage2_CH4 = (sum_CH4 / sum_all * 100).ToString("F2") + "%";
                toCreate.percentage2_N2O = (sum_N2O / sum_all * 100).ToString("F2") + "%";
                toCreate.percentage2_HFCS = (sum_HFCS / sum_all * 100).ToString("F2") + "%";
                toCreate.percentage2_PFCS = (sum_PFCS / sum_all * 100).ToString("F2") + "%";
                toCreate.percentage2_SF6 = (sum_SF6 / sum_all * 100).ToString("F2") + "%";
                toCreate.percentage2_NF3 = (sum_NF3 / sum_all * 100).ToString("F2") + "%";
                toCreate.percentage_nonMove = (sum_hardlymove / sum_all * 100).ToString("F2") + "%";
                toCreate.percentage_Move = (sum_move / sum_all * 100).ToString("F2") + "%";
                toCreate.percentage_Escape = (sum_escape / sum_all * 100).ToString("F2") + "%";
                toCreate.percentage_Process = (sum_process / sum_all * 100).ToString("F2") + "%";
                toCreate.percentage_Scope1 = (sum_Scope1 / sum_all * 100).ToString("F2") + "%";
                toCreate.percentage_Scope2 = (sum_Scope2 / sum_all * 100).ToString("F2") + "%";
                toCreate.cal_all = sum_Uncertainty.ToString("F4");
                toCreate.percentage_CalAll = (sum_Uncertainty / sum_all * 100).ToString("F2") + "%";
                toCreate.no1_Grade = no1_Grade.ToString();
                toCreate.no2_Grade = no2_Grade.ToString();
                toCreate.no3_Grade = no3_Grade.ToString();
                toCreate.avg_Grade = avg_Grade.ToString("F2");
                toCreate.all_Grade = avg_Grade < 10 ? "第一級" : (avg_Grade < 19 ? "第二級" : "第三級");
                toCreate.ULL = "-" + all_ULL.ToString("P2");
                toCreate.UUL = "+" + all_UUL.ToString("P2");
                _context.Add(toCreate);
            }

            await _context.SaveChangesAsync();
            return toCreate;
        }
        public async Task<IActionResult> IndexAsync(Guid? id)
        {
            var CountEmission = await CountEmissionAsync(id);
            TempData["Company"] = await _context.Areas
            .Where(a => a.Id == id)
            .Include(a => a.Company)
            .Select(a => a.Company.Name)
            .FirstOrDefaultAsync();
            var emissions = await _context.emissions
                .FindAsync(id);

            // 以下是 ViewBag 設定，您可以根據需要進行修改
            ViewBag.Scope1_CO2 = emissions.Scope1_CO2;
            ViewBag.sum_CO2 = emissions.CO2;
            ViewBag.sum_CH4 = emissions.CH4;

            ViewBag.sum_N2O = emissions.N2O;


            ViewBag.sum_HFCS = emissions.HFCS;
            

            ViewBag.sum_PFCS = emissions.PFCS;
           

            ViewBag.sum_SF6 = emissions.SF6;

            ViewBag.sum_NF3 = emissions.NF3;

            ViewBag.sum_Scope1 = emissions.Scope1;

            ViewBag.sum_Scope2 = emissions.Scope2;
            
            ViewBag.sum_All = emissions.All;

            ViewBag.sum_non_move = emissions.non_move;

            ViewBag.sum_move = emissions.move;

            ViewBag.sum_escape = emissions.escape;

            ViewBag.sum_process = emissions.process;



            ViewBag.no1_Grade = emissions.no1_Grade;
            ViewBag.no2_Grade = emissions.no2_Grade;
            ViewBag.no3_Grade = emissions.no3_Grade;
            ViewBag.avg_Grade = emissions.avg_Grade;
            ViewBag.all_Grade = emissions.all_Grade;

            ViewBag.percentage1_CO2 = emissions.percentage1_CO2;

            ViewBag.percentage1_CH4 = emissions.percentage1_CH4;

            ViewBag.percentage1_N2O = emissions.percentage1_N2O;

            ViewBag.percentage1_HFCS = emissions.percentage1_HFCS;

            ViewBag.percentage1_PFCS = emissions.percentage1_PFCS;

            ViewBag.percentage1_SF6 = emissions.percentage1_SF6;

            ViewBag.percentage1_NF3 = emissions.percentage1_NF3;


            ViewBag.percentage2_CO2 = emissions.percentage2_CO2;

            ViewBag.percentage2_CH4 = emissions.percentage2_CH4;
            ViewBag.percentage2_N2O = emissions.percentage2_N2O;

            ViewBag.percentage2_HFCS = emissions.percentage2_HFCS;

            ViewBag.percentage2_PFCS = emissions.percentage2_PFCS;

            ViewBag.percentage2_SF6 = emissions.percentage2_SF6;

            ViewBag.percentage2_NF3 = emissions.percentage2_NF3;

            ViewBag.percentage_nonMove = emissions.percentage_nonMove;

            ViewBag.percentage_move = emissions.percentage_Move;

            ViewBag.percentage_escape = emissions.percentage_Escape;

            ViewBag.percentage_process = emissions.percentage_Process;

            ViewBag.percentage_Scope1 = emissions.percentage_Scope1;

            ViewBag.percentage_Scope2 = emissions.percentage_Scope2;

            ViewBag.sum_Uncertainty = emissions.cal_all;
            ViewBag.ULL = emissions.ULL;
            ViewBag.UUL = emissions.UUL;
            ViewBag.percentage_CalAll = emissions.percentage_CalAll;

            return View();


        }
    }
}
