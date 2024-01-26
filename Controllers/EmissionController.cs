using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
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
        public async Task<Emission> CountEmissionAsync(Guid? id, int Year)
        {

            var devices = await _context.Devices
                .Where(x => x.isDeleted == 0 && x.AreaId == id && x.year == Year)
                .Include(x => x.Areas)
                .ToListAsync();
            var Emission = await _context.emissions.Where(x => x.AreaId == id && x.Year == Year).ToListAsync();
            var toCreate = new Emission();
            float sum_hardlymove = 0, sum_move = 0, sum_escape = 0, sum_process = 0, sum_electricity = 0,
                sum1_CO2 = 0, sum2_CO2 = 0, sum_CH4 = 0, sum_N2O = 0, sum_HFCS = 0, sum_PFCS = 0,
                sum_SF6 = 0, sum_NF3 = 0, sum_Scope1 = 0, sum_Scope2 = 0, sum_all = 0;
            int no1_Grade = 0, no2_Grade = 0, no3_Grade = 0;
            float avg_Grade = 0;
            float all_UUL = 0, all_ULL = 0, all_countUUL = 0, all_countULL = 0, sum_Uncertainty = 0;

            foreach (var item in devices)
            {
                switch (item.EmissionPattern) //計算各排放型式
                {
                    case "固定": sum_hardlymove += item.Emissions; break;
                    case "移動": sum_move += item.Emissions; break;
                    case "逸散": sum_escape += item.Emissions; break;
                    case "處理": sum_process += item.Emissions; break;
                    case "外購電力": sum_electricity += item.Emissions; break;
                }

                if (item.Scope == "類別一") //計算類別一各溫室氣體排放量
                {
                    sum1_CO2 += item.CO2;
                    sum_CH4 += item.CH4;
                    sum_N2O += item.N2O;
                    sum_HFCS += item.HFCS;
                    sum_PFCS += item.PFCS;
                    sum_SF6 += item.SF6;
                    sum_NF3 += item.NF3;
                }
                else if (item.Scope == "類別二") //計算類別二各溫室氣體排放量
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
            bool sumMatch = Emission.Any(x => x.All != sum_all.ToString("F3"));
            bool gradeMatch = Emission.Any(x => x.avg_Grade != avg_Grade.ToString("F2"));
            if (Emission.Count != 0 )
            {
                if(sumMatch || gradeMatch) //如果總量和總分有差，則修正進資料庫。
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
                    toCreate.Year = Year;
                }
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
                toCreate.Year = Year;
                _context.Add(toCreate);
            }

            await _context.SaveChangesAsync();
            return toCreate;
        }
        public async Task<IActionResult> WordAsync(Guid id, int Year)
        {
            var CountEmission = await CountEmissionAsync(id, Year);
            // 這裡要替換成你 MVC 應用程式中正確的檔案路徑
            var data = await _context.Areas.Where(x => x.Id == id).Include(x => x.Company).FirstOrDefaultAsync();
            var device = await _context.Devices.Where(x => x.AreaId == id && x.isDeleted == 0 && x.year == Year).OrderBy(x => x.Scope).ThenBy(x => x.EmissionPattern).ToListAsync();
            var scope1_device = await _context.Devices.Where(x => x.AreaId == id && x.isDeleted == 0 && x.year == Year && x.Scope != "類別二").OrderBy(x => x.Scope).ThenBy(x => x.EmissionPattern).ToListAsync();
            var Material = await _context.Materials.ToListAsync();
            var emission = await _context.emissions.Where(x => x.AreaId == id).FirstOrDefaultAsync();
            var nonMove = device.Where(x => x.EmissionPattern == "固定" && x.isDeleted == 0 && x.year == Year).Select(d => d.Name).ToList();
            var move = device.Where(x => x.EmissionPattern == "移動" && x.isDeleted == 0 && x.year == Year).Where(x => x.isDeleted == 0).Select(d => d.Name).ToList();
            var escape = device.Where(x => x.EmissionPattern == "逸散" && x.isDeleted == 0 && x.year == Year).Select(d => d.Name).ToList();
            var process = device.Where(x => x.EmissionPattern == "製程" && x.isDeleted == 0 && x.year == Year).Select(d => d.Name).ToList();

            string Name = data.Company.Name;

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\doc\\", "複雜溫盤報告書範本.docx");
            string newFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\doc\\", Year + Name + "-溫室氣體盤查報告書.docx");

            // 複製文件
            using (DocX doc = DocX.Load(filePath))
            {

                List<Xceed.Document.NET.Paragraph> paragraphsToUpdate = new List<Xceed.Document.NET.Paragraph>();

                // 設定要查找和替換的文本
                foreach (var paragraph in doc.Paragraphs)
                {
                    if (paragraph.Text.Contains("類別表補充"))
                    {
                        paragraphsToUpdate.Add(paragraph);
                    }

                    if (paragraph.Text.Contains("類別一表補充"))
                    {
                        paragraphsToUpdate.Add(paragraph);
                    }

                    if (paragraph.Text.Contains("排放源活動數據表替換"))
                    {
                        paragraphsToUpdate.Add(paragraph);
                    }

                    if (paragraph.Text.Contains("排放係數表替換"))
                    {
                        paragraphsToUpdate.Add(paragraph);
                    }

                    paragraph.ReplaceText("補充公司基本資料", data.Company.Information);
                    paragraph.ReplaceText("補充盤查年度", (data.Year + 1911).ToString() + "年");
                    paragraph.ReplaceText("民國年份補充", (data.Year).ToString());
                    paragraph.ReplaceText("補充公司場所名稱", data.Company.Name);
                    paragraph.ReplaceText("補充公司場所英文名稱", data.Company.EnglishName);
                    paragraph.ReplaceText("補充公司場所簡稱", data.Company.EasyName);
                    paragraph.ReplaceText("補充公司場所英文簡稱", data.Company.EasyEnglishName);
                    paragraph.ReplaceText("補充統一編號", data.UniqueCode.ToString());
                    paragraph.ReplaceText("補充工廠登記編號", data.FactorCode.ToString());
                    paragraph.ReplaceText("補充地址", data.City + data.District + data.Address);
                    if (nonMove.Count != 0)
                    {
                        paragraph.ReplaceText("組織邊界的各據點內所擁有的固定式化石燃料燃燒排放源。", "組織邊界的各據點內所擁有的固定式化石燃料燃燒排放源，固定排放源包含" + string.Join(", ", nonMove) + "。");
                    }
                    if (move.Count != 0)
                    {
                        paragraph.ReplaceText("組織邊界的各據點內所擁有的可移動且燃燒化石燃料的排放源。", "組織邊界的各據點內所擁有的可移動且燃燒化石燃料的排放源，移動排放源包含" + string.Join(", ", move) + "。");
                    }
                    if (escape.Count != 0)
                    {
                        paragraph.ReplaceText("組織邊界的各據點內所擁有的人為逸散溫室氣體排放源。", "組織邊界的各據點內所擁有的人為逸散溫室氣體排放源，逸散源包含" + string.Join(", ", escape) + "。");
                    }
                    if (process.Count != 0)
                    {
                        paragraph.ReplaceText("組織邊界內在製程中化學反應產生的溫室氣體排放源。", "組織邊界內在製程中化學反應產生的溫室氣體排放源，如" + string.Join(", ", process) + "。");
                    }
                    paragraph.ReplaceText("基準年補充", (data.Year + 1911).ToString() + "年");
                    paragraph.ReplaceText("類別一CO2排放", emission.Scope1_CO2);
                    paragraph.ReplaceText("CO2排放", emission.CO2);
                    paragraph.ReplaceText("CH4排放", emission.CH4);
                    paragraph.ReplaceText("N2O排放", emission.N2O);
                    paragraph.ReplaceText("HFCS排放", emission.HFCS);
                    paragraph.ReplaceText("PFCS排放", emission.PFCS);
                    paragraph.ReplaceText("SF6排放", emission.SF6);
                    paragraph.ReplaceText("NF3排放", emission.NF3);
                    paragraph.ReplaceText("類別一CO2占比", emission.percentage1_CO2);
                    paragraph.ReplaceText("類別一CH4占比", emission.percentage1_CH4);
                    paragraph.ReplaceText("類別一N2O占比", emission.percentage1_N2O);
                    paragraph.ReplaceText("類別一HFCS占比", emission.percentage1_HFCS);
                    paragraph.ReplaceText("類別一PFCS占比", emission.percentage1_PFCS);
                    paragraph.ReplaceText("類別一SF6占比", emission.percentage1_SF6);
                    paragraph.ReplaceText("類別一NF3占比", emission.percentage2_NF3);
                    paragraph.ReplaceText("類別一CO2排放", emission.Scope1);
                    paragraph.ReplaceText("CO2占比", emission.percentage2_CO2);
                    paragraph.ReplaceText("CH4占比", emission.percentage2_CH4);
                    paragraph.ReplaceText("N2O占比", emission.percentage2_N2O);
                    paragraph.ReplaceText("HFCS占比", emission.percentage2_HFCS);
                    paragraph.ReplaceText("PFCS占比", emission.percentage2_PFCS);
                    paragraph.ReplaceText("SF6占比", emission.percentage2_SF6);
                    paragraph.ReplaceText("NF3占比", emission.percentage2_NF3);
                    paragraph.ReplaceText("總排放當量", emission.All);
                    paragraph.ReplaceText("固定排放量", emission.non_move);
                    paragraph.ReplaceText("移動排放量", emission.move);
                    paragraph.ReplaceText("製程排放量", emission.process);
                    paragraph.ReplaceText("逸散排放量", emission.escape);
                    paragraph.ReplaceText("固定排放比例", emission.percentage_nonMove);
                    paragraph.ReplaceText("製程排放比例", emission.percentage_Process);
                    paragraph.ReplaceText("移動排放比例", emission.percentage_Move);
                    paragraph.ReplaceText("逸散排放比例", emission.percentage_Escape);
                    paragraph.ReplaceText("類別一占比", emission.percentage_Scope1);
                    paragraph.ReplaceText("類別二占比", emission.percentage_Scope2);
                    paragraph.ReplaceText("類別一總排放", emission.Scope1);
                    paragraph.ReplaceText("類別二總排放", emission.Scope2);
                    paragraph.ReplaceText("進行評估排放當量", emission.cal_all);
                    paragraph.ReplaceText("不確定性評估占比", emission.percentage_CalAll);
                    paragraph.ReplaceText("第1級評分", emission.no1_Grade);
                    paragraph.ReplaceText("第2級評分", emission.no2_Grade);
                    paragraph.ReplaceText("第3級評分", emission.no3_Grade);
                    paragraph.ReplaceText("清冊等級分數補充", emission.avg_Grade);
                    paragraph.ReplaceText("清冊級別補充", emission.all_Grade);
                    paragraph.ReplaceText("95上", emission.UUL);
                    paragraph.ReplaceText("95下", emission.ULL);
                    paragraph.ReplaceText("補充姓名", data.Company.Owner);
                    paragraph.ReplaceText("補充電話", data.Company.Phone);
                    paragraph.ReplaceText("補充電子信箱", data.Company.Email);
                }

                foreach (var paragraph in paragraphsToUpdate) //類別表補充
                {
                    Xceed.Document.NET.Table table = doc.AddTable(device.Count() + 1, 4);
                    // 填充表格標題
                    table.Rows[0].Cells[0].Paragraphs.First().Append("類別");
                    table.Rows[0].Cells[1].Paragraphs.First().Append("型式");
                    table.Rows[0].Cells[2].Paragraphs.First().Append("排放源");
                    table.Rows[0].Cells[3].Paragraphs.First().Append("產生之溫室氣體");

                    for (int x = 0; x < device.Count(); x++)
                    {
                        table.Rows[x + 1].Cells[0].Paragraphs.First().Append(device[x].Scope);
                        table.Rows[x + 1].Cells[1].Paragraphs.First().Append(device[x].EmissionPattern);
                        table.Rows[x + 1].Cells[2].Paragraphs.First().Append(device[x].Name + "(" + device[x].Material + ")");
                        if (device[x].CO2_Emission == true && device[x].CH4_Emission == true && device[x].N2O_Emission == true)
                        {
                            table.Rows[x + 1].Cells[3].Paragraphs.First().Append("CO₂、CH₄、N₂O");
                        }
                        else if (device[x].CO2_Emission == true)
                        {
                            table.Rows[x + 1].Cells[3].Paragraphs.First().Append("CO₂");
                        }
                        else if (device[x].HFCS_Emission == true)
                        {
                            table.Rows[x + 1].Cells[3].Paragraphs.First().Append("HFCs");
                        }
                        else if (device[x].CH4_Emission == true)
                        {
                            table.Rows[x + 1].Cells[3].Paragraphs.First().Append("CH₄");
                        }
                    }
                    paragraph.ReplaceTextWithObject("類別表補充", table);
                }

                foreach (var paragraph in paragraphsToUpdate) //類別一表補充
                {
                    Xceed.Document.NET.Table table = doc.AddTable(scope1_device.Count() + 1, 4);
                    table.SetWidths(new float[] { 100, 150, 200, 100 });

                    // 填充表格標題
                    table.Rows[0].Cells[0].Paragraphs.First().Append("類別");
                    table.Rows[0].Cells[1].Paragraphs.First().Append("型式");
                    table.Rows[0].Cells[2].Paragraphs.First().Append("排放源");
                    table.Rows[0].Cells[3].Paragraphs.First().Append("產生之溫室氣體");

                    for (int x = 0; x < scope1_device.Count(); x++)
                    {
                        table.Rows[x + 1].Cells[0].Paragraphs.First().Append(scope1_device[x].Scope);
                        table.Rows[x + 1].Cells[1].Paragraphs.First().Append(scope1_device[x].EmissionPattern);
                        table.Rows[x + 1].Cells[2].Paragraphs.First().Append(scope1_device[x].Name + "(" + scope1_device[x].Material + ")");
                        if (scope1_device[x].CO2_Emission == true && scope1_device[x].CH4_Emission == true && scope1_device[x].N2O_Emission == true)
                        {
                            table.Rows[x + 1].Cells[3].Paragraphs.First().Append("CO₂、CH₄、N₂O");
                        }
                        else if (scope1_device[x].CO2_Emission == true)
                        {
                            table.Rows[x + 1].Cells[3].Paragraphs.First().Append("CO₂");
                        }
                        else if (scope1_device[x].HFCS_Emission == true)
                        {
                            table.Rows[x + 1].Cells[3].Paragraphs.First().Append("HFCs");
                        }
                        else if (scope1_device[x].CH4_Emission == true)
                        {
                            table.Rows[x + 1].Cells[3].Paragraphs.First().Append("CH₄");
                        }
                    }
                    paragraph.ReplaceTextWithObject("類別一表補充", table);
                }

                foreach (var paragraph in paragraphsToUpdate) //排放源活動數據表替換
                {
                    Xceed.Document.NET.Table table = doc.AddTable(device.Count() + 1, 5);
                    //table.SetWidths(new float[] { 100, 150, 200, 100 });

                    // 填充表格標題
                    table.Rows[0].Cells[0].Paragraphs.First().Append("類別");
                    table.Rows[0].Cells[1].Paragraphs.First().Append("排放型式");
                    table.Rows[0].Cells[2].Paragraphs.First().Append("原燃物料");
                    table.Rows[0].Cells[3].Paragraphs.First().Append("活動數據");
                    table.Rows[0].Cells[4].Paragraphs.First().Append("單位");


                    for (int x = 0; x < device.Count(); x++)
                    {
                        table.Rows[x + 1].Cells[0].Paragraphs.First().Append(device[x].Scope);
                        table.Rows[x + 1].Cells[1].Paragraphs.First().Append(device[x].EmissionPattern);
                        table.Rows[x + 1].Cells[2].Paragraphs.First().Append(device[x].Material + "(" + device[x].Name + ")");
                        table.Rows[x + 1].Cells[3].Paragraphs.First().Append((device[x].Num / 1000).ToString("F4"));
                        if (device[x].Unit == "人")
                        {
                            table.Rows[x + 1].Cells[4].Paragraphs.First().Append(device[x].Unit);
                        }
                        else
                        {
                            string displayUnit;
                            switch (device[x].Unit)
                            {
                                case "公斤":
                                    displayUnit = "公噸";
                                    break;
                                case "公升":
                                    displayUnit = "公秉";
                                    break;
                                case "立方公尺":
                                    displayUnit = "千立方公尺";
                                    break;
                                case "度":
                                    displayUnit = "千度";
                                    break;
                                default:
                                    displayUnit = device[x].Unit;
                                    break;
                            }
                            table.Rows[x + 1].Cells[4].Paragraphs.First().Append(displayUnit);
                        }
                    }
                    paragraph.ReplaceTextWithObject("排放源活動數據表替換", table);
                }
                foreach (var paragraph in paragraphsToUpdate) //類別表補充
                {
                    int nonMove_num = device.Where(x => x.EmissionPattern == "固定").Count();
                    int move_num = device.Where(x => x.EmissionPattern == "移動").Count();
                    int max = device.Count() - nonMove_num - move_num + nonMove_num * 3 + move_num * 3;
                    Xceed.Document.NET.Table table = doc.AddTable(max + 1, 6);
                    table.SetWidths(new float[] { 100, 150, 200, 100 });
                    table.Design = TableDesign.TableGrid;
                    table.AutoFit = AutoFit.Window;
                    table.Alignment = Alignment.left;

                    // 填充表格標題
                    table.Rows[0].Cells[0].Paragraphs.First().Append("原燃物料");
                    table.Rows[0].Cells[1].Paragraphs.First().Append("溫室氣體");
                    table.Rows[0].Cells[2].Paragraphs.First().Append("排放係數");
                    table.Rows[0].Cells[3].Paragraphs.First().Append("係數來源");
                    table.Rows[0].Cells[4].Paragraphs.First().Append("係數單位");
                    table.Rows[0].Cells[5].Paragraphs.First().Append("GWP(AR6)");

                    //table.Paragraphs.First().Alignment = Alignment.left;
                    //table.Paragraphs.

                    //int x = 0; //格子
                    int device_num = 0;

                    for (int x = 0; x < max; x++) //格子
                    {
                        while (device_num < device.Count())
                        {
                            string displayUnit;
                            switch (device[device_num].Unit)
                            {
                                case "公斤":
                                    displayUnit = "公噸";
                                    break;
                                case "公升":
                                    displayUnit = "公秉";
                                    break;
                                case "立方公尺":
                                    displayUnit = "千立方公尺";
                                    break;
                                case "度":
                                    displayUnit = "千度";
                                    break;
                                default:
                                    displayUnit = device[device_num].Unit;
                                    break;
                            }

                            // CO2, CH4, N2O 三種溫室氣體排放
                            if (device[device_num].EmissionPattern == "固定" || device[device_num].EmissionPattern == "移動")
                            {
                                for (int num = 1; num <= 3; num++)
                                {
                                    table.Rows[x + 1].Cells[0].Paragraphs.First().Append(device[device_num].Material + "(" + device[device_num].EmissionPattern + ")");
                                    table.Rows[x + 1].Cells[1].Paragraphs.First().Append(num == 1 ? "CO₂" : (num == 2 ? "CH₄" : "N₂O"));

                                    string gasType = num == 1 ? "CO2" : (num == 2 ? "CH4" : "N2O");

                                    table.Rows[x + 1].Cells[2].Paragraphs.First().Append(
                                        gasType == "CO2" ? _context.Materials.FirstOrDefault(m => m.Name == device[device_num].Material)?.CO2CEF.ToString("F10") :
                                        gasType == "CH4" ? _context.Materials.FirstOrDefault(m => m.Name == device[device_num].Material)?.CH4CEF.ToString("F10") :
                                        gasType == "N2O" ? _context.Materials.FirstOrDefault(m => m.Name == device[device_num].Material)?.N2OCEF.ToString("F10") :
                                        "");

                                    table.Rows[x + 1].Cells[3].Paragraphs.First().Append("溫室氣體排放係數管理表 6.0.4 版");
                                    table.Rows[x + 1].Cells[4].Paragraphs.First().Append("公噸/" + displayUnit);
                                    table.Rows[x + 1].Cells[5].Paragraphs.First().Append(
                                        gasType == "CO2" ? _context.GWPs.FirstOrDefault(x => x.Name == "CO2").Num.ToString() :
                                        gasType == "CH4" ? _context.GWPs.FirstOrDefault(x => x.Name == "CH4").Num.ToString() :
                                        _context.GWPs.FirstOrDefault(x => x.Name == "N2O").Num.ToString());

                                    if (num < 3)
                                    {
                                        x++; //下一格
                                    }
                                }

                            }
                            else
                            {
                                // 其他情況
                                string gasType;
                                if (_context.Materials.FirstOrDefault(m => m.Name == device[device_num].Material) != null)
                                {
                                    if (_context.Materials.FirstOrDefault(m => m.Name == device[device_num].Material)?.CO2CEF != 0)
                                    {
                                        gasType = "CO2";
                                    }
                                    else if (_context.Materials.FirstOrDefault(m => m.Name == device[device_num].Material)?.CH4CEF != 0)
                                    {
                                        gasType = "CH4";
                                    }
                                    else
                                    {
                                        // 可能有其他情況需要處理
                                        gasType = "其他";
                                    }
                                }
                                else
                                {
                                    gasType = "HFCs";
                                }


                                table.Rows[x + 1].Cells[0].Paragraphs.First().Append(device[device_num].Material + "(" + device[device_num].EmissionPattern + ")");
                                table.Rows[x + 1].Cells[1].Paragraphs.First().Append(gasType);
                                table.Rows[x + 1].Cells[2].Paragraphs.First().Append(
                                        gasType == "HFCs" ? _context.Materials.FirstOrDefault(m => m.Name == device[device_num].Name)?.HFCSCEF.ToString() :
                                        gasType == "CO2" ? _context.Materials.FirstOrDefault(m => m.Name == device[device_num].Material)?.CO2CEF.ToString() :
                                        gasType == "CH4" ? _context.Materials.FirstOrDefault(m => m.Name == device[device_num].Material)?.CH4CEF.ToString() :
                                        gasType == "N2O" ? _context.Materials.FirstOrDefault(m => m.Name == device[device_num].Material)?.N2OCEF.ToString() :
                                        ""); //增加其他

                                table.Rows[x + 1].Cells[3].Paragraphs.First().Append("溫室氣體排放係數管理表 6.0.4 版");
                                table.Rows[x + 1].Cells[4].Paragraphs.First().Append("公噸/" + displayUnit);
                                table.Rows[x + 1].Cells[5].Paragraphs.First().Append(!string.IsNullOrEmpty(gasType) ?
                                                                (gasType == "HFCs" ? _context.GWPs.FirstOrDefault(x => x.Name == device[device_num].Material)?.Num.ToString() :
                                                                                     _context.GWPs.FirstOrDefault(x => x.Name == gasType)?.Num.ToString()) : "0");

                            }

                            x++;
                            device_num++;
                        }
                    }

                    paragraph.ReplaceTextWithObject("排放係數表替換", table);
                }

                // 保存新文檔
                doc.SaveAs(newFilePath);
            }
            // 返回一個視圖或其他操作，根據你的需求
            var fileBytes = System.IO.File.ReadAllBytes(newFilePath);
            var fileName = Year + Name + "-溫室氣體盤查報告書.docx"; // 可以自行定義檔名
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
        }
        public async Task<IActionResult> IndexAsync(Guid? id, int Year)
        {
            var CountEmission = await CountEmissionAsync(id, Year);
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

        public IActionResult DownloadFile(string fileName)
{
    // 設定要下載的檔案路徑
    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileName);

    // 檢查檔案是否存在
    if (!System.IO.File.Exists(filePath))
    {
        return NotFound();
    }

    // 讀取檔案內容
    var fileContent = System.IO.File.ReadAllBytes(filePath);

    // 指定檔案型別
    var contentType = "application/octet-stream";

    // 建立一個 FileResult 物件
    var fileResult = new FileContentResult(fileContent, contentType)
    {
        FileDownloadName = fileName
    };

    return fileResult;
}

    }
}
