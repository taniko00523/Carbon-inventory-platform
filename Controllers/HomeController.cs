using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Office.Interop.Word;
using Xceed.Words.NET;


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
        //public IActionResult Word([FromServices] IWebHostEnvironment env)
        //{
        //    // 這裡要替換成你 MVC 應用程式中正確的檔案路徑
        //    string fileName = "87.docx";
        //    string filePath = Path.Combine(env.WebRootPath, fileName);

        //    // 初始化 Word 應用程式
        //    var wordApp = new Microsoft.Office.Interop.Word.Application();

        //    // 開啟文件
        //    Document doc = wordApp.Documents.Open(filePath);

        //    // 在文檔中查找要替換的文本
        //    doc.Content.Copy();

        //    // 創建一個新的文檔
        //    Document newDoc = wordApp.Documents.Add();

        //    // 將剪貼板中的內容粘貼到新文檔
        //    newDoc.Content.Paste();

        //    // 設定要查找和替換的文本
        //    string searchText = "公司概況";
        //    string replacementText = "合為至";

        //    // 遍歷文檔的所有範圍，執行查找和替換
        //    foreach (Microsoft.Office.Interop.Word.Range range in newDoc.StoryRanges)
        //    {
        //        range.Find.ClearFormatting();
        //        range.Find.Execute(searchText, ReplaceWith: replacementText, Replace: WdReplace.wdReplaceAll);
        //    }

        //    // 保存新文檔
        //    fileName = "1.docx";
        //    string newFilePath = Path.Combine(env.WebRootPath, fileName);
        //    newDoc.SaveAs2(newFilePath);

        //    // 關閉文檔
        //    doc.Close(false);
        //    newDoc.Save();
        //    newDoc.Close();

        //    // 退出 Word 應用程式
        //    wordApp.Quit();

        //    // 釋放資源
        //    System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);
        //    System.Runtime.InteropServices.Marshal.ReleaseComObject(newDoc);
        //    System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);

        //    // 返回一個視圖或其他操作，根據你的需求
        //    return View();
        //}

        public async Task<IActionResult> WordAsync(Guid id)
        {
            // 這裡要替換成你 MVC 應用程式中正確的檔案路徑
            string filePath = "D:\\專題\\Test.docx";
            string newFilePath = "D:\\專題\\1.docx";

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




                var data = await _context.Areas.Where(x => x.Id == id).Include(x => x.Company).FirstOrDefaultAsync();
                var device = await _context.Devices.Where(x => x.AreaId == id).ToListAsync();
                //var area = _context.Areas.Where(x => x.Id != id).ToListAsync();
                string rep_baseInfomation = "公司基本資料";
                string rep_Year = "111年";
                string rep_companyName = "2";
                string rep_uniformNumber = "3";
                string rep_factoryNumber = "4";
                string rep_location = "5";
                string rep_address = "6";

                // 遍歷文檔的所有段落，執行查找和替換
                foreach (var paragraph in doc.Paragraphs)
                {
                    paragraph.ReplaceText("補充公司基本資料", rep_baseInfomation);
                    paragraph.ReplaceText("補充盤查年度", (data.Year+1911).ToString() + "年");
                    paragraph.ReplaceText("補充公司場所名稱", data.Company.Name);
                    paragraph.ReplaceText("補充統一編號", data.UniqueCode.ToString());
                    paragraph.ReplaceText("補充工廠登記編號", data.FactorCode.ToString());
                    paragraph.ReplaceText("補充地址", data.City + data.District + data.Address);
                    paragraph.ReplaceText("固定排放源補充", "123");
                    paragraph.ReplaceText("移動排放源補充", rep_address);
                    paragraph.ReplaceText("逸散源補充", rep_address);
                    paragraph.ReplaceText("製程排放源補充", rep_address);
                    paragraph.ReplaceText("基準年補充", (data.Year + 1911).ToString() + "年");


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