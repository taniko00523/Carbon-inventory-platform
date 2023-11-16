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
            //        var devices = await _context.Devices
            //.Where(x => x.isDeleted == 0)
            //.OrderBy(x => x.CreateTime)
            //.Include(x => x.Areas)
            //.ToListAsync();

            //        float sum_hardlymove = 0;
            //        float sum_move = 0;
            //        float sum_escape = 0;
            //        float sum_process = 0;
            //        float sum_electricity = 0;
            //        float sum1_CO2 = 0;
            //        float sum2_CO2 = 0;
            //        float sum_CH4 = 0;
            //        float sum_N2O = 0;
            //        float sum_HFCS = 0;
            //        float sum_PFCS = 0;
            //        float sum_SF6 = 0;
            //        float sum_NF3 = 0;
            //        float sum_Scope1 = 0;
            //        float sum_Scope2 = 0;
            //        float sum_all = 0;
            //        int no1_Grade = 0;
            //        int no2_Grade = 0;
            //        int no3_Grade = 0;
            //        float avg_Grade = 0;
            //        string all_Grade = "";
            //        foreach (var item in devices)
            //        {
            //            if (item.EmissionPattern == "固定")
            //            {
            //                sum_hardlymove = sum_hardlymove + item.Emissions;
            //            }
            //            else if (item.EmissionPattern == "移動")
            //            {
            //                sum_move = sum_move + item.Emissions;
            //            }
            //            else if (item.EmissionPattern == "逸散")
            //            {
            //                sum_escape = sum_escape + item.Emissions;
            //            }
            //            else if (item.EmissionPattern == "移動")
            //            {
            //                sum_process = sum_process + item.Emissions;
            //            }
            //            else if (item.EmissionPattern == "外購電力")
            //            {
            //                sum_electricity = sum_electricity + item.Emissions;
            //            }
            //        }
            //        foreach (var item in devices)
            //        {
            //            if (item.Scope == "類別一")
            //            {
            //                sum1_CO2 += item.CO2;
            //                sum_CH4 += item.CH4;
            //                sum_N2O += item.N2O;
            //                sum_HFCS += item.HFCS;
            //                sum_PFCS += item.PFCS;
            //                sum_SF6 += item.SF6;
            //                sum_NF3 += item.NF3;
            //            }
            //            else if (item.Scope == "類別二")
            //            {
            //                sum2_CO2 += item.CO2; //可能會重複加總
            //            }
            //        }
            //        foreach (var item in devices)
            //        {
            //            if (item.Grade < 10 && item.Grade > 0)
            //            {
            //                no1_Grade += 1;
            //            }
            //            else if (item.Grade < 19 && item.Grade >= 10)
            //            {
            //                no2_Grade += 1;
            //            }
            //            else
            //            {
            //                no3_Grade += 1;
            //            }
            //        }
            //        sum_Scope1 = sum1_CO2 + sum_CH4 + sum_N2O + sum_HFCS + sum_PFCS + sum_NF3 + sum_SF6;
            //        sum_Scope2 = sum2_CO2;
            //        sum_all = sum_Scope1 + sum_Scope2;

            //        foreach (var item in devices)
            //        {
            //            avg_Grade += (float)Math.Round((float)(Math.Round(item.Emissions / sum_all, 4) * item.Grade), 2);
            //        }
            //        if (avg_Grade < 10)
            //        {
            //            all_Grade = "第一級";
            //        }
            //        else if (avg_Grade < 19)
            //        {
            //            all_Grade = "第二級";
            //        }
            //        else
            //        {
            //            all_Grade = "第三級";
            //        }

            //        foreach (var item in devices)
            //        {

            //        }
            //        if (avg_Grade < 10)
            //        {
            //            all_Grade = "第一級";
            //        }
            //        else if (avg_Grade < 19)
            //        {
            //            all_Grade = "第二級";
            //        }
            //        else
            //        {
            //            all_Grade = "第三級";
            //        }

            //        float all_UUL = 0;
            //        float all_ULL = 0;
            //        foreach (var item in devices)
            //        {
            //            all_ULL += item.ULL;
            //            all_UUL += item.UUL;
            //        }
            //        float sum_Uncertainty = 0;
            //        foreach (var item in devices)
            //        {
            //            if (all_UUL != 0)
            //            {
            //                sum_Uncertainty += item.Emissions;
            //            }
            //        }

            //        all_ULL = (float)(Math.Pow(all_ULL, 0.5) /sum_all);
            //        all_UUL = (float)(Math.Pow(all_UUL, 0.5) / sum_all);

            //        ViewBag.sum_hardlymove = sum_hardlymove.ToString("F4");
            //        ViewBag.sum_move = sum_move.ToString("F4");
            //        ViewBag.sum_escape = sum_escape.ToString("F4");
            //        ViewBag.sum_process = sum_process.ToString("F4");
            //        ViewBag.sum_electricity = sum_electricity.ToString("F4");
            //        ViewBag.sum1_CO2 = sum1_CO2.ToString("F4");
            //        ViewBag.sum2_CO2 = (sum1_CO2 + sum2_CO2).ToString("F4");
            //        ViewBag.sum_CH4 = sum_CH4.ToString("F4");
            //        ViewBag.sum_N2O = sum_N2O.ToString("F4");
            //        ViewBag.sum_HFCS = sum_HFCS.ToString("F4");
            //        ViewBag.sum_PFCS = sum_PFCS.ToString("F4");
            //        ViewBag.sum_SF6 = sum_SF6.ToString("F4");
            //        ViewBag.sum_NF3 = sum_NF3.ToString("F4");
            //        ViewBag.sum_Scope1 = sum_Scope1.ToString("F4");
            //        ViewBag.sum_Scope2 = sum_Scope2.ToString("F4");
            //        ViewBag.sum_All = sum_all.ToString("F4");

            //        ViewBag.no1_Grade = no1_Grade;
            //        ViewBag.no2_Grade = no2_Grade;
            //        ViewBag.no3_Grade = no3_Grade;
            //        ViewBag.avg_Grade = avg_Grade;
            //        ViewBag.all_Grade = all_Grade;

            //        ViewBag.percentage1_CO2 = (sum1_CO2 / sum_Scope1 * 100).ToString("F2") + "%";
            //        ViewBag.percentage1_CH4 = (sum_CH4 / sum_Scope1 * 100).ToString("F2") + "%";
            //        ViewBag.percentage1_N2O = (sum_N2O / sum_Scope1 * 100).ToString("F2") + "%";
            //        ViewBag.percentage1_HFCS = (sum_HFCS / sum_Scope1 * 100).ToString("F2") + "%";
            //        ViewBag.percentage1_PFCS = (sum_PFCS / sum_Scope1 * 100).ToString("F2") + "%";
            //        ViewBag.percentage1_SF6 = (sum_SF6 / sum_Scope1 * 100).ToString("F2") + "%";
            //        ViewBag.percentage1_NF3 = (sum_NF3 / sum_Scope1 * 100).ToString("F2") + "%";

            //        ViewBag.percentage2_CO2 = ((sum1_CO2 + sum2_CO2) / sum_all * 100).ToString("F2") + "%";
            //        ViewBag.percentage2_CH4 = (sum_CH4 / sum_all * 100).ToString("F2") + "%";
            //        ViewBag.percentage2_N2O = (sum_N2O / sum_all * 100).ToString("F2") + "%";
            //        ViewBag.percentage2_HFCS = (sum_HFCS / sum_all * 100).ToString("F2") + "%";
            //        ViewBag.percentage2_PFCS = (sum_PFCS / sum_all * 100).ToString("F2") + "%";
            //        ViewBag.percentage2_SF6 = (sum_SF6 / sum_all * 100).ToString("F2") + "%";
            //        ViewBag.percentage2_NF3 = (sum_NF3 / sum_all * 100).ToString("F2") + "%";

            //        ViewBag.percentage_hardlymove = (sum_hardlymove / sum_all * 100).ToString("F2") + "%";
            //        ViewBag.percentage_move = (sum_move / sum_all * 100).ToString("F2") + "%";
            //        ViewBag.percentage_escape = (sum_escape / sum_all * 100).ToString("F2") + "%";
            //        ViewBag.percentage_process = (sum_process / sum_all * 100).ToString("F2") + "%";
            //        ViewBag.percentage_Scope1 = (sum_Scope1 / sum_all * 100).ToString("F2") + "%";

            //        ViewBag.percentage_Scope2 = (sum_Scope2 / sum_all * 100).ToString("F2") + "%";

            //        ViewBag.all_UUL = all_UUL;
            //        ViewBag.all_ULL = all_ULL;

            //        ViewBag.sum_Uncertainty = sum_Uncertainty;

            //        return _context.Devices != null ? //如果有抓到資料表Null
            //                      View(await _context.Devices
            //                      .Where(x => x.isDeleted == 0) //抓出資料表裡面沒被刪除的
            //                      .OrderBy(x => x.CreateTime)
            //                      .Include(x => x.Areas)
            //                      .ToListAsync()) : //非同步方法
            //                      Problem("沒有找到資料表"); //否則回報問題 Entity set 'ApplicationDbContext.Companies'  is null.

            var devices = await _context.Devices
                .Where(x => x.isDeleted == 0)
                .OrderBy(x => x.CreateTime)
                .Include(x => x.Areas)
                .ToListAsync();

            float sum_hardlymove = 0, sum_move = 0, sum_escape = 0, sum_process = 0, sum_electricity = 0,
                sum1_CO2 = 0, sum2_CO2 = 0, sum_CH4 = 0, sum_N2O = 0, sum_HFCS = 0, sum_PFCS = 0,
                sum_SF6 = 0, sum_NF3 = 0, sum_Scope1 = 0, sum_Scope2 = 0, sum_all = 0;
            int no1_Grade = 0, no2_Grade = 0, no3_Grade = 0;
            float avg_Grade = 0;
            string all_Grade = "";
            float all_UUL = 0, all_ULL = 0, sum_Uncertainty = 0;

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

                
                all_ULL += item.ULL;
                all_UUL += item.UUL;
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
            
            all_ULL = (float)(Math.Pow(all_ULL, 0.5) / sum_Uncertainty);
            all_UUL = (float)(Math.Pow(all_UUL, 0.5) / sum_Uncertainty);

            // 以下是 ViewBag 設定，您可以根據需要進行修改

            ViewBag.sum_hardlymove = sum_hardlymove.ToString("F4");
            ViewBag.sum_move = sum_move.ToString("F4");
            ViewBag.sum_escape = sum_escape.ToString("F4");
            ViewBag.sum_process = sum_process.ToString("F4");
            ViewBag.sum_electricity = sum_electricity.ToString("F4");
            ViewBag.sum1_CO2 = sum1_CO2.ToString("F4");
            ViewBag.sum2_CO2 = (sum1_CO2 + sum2_CO2).ToString("F4");
            ViewBag.sum_CH4 = sum_CH4.ToString("F4");
            ViewBag.sum_N2O = sum_N2O.ToString("F4");
            ViewBag.sum_HFCS = sum_HFCS.ToString("F4");
            ViewBag.sum_PFCS = sum_PFCS.ToString("F4");
            ViewBag.sum_SF6 = sum_SF6.ToString("F4");
            ViewBag.sum_NF3 = sum_NF3.ToString("F4");
            ViewBag.sum_Scope1 = sum_Scope1.ToString("F4");
            ViewBag.sum_Scope2 = sum_Scope2.ToString("F4");
            ViewBag.sum_All = sum_all.ToString("F4");

            ViewBag.no1_Grade = no1_Grade;
            ViewBag.no2_Grade = no2_Grade;
            ViewBag.no3_Grade = no3_Grade;
            ViewBag.avg_Grade = avg_Grade;
            ViewBag.all_Grade = avg_Grade < 10 ? "第一級" : (avg_Grade < 19 ? "第二級" : "第三級");

            ViewBag.percentage1_CO2 = (sum1_CO2 / sum_Scope1 * 100).ToString("F2") + "%";
            ViewBag.percentage1_CH4 = (sum_CH4 / sum_Scope1 * 100).ToString("F2") + "%";
            ViewBag.percentage1_N2O = (sum_N2O / sum_Scope1 * 100).ToString("F2") + "%";
            ViewBag.percentage1_HFCS = (sum_HFCS / sum_Scope1 * 100).ToString("F2") + "%";
            ViewBag.percentage1_PFCS = (sum_PFCS / sum_Scope1 * 100).ToString("F2") + "%";
            ViewBag.percentage1_SF6 = (sum_SF6 / sum_Scope1 * 100).ToString("F2") + "%";
            ViewBag.percentage1_NF3 = (sum_NF3 / sum_Scope1 * 100).ToString("F2") + "%";

            ViewBag.percentage2_CO2 = ((sum1_CO2 + sum2_CO2) / sum_all * 100).ToString("F2") + "%";
            ViewBag.percentage2_CH4 = (sum_CH4 / sum_all * 100).ToString("F2") + "%";
            ViewBag.percentage2_N2O = (sum_N2O / sum_all * 100).ToString("F2") + "%";
            ViewBag.percentage2_HFCS = (sum_HFCS / sum_all * 100).ToString("F2") + "%";
            ViewBag.percentage2_PFCS = (sum_PFCS / sum_all * 100).ToString("F2") + "%";
            ViewBag.percentage2_SF6 = (sum_SF6 / sum_all * 100).ToString("F2") + "%";
            ViewBag.percentage2_NF3 = (sum_NF3 / sum_all * 100).ToString("F2") + "%";

            ViewBag.percentage_hardlymove = (sum_hardlymove / sum_all * 100).ToString("F2") + "%";
            ViewBag.percentage_move = (sum_move / sum_all * 100).ToString("F2") + "%";
            ViewBag.percentage_escape = (sum_escape / sum_all * 100).ToString("F2") + "%";
            ViewBag.percentage_process = (sum_process / sum_all * 100).ToString("F2") + "%";
            ViewBag.percentage_Scope1 = (sum_Scope1 / sum_all * 100).ToString("F2") + "%";

            ViewBag.percentage_Scope2 = (sum_Scope2 / sum_all * 100).ToString("F2") + "%";

            ViewBag.all_ULL = "-" + all_ULL.ToString("P2");
            ViewBag.all_UUL = "+" + all_UUL.ToString("P2");
            

            ViewBag.sum_Uncertainty = sum_Uncertainty;
            ViewBag.percentage_Uncertainty = (sum_Uncertainty/sum_all * 100).ToString("F2") + "%";

            return _context.Devices != null ?
                View(await _context.Devices
                    .Where(x => x.isDeleted == 0)
                    .OrderBy(x => x.CreateTime)
                    .Include(x => x.Areas)
                    .ToListAsync()) :
                Problem("沒有找到資料表");


        }
    }
}
