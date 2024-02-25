using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using ElectronNET.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Office.Interop.Word;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace Carbon_inventory_platform.Controllers
{
    public class EmissionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmissionController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Year> CountEmissionAsync(Guid? id)
        {

            var Devices = await _context.Devices
                .Where(x => x.isDeleted == 0 && x.YearId == id)
                .ToListAsync();
            var GHGs = await _context.GHGs
                .Include(x => x.Device)
                .Where(x => x.isDeleted == 0 && x.Device.YearId == id)
                .ToListAsync();
            var Emission = await _context.Years.Where(x => x.Id == id).ToListAsync();
            var toCreate = new Year();
            decimal sum_hardlymove = 0, sum_move = 0, sum_escape = 0, sum_process = 0, sum_electricity = 0,
                sum1_CO2 = 0, sum2_CO2 = 0, sum1_CH4 = 0, sum2_CH4 = 0, sum1_N2O = 0, sum2_N2O = 0, sum1_HFCS = 0, sum2_HFCS = 0, sum1_PFCS = 0, sum2_PFCS = 0,
                sum1_SF6 = 0, sum2_SF6 = 0, sum1_NF3 = 0, sum2_NF3 = 0, sum_Scope1 = 0, sum_Scope2 = 0, sum_all = 0;
            int no1_Grade = 0, no2_Grade = 0, no3_Grade = 0;
            float avg_Grade = 0;
            decimal all_UUL = 0, all_ULL = 0, all_countUUL = 0, all_countULL = 0, sum_Uncertainty = 0;

            foreach (var GHG in GHGs)
            {
                if (GHG.Device.Scope == "類別一") //計算類別一各溫室氣體排放量
                {
                    switch (GHG.Name)
                    {
                        case "CO2": sum1_CO2 += GHG.Emission; break;
                        case "CH4": sum1_CH4 += GHG.Emission; break;
                        case "N2O": sum1_N2O += GHG.Emission; break;
                        case "HFCS": sum1_HFCS += GHG.Emission; break;
                        case "PFCS": sum1_PFCS += GHG.Emission; break;
                        case "SF6": sum1_SF6 += GHG.Emission; break;
                        case "NF3": sum1_NF3 += GHG.Emission; break;
                    }
                }
                else if (GHG.Device.Scope == "類別二") //計算類別二各溫室氣體排放量
                {
                    switch (GHG.Name)
                    {
                        case "CO2": sum2_CO2 += GHG.Emission; break;
                        case "CH4": sum2_CH4 += GHG.Emission; break;
                        case "N2O": sum2_N2O += GHG.Emission; break;
                        case "HFCS": sum2_HFCS += GHG.Emission; break;
                        case "PFCS": sum2_PFCS += GHG.Emission; break;
                        case "SF6": sum2_SF6 += GHG.Emission; break;
                        case "NF3": sum2_NF3 += GHG.Emission; break;
                    }
                }
            }

            foreach (var device in Devices)
            {
                switch (device.EmissionPattern) //計算各排放型式
                {
                    case "固定": sum_hardlymove += device.Emissions; break;
                    case "移動": sum_move += device.Emissions; break;
                    case "逸散": sum_escape += device.Emissions; break;
                    case "製程": sum_process += device.Emissions; break;
                    case "外購電力": sum_electricity += device.Emissions; break;
                }


                if (device.Grade < 10 && device.Grade > 0) no1_Grade++;
                else if (device.Grade < 19) no2_Grade++;
                else no3_Grade++;

                all_countULL += device.all_ULL;
                all_countUUL += device.count_UUL;

                if (device.all_ULL != 0) //計算有計算不確定性的排放量
                {
                    sum_Uncertainty += device.Emissions;
                }
            }
            decimal CO2 = sum1_CO2 + sum2_CO2;
            decimal CH4 = sum1_CH4 + sum2_CH4;
            decimal N2O = sum1_N2O + sum2_N2O;
            decimal HFCS = sum1_HFCS + sum2_HFCS;
            decimal PFCS = sum1_PFCS + sum2_PFCS;
            decimal SF6 = sum1_SF6 + sum2_SF6;
            decimal NF3 = sum1_NF3 + sum2_SF6;

            sum_Scope1 = sum1_CO2 + sum1_CH4 + sum1_N2O + sum1_HFCS + sum1_PFCS + sum1_NF3 + sum1_SF6;
            sum_Scope2 = sum2_CO2 + sum2_CH4 + sum2_N2O + sum2_HFCS + sum2_PFCS + sum2_NF3 + sum2_SF6;
            sum_all = sum_Scope1 + sum_Scope2;
            foreach (var item in Devices)
            {
                avg_Grade += (float)Math.Round((float)(Math.Round(item.Emissions / sum_all, 4) * item.Grade), 2);
            }
            if (sum_Uncertainty != 0)
            {
                all_UUL = DecimalSqrt(all_countUUL) / sum_Uncertainty;
                all_ULL = DecimalSqrt(all_countULL) / sum_Uncertainty;
            }


            bool sumMatch = Emission.Any(x => x.All != Math.Round(sum_all, 3));
            bool uncertaintyMatch = Emission.Any(x => x.ULL != Math.Round(all_ULL, 2));
            if (Emission.Count != 0)
            {
                if (sumMatch || uncertaintyMatch) //如果總量和不確定性上限有差，則修正進資料庫。
                {
                    toCreate = await _context.Years.FindAsync(id);
                    toCreate.Scope1_CO2 = sum1_CO2;
                    toCreate.Scope1_CH4 = sum1_CH4;
                    toCreate.Scope1_N2O = sum1_N2O;
                    toCreate.Scope1_HFCS = sum1_HFCS;
                    toCreate.Scope1_PFCS = sum1_PFCS;
                    toCreate.Scope1_SF6 = sum1_SF6;
                    toCreate.Scope1_NF3 = sum1_NF3;

                    toCreate.Scope2_CO2 = sum2_CO2;
                    toCreate.Scope2_CH4 = sum2_CH4;
                    toCreate.Scope2_N2O = sum2_N2O;
                    toCreate.Scope2_HFCS = sum2_HFCS;
                    toCreate.Scope2_PFCS = sum2_PFCS;
                    toCreate.Scope2_SF6 = sum2_SF6;
                    toCreate.Scope2_NF3 = sum2_NF3;

                    toCreate.CO2 = CO2;
                    toCreate.CH4 = CH4;
                    toCreate.N2O = N2O;
                    toCreate.HFCS = HFCS;
                    toCreate.PFCS = PFCS;
                    toCreate.SF6 = SF6;
                    toCreate.NF3 = NF3;

                    toCreate.Scope1 = sum_Scope1;
                    toCreate.Scope2 = sum_Scope2;

                    toCreate.All = sum_all;

                    toCreate.non_move = sum_hardlymove;
                    toCreate.move = sum_move;
                    toCreate.escape = sum_escape;
                    toCreate.process = sum_process;

                    toCreate.percentage1_CO2 = (sum1_CO2 / sum_Scope1 * 100);
                    toCreate.percentage1_CH4 = (sum1_CH4 / sum_Scope1 * 100);
                    toCreate.percentage1_N2O = (sum1_N2O / sum_Scope1 * 100);
                    toCreate.percentage1_HFCS = (sum1_HFCS / sum_Scope1 * 100);
                    toCreate.percentage1_PFCS = (sum1_PFCS / sum_Scope1 * 100);
                    toCreate.percentage1_SF6 = (sum1_SF6 / sum_Scope1 * 100);
                    toCreate.percentage1_NF3 = (sum1_NF3 / sum_Scope1 * 100);

                    toCreate.percentage2_CO2 = (CO2 / sum_all * 100);
                    toCreate.percentage2_CH4 = (CH4 / sum_all * 100);
                    toCreate.percentage2_N2O = (N2O / sum_all * 100);
                    toCreate.percentage2_HFCS = (HFCS / sum_all * 100);
                    toCreate.percentage2_PFCS = (PFCS / sum_all * 100);
                    toCreate.percentage2_SF6 = (SF6 / sum_all * 100);
                    toCreate.percentage2_NF3 = (NF3 / sum_all * 100);

                    toCreate.percentage_nonMove = (sum_hardlymove / sum_all * 100);
                    toCreate.percentage_Move = (sum_move / sum_all * 100);
                    toCreate.percentage_Escape = (sum_escape / sum_all * 100);
                    toCreate.percentage_Process = (sum_process / sum_all * 100);
                    toCreate.percentage_Scope1 = (sum_Scope1 / sum_all * 100);
                    toCreate.percentage_Scope2 = (sum_Scope2 / sum_all * 100);

                    toCreate.cal_all = sum_Uncertainty;
                    toCreate.no1_Grade = no1_Grade;
                    toCreate.no2_Grade = no2_Grade;
                    toCreate.no3_Grade = no3_Grade;
                    toCreate.avg_Grade = avg_Grade;
                    toCreate.all_Grade = avg_Grade < 10 ? "第一級" : (avg_Grade < 19 ? "第二級" : "第三級");
                    toCreate.percentage_CalAll = (sum_Uncertainty / sum_all * 100);
                    toCreate.ULL = all_ULL;
                    toCreate.UUL = all_UUL;
                }
            }
            else
            {
                toCreate.AreaId = (Guid)id;
                toCreate.Scope1_CO2 = sum1_CO2;
                toCreate.Scope1_CH4 = sum1_CH4;
                toCreate.Scope1_N2O = sum1_N2O;
                toCreate.Scope1_HFCS = sum1_HFCS;
                toCreate.Scope1_PFCS = sum1_PFCS;
                toCreate.Scope1_SF6 = sum1_SF6;
                toCreate.Scope1_NF3 = sum1_NF3;

                toCreate.Scope2_CO2 = sum2_CO2;
                toCreate.Scope2_CH4 = sum2_CH4;
                toCreate.Scope2_N2O = sum2_N2O;
                toCreate.Scope2_HFCS = sum2_HFCS;
                toCreate.Scope2_PFCS = sum2_PFCS;
                toCreate.Scope2_SF6 = sum2_SF6;
                toCreate.Scope2_NF3 = sum2_NF3;

                toCreate.CO2 = CO2;
                toCreate.CH4 = CH4;
                toCreate.N2O = N2O;
                toCreate.HFCS = HFCS;
                toCreate.PFCS = PFCS;
                toCreate.SF6 = SF6;
                toCreate.NF3 = NF3;

                toCreate.Scope1 = sum_Scope1;
                toCreate.Scope2 = sum_Scope2;

                toCreate.All = sum_all;

                toCreate.non_move = sum_hardlymove;
                toCreate.move = sum_move;
                toCreate.escape = sum_escape;
                toCreate.process = sum_process;

                toCreate.percentage1_CO2 = (sum1_CO2 / sum_Scope1 * 100);
                toCreate.percentage1_CH4 = (sum1_CH4 / sum_Scope1 * 100);
                toCreate.percentage1_N2O = (sum1_N2O / sum_Scope1 * 100);
                toCreate.percentage1_HFCS = (sum1_HFCS / sum_Scope1 * 100);
                toCreate.percentage1_PFCS = (sum1_PFCS / sum_Scope1 * 100);
                toCreate.percentage1_SF6 = (sum1_SF6 / sum_Scope1 * 100);
                toCreate.percentage1_NF3 = (sum1_NF3 / sum_Scope1 * 100);

                toCreate.percentage2_CO2 = (CO2 / sum_all * 100);
                toCreate.percentage2_CH4 = (CH4 / sum_all * 100);
                toCreate.percentage2_N2O = (N2O / sum_all * 100);
                toCreate.percentage2_HFCS = (HFCS / sum_all * 100);
                toCreate.percentage2_PFCS = (PFCS / sum_all * 100);
                toCreate.percentage2_SF6 = (SF6 / sum_all * 100);
                toCreate.percentage2_NF3 = (NF3 / sum_all * 100);

                toCreate.percentage_nonMove = (sum_hardlymove / sum_all * 100);
                toCreate.percentage_Move = (sum_move / sum_all * 100);
                toCreate.percentage_Escape = (sum_escape / sum_all * 100);
                toCreate.percentage_Process = (sum_process / sum_all * 100);
                toCreate.percentage_Scope1 = (sum_Scope1 / sum_all * 100);
                toCreate.percentage_Scope2 = (sum_Scope2 / sum_all * 100);

                toCreate.cal_all = sum_Uncertainty;
                toCreate.no1_Grade = no1_Grade;
                toCreate.no2_Grade = no2_Grade;
                toCreate.no3_Grade = no3_Grade;
                toCreate.avg_Grade = avg_Grade;
                toCreate.all_Grade = avg_Grade < 10 ? "第一級" : (avg_Grade < 19 ? "第二級" : "第三級");
                toCreate.percentage_CalAll = (sum_Uncertainty / sum_all * 100);
                toCreate.ULL = all_ULL;
                toCreate.UUL = all_UUL;
                _context.Add(toCreate);
            }

            await _context.SaveChangesAsync();
            return toCreate;
        }

        public static decimal DecimalSqrt(decimal value, int iterations = 20) //用牛頓法逼近Decimal的平方根
        {
            if (value < 0)
            {
                throw new ArgumentException("不能計算負數的平方根");
            }

            if (value != 0)
            {
                decimal guess = value / 2;
                for (int i = 0; i < iterations; i++)
                {
                    guess = 0.5m * (guess + value / guess);
                }

                return guess;
            }
            else
            {
                return 0;
            }

        }

        //public async Task<IActionResult> WordAsync(Guid id)
        //{
        //    await CountEmissionAsync(id);
        //    // 這裡要替換成你 MVC 應用程式中正確的檔案路徑
        //    var data = await _context.Areas.Where(x => x.Id == id).Include(x => x.Company).FirstOrDefaultAsync();
        //    var device = await _context.Devices.Where(x => x.YearId == id && x.isDeleted == 0).OrderBy(x => x.Scope).ThenBy(x => x.EmissionPattern).ToListAsync();
        //    var scope1_device = await _context.Devices.Where(x => x.YearId == id && x.isDeleted == 0 && x.Scope != "類別二").OrderBy(x => x.Scope).ThenBy(x => x.EmissionPattern).ToListAsync();
        //    var Material = await _context.Materials.ToListAsync();
        //    var emission = await _context.Years.Where(x => x.AreaId == id).FirstOrDefaultAsync();
        //    var nonMove = device.Where(x => x.EmissionPattern == "固定" && x.isDeleted == 0).Select(d => d.Name).ToList();
        //    var move = device.Where(x => x.EmissionPattern == "移動" && x.isDeleted == 0).Where(x => x.isDeleted == 0).Select(d => d.Name).ToList();
        //    var escape = device.Where(x => x.EmissionPattern == "逸散" && x.isDeleted == 0).Select(d => d.Name).ToList();
        //    var process = device.Where(x => x.EmissionPattern == "製程" && x.isDeleted == 0).Select(d => d.Name).ToList();



        //    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\doc\\", "溫盤報告書範本.docx");
        //    string newFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\doc\\", TempData.Peek("year").ToString() + TempData.Peek("companyName").ToString() + "-溫室氣體盤查報告書.docx");

        //    // 複製文件
        //    using (DocX doc = DocX.Load(filePath))
        //    {

        //        List<Xceed.Document.NET.Paragraph> paragraphsToUpdate = new List<Xceed.Document.NET.Paragraph>();

        //        // 設定要查找和替換的文本
        //        foreach (var paragraph in doc.Paragraphs)
        //        {
        //            if (paragraph.Text.Contains("類別表補充"))
        //            {
        //                paragraphsToUpdate.Add(paragraph);
        //            }

        //            if (paragraph.Text.Contains("類別一表補充"))
        //            {
        //                paragraphsToUpdate.Add(paragraph);
        //            }

        //            if (paragraph.Text.Contains("排放源活動數據表替換"))
        //            {
        //                paragraphsToUpdate.Add(paragraph);
        //            }

        //            if (paragraph.Text.Contains("排放係數表替換"))
        //            {
        //                paragraphsToUpdate.Add(paragraph);
        //            }

        //            paragraph.ReplaceText("補充公司基本資料", data.Company.Information);
        //            paragraph.ReplaceText("補充盤查年度", (data.Year + 1911).ToString() + "年");
        //            paragraph.ReplaceText("民國年份補充", (data.Year).ToString());
        //            paragraph.ReplaceText("補充公司場所名稱", data.Company.Name);
        //            paragraph.ReplaceText("補充公司場所英文名稱", data.Company.EnglishName);
        //            paragraph.ReplaceText("補充公司場所簡稱", data.Company.EasyName);
        //            paragraph.ReplaceText("補充公司場所英文簡稱", data.Company.EasyEnglishName);
        //            paragraph.ReplaceText("補充統一編號", data.UniqueCode.ToString());
        //            paragraph.ReplaceText("補充工廠登記編號", data.FactorCode.ToString());
        //            paragraph.ReplaceText("補充地址", data.City + data.District + data.Address);
        //            if (nonMove.Count != 0)
        //            {
        //                paragraph.ReplaceText("組織邊界的各據點內所擁有的固定式化石燃料燃燒排放源。", "組織邊界的各據點內所擁有的固定式化石燃料燃燒排放源，固定排放源包含" + string.Join(", ", nonMove) + "。");
        //            }
        //            if (move.Count != 0)
        //            {
        //                paragraph.ReplaceText("組織邊界的各據點內所擁有的可移動且燃燒化石燃料的排放源。", "組織邊界的各據點內所擁有的可移動且燃燒化石燃料的排放源，移動排放源包含" + string.Join(", ", move) + "。");
        //            }
        //            if (escape.Count != 0)
        //            {
        //                paragraph.ReplaceText("組織邊界的各據點內所擁有的人為逸散溫室氣體排放源。", "組織邊界的各據點內所擁有的人為逸散溫室氣體排放源，逸散源包含" + string.Join(", ", escape) + "。");
        //            }
        //            if (process.Count != 0)
        //            {
        //                paragraph.ReplaceText("組織邊界內在製程中化學反應產生的溫室氣體排放源。", "組織邊界內在製程中化學反應產生的溫室氣體排放源，如" + string.Join(", ", process) + "。");
        //            }
        //            paragraph.ReplaceText("基準年補充", (data.Year + 1911).ToString() + "年");
        //            paragraph.ReplaceText("類別一CO2排放", emission.Scope1_CO2.ToString());
        //            paragraph.ReplaceText("CO2排放", emission.CO2.ToString());
        //            paragraph.ReplaceText("CH4排放", emission.CH4.ToString());
        //            paragraph.ReplaceText("N2O排放", emission.N2O.ToString());
        //            paragraph.ReplaceText("HFCS排放", emission.HFCS.ToString());
        //            paragraph.ReplaceText("PFCS排放", emission.PFCS.ToString());
        //            paragraph.ReplaceText("SF6排放", emission.SF6.ToString());
        //            paragraph.ReplaceText("NF3排放", emission.NF3.ToString());
        //            paragraph.ReplaceText("類別一CO2占比", emission.percentage1_CO2.ToString());
        //            paragraph.ReplaceText("類別一CH4占比", emission.percentage1_CH4.ToString());
        //            paragraph.ReplaceText("類別一N2O占比", emission.percentage1_N2O.ToString());
        //            paragraph.ReplaceText("類別一HFCS占比", emission.percentage1_HFCS.ToString());
        //            paragraph.ReplaceText("類別一PFCS占比", emission.percentage1_PFCS.ToString());
        //            paragraph.ReplaceText("類別一SF6占比", emission.percentage1_SF6.ToString());
        //            paragraph.ReplaceText("類別一NF3占比", emission.percentage2_NF3.ToString());
        //            paragraph.ReplaceText("類別一CO2排放", emission.Scope1.ToString());
        //            paragraph.ReplaceText("CO2占比", emission.percentage2_CO2.ToString());
        //            paragraph.ReplaceText("CH4占比", emission.percentage2_CH4.ToString());
        //            paragraph.ReplaceText("N2O占比", emission.percentage2_N2O.ToString());
        //            paragraph.ReplaceText("HFCS占比", emission.percentage2_HFCS.ToString());
        //            paragraph.ReplaceText("PFCS占比", emission.percentage2_PFCS.ToString());
        //            paragraph.ReplaceText("SF6占比", emission.percentage2_SF6.ToString());
        //            paragraph.ReplaceText("NF3占比", emission.percentage2_NF3.ToString());
        //            paragraph.ReplaceText("總排放當量", emission.All.ToString());
        //            paragraph.ReplaceText("固定排放量", emission.non_move.ToString());
        //            paragraph.ReplaceText("移動排放量", emission.move.ToString());
        //            paragraph.ReplaceText("製程排放量", emission.process.ToString());
        //            paragraph.ReplaceText("逸散排放量", emission.escape.ToString());
        //            paragraph.ReplaceText("固定排放比例", emission.percentage_nonMove.ToString());
        //            paragraph.ReplaceText("製程排放比例", emission.percentage_Process.ToString());
        //            paragraph.ReplaceText("移動排放比例", emission.percentage_Move.ToString());
        //            paragraph.ReplaceText("逸散排放比例", emission.percentage_Escape.ToString());
        //            paragraph.ReplaceText("類別一占比", emission.percentage_Scope1.ToString());
        //            paragraph.ReplaceText("類別二占比", emission.percentage_Scope2.ToString());
        //            paragraph.ReplaceText("類別一總排放", emission.Scope1.ToString());
        //            paragraph.ReplaceText("類別二總排放", emission.Scope2.ToString());
        //            paragraph.ReplaceText("進行評估排放當量", emission.cal_all.ToString());
        //            paragraph.ReplaceText("不確定性評估占比", emission.percentage_CalAll.ToString());
        //            paragraph.ReplaceText("第1級評分", emission.no1_Grade.ToString());
        //            paragraph.ReplaceText("第2級評分", emission.no2_Grade.ToString());
        //            paragraph.ReplaceText("第3級評分", emission.no3_Grade.ToString());
        //            paragraph.ReplaceText("清冊等級分數補充", emission.avg_Grade.ToString());
        //            paragraph.ReplaceText("清冊級別補充", emission.all_Grade.ToString());
        //            paragraph.ReplaceText("95上", emission.UUL.ToString());
        //            paragraph.ReplaceText("95下", emission.ULL.ToString());
        //            paragraph.ReplaceText("補充姓名", data.Company.Owner);
        //            paragraph.ReplaceText("補充電話", data.Company.Phone);
        //            paragraph.ReplaceText("補充電子信箱", data.Company.Email);
        //        }

        //        foreach (var paragraph in paragraphsToUpdate) //類別表補充
        //        {
        //            Xceed.Document.NET.Table table = doc.AddTable(device.Count() + 1, 4);
        //            // 填充表格標題
        //            table.Rows[0].Cells[0].Paragraphs.First().Append("類別");
        //            table.Rows[0].Cells[1].Paragraphs.First().Append("型式");
        //            table.Rows[0].Cells[2].Paragraphs.First().Append("排放源");
        //            table.Rows[0].Cells[3].Paragraphs.First().Append("產生之溫室氣體");

        //            for (int x = 0; x < device.Count(); x++)
        //            {
        //                table.Rows[x + 1].Cells[0].Paragraphs.First().Append(device[x].Scope);
        //                table.Rows[x + 1].Cells[1].Paragraphs.First().Append(device[x].EmissionPattern);
        //                table.Rows[x + 1].Cells[2].Paragraphs.First().Append(device[x].Name + "(" + device[x].Material + ")");
        //                if (device[x].CO2_Emission == true && device[x].CH4_Emission == true && device[x].N2O_Emission == true)
        //                {
        //                    table.Rows[x + 1].Cells[3].Paragraphs.First().Append("CO₂、CH₄、N₂O");
        //                }
        //                else if (device[x].CO2_Emission == true)
        //                {
        //                    table.Rows[x + 1].Cells[3].Paragraphs.First().Append("CO₂");
        //                }
        //                else if (device[x].HFCS_Emission == true)
        //                {
        //                    table.Rows[x + 1].Cells[3].Paragraphs.First().Append("HFCs");
        //                }
        //                else if (device[x].CH4_Emission == true)
        //                {
        //                    table.Rows[x + 1].Cells[3].Paragraphs.First().Append("CH₄");
        //                }
        //            }
        //            paragraph.ReplaceTextWithObject("類別表補充", table);
        //        }

        //        foreach (var paragraph in paragraphsToUpdate) //類別一表補充
        //        {
        //            Xceed.Document.NET.Table table = doc.AddTable(scope1_device.Count() + 1, 4);
        //            table.SetWidths(new float[] { 100, 150, 200, 100 });

        //            // 填充表格標題
        //            table.Rows[0].Cells[0].Paragraphs.First().Append("類別");
        //            table.Rows[0].Cells[1].Paragraphs.First().Append("型式");
        //            table.Rows[0].Cells[2].Paragraphs.First().Append("排放源");
        //            table.Rows[0].Cells[3].Paragraphs.First().Append("產生之溫室氣體");

        //            for (int x = 0; x < scope1_device.Count(); x++)
        //            {
        //                table.Rows[x + 1].Cells[0].Paragraphs.First().Append(scope1_device[x].Scope);
        //                table.Rows[x + 1].Cells[1].Paragraphs.First().Append(scope1_device[x].EmissionPattern);
        //                table.Rows[x + 1].Cells[2].Paragraphs.First().Append(scope1_device[x].Name + "(" + scope1_device[x].Material + ")");
        //                if (scope1_device[x].CO2_Emission == true && scope1_device[x].CH4_Emission == true && scope1_device[x].N2O_Emission == true)
        //                {
        //                    table.Rows[x + 1].Cells[3].Paragraphs.First().Append("CO₂、CH₄、N₂O");
        //                }
        //                else if (scope1_device[x].CO2_Emission == true)
        //                {
        //                    table.Rows[x + 1].Cells[3].Paragraphs.First().Append("CO₂");
        //                }
        //                else if (scope1_device[x].HFCS_Emission == true)
        //                {
        //                    table.Rows[x + 1].Cells[3].Paragraphs.First().Append("HFCs");
        //                }
        //                else if (scope1_device[x].CH4_Emission == true)
        //                {
        //                    table.Rows[x + 1].Cells[3].Paragraphs.First().Append("CH₄");
        //                }
        //            }
        //            paragraph.ReplaceTextWithObject("類別一表補充", table);
        //        }

        //        foreach (var paragraph in paragraphsToUpdate) //排放源活動數據表替換
        //        {
        //            Xceed.Document.NET.Table table = doc.AddTable(device.Count() + 1, 5);
        //            //table.SetWidths(new float[] { 100, 150, 200, 100 });

        //            // 填充表格標題
        //            table.Rows[0].Cells[0].Paragraphs.First().Append("類別");
        //            table.Rows[0].Cells[1].Paragraphs.First().Append("排放型式");
        //            table.Rows[0].Cells[2].Paragraphs.First().Append("原燃物料");
        //            table.Rows[0].Cells[3].Paragraphs.First().Append("活動數據");
        //            table.Rows[0].Cells[4].Paragraphs.First().Append("單位");


        //            for (int x = 0; x < device.Count(); x++)
        //            {
        //                table.Rows[x + 1].Cells[0].Paragraphs.First().Append(device[x].Scope);
        //                table.Rows[x + 1].Cells[1].Paragraphs.First().Append(device[x].EmissionPattern);
        //                table.Rows[x + 1].Cells[2].Paragraphs.First().Append(device[x].Material + "(" + device[x].Name + ")");
        //                table.Rows[x + 1].Cells[3].Paragraphs.First().Append((device[x].Num / 1000).ToString("F4"));
        //                if (device[x].Unit == "人")
        //                {
        //                    table.Rows[x + 1].Cells[4].Paragraphs.First().Append(device[x].Unit);
        //                }
        //                else
        //                {
        //                    string displayUnit;
        //                    switch (device[x].Unit)
        //                    {
        //                        case "公斤":
        //                            displayUnit = "公噸";
        //                            break;
        //                        case "公升":
        //                            displayUnit = "公秉";
        //                            break;
        //                        case "立方公尺":
        //                            displayUnit = "千立方公尺";
        //                            break;
        //                        case "度":
        //                            displayUnit = "千度";
        //                            break;
        //                        default:
        //                            displayUnit = device[x].Unit;
        //                            break;
        //                    }
        //                    table.Rows[x + 1].Cells[4].Paragraphs.First().Append(displayUnit);
        //                }
        //            }
        //            paragraph.ReplaceTextWithObject("排放源活動數據表替換", table);
        //        }
        //        foreach (var paragraph in paragraphsToUpdate) //類別表補充
        //        {
        //            int nonMove_num = device.Where(x => x.EmissionPattern == "固定").Count();
        //            int move_num = device.Where(x => x.EmissionPattern == "移動").Count();
        //            int max = device.Count() - nonMove_num - move_num + nonMove_num * 3 + move_num * 3;
        //            Xceed.Document.NET.Table table = doc.AddTable(max + 1, 6);
        //            table.SetWidths(new float[] { 100, 150, 200, 100 });
        //            table.Design = TableDesign.TableGrid;
        //            table.AutoFit = AutoFit.Window;
        //            table.Alignment = Alignment.left;

        //            // 填充表格標題
        //            table.Rows[0].Cells[0].Paragraphs.First().Append("原燃物料");
        //            table.Rows[0].Cells[1].Paragraphs.First().Append("溫室氣體");
        //            table.Rows[0].Cells[2].Paragraphs.First().Append("排放係數");
        //            table.Rows[0].Cells[3].Paragraphs.First().Append("係數來源");
        //            table.Rows[0].Cells[4].Paragraphs.First().Append("係數單位");
        //            table.Rows[0].Cells[5].Paragraphs.First().Append("GWP(AR6)");

        //            //table.Paragraphs.First().Alignment = Alignment.left;
        //            //table.Paragraphs.

        //            //int x = 0; //格子
        //            int device_num = 0;

        //            for (int x = 0; x < max; x++) //格子
        //            {
        //                while (device_num < device.Count())
        //                {
        //                    string displayUnit;
        //                    switch (device[device_num].Unit)
        //                    {
        //                        case "公斤":
        //                            displayUnit = "公噸";
        //                            break;
        //                        case "公升":
        //                            displayUnit = "公秉";
        //                            break;
        //                        case "立方公尺":
        //                            displayUnit = "千立方公尺";
        //                            break;
        //                        case "度":
        //                            displayUnit = "千度";
        //                            break;
        //                        default:
        //                            displayUnit = device[device_num].Unit;
        //                            break;
        //                    }

        //                    // CO2, CH4, N2O 三種溫室氣體排放
        //                    if (device[device_num].EmissionPattern == "固定" || device[device_num].EmissionPattern == "移動")
        //                    {
        //                        for (int num = 1; num <= 3; num++)
        //                        {
        //                            table.Rows[x + 1].Cells[0].Paragraphs.First().Append(device[device_num].Material + "(" + device[device_num].EmissionPattern + ")");
        //                            table.Rows[x + 1].Cells[1].Paragraphs.First().Append(num == 1 ? "CO₂" : (num == 2 ? "CH₄" : "N₂O"));

        //                            string gasType = num == 1 ? "CO2" : (num == 2 ? "CH4" : "N2O");

        //                            table.Rows[x + 1].Cells[2].Paragraphs.First().Append(
        //                                gasType == "CO2" ? _context.Materials.FirstOrDefault(m => m.Name == device[device_num].Material)?.CO2CEF.ToString("F10") :
        //                                gasType == "CH4" ? _context.Materials.FirstOrDefault(m => m.Name == device[device_num].Material)?.CH4CEF.ToString("F10") :
        //                                gasType == "N2O" ? _context.Materials.FirstOrDefault(m => m.Name == device[device_num].Material)?.N2OCEF.ToString("F10") :
        //                                "");

        //                            table.Rows[x + 1].Cells[3].Paragraphs.First().Append("溫室氣體排放係數管理表 6.0.4 版");
        //                            table.Rows[x + 1].Cells[4].Paragraphs.First().Append("公噸/" + displayUnit);
        //                            table.Rows[x + 1].Cells[5].Paragraphs.First().Append(
        //                                gasType == "CO2" ? _context.GWPs.FirstOrDefault(x => x.Name == "CO2").Num.ToString() :
        //                                gasType == "CH4" ? _context.GWPs.FirstOrDefault(x => x.Name == "CH4").Num.ToString() :
        //                                _context.GWPs.FirstOrDefault(x => x.Name == "N2O").Num.ToString());

        //                            if (num < 3)
        //                            {
        //                                x++; //下一格
        //                            }
        //                        }

        //                    }
        //                    else
        //                    {
        //                        // 其他情況
        //                        string gasType;
        //                        if (_context.Materials.FirstOrDefault(m => m.Name == device[device_num].Material) != null)
        //                        {
        //                            if (_context.Materials.FirstOrDefault(m => m.Name == device[device_num].Material)?.CO2CEF != 0)
        //                            {
        //                                gasType = "CO2";
        //                            }
        //                            else if (_context.Materials.FirstOrDefault(m => m.Name == device[device_num].Material)?.CH4CEF != 0)
        //                            {
        //                                gasType = "CH4";
        //                            }
        //                            else
        //                            {
        //                                // 可能有其他情況需要處理
        //                                gasType = "其他";
        //                            }
        //                        }
        //                        else
        //                        {
        //                            gasType = "HFCs";
        //                        }


        //                        table.Rows[x + 1].Cells[0].Paragraphs.First().Append(device[device_num].Material + "(" + device[device_num].EmissionPattern + ")");
        //                        table.Rows[x + 1].Cells[1].Paragraphs.First().Append(gasType);
        //                        table.Rows[x + 1].Cells[2].Paragraphs.First().Append(
        //                                gasType == "HFCs" ? _context.Materials.FirstOrDefault(m => m.Name == device[device_num].Name)?.HFCSCEF.ToString() :
        //                                gasType == "CO2" ? _context.Materials.FirstOrDefault(m => m.Name == device[device_num].Material)?.CO2CEF.ToString() :
        //                                gasType == "CH4" ? _context.Materials.FirstOrDefault(m => m.Name == device[device_num].Material)?.CH4CEF.ToString() :
        //                                gasType == "N2O" ? _context.Materials.FirstOrDefault(m => m.Name == device[device_num].Material)?.N2OCEF.ToString() :
        //                                ""); //增加其他

        //                        table.Rows[x + 1].Cells[3].Paragraphs.First().Append("溫室氣體排放係數管理表 6.0.4 版");
        //                        table.Rows[x + 1].Cells[4].Paragraphs.First().Append("公噸/" + displayUnit);
        //                        table.Rows[x + 1].Cells[5].Paragraphs.First().Append(!string.IsNullOrEmpty(gasType) ?
        //                                                        (gasType == "HFCs" ? _context.GWPs.FirstOrDefault(x => x.Name == device[device_num].Material)?.Num.ToString() :
        //                                                                             _context.GWPs.FirstOrDefault(x => x.Name == gasType)?.Num.ToString()) : "0");

        //                    }

        //                    x++;
        //                    device_num++;
        //                }
        //            }

        //            paragraph.ReplaceTextWithObject("排放係數表替換", table);
        //        }

        //        // 保存新文檔
        //        doc.SaveAs(newFilePath);
        //    }
        //    // 返回一個視圖或其他操作，根據你的需求
        //    var fileBytes = System.IO.File.ReadAllBytes(newFilePath);
        //    var fileName = Year + Name + "-溫室氣體盤查報告書.docx"; // 可以自行定義檔名
        //    return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
        //}

        public async Task<IActionResult> Word3Async(Guid id)
        {
            await CountEmissionAsync(id);
            // 這裡要替換成你 MVC 應用程式中正確的檔案路徑
            var data = await _context.Years.Where(x => x.Id == id && x.isDeleted == 0).Include(x => x.Area).ThenInclude(x => x.Company).FirstOrDefaultAsync();
            var device = await _context.Devices.Where(x => x.YearId == id && x.isDeleted == 0).OrderBy(x => x.Scope).ThenBy(x => x.EmissionPattern).ToListAsync();
            var ManyGHGs = await _context.Devices.Where(x => x.isDeleted == 0).SelectMany(x => x.GHGs).ToListAsync();

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\doc\\", "溫盤報告書範本3.docx");
            string newFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\output\\", data.Num + "-" + data.Area.Name + "-" + data.Area.Company.Name + "-溫室氣體盤查報告書.docx");
            string Scope1 = "";
            int i = 0;
            foreach (var item in device)
            {

                if (item.Scope == "類別一")
                {
                    i++;
                    Scope1 += i + "." + item.Name + "(" + item.Material + ")" + "\n";
                }
            }

            // 複製文件
            using (DocX doc = DocX.Load(filePath))
            {

                List<Xceed.Document.NET.Paragraph> paragraphsToUpdate = new List<Xceed.Document.NET.Paragraph>();

                // 設定要查找和替換的文本
                foreach (var paragraph in doc.Paragraphs)
                {
                    paragraph.ReplaceText("[直接排放源]", Scope1.Trim());
                    paragraph.ReplaceText("[公司中文名稱]", data.Area.Company.Name);
                    paragraph.ReplaceText("[公司基本資料]", data.Area.Company.Information);
                    paragraph.ReplaceText("[西元盤查年度]", (data.Num + 1911).ToString());
                    paragraph.ReplaceText("[民國盤查年份]", (data.Num).ToString());
                    paragraph.ReplaceText("[廠區名稱]", data.Area.Name.ToString());

                    paragraph.ReplaceText("[盤查月]", DateTime.Now.Month.ToString());
                    paragraph.ReplaceText("[盤查日]", DateTime.Now.Day.ToString());
                    paragraph.ReplaceText("[地址]", data.Area.FullAddress);
                    paragraph.ReplaceText("[民國基準年]", data.Area.Year.ToString());

                    paragraph.ReplaceText("[電力使用量]", (device.Find(x => x.Name == "電力").Num / 1000).ToString());
                    paragraph.ReplaceText("[類別一CO2排放]", data.Scope1_CO2.ToString());
                    paragraph.ReplaceText("[類別一CH4排放]", data.Scope1_CH4.ToString());
                    paragraph.ReplaceText("[類別一N2O排放]", data.Scope1_N2O.ToString());
                    paragraph.ReplaceText("[類別一HFCS排放]", data.Scope1_HFCS.ToString());
                    paragraph.ReplaceText("[類別一PFCS排放]", data.Scope1_PFCS.ToString());
                    paragraph.ReplaceText("[類別一SF6排放]", data.Scope1_SF6.ToString());
                    paragraph.ReplaceText("[類別一NF3排放]", data.Scope1_NF3.ToString());

                    paragraph.ReplaceText("[類別一CO2占比]", data.percentage1_CO2.ToString() + "%");
                    paragraph.ReplaceText("[類別一CH4占比]", data.percentage1_CH4.ToString() + "%");
                    paragraph.ReplaceText("[類別一N2O占比]", data.percentage1_N2O.ToString() + "%");
                    paragraph.ReplaceText("[類別一HFCS占比]", data.percentage1_HFCS.ToString() + "%");
                    paragraph.ReplaceText("[類別一PFCS占比]", data.percentage1_PFCS.ToString() + "%");
                    paragraph.ReplaceText("[類別一SF6占比]", data.percentage1_SF6.ToString() + "%");
                    paragraph.ReplaceText("[類別一NF3占比]", data.percentage2_NF3.ToString() + "%");

                    paragraph.ReplaceText("[CO2排放]", data.CO2.ToString());
                    paragraph.ReplaceText("[CH4排放]", data.CH4.ToString());
                    paragraph.ReplaceText("[N2O排放]", data.N2O.ToString());
                    paragraph.ReplaceText("[HFCS排放]", data.HFCS.ToString());
                    paragraph.ReplaceText("[PFCS排放]", data.PFCS.ToString());
                    paragraph.ReplaceText("[SF6排放]", data.SF6.ToString());
                    paragraph.ReplaceText("[NF3排放]", data.NF3.ToString());

                    paragraph.ReplaceText("[CO2占比]", data.percentage2_CO2.ToString() + "%");
                    paragraph.ReplaceText("[CH4占比]", data.percentage2_CH4.ToString() + "%");
                    paragraph.ReplaceText("[N2O占比]", data.percentage2_N2O.ToString() + "%");
                    paragraph.ReplaceText("[HFCS占比]", data.percentage2_HFCS.ToString() + "%");
                    paragraph.ReplaceText("[PFCS占比]", data.percentage2_PFCS.ToString() + "%");
                    paragraph.ReplaceText("[SF6占比]", data.percentage2_SF6.ToString() + "%");
                    paragraph.ReplaceText("[NF3占比]", data.percentage2_NF3.ToString() + "%");
                    paragraph.ReplaceText("[總排放當量]", data.All.ToString());
                    paragraph.ReplaceText("[固定排放量]", data.non_move.ToString());
                    paragraph.ReplaceText("[移動排放量]", data.move.ToString());
                    paragraph.ReplaceText("[製程排放量]", data.process.ToString());
                    paragraph.ReplaceText("[逸散排放量]", data.escape.ToString());
                    paragraph.ReplaceText("[固定排放比例]", data.percentage_nonMove.ToString() + "%");
                    paragraph.ReplaceText("[製程排放比例]", data.percentage_Process.ToString() + "%");
                    paragraph.ReplaceText("[移動排放比例]", data.percentage_Move.ToString() + "%");
                    paragraph.ReplaceText("[逸散排放比例]", data.percentage_Escape.ToString() + "%");
                    paragraph.ReplaceText("[類別一占比]", data.percentage_Scope1.ToString() + "%");
                    paragraph.ReplaceText("[類別二占比]", data.percentage_Scope2.ToString() + "%");
                    paragraph.ReplaceText("[類別一總排放]", data.Scope1.ToString());
                    paragraph.ReplaceText("[類別二總排放]", data.Scope2.ToString());
                }
                //--------------------------------固定排放源溫室氣體表表
                int nonMove_Num = device.Where(x => x.EmissionPattern == "固定").Count();
                Xceed.Document.NET.Table nonMove_C2Otable = doc.AddTable(nonMove_Num + 1, 9);
                nonMove_C2Otable.Rows[0].Cells[0].Paragraphs.First().Append("類別");
                nonMove_C2Otable.Rows[0].Cells[1].Paragraphs.First().Append("排放型式");
                nonMove_C2Otable.Rows[0].Cells[2].Paragraphs.First().Append("活動數據");
                nonMove_C2Otable.Rows[0].Cells[3].Paragraphs.First().Append("溫室氣體");
                nonMove_C2Otable.Rows[0].Cells[4].Paragraphs.First().Append("排放係數");
                nonMove_C2Otable.Rows[0].Cells[5].Paragraphs.First().Append("係數來源");
                nonMove_C2Otable.Rows[0].Cells[6].Paragraphs.First().Append("排放量\n(公噸/年)");
                nonMove_C2Otable.Rows[0].Cells[7].Paragraphs.First().Append("GWP");
                nonMove_C2Otable.Rows[0].Cells[8].Paragraphs.First().Append("排放當量\n(公噸CO2e/年)\n");
                int nonMoveCO2_rowIndex = 1;
                Xceed.Document.NET.Table nonMove_CH4table = doc.AddTable(nonMove_Num + 1, 9);
                nonMove_CH4table.Rows[0].Cells[0].Paragraphs.First().Append("類別");
                nonMove_CH4table.Rows[0].Cells[1].Paragraphs.First().Append("排放型式");
                nonMove_CH4table.Rows[0].Cells[2].Paragraphs.First().Append("活動數據");
                nonMove_CH4table.Rows[0].Cells[3].Paragraphs.First().Append("溫室氣體");
                nonMove_CH4table.Rows[0].Cells[4].Paragraphs.First().Append("排放係數");
                nonMove_CH4table.Rows[0].Cells[5].Paragraphs.First().Append("係數來源");
                nonMove_CH4table.Rows[0].Cells[6].Paragraphs.First().Append("排放量\n(公噸/年)");
                nonMove_CH4table.Rows[0].Cells[7].Paragraphs.First().Append("GWP");
                nonMove_CH4table.Rows[0].Cells[8].Paragraphs.First().Append("排放當量\n(公噸CO2e/年)\n");
                int nonMoveCH4_rowIndex = 1;
                Xceed.Document.NET.Table nonMove_N2Otable = doc.AddTable(nonMove_Num + 1, 9);
                nonMove_N2Otable.Rows[0].Cells[0].Paragraphs.First().Append("類別");
                nonMove_N2Otable.Rows[0].Cells[1].Paragraphs.First().Append("排放型式");
                nonMove_N2Otable.Rows[0].Cells[2].Paragraphs.First().Append("活動數據");
                nonMove_N2Otable.Rows[0].Cells[3].Paragraphs.First().Append("溫室氣體");
                nonMove_N2Otable.Rows[0].Cells[4].Paragraphs.First().Append("排放係數");
                nonMove_N2Otable.Rows[0].Cells[5].Paragraphs.First().Append("係數來源");
                nonMove_N2Otable.Rows[0].Cells[6].Paragraphs.First().Append("排放量\n(公噸/年)");
                nonMove_N2Otable.Rows[0].Cells[7].Paragraphs.First().Append("GWP");
                nonMove_N2Otable.Rows[0].Cells[8].Paragraphs.First().Append("排放當量\n(公噸CO2e/年)\n");
                int nonMoveN2O_rowIndex = 1;
                foreach (var item in device.Where(x => x.EmissionPattern == "固定"))
                {
                    var GHGs = ManyGHGs.Where(x => x.DeviceId == item.Id).ToList();
                    foreach (var GHG in GHGs)
                    {
                        switch (GHG.Name)
                        {
                            case "CO2":
                                nonMove_C2Otable.Rows[nonMoveCO2_rowIndex].Cells[0].Paragraphs.First().Append(GHG.Device.Scope);
                                nonMove_C2Otable.Rows[nonMoveCO2_rowIndex].Cells[1].Paragraphs.First().Append(GHG.Device.EmissionPattern);
                                nonMove_C2Otable.Rows[nonMoveCO2_rowIndex].Cells[2].Paragraphs.First().Append(GHG.Device.Num.ToString() + GHG.Device.Unit);
                                nonMove_C2Otable.Rows[nonMoveCO2_rowIndex].Cells[3].Paragraphs.First().Append(GHG.Name); //可刪除
                                nonMove_C2Otable.Rows[nonMoveCO2_rowIndex].Cells[4].Paragraphs.First().Append(GHG.CEF.ToString() + "公噸/" + GHG.Device.Unit);
                                nonMove_C2Otable.Rows[nonMoveCO2_rowIndex].Cells[5].Paragraphs.First().Append("係數來源"); //待改
                                nonMove_C2Otable.Rows[nonMoveCO2_rowIndex].Cells[6].Paragraphs.First().Append("排放量\n(公噸/年)"); //待改
                                nonMove_C2Otable.Rows[nonMoveCO2_rowIndex].Cells[7].Paragraphs.First().Append(GHG.GWP.ToString());
                                nonMove_C2Otable.Rows[nonMoveCO2_rowIndex].Cells[8].Paragraphs.First().Append(GHG.Emission.ToString());
                                nonMoveCO2_rowIndex++;
                                break;
                            case "CH4":

                                nonMove_CH4table.Rows[nonMoveCH4_rowIndex].Cells[0].Paragraphs.First().Append(GHG.Device.Scope);
                                nonMove_CH4table.Rows[nonMoveCH4_rowIndex].Cells[1].Paragraphs.First().Append(GHG.Device.EmissionPattern);
                                nonMove_CH4table.Rows[nonMoveCH4_rowIndex].Cells[2].Paragraphs.First().Append(GHG.Device.Num.ToString() + GHG.Device.Unit);
                                nonMove_CH4table.Rows[nonMoveCH4_rowIndex].Cells[3].Paragraphs.First().Append(GHG.Name); //可刪除
                                nonMove_CH4table.Rows[nonMoveCH4_rowIndex].Cells[4].Paragraphs.First().Append(GHG.CEF.ToString() + "公噸/" + GHG.Device.Unit);
                                nonMove_CH4table.Rows[nonMoveCH4_rowIndex].Cells[5].Paragraphs.First().Append("係數來源"); //待改
                                nonMove_CH4table.Rows[nonMoveCH4_rowIndex].Cells[6].Paragraphs.First().Append("排放量\n(公噸/年)"); //待改
                                nonMove_CH4table.Rows[nonMoveCH4_rowIndex].Cells[7].Paragraphs.First().Append(GHG.GWP.ToString());
                                nonMove_CH4table.Rows[nonMoveCH4_rowIndex].Cells[8].Paragraphs.First().Append(GHG.Emission.ToString());
                                nonMoveCH4_rowIndex++;
                                break;
                            case "N2O":

                                nonMove_N2Otable.Rows[nonMoveN2O_rowIndex].Cells[0].Paragraphs.First().Append(GHG.Device.Scope);
                                nonMove_N2Otable.Rows[nonMoveN2O_rowIndex].Cells[1].Paragraphs.First().Append(GHG.Device.EmissionPattern);
                                nonMove_N2Otable.Rows[nonMoveN2O_rowIndex].Cells[2].Paragraphs.First().Append(GHG.Device.Num.ToString() + GHG.Device.Unit);
                                nonMove_N2Otable.Rows[nonMoveN2O_rowIndex].Cells[3].Paragraphs.First().Append(GHG.Name); //可刪除
                                nonMove_N2Otable.Rows[nonMoveN2O_rowIndex].Cells[4].Paragraphs.First().Append(GHG.CEF.ToString() + "公噸/" + GHG.Device.Unit);
                                nonMove_N2Otable.Rows[nonMoveN2O_rowIndex].Cells[5].Paragraphs.First().Append("係數來源"); //待改
                                nonMove_N2Otable.Rows[nonMoveN2O_rowIndex].Cells[6].Paragraphs.First().Append("排放量\n(公噸/年)"); //待改
                                nonMove_N2Otable.Rows[nonMoveN2O_rowIndex].Cells[7].Paragraphs.First().Append(GHG.GWP.ToString());
                                nonMove_N2Otable.Rows[nonMoveN2O_rowIndex].Cells[8].Paragraphs.First().Append(GHG.Emission.ToString());
                                nonMoveN2O_rowIndex++;
                                break;
                        }
                    }
                }
                doc.ReplaceTextWithObject("[固定CO2]", nonMove_C2Otable);
                doc.ReplaceTextWithObject("[固定CH4]", nonMove_CH4table);
                doc.ReplaceTextWithObject("[固定N2O]", nonMove_N2Otable);
                //--------------------------------固定排放源溫室氣體表表
                //--------------------------------移動排放源溫室氣體表表
                int Move_Num = device.Where(x => x.EmissionPattern == "移動").Count();
                Xceed.Document.NET.Table Move_CO2table = doc.AddTable(Move_Num + 1, 9);
                Move_CO2table.Rows[0].Cells[0].Paragraphs.First().Append("類別");
                Move_CO2table.Rows[0].Cells[1].Paragraphs.First().Append("排放型式");
                Move_CO2table.Rows[0].Cells[2].Paragraphs.First().Append("活動數據");
                Move_CO2table.Rows[0].Cells[3].Paragraphs.First().Append("溫室氣體");
                Move_CO2table.Rows[0].Cells[4].Paragraphs.First().Append("排放係數");
                Move_CO2table.Rows[0].Cells[5].Paragraphs.First().Append("係數來源");
                Move_CO2table.Rows[0].Cells[6].Paragraphs.First().Append("排放量\n(公噸/年)");
                Move_CO2table.Rows[0].Cells[7].Paragraphs.First().Append("GWP");
                Move_CO2table.Rows[0].Cells[8].Paragraphs.First().Append("排放當量\n(公噸CO2e/年)\n");
                int MoveCO2_rowIndex = 1;
                Xceed.Document.NET.Table Move_CH4table = doc.AddTable(Move_Num + 1, 9);
                Move_CH4table.Rows[0].Cells[0].Paragraphs.First().Append("類別");
                Move_CH4table.Rows[0].Cells[1].Paragraphs.First().Append("排放型式");
                Move_CH4table.Rows[0].Cells[2].Paragraphs.First().Append("活動數據");
                Move_CH4table.Rows[0].Cells[3].Paragraphs.First().Append("溫室氣體");
                Move_CH4table.Rows[0].Cells[4].Paragraphs.First().Append("排放係數");
                Move_CH4table.Rows[0].Cells[5].Paragraphs.First().Append("係數來源");
                Move_CH4table.Rows[0].Cells[6].Paragraphs.First().Append("排放量\n(公噸/年)");
                Move_CH4table.Rows[0].Cells[7].Paragraphs.First().Append("GWP");
                Move_CH4table.Rows[0].Cells[8].Paragraphs.First().Append("排放當量\n(公噸CO2e/年)\n");
                int MoveCH4_rowIndex = 1;
                Xceed.Document.NET.Table Move_N2Otable = doc.AddTable(Move_Num + 1, 9);
                Move_N2Otable.Rows[0].Cells[0].Paragraphs.First().Append("類別");
                Move_N2Otable.Rows[0].Cells[1].Paragraphs.First().Append("排放型式");
                Move_N2Otable.Rows[0].Cells[2].Paragraphs.First().Append("活動數據");
                Move_N2Otable.Rows[0].Cells[3].Paragraphs.First().Append("溫室氣體");
                Move_N2Otable.Rows[0].Cells[4].Paragraphs.First().Append("排放係數");
                Move_N2Otable.Rows[0].Cells[5].Paragraphs.First().Append("係數來源");
                Move_N2Otable.Rows[0].Cells[6].Paragraphs.First().Append("排放量\n(公噸/年)");
                Move_N2Otable.Rows[0].Cells[7].Paragraphs.First().Append("GWP");
                Move_N2Otable.Rows[0].Cells[8].Paragraphs.First().Append("排放當量\n(公噸CO2e/年)\n");
                int MoveN2O_rowIndex = 1;
                foreach (var item in device.Where(x => x.EmissionPattern == "移動"))
                {
                    var GHGs = ManyGHGs.Where(x => x.DeviceId == item.Id).ToList();
                    foreach (var GHG in GHGs)
                    {
                        switch (GHG.Name)
                        {
                            case "CO2":
                                Move_CO2table.Rows[MoveCO2_rowIndex].Cells[0].Paragraphs.First().Append(GHG.Device.Scope);
                                Move_CO2table.Rows[MoveCO2_rowIndex].Cells[1].Paragraphs.First().Append(GHG.Device.EmissionPattern);
                                Move_CO2table.Rows[MoveCO2_rowIndex].Cells[2].Paragraphs.First().Append(GHG.Device.Num.ToString() + GHG.Device.Unit);
                                Move_CO2table.Rows[MoveCO2_rowIndex].Cells[3].Paragraphs.First().Append(GHG.Name); //可刪除
                                Move_CO2table.Rows[MoveCO2_rowIndex].Cells[4].Paragraphs.First().Append(GHG.CEF.ToString() + "公噸/" + GHG.Device.Unit);
                                Move_CO2table.Rows[MoveCO2_rowIndex].Cells[5].Paragraphs.First().Append("係數來源"); //待改
                                Move_CO2table.Rows[MoveCO2_rowIndex].Cells[6].Paragraphs.First().Append("排放量\n(公噸/年)"); //待改
                                Move_CO2table.Rows[MoveCO2_rowIndex].Cells[7].Paragraphs.First().Append(GHG.GWP.ToString());
                                Move_CO2table.Rows[MoveCO2_rowIndex].Cells[8].Paragraphs.First().Append(GHG.Emission.ToString());
                                MoveCO2_rowIndex++;
                                break;
                            case "CH4":
                                Move_CH4table.Rows[MoveCH4_rowIndex].Cells[0].Paragraphs.First().Append(GHG.Device.Scope);
                                Move_CH4table.Rows[MoveCH4_rowIndex].Cells[1].Paragraphs.First().Append(GHG.Device.EmissionPattern);
                                Move_CH4table.Rows[MoveCH4_rowIndex].Cells[2].Paragraphs.First().Append(GHG.Device.Num.ToString() + GHG.Device.Unit);
                                Move_CH4table.Rows[MoveCH4_rowIndex].Cells[3].Paragraphs.First().Append(GHG.Name); //可刪除
                                Move_CH4table.Rows[MoveCH4_rowIndex].Cells[4].Paragraphs.First().Append(GHG.CEF.ToString() + "公噸/" + GHG.Device.Unit);
                                Move_CH4table.Rows[MoveCH4_rowIndex].Cells[5].Paragraphs.First().Append("係數來源"); //待改
                                Move_CH4table.Rows[MoveCH4_rowIndex].Cells[6].Paragraphs.First().Append("排放量\n(公噸/年)"); //待改
                                Move_CH4table.Rows[MoveCH4_rowIndex].Cells[7].Paragraphs.First().Append(GHG.GWP.ToString());
                                Move_CH4table.Rows[MoveCH4_rowIndex].Cells[8].Paragraphs.First().Append(GHG.Emission.ToString());
                                MoveCH4_rowIndex++;
                                break;
                            case "N2O":
                                Move_N2Otable.Rows[MoveN2O_rowIndex].Cells[0].Paragraphs.First().Append(GHG.Device.Scope);
                                Move_N2Otable.Rows[MoveN2O_rowIndex].Cells[1].Paragraphs.First().Append(GHG.Device.EmissionPattern);
                                Move_N2Otable.Rows[MoveN2O_rowIndex].Cells[2].Paragraphs.First().Append(GHG.Device.Num.ToString() + GHG.Device.Unit);
                                Move_N2Otable.Rows[MoveN2O_rowIndex].Cells[3].Paragraphs.First().Append(GHG.Name); //可刪除
                                Move_N2Otable.Rows[MoveN2O_rowIndex].Cells[4].Paragraphs.First().Append(GHG.CEF.ToString() + "公噸/" + GHG.Device.Unit);
                                Move_N2Otable.Rows[MoveN2O_rowIndex].Cells[5].Paragraphs.First().Append("係數來源"); //待改
                                Move_N2Otable.Rows[MoveN2O_rowIndex].Cells[6].Paragraphs.First().Append("排放量\n(公噸/年)"); //待改
                                Move_N2Otable.Rows[MoveN2O_rowIndex].Cells[7].Paragraphs.First().Append(GHG.GWP.ToString());
                                Move_N2Otable.Rows[MoveN2O_rowIndex].Cells[8].Paragraphs.First().Append(GHG.Emission.ToString());
                                MoveN2O_rowIndex++;
                                break;
                        }
                    }
                }
                doc.ReplaceTextWithObject("[移動CO2]", Move_CO2table);
                doc.ReplaceTextWithObject("[移動CH4]", Move_CH4table);
                doc.ReplaceTextWithObject("[移動N2O]", Move_N2Otable);
                //--------------------------------移動排放源溫室氣體表表
                //--------------------------------類別一表
                Xceed.Document.NET.Table Scope1_table = doc.AddTable(device.Where(x => x.Scope == "類別一").Count() + 1, 10); //需扣除電力一行，行數不須加一
                                                                                                                           // 填充表格標題
                Scope1_table.Rows[0].Cells[0].Paragraphs.First().Append("類別");
                Scope1_table.Rows[0].Cells[1].Paragraphs.First().Append("型式");
                Scope1_table.Rows[0].Cells[2].Paragraphs.First().Append("排放源");
                Scope1_table.Rows[0].Cells[3].Paragraphs.First().Append("CO₂");
                Scope1_table.Rows[0].Cells[4].Paragraphs.First().Append("CH₄");
                Scope1_table.Rows[0].Cells[5].Paragraphs.First().Append("N₂O");
                Scope1_table.Rows[0].Cells[6].Paragraphs.First().Append("HFCs");
                Scope1_table.Rows[0].Cells[7].Paragraphs.First().Append("PFCs");
                Scope1_table.Rows[0].Cells[8].Paragraphs.First().Append("SF₆");
                Scope1_table.Rows[0].Cells[9].Paragraphs.First().Append("NF₃");
                int Scope1_rowIndex = 1;
                foreach (var item in device.Where(x => x.Scope == "類別一"))
                {
                    var GHGs = _context.GHGs.Where(ghg => ghg.DeviceId == item.Id);
                    Scope1_table.Rows[Scope1_rowIndex].Cells[0].Paragraphs.First().Append(item.Scope);
                    Scope1_table.Rows[Scope1_rowIndex].Cells[1].Paragraphs.First().Append(item.EmissionPattern);
                    Scope1_table.Rows[Scope1_rowIndex].Cells[2].Paragraphs.First().Append(item.Name + "(" + item.Material + ")");
                    foreach (var GHG in GHGs)
                    {
                        switch (GHG.Name)
                        {
                            case "CO2":
                                Scope1_table.Rows[Scope1_rowIndex].Cells[3].Paragraphs.First().Append("v");
                                break;
                            case "CH4":
                                Scope1_table.Rows[Scope1_rowIndex].Cells[4].Paragraphs.First().Append("v");
                                break;
                            case "N2O":
                                Scope1_table.Rows[Scope1_rowIndex].Cells[5].Paragraphs.First().Append("v");
                                break;
                            case "HFCS":
                                Scope1_table.Rows[Scope1_rowIndex].Cells[6].Paragraphs.First().Append("v");
                                break;
                            case "PFCS":
                                Scope1_table.Rows[Scope1_rowIndex].Cells[7].Paragraphs.First().Append("v");
                                break;
                            case "SF6":
                                Scope1_table.Rows[Scope1_rowIndex].Cells[8].Paragraphs.First().Append("v");
                                break;
                            case "NF3":
                                Scope1_table.Rows[Scope1_rowIndex].Cells[9].Paragraphs.First().Append("v");
                                break;
                        }
                    }

                    Scope1_rowIndex++;
                }

                // 在 doc 中替換段落
                doc.ReplaceTextWithObject("[類別一表]", Scope1_table);
                //--------------------------------類別一表
                //--------------------------------類別二表
                Xceed.Document.NET.Table Scope2_table = doc.AddTable(device.Where(x => x.Scope == "類別二").Count() + 1, 10);
                // 填充表格標題
                Scope2_table.Rows[0].Cells[0].Paragraphs.First().Append("類別");
                Scope2_table.Rows[0].Cells[1].Paragraphs.First().Append("型式");
                Scope2_table.Rows[0].Cells[2].Paragraphs.First().Append("排放源");
                Scope2_table.Rows[0].Cells[3].Paragraphs.First().Append("CO₂");
                Scope2_table.Rows[0].Cells[4].Paragraphs.First().Append("CH₄");
                Scope2_table.Rows[0].Cells[5].Paragraphs.First().Append("N₂O");
                Scope2_table.Rows[0].Cells[6].Paragraphs.First().Append("HFCs");
                Scope2_table.Rows[0].Cells[7].Paragraphs.First().Append("PFCs");
                Scope2_table.Rows[0].Cells[8].Paragraphs.First().Append("SF₆");
                Scope2_table.Rows[0].Cells[9].Paragraphs.First().Append("NF₃");
                int Scope2_rowIndex = 1;
                foreach (var item in device.Where(x => x.Scope == "類別二"))
                {
                    var GHGs = _context.GHGs.Where(ghg => ghg.DeviceId == item.Id);
                    Scope2_table.Rows[Scope2_rowIndex].Cells[0].Paragraphs.First().Append(item.Scope);
                    Scope2_table.Rows[Scope2_rowIndex].Cells[1].Paragraphs.First().Append(item.EmissionPattern);
                    Scope2_table.Rows[Scope2_rowIndex].Cells[2].Paragraphs.First().Append(item.Name + "(" + item.Material + ")");
                    foreach (var GHG in GHGs)
                    {
                        switch (GHG.Name)
                        {
                            case "CO2":
                                Scope2_table.Rows[Scope2_rowIndex].Cells[3].Paragraphs.First().Append("v");
                                break;
                            case "CH4":
                                Scope2_table.Rows[Scope2_rowIndex].Cells[4].Paragraphs.First().Append("v");
                                break;
                            case "N2O":
                                Scope2_table.Rows[Scope2_rowIndex].Cells[5].Paragraphs.First().Append("v");
                                break;
                            case "HFCS":
                                Scope2_table.Rows[Scope2_rowIndex].Cells[6].Paragraphs.First().Append("v");
                                break;
                            case "PFCS":
                                Scope2_table.Rows[Scope2_rowIndex].Cells[7].Paragraphs.First().Append("v");
                                break;
                            case "SF6":
                                Scope2_table.Rows[Scope2_rowIndex].Cells[8].Paragraphs.First().Append("v");
                                break;
                            case "NF3":
                                Scope2_table.Rows[Scope2_rowIndex].Cells[9].Paragraphs.First().Append("v");
                                break;
                        }
                    }
                    Scope2_rowIndex++;
                }
                // 在 doc 中替換段落
                doc.ReplaceTextWithObject("[類別二表]", Scope2_table);
                //--------------------------------類別二表




                // 保存新文檔
                doc.SaveAs(newFilePath);
            }
            // 返回一個視圖或其他操作，根據你的需求
            var fileBytes = System.IO.File.ReadAllBytes(newFilePath);
            var fileName = data.Num + "-" + data.Area.Name + "-" + data.Area.Company.Name + "-溫室氣體盤查報告書.docx"; // 可以自行定義檔名
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
        }

        public async Task<IActionResult> IndexAsync(Guid? id)
        {
            TempData["yearId"] = id;
            await CountEmissionAsync(id);

            var emissions = await _context.Years
                .FindAsync(id);
            return View(emissions);

        }

        //public Xceed.Document.NET.Table GetTable(string tableName, string EmissionPartern, DocX doc)
        //{
        //    var device = _context.Devices.Where(x => x.YearId == id && x.isDeleted == 0).OrderBy(x => x.Scope).ThenBy(x => x.EmissionPattern).ToList();
        //    var ManyGHGs = _context.Devices.Where(x => x.isDeleted == 0).SelectMany(x => x.GHGs).ToList();

        //    int nonMove_Num = device.Where(x => x.EmissionPattern == "固定").Count();
        //    string TableName = tableName + "_C2Otable";
        //    Xceed.Document.NET.Table TableName  = doc.AddTable(nonMove_Num + 1, 9);
        //    "TableName" + _C2Otable.Rows[0].Cells[0].Paragraphs.First().Append("類別");
        //    nonMove_C2Otable.Rows[0].Cells[1].Paragraphs.First().Append("排放型式");
        //    nonMove_C2Otable.Rows[0].Cells[2].Paragraphs.First().Append("活動數據");
        //    nonMove_C2Otable.Rows[0].Cells[3].Paragraphs.First().Append("溫室氣體");
        //    nonMove_C2Otable.Rows[0].Cells[4].Paragraphs.First().Append("排放係數");
        //    nonMove_C2Otable.Rows[0].Cells[5].Paragraphs.First().Append("係數來源");
        //    nonMove_C2Otable.Rows[0].Cells[6].Paragraphs.First().Append("排放量\n(公噸/年)");
        //    nonMove_C2Otable.Rows[0].Cells[7].Paragraphs.First().Append("GWP");
        //    nonMove_C2Otable.Rows[0].Cells[8].Paragraphs.First().Append("排放當量\n(公噸CO2e/年)\n");
        //    int nonMoveCO2_rowIndex = 1;
        //    Xceed.Document.NET.Table nonMove_CH4table = doc.AddTable(nonMove_Num + 1, 9);
        //    nonMove_CH4table.Rows[0].Cells[0].Paragraphs.First().Append("類別");
        //    nonMove_CH4table.Rows[0].Cells[1].Paragraphs.First().Append("排放型式");
        //    nonMove_CH4table.Rows[0].Cells[2].Paragraphs.First().Append("活動數據");
        //    nonMove_CH4table.Rows[0].Cells[3].Paragraphs.First().Append("溫室氣體");
        //    nonMove_CH4table.Rows[0].Cells[4].Paragraphs.First().Append("排放係數");
        //    nonMove_CH4table.Rows[0].Cells[5].Paragraphs.First().Append("係數來源");
        //    nonMove_CH4table.Rows[0].Cells[6].Paragraphs.First().Append("排放量\n(公噸/年)");
        //    nonMove_CH4table.Rows[0].Cells[7].Paragraphs.First().Append("GWP");
        //    nonMove_CH4table.Rows[0].Cells[8].Paragraphs.First().Append("排放當量\n(公噸CO2e/年)\n");
        //    int nonMoveCH4_rowIndex = 1;
        //    Xceed.Document.NET.Table nonMove_N2Otable = doc.AddTable(nonMove_Num + 1, 9);
        //    nonMove_N2Otable.Rows[0].Cells[0].Paragraphs.First().Append("類別");
        //    nonMove_N2Otable.Rows[0].Cells[1].Paragraphs.First().Append("排放型式");
        //    nonMove_N2Otable.Rows[0].Cells[2].Paragraphs.First().Append("活動數據");
        //    nonMove_N2Otable.Rows[0].Cells[3].Paragraphs.First().Append("溫室氣體");
        //    nonMove_N2Otable.Rows[0].Cells[4].Paragraphs.First().Append("排放係數");
        //    nonMove_N2Otable.Rows[0].Cells[5].Paragraphs.First().Append("係數來源");
        //    nonMove_N2Otable.Rows[0].Cells[6].Paragraphs.First().Append("排放量\n(公噸/年)");
        //    nonMove_N2Otable.Rows[0].Cells[7].Paragraphs.First().Append("GWP");
        //    nonMove_N2Otable.Rows[0].Cells[8].Paragraphs.First().Append("排放當量\n(公噸CO2e/年)\n");
        //    int nonMoveN2O_rowIndex = 1;
        //    foreach (var item in device.Where(x => x.EmissionPattern == "固定"))
        //    {
        //        var GHGs = ManyGHGs.Where(x => x.DeviceId == item.Id).ToList();
        //        foreach (var GHG in GHGs)
        //        {
        //            switch (GHG.Name)
        //            {
        //                case "CO2":
        //                    nonMove_C2Otable.Rows[nonMoveCO2_rowIndex].Cells[0].Paragraphs.First().Append(GHG.Device.Scope);
        //                    nonMove_C2Otable.Rows[nonMoveCO2_rowIndex].Cells[1].Paragraphs.First().Append(GHG.Device.EmissionPattern);
        //                    nonMove_C2Otable.Rows[nonMoveCO2_rowIndex].Cells[2].Paragraphs.First().Append(GHG.Device.Num.ToString() + GHG.Device.Unit);
        //                    nonMove_C2Otable.Rows[nonMoveCO2_rowIndex].Cells[3].Paragraphs.First().Append(GHG.Name); //可刪除
        //                    nonMove_C2Otable.Rows[nonMoveCO2_rowIndex].Cells[4].Paragraphs.First().Append(GHG.CEF.ToString() + "公噸/" + GHG.Device.Unit);
        //                    nonMove_C2Otable.Rows[nonMoveCO2_rowIndex].Cells[5].Paragraphs.First().Append("係數來源"); //待改
        //                    nonMove_C2Otable.Rows[nonMoveCO2_rowIndex].Cells[6].Paragraphs.First().Append("排放量\n(公噸/年)"); //待改
        //                    nonMove_C2Otable.Rows[nonMoveCO2_rowIndex].Cells[7].Paragraphs.First().Append(GHG.GWP.ToString());
        //                    nonMove_C2Otable.Rows[nonMoveCO2_rowIndex].Cells[8].Paragraphs.First().Append(GHG.Emission.ToString());
        //                    nonMoveCO2_rowIndex++;
        //                    break;
        //                case "CH4":

        //                    nonMove_CH4table.Rows[nonMoveCH4_rowIndex].Cells[0].Paragraphs.First().Append(GHG.Device.Scope);
        //                    nonMove_CH4table.Rows[nonMoveCH4_rowIndex].Cells[1].Paragraphs.First().Append(GHG.Device.EmissionPattern);
        //                    nonMove_CH4table.Rows[nonMoveCH4_rowIndex].Cells[2].Paragraphs.First().Append(GHG.Device.Num.ToString() + GHG.Device.Unit);
        //                    nonMove_CH4table.Rows[nonMoveCH4_rowIndex].Cells[3].Paragraphs.First().Append(GHG.Name); //可刪除
        //                    nonMove_CH4table.Rows[nonMoveCH4_rowIndex].Cells[4].Paragraphs.First().Append(GHG.CEF.ToString() + "公噸/" + GHG.Device.Unit);
        //                    nonMove_CH4table.Rows[nonMoveCH4_rowIndex].Cells[5].Paragraphs.First().Append("係數來源"); //待改
        //                    nonMove_CH4table.Rows[nonMoveCH4_rowIndex].Cells[6].Paragraphs.First().Append("排放量\n(公噸/年)"); //待改
        //                    nonMove_CH4table.Rows[nonMoveCH4_rowIndex].Cells[7].Paragraphs.First().Append(GHG.GWP.ToString());
        //                    nonMove_CH4table.Rows[nonMoveCH4_rowIndex].Cells[8].Paragraphs.First().Append(GHG.Emission.ToString());
        //                    nonMoveCH4_rowIndex++;
        //                    break;
        //                case "N2O":

        //                    nonMove_N2Otable.Rows[nonMoveN2O_rowIndex].Cells[0].Paragraphs.First().Append(GHG.Device.Scope);
        //                    nonMove_N2Otable.Rows[nonMoveN2O_rowIndex].Cells[1].Paragraphs.First().Append(GHG.Device.EmissionPattern);
        //                    nonMove_N2Otable.Rows[nonMoveN2O_rowIndex].Cells[2].Paragraphs.First().Append(GHG.Device.Num.ToString() + GHG.Device.Unit);
        //                    nonMove_N2Otable.Rows[nonMoveN2O_rowIndex].Cells[3].Paragraphs.First().Append(GHG.Name); //可刪除
        //                    nonMove_N2Otable.Rows[nonMoveN2O_rowIndex].Cells[4].Paragraphs.First().Append(GHG.CEF.ToString() + "公噸/" + GHG.Device.Unit);
        //                    nonMove_N2Otable.Rows[nonMoveN2O_rowIndex].Cells[5].Paragraphs.First().Append("係數來源"); //待改
        //                    nonMove_N2Otable.Rows[nonMoveN2O_rowIndex].Cells[6].Paragraphs.First().Append("排放量\n(公噸/年)"); //待改
        //                    nonMove_N2Otable.Rows[nonMoveN2O_rowIndex].Cells[7].Paragraphs.First().Append(GHG.GWP.ToString());
        //                    nonMove_N2Otable.Rows[nonMoveN2O_rowIndex].Cells[8].Paragraphs.First().Append(GHG.Emission.ToString());
        //                    nonMoveN2O_rowIndex++;
        //                    break;
        //            }
        //        }
        //    }
        //    doc.ReplaceTextWithObject("[固定CO2]", nonMove_C2Otable);
        //    doc.ReplaceTextWithObject("[固定CH4]", nonMove_CH4table);
        //    doc.ReplaceTextWithObject("[固定N2O]", nonMove_N2Otable);  //都改英文 用TableName+ CO2 應該可以
        //    return Table;
        //}


    }
}
