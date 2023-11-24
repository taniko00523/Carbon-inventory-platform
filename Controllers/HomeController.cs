using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

using Xceed.Words.NET;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Xceed.Document.NET;


namespace Carbon_inventory_platform.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Companies
        public async Task<IActionResult> Index()
        {
            return _context.Areas != null ? //如果有抓到資料表Null
                        View(await _context.Areas
                        .Where(x => x.isDeleted == 0) //抓出資料表裡面沒被刪除的
                        .Include(x => x.Company)
                        .ToListAsync()) :
                        Problem("沒有找到資料"); //否則回報問題 Entity set 'ApplicationDbContext.Companies'  is null.
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> WordAsync(Guid id)
        {
            // 這裡要替換成你 MVC 應用程式中正確的檔案路徑
            var data = await _context.Areas.Where(x => x.Id == id).Include(x => x.Company).FirstOrDefaultAsync();
            var device = await _context.Devices.Where(x => x.AreaId == id).Where(x=>x.isDeleted == 0).ToListAsync();
            var emission = await _context.emissions.Where(x => x.AreaId == id).FirstOrDefaultAsync();
            var nonMove = device.Where(d => d.EmissionPattern == "固定").Where(x => x.isDeleted == 0).Select(d => d.Name).ToList();
            var move = device.Where(d => d.EmissionPattern == "移動").Where(x => x.isDeleted == 0).Select(d => d.Name).ToList();
            var escape = device.Where(d => d.EmissionPattern == "逸散").Where(x => x.isDeleted == 0).Select(d => d.Name).ToList();
            var process = device.Where(d => d.EmissionPattern == "製程").Where(x => x.isDeleted == 0).Select(d => d.Name).ToList();

            string Name = data.Company.Name;

            string filePath = "C:\\報告書\\溫室氣體盤查報告書範本.docx";
            string newFilePath = "C:\\報告書\\" + Name + "-溫室氣體盤查報告書" + ".docx";

            // 複製文件
            using (DocX doc = DocX.Load(filePath))
            {
                // 設定要查找和替換的文本

                string CO2_1 = "類別一CO2排放";
                string CH4 = "CH4排放";
                string N2O = "N2O排放";
                string HFCS = "HFCS排放";
                string PFCS = "PFCS排放";
                string SF6 = "SF6排放";
                string NF3 = "NF3排放";
                string Scope1 = "類別一總排放";
                //缺少類別二

                string rep_baseInfomation = "公司基本資料";
                string rep_Year = "111年";
                string rep_companyName = "2";
                string rep_uniformNumber = "3";
                string rep_factoryNumber = "4";
                string rep_location = "5";
                string rep_address = "6";

                //var t = doc.AddTable(5, 2);
                //t.Design = TableDesign.ColorfulListAccent1;
                //t.Alignment = Alignment.center;
                //t.Rows[0].Cells[0].Paragraphs[0].Append("Mike");
                //t.Rows[0].Cells[1].Paragraphs[0].Append("65");
                //t.Rows[1].Cells[0].Paragraphs[0].Append("Kevin");
                //t.Rows[1].Cells[1].Paragraphs[0].Append("62");
                //t.Rows[2].Cells[0].Paragraphs[0].Append("Carl");
                //t.Rows[2].Cells[1].Paragraphs[0].Append("60");
                //t.Rows[3].Cells[0].Paragraphs[0].Append("Michael");
                //t.Rows[3].Cells[1].Paragraphs[0].Append("59");
                //t.Rows[4].Cells[0].Paragraphs[0].Append("Shawn");
                //t.Rows[4].Cells[1].Paragraphs[0].Append("57");
                //// Add a row at the end of the table and sets its values.
                //var r = t.InsertRow();
                //r.Cells[0].Paragraphs[0].Append("Mario");
                //r.Cells[1].Paragraphs[0].Append("54");
                //// Insert a new Paragraph into the document.
                //var p = doc.InsertParagraph("Xceed Top Players Points:");
                //p.SpacingAfter(40d);
                //// Insert the Table after the Paragraph.
                //p.InsertTableAfterSelf(t);

                //List<Table> tablesToInsert = new List<Table>();
                // 遍歷文檔的所有段落，執行查找和替換
                foreach (var paragraph in doc.Paragraphs)
                {
                    // 找到包含特定文本的段落
                    //if (paragraph.Text.Contains("特定文本"))
                    //{
                    //    // 创建表格并填充数据
                    //    var table = doc.AddTable(3, 3);
                    //    int num = 1;
                    //    foreach (var row in table.Rows)
                    //    {
                    //        foreach (var cell in row.Cells)
                    //        {
                    //            cell.InsertParagraph(num.ToString());
                    //            num++;
                    //        }
                    //    }
                    //    table.Design = TableDesign.TableGrid;
                    //    table.Alignment = Alignment.center;

                    //    // 将表格插入到文档中
                    //    paragraph.InsertTableAfterSelf(table);
                    //}
                    paragraph.ReplaceText("補充公司基本資料", data.Company.Information);
                    paragraph.ReplaceText("補充盤查年度", (data.Year + 1911).ToString() + "年");
                    paragraph.ReplaceText("民國年份補充", (data.Year).ToString());
                    paragraph.ReplaceText("補充公司場所名稱", data.Company.Name);
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

                   // foreach (var item in device)
                   // {
                   //     if (item.CO2_Emission == true && item.CH4_Emission == true && item.N2O_Emission == true)
                   //     {
                   //         var materialsData = await _context.Materials//區分固定與移動
                   //.Where(x => x.EmissionPattern == item.EmissionPattern)
                   //.Where(x => x.Name == item.Material)
                   //.FirstOrDefaultAsync();

                   //     }
                   // }


                }

                // 保存新文檔
                doc.SaveAs(newFilePath);
            }

            // 返回一個視圖或其他操作，根據你的需求
            return View();
        }

        public async Task<IActionResult> Default()
        {
            var Company_id = Guid.NewGuid();
            var Area_id = Guid.NewGuid();
            await _context.Companies.AddAsync(new Company()
            {
                Id = Company_id,
                Name = "Default",
                Owner = "Default",
                Email = "Default",
                Phone = "Default",
                CreateTime = DateTime.Now
            });
            await _context.SaveChangesAsync();
            await _context.Areas.AddAsync(new Area()
            {
                Id = Area_id,
                CompanyId = Company_id,
                Name = "Default",
                PostalCode = 0,
                City = "Default",
                District = "Default",
                Address = "Default",
                Year = 111,
                Type = "Default",
                CreateTime = DateTime.Now
            });
            await _context.SaveChangesAsync();
            await _context.Devices.AddAsync(new Device()
            {
                Id = Guid.NewGuid(),
                AreaId = Area_id,
                AssetNo = "Default",
                Provess = "Default",
                Name = "緊急發電機",
                Material = "柴油",
                Scope = "類別一",

                EmissionPattern = "固定",
                CO2_Emission = true,
                CH4_Emission = true,
                N2O_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.Devices.AddAsync(new Device()
            {
                Id = new Guid(),
                AreaId = Area_id,
                AssetNo = "Default",
                Provess = "Default",
                Name = "公務車",
                Material = "柴油",
                Scope = "類別一",
                EmissionPattern = "移動",
                CO2_Emission = true,
                CH4_Emission = true,
                N2O_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.Devices.AddAsync(new Device()
            {
                Id = new Guid(),
                AreaId = Area_id,
                AssetNo = "Default",
                Provess = "Default",
                Name = "公務車",
                Material = "車用汽油",
                Scope = "類別一",
                EmissionPattern = "移動",
                CO2_Emission = true,
                CH4_Emission = true,
                N2O_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.Devices.AddAsync(new Device()
            {
                Id = new Guid(),
                AreaId = Area_id,
                AssetNo = "Default",
                Provess = "Default",
                Name = "冷氣機",
                Material = "R-410A",
                Scope = "類別一",
                EmissionPattern = "逸散",
                HFCS_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.Devices.AddAsync(new Device()
            {
                Id = new Guid(),
                AreaId = Area_id,
                AssetNo = "Default",
                Provess = "Default",
                Name = "飲水機",
                Material = "R-134A",
                Scope = "類別一",
                EmissionPattern = "逸散",
                HFCS_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.Devices.AddAsync(new Device()
            {
                Id = new Guid(),
                AreaId = Area_id,
                AssetNo = "Default",
                Provess = "Default",
                Name = "乾燥機",
                Material = "R-134A",
                Scope = "類別一",
                EmissionPattern = "逸散",
                HFCS_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.Devices.AddAsync(new Device()
            {
                Id = new Guid(),
                AreaId = Area_id,
                AssetNo = "Default",
                Provess = "Default",
                Name = "冰水主機",
                Material = "R-134A",
                Scope = "類別一",
                EmissionPattern = "逸散",
                HFCS_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.Devices.AddAsync(new Device()
            {
                Id = new Guid(),
                AreaId = Area_id,
                AssetNo = "Default",
                Provess = "Default",
                Name = "車用空調",
                Material = "R-134A",
                Scope = "類別一",
                EmissionPattern = "逸散",
                HFCS_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.Devices.AddAsync(new Device()
            {
                Id = new Guid(),
                AreaId = Area_id,
                AssetNo = "Default",
                Provess = "Default",
                Name = "化糞池",
                Material = "廢水處理",
                Scope = "類別一",
                EmissionPattern = "逸散",
                CH4_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.Devices.AddAsync(new Device()
            {
                Id = new Guid(),
                AreaId = Area_id,
                AssetNo = "Default",
                Provess = "Default",
                Name = "電力",
                Material = "外購電力",
                Scope = "類別二",
                EmissionPattern = "外購電力",
                CO2_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }


        //public IActionResult OtherAction(int id)
        //{
        //    // 從 TempData 中檢索 SelectedItemId
        //    TempData["SelectedItemId"] = id;

        //    // 重定向到 Devices 控制器的 Index 動作，並將 itemId 作為路由值傳遞
        //    return RedirectToAction("Index", "Devices", id);
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}