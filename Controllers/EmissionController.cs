using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Filters;
using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Model;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Xceed.Document.NET;
using Xceed.Words.NET;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.Mime.MediaTypeNames;

namespace Carbon_inventory_platform.Controllers
{
    [CheckSubscriptionData]
    [Authorize]
    public class EmissionController : CountController
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        public EmissionController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment, UserManager<ApplicationUser> userManager) : base(context)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
        }

        // 原本報表/圖表動作只用 Area Id 查詢，任何登入者拿到別家公司的 Area Id 就能取得對方完整盤查資料，會導致跨公司資料外洩。
        private async Task<bool> CanAccessAreaAsync(Guid? areaId)
        {
            if (areaId == null || areaId == Guid.Empty)
            {
                return false;
            }
            if (User.IsInRole("Admin") || User.IsInRole("SuperAdmin")) //管理者可檢視所有公司
            {
                return true;
            }
            string? userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }
            var companyIds = await _context.Companies
                                           .Where(c => c.UserId == userId)
                                           .Select(c => c.Id)
                                           .ToListAsync();
            return await _context.Areas
                                 .AnyAsync(a => a.Id == areaId && a.isDeleted == 0 && companyIds.Contains(a.CompanyId));
        }

        // 原本檔名直接串接使用者可編輯的公司/廠區名稱且未過濾路徑字元，會導致寫檔跳出輸出資料夾，或名稱含 / : 等字元時直接 500。
        private static string BuildReportFileName(int year, string companyName, string? areaName)
        {
            string raw = year + "年度" + "-" + companyName + (!string.IsNullOrWhiteSpace(areaName) ? ("-" + areaName) : "") + "-溫室氣體盤查報告書.docx";
            string safe = string.Concat(Path.GetFileName(raw).Split(Path.GetInvalidFileNameChars()));
            return string.IsNullOrWhiteSpace(safe) ? "溫室氣體盤查報告書.docx" : safe;
        }

        // 原本占比直接除以總排放量，總排放為 0（尚無排放源、或基準年廠區沒有資料）時會導致 DivideByZeroException 而 500。
        private static string PercentText(decimal part, decimal total)
        {
            return total == 0 ? "0.00%" : (part / total * 100).ToString("N2") + "%";
        }

        public async Task<IActionResult> IndexAsync(Guid? id)
        {
            // 原本沒有檢查 Area 是否屬於登入者的公司，會導致任何登入者看到別家公司的排放數據。
            if (!await CanAccessAreaAsync(id))
            {
                return NotFound();
            }
            TempData["yearId"] = id;
            bool success = await CountEmissionAsync(id);
            if (!success)
            {
                return NotFound();
            }

            var emissions = await _context.Areas
                .Include(y => y.Company)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            return View(emissions);
        }
        public async Task<IActionResult> AR5ReportAsync(Guid? id)
        {
            // 跳轉到Area修改AR版本
            // 完後跳到MOEReportAync
            var areaId = TempData.Peek("areaId");
            return RedirectToAction("Index", "Devices", new { id = areaId });
        }
        public async Task<IActionResult> AR6ReportAsync(Guid? id)
        {
            // 跳轉到Area修改AR版本
            // 完後跳到MOEReportAync
            var areaId = TempData.Peek("areaId");
            return RedirectToAction("Index", "Devices", new { id = areaId });
        }
        public async Task<IActionResult> ChartAsync(Guid? id)
        {
            // 原本沒有檢查 Area 是否屬於登入者的公司，會導致任何登入者看到別家公司的排放圖表。
            if (!await CanAccessAreaAsync(id))
            {
                return NotFound();
            }
            TempData["yearId"] = id;
            bool success = await CountEmissionAsync(id);
            if (!success)
            {
                return NotFound();
            }

            var emissions = await _context.Areas.Include(y => y.Company).Where(x => x.Id == id).FirstOrDefaultAsync();
            return View(emissions);

        }

        //public async Task<IActionResult> testAsync(Guid id)
        //{
        //    bool success = await CountEmissionAsync(id);
        //    if (!success)
        //    {
        //        return NotFound();
        //    }

        //    var dataResult = await _context.Areas
        //                                .Where(x => x.Id == id && x.isDeleted == 0)
        //                                .Include(x => x.Company)
        //                                .Select(area => new
        //                                {
        //                                    Area = area,
        //                                    Devices = _context.Devices
        //                                                      .Where(d => d.AreaId == area.Id && d.isDeleted == 0)
        //                                                      .OrderBy(d => d.Scope)
        //                                                      .ThenBy(d => d.EmissionPattern)
        //                                                      .ToList(),
        //                                    AllGHGs = _context.Devices
        //                                                      .Where(d => d.AreaId == area.Id && d.isDeleted == 0)
        //                                                      .SelectMany(d => d.GHGs)
        //                                                      .ToList(),
        //                                    AllActivityData = _context.ActivityDatas.ToList()
        //                                })
        //                                .FirstOrDefaultAsync();

        //    if (dataResult == null)
        //        return NotFound();

        //    var data = dataResult.Area;
        //    if (data.Company == null)
        //        return NotFound();

        //    var device = dataResult.Devices;
        //    var AllGHGs = dataResult.AllGHGs;
        //    var AllActivityData = dataResult.AllActivityData;

        //    var baseYear = await _context.Areas
        //        .Where(x => x.CompanyId == data.Company.Id && x.BaseYear && x.isDeleted == 0)
        //        .Select(x => x.Year)
        //        .FirstOrDefaultAsync();

        //    if (baseYear == null)
        //        return NotFound();


        //    //-----------------檔案設定
        //    string currentDirectory = Directory.GetCurrentDirectory();
        //    string filePath = Path.Combine(currentDirectory, "wwwroot\\doc\\", "test.docx");
        //    string newFilePath = Path.Combine(currentDirectory, "wwwroot\\output\\");
        //    string fileName = data.Year + "年度" + "-" + data.Company.Name + (data.Name != null ? ("-" + data.Name) : "") + "-溫室氣體盤查報告書.docx";
        //    string newFile = Path.Combine(newFilePath, fileName);

        //    if (!Directory.Exists(newFilePath))
        //    {
        //        Directory.CreateDirectory(newFilePath);
        //    }
        //    //-----------------檔案設定

        //    using (DocX doc = DocX.Load(filePath))
        //    {
        //        //-----------------替換的文本
        //        UpdateReplacePattern("聯絡人姓名", "<test>");
        //        UpdateReplacePattern("test", "check");

        //        //--------------------------------圖片
        //        // Check if all the replace patterns are used in the loaded document.
        //        bool finishReplaceText = true;
        //        while (finishReplaceText)
        //        {
        //            if (doc.FindUniqueByPattern(@"<[\w \=]{4,}>", RegexOptions.IgnoreCase).Count > 0)
        //            {
        //                // Do the replacement of all the found tags and with green bold strings.
        //                var replaceTextOptions = new FunctionReplaceTextOptions()
        //                {
        //                    FindPattern = "<(.*?)>",
        //                    RegexMatchHandler = ReplaceFunc,
        //                    RegExOptions = RegexOptions.IgnoreCase,
        //                };
        //                finishReplaceText = doc.ReplaceText(replaceTextOptions);
        //            }
        //            else
        //            {
        //                finishReplaceText = false;
        //            }
        //        }



        //        doc.SaveAs(newFile);
        //    }

        //    byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(newFile);
        //    return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
        //}
        public async Task<IActionResult> ISOReportAsync(Guid id)
        {
            // 原本沒有檢查 Area 是否屬於登入者的公司，會導致任何登入者下載別家公司的完整盤查報告書。
            if (!await CanAccessAreaAsync(id))
            {
                return NotFound();
            }

            bool success = await CountEmissionAsync(id);
            if (!success)
            {
                return NotFound();
            }

            // 原本只過濾 GHGs 沒有過濾 Devices，已軟刪除的排放源仍會被加總，會導致報告書數字與畫面不一致。
            Area? data = await _context.Areas
                                        .Where(a => a.Id == id && a.isDeleted == 0)
                                        .Include(a => a.Company)
                                        .Include(a => a.Devices.Where(d => d.isDeleted == 0))
                                            .ThenInclude(d => d.GHGs.Where(g => g.isDeleted == 0))
                                        .Include(a => a.Analysis)
                                        .FirstOrDefaultAsync();

            if (data == null)
                return NotFound();

            if (data.Company == null)
                return NotFound();

            Analysis analysis = data.Analysis;
            var deviceOrder = new Dictionary<string, int>
                {
                    { "類別一", 1 },
                    { "類別二", 2 },
                    { "類別三", 3 },
                    { "類別四", 4 },
                    { "類別五", 5 },
                    { "類別六", 6 },
                };

            // 定義排序方法
            int SortByDeviceScope(Device device)
            {
                // 如果 GHG 的名稱在映射中，則返回對應的排序數值，否則返回 int.MaxValue
                return deviceOrder.ContainsKey(device.Name) ? deviceOrder[device.Name] : int.MaxValue;
            }

            // 使用排序方法來排序 GHGs
            List<Device> device = data.Devices.OrderBy(SortByDeviceScope).ToList();
            List<GHG> AllGHGs = device.SelectMany(d => d.GHGs).ToList();
            List<ActivityData> AllActivityData = _context.ActivityDatas.ToList();
            Area? baseYear_Area = new Area();
            if (data.BaseYear)
            {
                baseYear_Area = data;
            }
            else
            {
                baseYear_Area = await _context.Areas
                                        .Where(a => a.CompanyId == data.CompanyId && a.isDeleted == 0 && a.BaseYear)
                                        .Include(a => a.Company)
                                        .Include(a => a.Devices.Where(d => d.isDeleted == 0))
                                            .ThenInclude(d => d.GHGs.Where(g => g.isDeleted == 0))
                                        .Include(a => a.Analysis)
                                        .FirstOrDefaultAsync();
            }
            // 原本沒有檢查基準年廠區是否存在就取 .Year / .Devices，公司尚未勾選基準年時會導致 NullReferenceException(500)。
            if (baseYear_Area == null)
            {
                TempData["Error"] = "請先設定基準年";
                return RedirectToAction("Index", "Devices", new { id = id });
            }
            var baseYear = baseYear_Area.Year;
            List<Device> baseYear_device = baseYear_Area.Devices.ToList();
            //-----------------檔案設定
            // 原本用 Directory.GetCurrentDirectory() 加上 "wwwroot\\doc\\" 硬編碼反斜線，IIS/Linux 部署時會導致找不到範本檔。
            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "doc", "ISOReport.docx");
            // 原本把產生的報告書存到 wwwroot\output\，該目錄由 UseStaticFiles 匿名對外提供，會導致任何人猜檔名就下載別家公司的報告書。
            string fileName = BuildReportFileName(data.Year, data.Company.Name, data.Name);
            //-----------------檔案設定

            // 原本 _replacePatterns 是跨請求共用的 static 欄位，多人同時產生報表時會互相覆蓋彼此公司的替換文本；改為每次請求各自建立區域字典。
            var replacePatterns = new Dictionary<string, string>();
            string LocalReplaceFunc(string findStr) => replacePatterns.TryGetValue(findStr, out var replaceValue) ? replaceValue : string.Empty;

            byte[] fileBytes;
            using (MemoryStream outputStream = new MemoryStream())
            using (DocX doc = DocX.Load(filePath))
            {
                decimal SumAll_Emission = device.Sum(x => x.Emissions);
                decimal SumScope1_Emission = device.Where(x => x.Scope == "類別一").Sum(x => x.Emissions);
                decimal SumScope2_Emission = device.Where(x => x.Scope == "類別二").Sum(x => x.Emissions);
                decimal SumScope3_Emission = device.Where(x => x.Scope == "類別三").Sum(x => x.Emissions);
                decimal SumScope4_Emission = device.Where(x => x.Scope == "類別四").Sum(x => x.Emissions);
                decimal SumScope5_Emission = device.Where(x => x.Scope == "類別五").Sum(x => x.Emissions);
                decimal SumScope6_Emission = device.Where(x => x.Scope == "類別六").Sum(x => x.Emissions);

                //-----------------替換的文本
                UpdateReplacePattern(replacePatterns, "聯絡人姓名", data.Company.ContactName);
                UpdateReplacePattern(replacePatterns, "聯絡人電話", data.Company.Phone);
                UpdateReplacePattern(replacePatterns, "聯絡人電子信箱", data.Company.Email);

                UpdateReplacePattern(replacePatterns, "公司中文名稱", data.Company.Name);
                UpdateReplacePattern(replacePatterns, "公司英文名稱", data.Company.EnglishName);
                UpdateReplacePattern(replacePatterns, "西元盤查年份", (data.Year + 1911).ToString());
                UpdateReplacePattern(replacePatterns, "民國盤查年份", (data.Year).ToString());
                // 原本只有 data.Name 有值時才設定「廠區名稱」，未設定的鍵會沿用先前的殘值，會導致別家廠區名稱出現在報告書中。
                UpdateReplacePattern(replacePatterns, "廠區名稱", string.IsNullOrWhiteSpace(data.Name) ? "" : data.Name);
                UpdateReplacePattern(replacePatterns, "進行評估排放當量", data.cal_all.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "不確定性評估占比", (SumAll_Emission != 0 ? (data.cal_all / SumAll_Emission) * 100 : 0).ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "第1級評分", data.no1_Grade.ToString());
                UpdateReplacePattern(replacePatterns, "第2級評分", data.no2_Grade.ToString());
                UpdateReplacePattern(replacePatterns, "第3級評分", data.no3_Grade.ToString());
                UpdateReplacePattern(replacePatterns, "清冊等級分數補充", data.avg_Grade.ToString());
                UpdateReplacePattern(replacePatterns, "清冊級別補充", data.all_Grade.ToString());
                // 原本上限印成負號、下限印成正號（與畫面相反），會導致報告書出現負的 95% 信賴區間上限。
                UpdateReplacePattern(replacePatterns, "95上", "+" + data.UUL.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "95下", "-" + data.ULL.ToString("N2") + "%");


                if (data.Company.CompanyInformation != null)
                {
                    string trimCM = data.Company.CompanyInformation.Replace(" ", "");
                    trimCM = trimCM.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern(replacePatterns, "公司簡介", trimCM);
                }
                else
                {
                    UpdateReplacePattern(replacePatterns, "公司簡介", " ");
                }

                if (data.Company.AddressInformation != null)
                {
                    string trimAI = data.Company.AddressInformation.Replace(" ", "");
                    trimAI = trimAI.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern(replacePatterns, "組織邊界設定", trimAI);
                }
                else
                {
                    UpdateReplacePattern(replacePatterns, "組織邊界設定", "本次組織邊界之設定，遵循ISO 14064-1:2018標準，採用營運控制權法。");
                }
                if (data.Company.ReportingInformation != null)
                {
                    string trimRI = data.Company.ReportingInformation.Replace(" ", "");
                    trimRI = trimRI.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern(replacePatterns, "報告邊界", trimRI);
                }
                else
                {
                    UpdateReplacePattern(replacePatterns, "報告邊界", "");

                }
                if (data.Company.GHGInformation != null)
                {
                    string trimGI = data.Company.GHGInformation.Replace(" ", "");
                    trimGI = trimGI.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern(replacePatterns, "溫室氣體排放類型與排放量說明", trimGI);
                }
                else
                {
                    UpdateReplacePattern(replacePatterns, "溫室氣體排放類型與排放量說明", "溫室氣體之種類係指上述標準定義之溫室氣體，包括二氧化碳(CO₂)、甲烷(CH₄)、氧化亞氮(N₂O)、氫氟碳化物(HFCs)、全氟碳化物(PFCs)、六氟化硫(SF₆)、三氟化氮(NF₃)以及其他經環境部公告者，但不包含蒙特婁議定書規範之氟氯碳化物(CFCs)。");
                }
                if (data.Company.Scope1Information != null)
                {
                    string trimS1I = data.Company.Scope1Information.Replace(" ", "");
                    trimS1I = trimS1I.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern(replacePatterns, "直接溫室氣體排放說明", trimS1I);
                }
                else
                {
                    string text = "包含";

                    var DeviceList = device.Where(x => x.EmissionPattern == "固定")
                                            .GroupBy(x => x.Name)
                                            .Select(group => group.First());
                    if (DeviceList != null && DeviceList.Count() != 0)
                    {
                        int count = 0;
                        text += "固定源燃燒的直接排放，例如：";
                        foreach (var item in DeviceList)
                        {
                            count++;
                            text += item.Name;
                            if (count < DeviceList.Count())
                            {
                                text += "、";
                            }
                            else
                            {
                                text += "，";
                            }
                        }
                    }
                    else
                    {
                        text += "本公司無固定式排放源";
                    }

                    DeviceList = device.Where(x => x.EmissionPattern == "移動")
                                        .GroupBy(x => x.Name)
                                        .Select(group => group.First());
                    if (DeviceList != null && DeviceList.Count() != 0)
                    {
                        int count = 0;
                        text += "移動源燃燒的直接排放，例如：";
                        foreach (var item in DeviceList)
                        {
                            count++;
                            text += item.Name;
                            if (count < DeviceList.Count())
                            {
                                text += "、";
                            }
                            else
                            {
                                text += "，";
                            }
                        }
                    }
                    else
                    {
                        text += "本公司無移動式排放源";
                    }

                    DeviceList = device.Where(x => x.EmissionPattern == "逸散")
                                        .GroupBy(x => x.Name)
                                        .Select(group => group.First());
                    if (DeviceList != null && DeviceList.Count() != 0)
                    {
                        int count = 0;
                        text += "人為活動產生的逸散排放，例如：";
                        foreach (var item in DeviceList)
                        {
                            count++;
                            text += item.Name;
                            if (count < DeviceList.Count())
                            {
                                text += "、";
                            }
                            else
                            {
                                text += "，";
                            }
                        }
                    }
                    else
                    {
                        text += "本公司無逸散式排放源";
                    }

                    DeviceList = device.Where(x => x.EmissionPattern == "製程")
                                        .GroupBy(x => x.Name)
                                        .Select(group => group.First());
                    if (DeviceList != null && DeviceList.Count() != 0)
                    {
                        int count = 0;
                        text += "產生溫室氣體排放製程，例如：";
                        foreach (var item in DeviceList)
                        {
                            count++;
                            text += item.Name;
                            if (count < DeviceList.Count())
                            {
                                text += "、";
                            }

                        }
                    }
                    else
                    {
                        text += "本公司無製程排放源";
                    }
                    text += "。此外，本次盤查範圍無土地利用變化，也無生質燃料直接排放。";
                    UpdateReplacePattern(replacePatterns, "直接溫室氣體排放說明", text);

                }
                if (data.Company.Scope2Information != null)
                {
                    string trimS2I = data.Company.Scope2Information.Replace(" ", "");
                    trimS2I = trimS2I.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern(replacePatterns, "能源間接溫室氣體排放說明", trimS2I);
                }
                else
                {
                    UpdateReplacePattern(replacePatterns, "能源間接溫室氣體排放說明", "本廠類別二能源間接溫室氣體排放皆來自於外購電力部份。");
                }
                if (device.Find(x => x.Name == "WD40") != null)
                {
                    UpdateReplacePattern(replacePatterns, "WD40排放係數描述", "依WD-40之安全資料表(SDS)，可知其組成包含2~3 Wt%之CO₂，取其平均值2.5%；因CO₂係作為WD-40之推進劑，當使用WD-40時，亦將造成CO₂逸散，故假設每使用1單位重量之WD-40時，會有2.5%之單位重量CO₂隨之逸散。");
                }
                else
                {
                    UpdateReplacePattern(replacePatterns, "WD40排放係數描述", "");
                }
                ScopeDeviceDirections(replacePatterns, device, "固定");
                ScopeDeviceDirections(replacePatterns, device, "移動");
                ScopeDeviceDirections(replacePatterns, device, "逸散");
                UpdateReplacePattern(replacePatterns, "盤查年", DateTime.Now.Year.ToString());
                UpdateReplacePattern(replacePatterns, "盤查月", DateTime.Now.Month.ToString());
                UpdateReplacePattern(replacePatterns, "盤查日", DateTime.Now.Day.ToString());
                UpdateReplacePattern(replacePatterns, "地址", data.FullAddress);
                UpdateReplacePattern(replacePatterns, "民國基準年", baseYear.ToString());
                UpdateReplacePattern(replacePatterns, "西元基準年", (baseYear + 1911).ToString());

                if (device.Find(x => x.Name == "電力") != null)
                {
                    Guid deviceId = device.Find(x => x.Name == "電力").Id;
                    var deviceActivityData = AllActivityData.Where(x => x.DeviceId == deviceId).ToList();


                    if (deviceActivityData != null)
                    {
                        UpdateReplacePattern(replacePatterns, "電力使用量", (deviceActivityData.Sum(ad => ad.Num) / 1000).ToString());
                    }
                    else
                    {
                        UpdateReplacePattern(replacePatterns, "電力使用量", "0");
                    }
                }


                UpdateReplacePattern(replacePatterns, "類別一CO2排放", data.Scope1_CO2.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "類別一CH4排放", data.Scope1_CH4.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "類別一N2O排放", data.Scope1_N2O.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "類別一HFCS排放", data.Scope1_HFCS.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "類別一PFCS排放", data.Scope1_PFCS.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "類別一SF6排放", data.Scope1_SF6.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "類別一NF3排放", data.Scope1_NF3.ToString("N4"));

                UpdateReplacePattern(replacePatterns, "類別一CO2占比", data.percentage1_CO2.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別一CH4占比", data.percentage1_CH4.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別一N2O占比", data.percentage1_N2O.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別一HFCS占比", data.percentage1_HFCS.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別一PFCS占比", data.percentage1_PFCS.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別一SF6占比", data.percentage1_SF6.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別一NF3占比", data.percentage2_NF3.ToString("N2") + "%");

                UpdateReplacePattern(replacePatterns, "CO2排放", data.CO2.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "CH4排放", data.CH4.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "N2O排放", data.N2O.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "HFCS排放", data.HFCS.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "PFCS排放", data.PFCS.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "SF6排放", data.SF6.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "NF3排放", data.NF3.ToString("N4"));

                UpdateReplacePattern(replacePatterns, "CO2占比", data.percentage2_CO2.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "CH4占比", data.percentage2_CH4.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "N2O占比", data.percentage2_N2O.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "HFCS占比", data.percentage2_HFCS.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "PFCS占比", data.percentage2_PFCS.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "SF6占比", data.percentage2_SF6.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "NF3占比", data.percentage2_NF3.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "總排放當量", SumAll_Emission.ToString("N3"));
                UpdateReplacePattern(replacePatterns, "固定排放量", data.non_move.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "移動排放量", data.move.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "製程排放量", data.process.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "逸散排放量", data.escape.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "固定排放比例", data.percentage_nonMove.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "製程排放比例", data.percentage_Process.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "移動排放比例", data.percentage_Move.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "逸散排放比例", data.percentage_Escape.ToString("N2") + "%");

                UpdateReplacePattern(replacePatterns, "類別一占比", (SumScope1_Emission / SumAll_Emission * 100).ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別二占比", (SumScope2_Emission / SumAll_Emission * 100).ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別三占比", (SumScope3_Emission / SumAll_Emission * 100).ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別四占比", (SumScope4_Emission / SumAll_Emission * 100).ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別五占比", (SumScope5_Emission / SumAll_Emission * 100).ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別六占比", (SumScope6_Emission / SumAll_Emission * 100).ToString("N2") + "%");

                UpdateReplacePattern(replacePatterns, "類別一總排放", SumScope1_Emission.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "類別二總排放", SumScope2_Emission.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "類別三總排放", SumScope3_Emission.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "類別四總排放", SumScope4_Emission.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "類別五總排放", SumScope5_Emission.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "類別六總排放", SumScope6_Emission.ToString("N4"));

                decimal SumBaseAreaAll_Emission = baseYear_device.Sum(x => x.Emissions);
                decimal SumBaseAreaScope1_Emission = baseYear_device.Where(x => x.Scope == "類別一").Sum(x => x.Emissions);
                decimal SumBaseAreaScope2_Emission = baseYear_device.Where(x => x.Scope == "類別二").Sum(x => x.Emissions);
                decimal SumBaseAreaScope3_Emission = baseYear_device.Where(x => x.Scope == "類別三").Sum(x => x.Emissions);
                decimal SumBaseAreaScope4_Emission = baseYear_device.Where(x => x.Scope == "類別四").Sum(x => x.Emissions);
                decimal SumBaseAreaScope5_Emission = baseYear_device.Where(x => x.Scope == "類別五").Sum(x => x.Emissions);
                decimal SumBaseAreaScope6_Emission = baseYear_device.Where(x => x.Scope == "類別六").Sum(x => x.Emissions);
                UpdateReplacePattern(replacePatterns, "基準年類別一總排放", SumBaseAreaScope1_Emission.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "基準年類別二總排放", SumBaseAreaScope2_Emission.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "基準年類別三總排放", SumBaseAreaScope3_Emission.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "基準年類別四總排放", SumBaseAreaScope4_Emission.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "基準年類別五總排放", SumBaseAreaScope5_Emission.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "基準年類別六總排放", SumBaseAreaScope6_Emission.ToString("N4"));

                UpdateReplacePattern(replacePatterns, "比較類別一排放差異", SumBaseAreaScope1_Emission != 0 ? ((SumScope1_Emission - SumBaseAreaScope1_Emission) / SumBaseAreaScope1_Emission * 100).ToString("N2") + "%" : "0.00%");
                UpdateReplacePattern(replacePatterns, "比較類別二排放差異", SumBaseAreaScope2_Emission != 0 ? ((SumScope2_Emission - SumBaseAreaScope2_Emission) / SumBaseAreaScope2_Emission * 100).ToString("N2") + "%" : "0.00%");
                UpdateReplacePattern(replacePatterns, "比較類別三排放差異", SumBaseAreaScope3_Emission != 0 ? ((SumScope3_Emission - SumBaseAreaScope3_Emission) / SumBaseAreaScope3_Emission * 100).ToString("N2") + "%" : "0.00%");
                UpdateReplacePattern(replacePatterns, "比較類別四排放差異", SumBaseAreaScope4_Emission != 0 ? ((SumScope4_Emission - SumBaseAreaScope4_Emission) / SumBaseAreaScope4_Emission * 100).ToString("N2") + "%" : "0.00%");
                UpdateReplacePattern(replacePatterns, "比較類別五排放差異", SumBaseAreaScope5_Emission != 0 ? ((SumScope5_Emission - SumBaseAreaScope5_Emission) / SumBaseAreaScope5_Emission * 100).ToString("N2") + "%" : "0.00%");
                UpdateReplacePattern(replacePatterns, "比較類別六排放差異", SumBaseAreaScope6_Emission != 0 ? ((SumScope6_Emission - SumBaseAreaScope6_Emission) / SumBaseAreaScope6_Emission * 100).ToString("N2") + "%" : "0.00%");
                UpdateReplacePattern(replacePatterns, "比較總排放差異", SumBaseAreaAll_Emission != 0 ? ((SumAll_Emission - SumBaseAreaAll_Emission) / SumBaseAreaAll_Emission).ToString("N2") + "%" : "0.00%");

                UpdateReplacePattern(replacePatterns, "基準年類別一佔比", (SumBaseAreaScope1_Emission / SumBaseAreaAll_Emission * 100).ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "基準年類別二佔比", (SumBaseAreaScope2_Emission / SumBaseAreaAll_Emission * 100).ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "基準年類別三佔比", (SumBaseAreaScope3_Emission / SumBaseAreaAll_Emission * 100).ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "基準年類別四佔比", (SumBaseAreaScope4_Emission / SumBaseAreaAll_Emission * 100).ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "基準年類別五佔比", (SumBaseAreaScope5_Emission / SumBaseAreaAll_Emission * 100).ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "基準年類別六佔比", (SumBaseAreaScope6_Emission / SumBaseAreaAll_Emission * 100).ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "基準年總排放當量", SumBaseAreaAll_Emission.ToString("N3"));

                if (data.BaseYear) //如果當年為基準年
                {
                    UpdateReplacePattern(replacePatterns, "當年度與基準年排放比較敘述", "因本年度為基準年，故無法進行差異性比較。");
                }
                else
                {
                    UpdateReplacePattern(replacePatterns, "當年度與基準年排放比較敘述", "");
                }
                //-----------------替換的文本
                //--------------------------------排放源溫室氣體表表
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "固定", "CO2");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "固定", "CH4");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "固定", "N2O");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "移動", "CO2");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "移動", "CH4");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "移動", "N2O");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "逸散", "CO2");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "逸散", "CH4");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "逸散", "HFCS");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "製程", "CO2");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "外購電力", "CO2");
                //--------------------------------排放源溫室氣體表表
                //--------------------------------類別表
                if (device.Any(x => x.Scope == "類別一"))
                {
                    GenerateScopeTable(doc, device, AllGHGs, "類別一");
                }
                // 檢查是否有類別二設備
                if (device.Any(x => x.Scope == "類別二"))
                {
                    GenerateScopeTable(doc, device, AllGHGs, "類別二");
                }
                //--------------------------------類別表
                //--------------------------------報告邊界
                GenerateReport(doc, device, AllGHGs);
                //--------------------------------報告邊界
                GenerateActivityDataTable(doc, device, AllActivityData);

                //--------------------------------圖片                               
                if (!string.IsNullOrEmpty(data.ShopDrawingsPath))
                {
                    try
                    {
                        string showDrawingPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", data.CompanyId.ToString(), data.Id.ToString(), data.ShopDrawingsPath);
                        replaceImage(doc, "廠區圖", showDrawingPath);
                    }
                    catch (Exception e)
                    {
                        TempData["Error"] = "請上傳圖片檔案";
                        Guid areaId = (Guid)TempData.Peek("areaId");
                        return RedirectToAction("Index", "Devices", new { id = areaId });
                        throw;
                    }
                }
                else
                {
                    UpdateReplacePattern(replacePatterns, "廠區圖", "");
                }
                if (!string.IsNullOrEmpty(data.MapImagePath))
                {
                    try
                    {
                        string mapPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", data.CompanyId.ToString(), data.Id.ToString(), data.MapImagePath);
                        replaceImage(doc, "地理位置圖", mapPath);
                    }
                    catch (Exception e)
                    {
                        TempData["Error"] = "請上傳圖片檔案";
                        Guid areaId = (Guid)TempData.Peek("areaId");
                        return RedirectToAction("Index", "Devices", new { id = areaId });
                        throw;
                    }

                }
                else
                {
                    UpdateReplacePattern(replacePatterns, "地理位置圖", "");
                }
                //--------------------------------圖片
                // Check if all the replace patterns are used in the loaded document.
                if (doc.FindUniqueByPattern(@"<[\w \=]{4,}>", RegexOptions.IgnoreCase).Count > 0)
                {
                    // Do the replacement of all the found tags and with green bold strings.
                    var replaceTextOptions = new FunctionReplaceTextOptions()
                    {
                        FindPattern = "<(.*?)>",
                        RegexMatchHandler = LocalReplaceFunc,
                        RegExOptions = RegexOptions.IgnoreCase,
                    };
                    doc.ReplaceText(replaceTextOptions);
                }

                // 原本 SaveAs 到 wwwroot\output\ 再讀回，除了公開外洩外，同名檔案也會讓並行下載互相鎖檔；改為直接輸出到記憶體。
                doc.SaveAs(outputStream);
                fileBytes = outputStream.ToArray();
            }

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
        }
        public async Task<IActionResult> MOEReportAsync(Guid id)
        {
            // 原本沒有檢查 Area 是否屬於登入者的公司，會導致任何登入者下載別家公司的完整盤查報告書。
            if (!await CanAccessAreaAsync(id))
            {
                return NotFound();
            }

            bool success = await CountEmissionAsync(id);
            if (!success)
            {
                return NotFound();
            }

            var dataResult = await _context.Areas
                                        .Where(x => x.Id == id && x.isDeleted == 0)
                                        .Include(x => x.Company)
                                        .Select(area => new
                                        {
                                            Area = area,
                                            Devices = _context.Devices
                                                              .Where(d => d.AreaId == area.Id && d.isDeleted == 0)
                                                              .OrderBy(d => d.Scope)
                                                              .ThenBy(d => d.EmissionPattern)
                                                              .ToList(),
                                            AllGHGs = _context.Devices
                                                              .Where(d => d.AreaId == area.Id && d.isDeleted == 0)
                                                              .SelectMany(d => d.GHGs)
                                                              .ToList(),
                                            AllActivityData = _context.ActivityDatas.ToList()
                                        })
                                        .FirstOrDefaultAsync();

            if (dataResult == null)
                return NotFound();

            var data = dataResult.Area;
            if (data.Company == null)
                return NotFound();

            var device = dataResult.Devices;
            var AllGHGs = dataResult.AllGHGs;
            var AllActivityData = dataResult.AllActivityData;

            // 原本用 .Select(x => x.Year) 取得不可為 null 的 int，「baseYear == null」永遠不成立，公司尚未設定基準年時會直接把 0 當作基準年印進報告書。
            var baseYear_Area = await _context.Areas
                .Where(x => x.CompanyId == data.Company.Id && x.BaseYear && x.isDeleted == 0)
                .FirstOrDefaultAsync();

            if (baseYear_Area == null)
            {
                TempData["Error"] = "請先設定基準年";
                return RedirectToAction("Index", "Devices", new { id = id });
            }
            var baseYear = baseYear_Area.Year;

            //-----------------檔案設定
            // 原本用 Directory.GetCurrentDirectory() 加上 "wwwroot\\doc\\" 硬編碼反斜線，IIS/Linux 部署時會導致找不到範本檔。
            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "doc", "MOEReport.docx");
            // 原本把產生的報告書存到 wwwroot\output\，該目錄由 UseStaticFiles 匿名對外提供，會導致任何人猜檔名就下載別家公司的報告書。
            string fileName = BuildReportFileName(data.Year, data.Company.Name, data.Name);
            //-----------------檔案設定

            // 原本 _replacePatterns 是跨請求共用的 static 欄位，多人同時產生報表時會互相覆蓋彼此公司的替換文本；改為每次請求各自建立區域字典。
            var replacePatterns = new Dictionary<string, string>();
            string LocalReplaceFunc(string findStr) => replacePatterns.TryGetValue(findStr, out var replaceValue) ? replaceValue : string.Empty;

            byte[] fileBytes;
            using (MemoryStream outputStream = new MemoryStream())
            using (DocX doc = DocX.Load(filePath))
            {
                //-----------------替換的文本
                UpdateReplacePattern(replacePatterns, "聯絡人姓名", data.Company.ContactName);
                UpdateReplacePattern(replacePatterns, "聯絡人電話", data.Company.Phone);
                UpdateReplacePattern(replacePatterns, "聯絡人電子信箱", data.Company.Email);

                UpdateReplacePattern(replacePatterns, "公司中文名稱", data.Company.Name);
                UpdateReplacePattern(replacePatterns, "西元盤查年份", (data.Year + 1911).ToString());
                UpdateReplacePattern(replacePatterns, "民國盤查年份", (data.Year).ToString());
                if (data.Name != null)
                {
                    if (data.Name.Trim() != "")
                    {
                        UpdateReplacePattern(replacePatterns, "廠區名稱", data.Name);
                    }
                }
                UpdateReplacePattern(replacePatterns, "進行評估排放當量", data.cal_all.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "不確定性評估占比", data.percentage_CalAll.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "第1級評分", data.no1_Grade.ToString());
                UpdateReplacePattern(replacePatterns, "第2級評分", data.no2_Grade.ToString());
                UpdateReplacePattern(replacePatterns, "第3級評分", data.no3_Grade.ToString());
                UpdateReplacePattern(replacePatterns, "清冊等級分數補充", data.avg_Grade.ToString());
                UpdateReplacePattern(replacePatterns, "清冊級別補充", data.all_Grade.ToString());
                // 原本上限印成負號、下限印成正號（與畫面相反），會導致報告書出現負的 95% 信賴區間上限。
                UpdateReplacePattern(replacePatterns, "95上", "+" + data.UUL.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "95下", "-" + data.ULL.ToString("N2") + "%");

                if (data.Company.ReportOpening != null)
                {
                    string trimRO = data.Company.ReportOpening.Replace(" ", "");
                    trimRO = trimRO.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern(replacePatterns, "前言", trimRO);
                }
                else
                {
                    UpdateReplacePattern(replacePatterns, "前言", "自1997年12月第三次締約國大會(COP3)簽署京都議定書後，全球先進國家均研擬因應溫室氣體減量的方法與措施，2005年2月京都議定書正式生效後，全球各國更積極建立了溫室氣體排放管制的共識，2007年12月巴里路線圖進一步強調開發中國家應推動可量測(Measurable)、報告(Reportable)及可供查證(Verifiable)之適當減緩行動，2009年12月丹麥哥本哈根會議更針對後京都世界各國溫室氣體減量提出可行方案。\r\n    聯合國環境規劃署在2014年「溫室氣體排放缺口報告」指出，全球碳中和應於2055年至2070年間達成，否則地球將面臨氣候變遷災難。2015年聯合國發布「2030年永續發展方針」，公布17項永續發展目標(SDGs)，為人類和地球的「和平與繁榮」提供了現在與未來的共享藍圖。基於全球減碳趨勢，我國響應聯合國氣候變遷的目標，亦提出「2050淨零路徑里程碑」，隨後於112年1月10通過「氣候變遷因應法」，以達成節能減碳之永續發展目。\r\n    " + data.Company.Name + "(以下簡稱本公司)為因應全球永續發展趨勢於 2015年底《聯合國氣候變化綱要公約》第21屆締約國(COP21)後「巴黎協議」產生，加上環境部《溫室氣體減量及管理法》於 2015年7月正式公布實施，配合國家整體溫室氣體減量策略發展，以達成節能減碳之永續發展目標，特配合政府政策，持續進行公司內部溫室氣體盤查，以瞭解溫室氣體排放實況，進而訂定改善措施，以求達成二氧化碳排放減量之目標。 \r\n    本公司基於關心全球氣候變遷、善用資源及善盡企業的責任，根據ISO/IEC 14064-1： 2018要求，對溫室氣體管制發展趨勢及因應未來溫室氣體減量之要求，進行系統化的溫室氣體排放盤查 與清冊建置及查證程序等推動計畫，提供日後實施有效的減量改善方案作參考。今後除將持續推動溫室氣體排放管制以降低成本外，並期盼能達成兼顧資源效率、能源節約、環境保護的永續能源發展，共同為產業朝向低碳型經濟社會來努力。");
                }
                if (data.Company.CompanyInformation != null)
                {
                    string trimCM = data.Company.CompanyInformation.Replace(" ", "");
                    trimCM = trimCM.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern(replacePatterns, "公司簡介", trimCM);
                }
                else
                {

                    UpdateReplacePattern(replacePatterns, "公司簡介", " ");
                }
                if (data.Company.ReportingPurposes != null)
                {
                    string trimRP = data.Company.ReportingPurposes.Replace(" ", "");
                    trimRP = trimRP.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern(replacePatterns, "預期用途", trimRP);
                }
                else
                {
                    UpdateReplacePattern(replacePatterns, "預期用途", "為接軌國際議題，提升競爭力，以及實施社會責任，進行本報告書撰寫，以展現本公司溫室氣體盤查結果及推動減碳的決心。");
                }

                if (data.Company.AddressInformation != null)
                {
                    string trimAI = data.Company.AddressInformation.Replace(" ", "");
                    trimAI = trimAI.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern(replacePatterns, "組織邊界設定", trimAI);
                }
                else
                {
                    UpdateReplacePattern(replacePatterns, "組織邊界設定", "本次組織邊界之設定，遵循ISO 14064-1:2018標準，採用營運控制權法。");
                }
                if (data.Company.ReportingInformation != null)
                {
                    string trimRI = data.Company.ReportingInformation.Replace(" ", "");
                    trimRI = trimRI.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern(replacePatterns, "報告邊界", trimRI);
                }
                else
                {
                    UpdateReplacePattern(replacePatterns, "報告邊界", "");

                }
                if (data.Company.GHGInformation != null)
                {
                    string trimGI = data.Company.GHGInformation.Replace(" ", "");
                    trimGI = trimGI.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern(replacePatterns, "溫室氣體排放類型與排放量說明", trimGI);
                }
                else
                {
                    UpdateReplacePattern(replacePatterns, "溫室氣體排放類型與排放量說明", "溫室氣體之種類係指上述標準定義之溫室氣體，包括二氧化碳(CO₂)、甲烷(CH₄)、氧化亞氮(N₂O)、氫氟碳化物(HFCs)、全氟碳化物(PFCs)、六氟化硫(SF₆)、三氟化氮(NF₃)以及其他經環境部公告者，但不包含蒙特婁議定書規範之氟氯碳化物(CFCs)。");
                }
                if (data.Company.Scope1Information != null)
                {
                    string trimS1I = data.Company.Scope1Information.Replace(" ", "");
                    trimS1I = trimS1I.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern(replacePatterns, "直接溫室氣體排放說明", trimS1I);
                }
                else
                {
                    string text = "包含";

                    var DeviceList = device.Where(x => x.EmissionPattern == "固定")
                                            .GroupBy(x => x.Name)
                                            .Select(group => group.First());
                    if (DeviceList != null && DeviceList.Count() != 0)
                    {
                        int count = 0;
                        text += "固定源燃燒的直接排放，例如：";
                        foreach (var item in DeviceList)
                        {
                            count++;
                            text += item.Name;
                            if (count < DeviceList.Count())
                            {
                                text += "、";
                            }
                            else
                            {
                                text += "，";
                            }
                        }
                    }
                    else
                    {
                        text += "本公司無固定式排放源";
                    }

                    DeviceList = device.Where(x => x.EmissionPattern == "移動")
                                        .GroupBy(x => x.Name)
                                        .Select(group => group.First());
                    if (DeviceList != null && DeviceList.Count() != 0)
                    {
                        int count = 0;
                        text += "移動源燃燒的直接排放，例如：";
                        foreach (var item in DeviceList)
                        {
                            count++;
                            text += item.Name;
                            if (count < DeviceList.Count())
                            {
                                text += "、";
                            }
                            else
                            {
                                text += "，";
                            }
                        }
                    }
                    else
                    {
                        text += "本公司無移動式排放源";
                    }

                    DeviceList = device.Where(x => x.EmissionPattern == "逸散")
                                        .GroupBy(x => x.Name)
                                        .Select(group => group.First());
                    if (DeviceList != null && DeviceList.Count() != 0)
                    {
                        int count = 0;
                        text += "人為活動產生的逸散排放，例如：";
                        foreach (var item in DeviceList)
                        {
                            count++;
                            text += item.Name;
                            if (count < DeviceList.Count())
                            {
                                text += "、";
                            }
                            else
                            {
                                text += "，";
                            }
                        }
                    }
                    else
                    {
                        text += "本公司無逸散式排放源";
                    }

                    DeviceList = device.Where(x => x.EmissionPattern == "製程")
                                        .GroupBy(x => x.Name)
                                        .Select(group => group.First());
                    if (DeviceList != null && DeviceList.Count() != 0)
                    {
                        int count = 0;
                        text += "產生溫室氣體排放製程，例如：";
                        foreach (var item in DeviceList)
                        {
                            count++;
                            text += item.Name;
                            if (count < DeviceList.Count())
                            {
                                text += "、";
                            }

                        }
                    }
                    else
                    {
                        text += "本公司無製程排放源";
                    }
                    text += "。此外，本次盤查範圍無土地利用變化，也無生質燃料直接排放。";
                    UpdateReplacePattern(replacePatterns, "直接溫室氣體排放說明", text);

                }
                if (data.Company.Scope2Information != null)
                {
                    string trimS2I = data.Company.Scope2Information.Replace(" ", "");
                    trimS2I = trimS2I.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern(replacePatterns, "能源間接溫室氣體排放說明", trimS2I);
                }
                else
                {
                    UpdateReplacePattern(replacePatterns, "能源間接溫室氣體排放說明", "本廠類別二能源間接溫室氣體排放皆來自於外購電力部份。");
                }
                if (device.Find(x => x.Name == "WD40") != null)
                {
                    UpdateReplacePattern(replacePatterns, "WD40排放係數描述", "依WD-40之安全資料表(SDS)，可知其組成包含2~3 Wt%之CO₂，取其平均值2.5%；因CO₂係作為WD-40之推進劑，當使用WD-40時，亦將造成CO₂逸散，故假設每使用1單位重量之WD-40時，會有2.5%之單位重量CO₂隨之逸散。");
                }
                else
                {
                    UpdateReplacePattern(replacePatterns, "WD40排放係數描述", "");
                }
                UpdateReplacePattern(replacePatterns, "盤查年", DateTime.Now.Year.ToString());
                UpdateReplacePattern(replacePatterns, "盤查月", DateTime.Now.Month.ToString());
                UpdateReplacePattern(replacePatterns, "盤查日", DateTime.Now.Day.ToString());
                UpdateReplacePattern(replacePatterns, "地址", data.FullAddress);
                UpdateReplacePattern(replacePatterns, "民國基準年", baseYear.ToString());
                UpdateReplacePattern(replacePatterns, "西元基準年", (baseYear + 1911).ToString());

                ScopeDevice(replacePatterns, device, "類別一");
                ScopeDevice(replacePatterns, device, "類別二");
                if (device.Find(x => x.Name == "電力") != null)
                {
                    Guid deviceId = device.Find(x => x.Name == "電力").Id;
                    var deviceActivityData = AllActivityData.Where(x => x.DeviceId == deviceId).ToList();


                    if (deviceActivityData != null)
                    {
                        UpdateReplacePattern(replacePatterns, "電力使用量", (deviceActivityData.Sum(ad => ad.Num) / 1000).ToString());
                    }
                    else
                    {
                        UpdateReplacePattern(replacePatterns, "電力使用量", "0");
                    }
                }


                UpdateReplacePattern(replacePatterns, "類別一CO2排放", data.Scope1_CO2.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "類別一CH4排放", data.Scope1_CH4.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "類別一N2O排放", data.Scope1_N2O.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "類別一HFCS排放", data.Scope1_HFCS.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "類別一PFCS排放", data.Scope1_PFCS.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "類別一SF6排放", data.Scope1_SF6.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "類別一NF3排放", data.Scope1_NF3.ToString("N4"));

                UpdateReplacePattern(replacePatterns, "類別一CO2占比", data.percentage1_CO2.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別一CH4占比", data.percentage1_CH4.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別一N2O占比", data.percentage1_N2O.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別一HFCS占比", data.percentage1_HFCS.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別一PFCS占比", data.percentage1_PFCS.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別一SF6占比", data.percentage1_SF6.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別一NF3占比", data.percentage2_NF3.ToString("N2") + "%");

                UpdateReplacePattern(replacePatterns, "CO2排放", data.CO2.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "CH4排放", data.CH4.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "N2O排放", data.N2O.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "HFCS排放", data.HFCS.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "PFCS排放", data.PFCS.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "SF6排放", data.SF6.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "NF3排放", data.NF3.ToString("N4"));

                UpdateReplacePattern(replacePatterns, "CO2占比", data.percentage2_CO2.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "CH4占比", data.percentage2_CH4.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "N2O占比", data.percentage2_N2O.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "HFCS占比", data.percentage2_HFCS.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "PFCS占比", data.percentage2_PFCS.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "SF6占比", data.percentage2_SF6.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "NF3占比", data.percentage2_NF3.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "總排放當量", data.All.ToString("N3"));
                UpdateReplacePattern(replacePatterns, "固定排放量", data.non_move.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "移動排放量", data.move.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "製程排放量", data.process.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "逸散排放量", data.escape.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "固定排放比例", data.percentage_nonMove.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "製程排放比例", data.percentage_Process.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "移動排放比例", data.percentage_Move.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "逸散排放比例", data.percentage_Escape.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別一占比", data.percentage_Scope1.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別二占比", data.percentage_Scope2.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別一總排放", data.Scope1.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "類別二總排放", data.Scope2.ToString("N4"));
                //-----------------替換的文本
                //--------------------------------排放源溫室氣體表表
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "固定", "CO2");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "固定", "CH4");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "固定", "N2O");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "移動", "CO2");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "移動", "CH4");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "移動", "N2O");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "逸散", "CO2");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "逸散", "CH4");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "逸散", "HFCS");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "製程", "CO2");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "外購電力", "CO2");
                //--------------------------------排放源溫室氣體表表
                //--------------------------------類別表
                if (device.Any(x => x.Scope == "類別一"))
                {
                    GenerateScopeTable(doc, device, AllGHGs, "類別一");
                }
                // 檢查是否有類別二設備
                if (device.Any(x => x.Scope == "類別二"))
                {
                    GenerateScopeTable(doc, device, AllGHGs, "類別二");
                }
                //--------------------------------類別表
                //--------------------------------報告邊界
                GenerateReport(doc, device, AllGHGs);
                //--------------------------------報告邊界
                GenerateActivityDataTable(doc, device, AllActivityData);

                //--------------------------------圖片                               
                if (!string.IsNullOrEmpty(data.ShopDrawingsPath))
                {
                    try
                    {
                        string showDrawingPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", data.CompanyId.ToString(), data.Id.ToString(), data.ShopDrawingsPath);
                        // 原本用 "廠區圖"（無角括號）比對，範本內實際文字為 "<廠區圖>"，Contains 雖仍能命中，但 ReplaceText 只會清掉內層文字，留下多餘的 "<>"；改用與範本一致的角括號版本。
                        replaceImage(doc, "<廠區圖>", showDrawingPath);
                    }
                    catch (Exception e)
                    {
                        TempData["Error"] = "請上傳圖片檔案";
                        Guid areaId = (Guid)TempData.Peek("areaId");
                        return RedirectToAction("Index", "Devices", new { id = areaId });
                        throw;
                    }
                }
                else
                {
                    UpdateReplacePattern(replacePatterns, "廠區圖", "");
                }
                if (!string.IsNullOrEmpty(data.MapImagePath))
                {
                    try
                    {
                        string mapPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", data.CompanyId.ToString(), data.Id.ToString(), data.MapImagePath);
                        replaceImage(doc, "<地理位置圖>", mapPath);
                    }
                    catch (Exception e)
                    {
                        TempData["Error"] = "請上傳圖片檔案";
                        Guid areaId = (Guid)TempData.Peek("areaId");
                        return RedirectToAction("Index", "Devices", new { id = areaId });
                        throw;
                    }

                }
                else
                {
                    UpdateReplacePattern(replacePatterns, "地理位置圖", "");
                }
                //--------------------------------圖片
                // Check if all the replace patterns are used in the loaded document.
                if (doc.FindUniqueByPattern(@"<[\w \=]{4,}>", RegexOptions.IgnoreCase).Count > 0)
                {
                    // Do the replacement of all the found tags and with green bold strings.
                    var replaceTextOptions = new FunctionReplaceTextOptions()
                    {
                        FindPattern = "<(.*?)>",
                        RegexMatchHandler = LocalReplaceFunc,
                        RegExOptions = RegexOptions.IgnoreCase,
                    };
                    doc.ReplaceText(replaceTextOptions);
                }

                // 原本 SaveAs 到 wwwroot\output\ 再讀回，除了公開外洩外，同名檔案也會讓並行下載互相鎖檔；改為直接輸出到記憶體。
                doc.SaveAs(outputStream);
                fileBytes = outputStream.ToArray();
            }

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
        }
        public async Task<IActionResult> IISReportAsync(Guid id)
        {
            // 原本沒有檢查 Area 是否屬於登入者的公司，會導致任何登入者下載別家公司的完整盤查報告書。
            if (!await CanAccessAreaAsync(id))
            {
                return NotFound();
            }

            bool success = await CountEmissionAsync(id);
            if (!success)
            {
                return NotFound();
            }


            var dataResult = await _context.Areas
                                        .Where(x => x.Id == id && x.isDeleted == 0)
                                        .Include(x => x.Company)
                                        .Select(area => new
                                        {
                                            Area = area,
                                            Devices = _context.Devices
                                                              .Where(d => d.AreaId == area.Id && d.isDeleted == 0)
                                                              .OrderBy(d => d.Scope)
                                                              .ThenBy(d => d.EmissionPattern)
                                                              .ToList(),
                                            AllGHGs = _context.Devices
                                                              .Where(d => d.AreaId == area.Id && d.isDeleted == 0)
                                                              .SelectMany(d => d.GHGs)
                                                              .ToList(),
                                            AllActivityData = _context.ActivityDatas.ToList()
                                        })
                                        .FirstOrDefaultAsync();

            if (dataResult == null)
                return NotFound();

            var data = dataResult.Area;
            if (data.Company == null)
                return NotFound();

            var device = dataResult.Devices;
            var AllGHGs = dataResult.AllGHGs;
            var AllActivityData = dataResult.AllActivityData;

            // 原本用 .Select(x => x.Year) 取得不可為 null 的 int，「baseYear == null」永遠不成立；且沒有濾除已軟刪除的基準年廠區，公司尚未設定基準年時會把 0 當作基準年印進報告書。
            var baseYear_Area = await _context.Areas
                .Where(x => x.CompanyId == data.Company.Id && x.BaseYear && x.isDeleted == 0)
                .FirstOrDefaultAsync();

            if (baseYear_Area == null)
            {
                TempData["Error"] = "請先設定基準年";
                return RedirectToAction("Index", "Devices", new { id = id });
            }
            var baseYear = baseYear_Area.Year;

            //-----------------檔案設定
            // 原本用 Directory.GetCurrentDirectory() 加上 "wwwroot\\doc\\" 硬編碼反斜線，IIS/Linux 部署時會導致找不到範本檔。
            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "doc", "IIIReport.docx");
            // 原本把產生的報告書存到 wwwroot\output\，該目錄由 UseStaticFiles 匿名對外提供，會導致任何人猜檔名就下載別家公司的報告書。
            string fileName = BuildReportFileName(data.Year, data.Company.Name, data.Name);
            //-----------------檔案設定

            // 原本 _replacePatterns 是跨請求共用的 static 欄位，多人同時產生報表時會互相覆蓋彼此公司的替換文本；改為每次請求各自建立區域字典。
            var replacePatterns = new Dictionary<string, string>();
            string LocalReplaceFunc(string findStr) => replacePatterns.TryGetValue(findStr, out var replaceValue) ? replaceValue : string.Empty;

            byte[] fileBytes;
            using (MemoryStream outputStream = new MemoryStream())
            using (DocX doc = DocX.Load(filePath))
            {
                //-----------------替換的文本
                UpdateReplacePattern(replacePatterns, "聯絡人姓名", data.Company.ContactName);
                UpdateReplacePattern(replacePatterns, "聯絡人電話", data.Company.Phone);
                UpdateReplacePattern(replacePatterns, "聯絡人電子信箱", data.Company.Email);

                UpdateReplacePattern(replacePatterns, "公司中文名稱", data.Company.Name);
                UpdateReplacePattern(replacePatterns, "西元盤查年份", (data.Year + 1911).ToString());
                UpdateReplacePattern(replacePatterns, "民國盤查年份", (data.Year).ToString());
                if (data.Name != null)
                {
                    if (data.Name.Trim() != "")
                    {
                        UpdateReplacePattern(replacePatterns, "廠區名稱", data.Name);
                    }
                }
                UpdateReplacePattern(replacePatterns, "進行評估排放當量", data.cal_all.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "不確定性評估占比", data.percentage_CalAll.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "第1級評分", data.no1_Grade.ToString());
                UpdateReplacePattern(replacePatterns, "第2級評分", data.no2_Grade.ToString());
                UpdateReplacePattern(replacePatterns, "第3級評分", data.no3_Grade.ToString());
                UpdateReplacePattern(replacePatterns, "清冊等級分數補充", data.avg_Grade.ToString());
                UpdateReplacePattern(replacePatterns, "清冊級別補充", data.all_Grade.ToString());
                // 原本上限印成負號、下限印成正號（與畫面相反），會導致報告書出現負的 95% 信賴區間上限。
                UpdateReplacePattern(replacePatterns, "95上", "+" + data.UUL.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "95下", "-" + data.ULL.ToString("N2") + "%");

                if (data.Company.ReportOpening != null)
                {
                    string trimRO = data.Company.ReportOpening.Replace(" ", "");
                    trimRO = trimRO.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern(replacePatterns, "前言", trimRO);
                }
                else
                {
                    UpdateReplacePattern(replacePatterns, "前言", "自1997年12月第三次締約國大會(COP3)簽署京都議定書後，全球先進國家均研擬因應溫室氣體減量的方法與措施，2005年2月京都議定書正式生效後，全球各國更積極建立了溫室氣體排放管制的共識，2007年12月巴里路線圖進一步強調開發中國家應推動可量測(Measurable)、報告(Reportable)及可供查證(Verifiable)之適當減緩行動，2009年12月丹麥哥本哈根會議更針對後京都世界各國溫室氣體減量提出可行方案。\r\n    聯合國環境規劃署在2014年「溫室氣體排放缺口報告」指出，全球碳中和應於2055年至2070年間達成，否則地球將面臨氣候變遷災難。2015年聯合國發布「2030年永續發展方針」，公布17項永續發展目標(SDGs)，為人類和地球的「和平與繁榮」提供了現在與未來的共享藍圖。基於全球減碳趨勢，我國響應聯合國氣候變遷的目標，亦提出「2050淨零路徑里程碑」，隨後於112年1月10通過「氣候變遷因應法」，以達成節能減碳之永續發展目。\r\n    " + data.Company.Name + "(以下簡稱本公司)為因應全球永續發展趨勢於 2015年底《聯合國氣候變化綱要公約》第21屆締約國(COP21)後「巴黎協議」產生，加上環境部《溫室氣體減量及管理法》於 2015年7月正式公布實施，配合國家整體溫室氣體減量策略發展，以達成節能減碳之永續發展目標，特配合政府政策，持續進行公司內部溫室氣體盤查，以瞭解溫室氣體排放實況，進而訂定改善措施，以求達成二氧化碳排放減量之目標。 \r\n    本公司基於關心全球氣候變遷、善用資源及善盡企業的責任，根據ISO/IEC 14064-1： 2018要求，對溫室氣體管制發展趨勢及因應未來溫室氣體減量之要求，進行系統化的溫室氣體排放盤查 與清冊建置及查證程序等推動計畫，提供日後實施有效的減量改善方案作參考。今後除將持續推動溫室氣體排放管制以降低成本外，並期盼能達成兼顧資源效率、能源節約、環境保護的永續能源發展，共同為產業朝向低碳型經濟社會來努力。");
                }
                if (data.Company.CompanyInformation != null)
                {
                    string trimCM = data.Company.CompanyInformation.Replace(" ", "");
                    trimCM = trimCM.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern(replacePatterns, "公司簡介", trimCM);
                }
                else
                {

                    UpdateReplacePattern(replacePatterns, "公司簡介", " ");
                }
                if (data.Company.ReportingPurposes != null)
                {
                    string trimRP = data.Company.ReportingPurposes.Replace(" ", "");
                    trimRP = trimRP.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern(replacePatterns, "預期用途", trimRP);
                }
                else
                {
                    UpdateReplacePattern(replacePatterns, "預期用途", "為接軌國際議題，提升競爭力，以及實施社會責任，進行本報告書撰寫，以展現本公司溫室氣體盤查結果及推動減碳的決心。");
                }

                if (data.Company.AddressInformation != null)
                {
                    string trimAI = data.Company.AddressInformation.Replace(" ", "");
                    trimAI = trimAI.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern(replacePatterns, "組織邊界設定", trimAI);
                }
                else
                {
                    UpdateReplacePattern(replacePatterns, "組織邊界設定", "本次組織邊界之設定，遵循ISO 14064-1:2018標準，採用營運控制權法。");
                }
                if (data.Company.ReportingInformation != null)
                {
                    string trimRI = data.Company.ReportingInformation.Replace(" ", "");
                    trimRI = trimRI.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern(replacePatterns, "報告邊界", trimRI);
                }
                else
                {
                    UpdateReplacePattern(replacePatterns, "報告邊界", "");

                }
                if (data.Company.GHGInformation != null)
                {
                    string trimGI = data.Company.GHGInformation.Replace(" ", "");
                    trimGI = trimGI.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern(replacePatterns, "溫室氣體排放類型與排放量說明", trimGI);
                }
                else
                {
                    UpdateReplacePattern(replacePatterns, "溫室氣體排放類型與排放量說明", "溫室氣體之種類係指上述標準定義之溫室氣體，包括二氧化碳(CO₂)、甲烷(CH₄)、氧化亞氮(N₂O)、氫氟碳化物(HFCs)、全氟碳化物(PFCs)、六氟化硫(SF₆)、三氟化氮(NF₃)以及其他經環境部公告者，但不包含蒙特婁議定書規範之氟氯碳化物(CFCs)。");
                }
                if (data.Company.Scope1Information != null)
                {
                    string trimS1I = data.Company.Scope1Information.Replace(" ", "");
                    trimS1I = trimS1I.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern(replacePatterns, "直接溫室氣體排放說明", trimS1I);
                }
                else
                {
                    string text = "包含";

                    var DeviceList = device.Where(x => x.EmissionPattern == "固定")
                                            .GroupBy(x => x.Name)
                                            .Select(group => group.First());
                    if (DeviceList != null && DeviceList.Count() != 0)
                    {
                        int count = 0;
                        text += "固定源燃燒的直接排放，例如：";
                        foreach (var item in DeviceList)
                        {
                            count++;
                            text += item.Name;
                            if (count < DeviceList.Count())
                            {
                                text += "、";
                            }
                            else
                            {
                                text += "，";
                            }
                        }
                    }
                    else
                    {
                        text += "本公司無固定式排放源";
                    }

                    DeviceList = device.Where(x => x.EmissionPattern == "移動")
                                        .GroupBy(x => x.Name)
                                        .Select(group => group.First());
                    if (DeviceList != null && DeviceList.Count() != 0)
                    {
                        int count = 0;
                        text += "移動源燃燒的直接排放，例如：";
                        foreach (var item in DeviceList)
                        {
                            count++;
                            text += item.Name;
                            if (count < DeviceList.Count())
                            {
                                text += "、";
                            }
                            else
                            {
                                text += "，";
                            }
                        }
                    }
                    else
                    {
                        text += "本公司無移動式排放源";
                    }

                    DeviceList = device.Where(x => x.EmissionPattern == "逸散")
                                        .GroupBy(x => x.Name)
                                        .Select(group => group.First());
                    if (DeviceList != null && DeviceList.Count() != 0)
                    {
                        int count = 0;
                        text += "人為活動產生的逸散排放，例如：";
                        foreach (var item in DeviceList)
                        {
                            count++;
                            text += item.Name;
                            if (count < DeviceList.Count())
                            {
                                text += "、";
                            }
                            else
                            {
                                text += "，";
                            }
                        }
                    }
                    else
                    {
                        text += "本公司無逸散式排放源";
                    }

                    DeviceList = device.Where(x => x.EmissionPattern == "製程")
                                        .GroupBy(x => x.Name)
                                        .Select(group => group.First());
                    if (DeviceList != null && DeviceList.Count() != 0)
                    {
                        int count = 0;
                        text += "產生溫室氣體排放製程，例如：";
                        foreach (var item in DeviceList)
                        {
                            count++;
                            text += item.Name;
                            if (count < DeviceList.Count())
                            {
                                text += "、";
                            }

                        }
                    }
                    else
                    {
                        text += "本公司無製程排放源";
                    }
                    text += "。此外，本次盤查範圍無土地利用變化，也無生質燃料直接排放。";
                    UpdateReplacePattern(replacePatterns, "直接溫室氣體排放說明", text);

                }
                if (data.Company.Scope2Information != null)
                {
                    string trimS2I = data.Company.Scope2Information.Replace(" ", "");
                    trimS2I = trimS2I.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern(replacePatterns, "能源間接溫室氣體排放說明", trimS2I);
                }
                else
                {
                    UpdateReplacePattern(replacePatterns, "能源間接溫室氣體排放說明", "本廠類別二能源間接溫室氣體排放皆來自於外購電力部份。");
                }
                if (device.Find(x => x.Name == "WD40") != null)
                {
                    UpdateReplacePattern(replacePatterns, "WD40排放係數描述", "依WD-40之安全資料表(SDS)，可知其組成包含2~3 Wt%之CO₂，取其平均值2.5%；因CO₂係作為WD-40之推進劑，當使用WD-40時，亦將造成CO₂逸散，故假設每使用1單位重量之WD-40時，會有2.5%之單位重量CO₂隨之逸散。");
                }
                else
                {
                    UpdateReplacePattern(replacePatterns, "WD40排放係數描述", "");
                }
                UpdateReplacePattern(replacePatterns, "盤查月", DateTime.Now.Month.ToString());
                UpdateReplacePattern(replacePatterns, "盤查日", DateTime.Now.Day.ToString());
                UpdateReplacePattern(replacePatterns, "地址", data.FullAddress);
                UpdateReplacePattern(replacePatterns, "民國基準年", baseYear.ToString());
                UpdateReplacePattern(replacePatterns, "西元基準年", (baseYear + 1911).ToString());

                ScopeDevice(replacePatterns, device, "類別一");
                ScopeDevice(replacePatterns, device, "類別二");
                if (device.Find(x => x.Name == "電力") != null)
                {
                    Guid deviceId = device.Find(x => x.Name == "電力").Id;
                    var deviceActivityData = AllActivityData.Where(x => x.DeviceId == deviceId).ToList();


                    if (deviceActivityData != null)
                    {
                        UpdateReplacePattern(replacePatterns, "電力使用量", (deviceActivityData.Sum(ad => ad.Num) / 1000).ToString());
                    }
                    else
                    {
                        UpdateReplacePattern(replacePatterns, "電力使用量", "0");
                    }
                }


                UpdateReplacePattern(replacePatterns, "類別一CO2排放", data.Scope1_CO2.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "類別一CH4排放", data.Scope1_CH4.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "類別一N2O排放", data.Scope1_N2O.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "類別一HFCS排放", data.Scope1_HFCS.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "類別一PFCS排放", data.Scope1_PFCS.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "類別一SF6排放", data.Scope1_SF6.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "類別一NF3排放", data.Scope1_NF3.ToString("N4"));

                UpdateReplacePattern(replacePatterns, "類別一CO2占比", data.percentage1_CO2.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別一CH4占比", data.percentage1_CH4.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別一N2O占比", data.percentage1_N2O.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別一HFCS占比", data.percentage1_HFCS.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別一PFCS占比", data.percentage1_PFCS.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別一SF6占比", data.percentage1_SF6.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別一NF3占比", data.percentage2_NF3.ToString("N2") + "%");

                UpdateReplacePattern(replacePatterns, "CO2排放", data.CO2.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "CH4排放", data.CH4.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "N2O排放", data.N2O.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "HFCS排放", data.HFCS.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "PFCS排放", data.PFCS.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "SF6排放", data.SF6.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "NF3排放", data.NF3.ToString("N4"));

                UpdateReplacePattern(replacePatterns, "CO2占比", data.percentage2_CO2.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "CH4占比", data.percentage2_CH4.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "N2O占比", data.percentage2_N2O.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "HFCS占比", data.percentage2_HFCS.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "PFCS占比", data.percentage2_PFCS.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "SF6占比", data.percentage2_SF6.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "NF3占比", data.percentage2_NF3.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "總排放當量", data.All.ToString("N3"));
                UpdateReplacePattern(replacePatterns, "固定排放量", data.non_move.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "移動排放量", data.move.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "製程排放量", data.process.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "逸散排放量", data.escape.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "固定排放比例", data.percentage_nonMove.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "製程排放比例", data.percentage_Process.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "移動排放比例", data.percentage_Move.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "逸散排放比例", data.percentage_Escape.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別一占比", data.percentage_Scope1.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別二占比", data.percentage_Scope2.ToString("N2") + "%");
                UpdateReplacePattern(replacePatterns, "類別一總排放", data.Scope1.ToString("N4"));
                UpdateReplacePattern(replacePatterns, "類別二總排放", data.Scope2.ToString("N4"));
                //-----------------替換的文本
                //--------------------------------排放源溫室氣體表表
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "固定", "CO2");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "固定", "CH4");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "固定", "N2O");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "移動", "CO2");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "移動", "CH4");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "移動", "N2O");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "逸散", "CO2");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "逸散", "CH4");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "逸散", "HFCS");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "製程", "CO2");
                GenerateGHGsTable(doc, AllGHGs, AllActivityData, "外購電力", "CO2");
                //--------------------------------排放源溫室氣體表表
                //--------------------------------類別表
                if (device.Any(x => x.Scope == "類別一"))
                {
                    GenerateScopeTable(doc, device, AllGHGs, "類別一");
                }
                // 檢查是否有類別二設備
                if (device.Any(x => x.Scope == "類別二"))
                {
                    GenerateScopeTable(doc, device, AllGHGs, "類別二");
                }
                //--------------------------------類別表
                //--------------------------------報告邊界
                GenerateReport(doc, device, AllGHGs);
                //--------------------------------報告邊界
                GenerateActivityDataTable(doc, device, AllActivityData);

                //--------------------------------圖片
                if (!string.IsNullOrEmpty(data.ShopDrawingsPath))
                {
                    try
                    {
                        string showDrawingPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", data.CompanyId.ToString(), data.Id.ToString(), data.ShopDrawingsPath);
                        replaceImage(doc, "<廠區圖>", showDrawingPath);
                    }
                    catch (Exception e)
                    {
                        TempData["Error"] = "請上傳圖片檔案";
                        Guid areaId = (Guid)TempData.Peek("areaId");
                        return RedirectToAction("Index", "Devices", new { id = areaId });
                        throw;
                    }
                }
                else
                {
                    UpdateReplacePattern(replacePatterns, "廠區圖", "");
                }

                if (!string.IsNullOrEmpty(data.OrganizationImagePath))
                {
                    try
                    {
                        string organiztionPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", data.CompanyId.ToString(), data.Id.ToString(), data.OrganizationImagePath);
                        replaceImage(doc, "<公司組織圖>", organiztionPath);
                    }
                    catch (Exception e)
                    {
                        TempData["Error"] = "請上傳圖片檔案";
                        Guid areaId = (Guid)TempData.Peek("areaId");
                        return RedirectToAction("Index", "Devices", new { id = areaId });
                        throw;
                    }

                }
                else
                {
                    UpdateReplacePattern(replacePatterns, "公司組織圖", "");
                }
                if (!string.IsNullOrEmpty(data.MapImagePath))
                {
                    try
                    {
                        string mapPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", data.CompanyId.ToString(), data.Id.ToString(), data.MapImagePath);
                        replaceImage(doc, "<地理位置圖>", mapPath);
                    }
                    catch (Exception e)
                    {
                        //TempData["Error"] = e.Message;
                        TempData["Error"] = "請上傳圖片檔案";
                        Guid areaId = (Guid)TempData.Peek("areaId");
                        return RedirectToAction("Index", "Devices", new { id = areaId });
                        throw;
                    }

                }
                else
                {
                    UpdateReplacePattern(replacePatterns, "地理位置圖", "");
                }
                //--------------------------------圖片
                // Check if all the replace patterns are used in the loaded document.
                if (doc.FindUniqueByPattern(@"<[\w \=]{4,}>", RegexOptions.IgnoreCase).Count > 0)
                {
                    // Do the replacement of all the found tags and with green bold strings.
                    var replaceTextOptions = new FunctionReplaceTextOptions()
                    {
                        FindPattern = "<(.*?)>",
                        RegexMatchHandler = LocalReplaceFunc,
                        RegExOptions = RegexOptions.IgnoreCase,

                    };
                    doc.ReplaceText(replaceTextOptions);
                }
                // 原本 SaveAs 到 wwwroot\output\ 再讀回，除了公開外洩外，同名檔案也會讓並行下載互相鎖檔；改為直接輸出到記憶體。
                doc.SaveAs(outputStream);
                fileBytes = outputStream.ToArray();
            }

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
        }
        // 原本 _replacePatterns 是 static 欄位、UpdateReplacePattern 是 static 方法，跨請求共用同一份字典；併發時甲公司的替換文本會被乙公司覆蓋，導致下載到別家公司的報告書內容。
        // 改為呼叫端各自建立區域 Dictionary（見 ISOReportAsync/MOEReportAsync/IISReportAsync 開頭的 replacePatterns/LocalReplaceFunc），此處只保留寫入字典的共用邏輯，並改由參數傳入該字典。
        public static void UpdateReplacePattern(Dictionary<string, string> patterns, string key, string value)
        {
            if (patterns.ContainsKey(key))
            {
                patterns[key] = value;
            }
            else
            {
                patterns.Add(key, value);
            }
        }
        public void GenerateActivityDataTable(DocX doc, List<Device> devices, List<ActivityData> activityDatas)
        {
            var font = new Xceed.Document.NET.Font("標楷體");
            Xceed.Document.NET.Table table = doc.AddTable(devices.Count() + 1, 6);
            table.AutoFit = AutoFit.Contents;
            table.Design = TableDesign.MediumShading1Accent3;
            // 填充表格標題
            table.Rows[0].Cells[0].Paragraphs.First().Append("類別").Font(font).FontSize(11d);
            table.Rows[0].Cells[0].Width = CountWidth(1.6);
            table.Rows[0].Cells[1].Paragraphs.First().Append("排放型式").Font(font).FontSize(11d);
            table.Rows[0].Cells[2].Paragraphs.First().Append("原燃物料或產品").Font(font).FontSize(11d);
            table.Rows[0].Cells[3].Paragraphs.First().Append("活動數據").Font(font).FontSize(11d);
            table.Rows[0].Cells[4].Paragraphs.First().Append("單位").Font(font).FontSize(11d);
            table.Rows[0].Cells[5].Paragraphs.First().Append("數據來源表單名稱").Font(font).FontSize(11d);

            for (int x = 0; x < devices.Count(); x++)
            {
                table.Rows[x + 1].Cells[0].Paragraphs.First().Append(devices[x].Scope).Font(font).FontSize(11d);
                table.Rows[x + 1].Cells[1].Paragraphs.First().Append(devices[x].EmissionPattern).Font(font).FontSize(11d);
                string deviceCellString = (devices[x].Name != "WD40" ? devices[x].Material : "二氧化碳") + ("(" + devices[x].Name + ")");
                table.Rows[x + 1].Cells[2].Paragraphs.First().Append(deviceCellString).Font(font).FontSize(11d);

                var DeviceId = devices[x].Id;
                var activityData = activityDatas.Where(x => x.DeviceId == DeviceId).ToList();
                table.Rows[x + 1].Cells[3].Paragraphs.First().Append((activityData.Sum(x => x.Num) / 1000).ToString("F4")).Font(font).FontSize(11d);
                if (devices[x].Unit == "人")
                {
                    table.Rows[x + 1].Cells[4].Paragraphs.First().Append(devices[x].Unit).Font(font).FontSize(11d);
                }
                else
                {
                    string displayUnit;
                    switch (devices[x].Unit)
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
                            displayUnit = devices[x].Unit;
                            break;
                    }
                    table.Rows[x + 1].Cells[4].Paragraphs.First().Append(displayUnit).Font(font).FontSize(11d);
                }
                table.Rows[x + 1].Cells[5].Paragraphs.First().Append(devices[x].Source).Font(font).FontSize(11d);
            }
            doc.ReplaceTextWithObject("<排放源活動數據表>", table);

        }
        public double CountWidth(double cmWidth)
        {
            double widthInPoints = cmWidth * 0.393701 * 72; // 1英寸 ≈ 2.54厘米，1磅 ≈ 0.0353厘米
            return widthInPoints;
        }
        public void GenerateGHGsTable(DocX doc, List<GHG> GHGs, List<ActivityData> activityDatas, string emissionPattern, string gasName)
        {
            var relevantGHGs = GHGs.Where(x => x.Device.EmissionPattern == emissionPattern && x.Name == gasName).ToList();
            int num = relevantGHGs.Count;
            string tableName = $"{emissionPattern}{gasName}";
            var font = new Xceed.Document.NET.Font("標楷體");
            Xceed.Document.NET.Table table = doc.AddTable(num > 0 ? (num + 1) : 2, 8);
            table.AutoFit = AutoFit.Contents;
            table.Design = TableDesign.MediumShading1Accent3;

            // 設定表頭
            string[] headers = { "排放源名稱", "原燃物料或產品", "活動數據", "排放係數", "係數來源", "排放量\n(公噸/年)", "GWP", "CO₂排放當量\n(公噸CO₂e/年)" };
            for (int i = 0; i < headers.Length; i++)
            {
                table.Rows[0].Cells[i].Paragraphs.First().Append(headers[i]).Font(font).FontSize(9d);
            }

            if (num > 0)
            {
                int rowIndex = 1;
                foreach (var GHG in relevantGHGs)
                {
                    string deviceCellString = GHG.Device.Name + (!string.IsNullOrWhiteSpace(GHG.Device.NameRemark) ? $"({GHG.Device.NameRemark})" : "");

                    if (GHG.GWP == 0)
                    {
                        table.Rows[rowIndex].MergeCells(0, 7);
                        var paragraph = table.Rows[rowIndex].Cells[0].Paragraphs.First().Append($"找不到{deviceCellString}{GHG.Device.Material}{GHG.Name}的GWP，請確認是否有誤").Font(font).FontSize(19d);
                        paragraph.Alignment = Alignment.center;
                    }
                    else
                    {
                        table.Rows[rowIndex].Cells[0].Paragraphs.First().Append(deviceCellString).Font(font).FontSize(9d);
                        table.Rows[rowIndex].Cells[1].Paragraphs.First().Append(GHG.Device.Name != "WD40" ? GHG.Device.Material : "二氧化碳").Font(font).FontSize(9d);
                        var totalActivityData = activityDatas.Where(x => x.DeviceId == GHG.DeviceId).Sum(ad => ad.Num);
                        table.Rows[rowIndex].Cells[2].Paragraphs.First().Append(totalActivityData + GHG.Device.Unit).Font(font).FontSize(9d);
                        table.Rows[rowIndex].Cells[3].Paragraphs.First().Append($"{GHG.CEF}公斤/{GHG.Device.Unit}").Font(font).FontSize(9d);
                        string CEF_Source = GHG.Device.CEF_Correction == 1 ? "自廠發展係數" : "溫室氣體排放係數管理表 6.0.4 版";
                        table.Rows[rowIndex].Cells[4].Paragraphs.First().Append(CEF_Source).Font(font).FontSize(9d);
                        table.Rows[rowIndex].Cells[5].Paragraphs.First().Append((GHG.Emission / GHG.GWP).ToString("N4")).Font(font).FontSize(9d); //如果切換GWP版本 可能會造成GWP為0報錯
                        table.Rows[rowIndex].Cells[6].Paragraphs.First().Append(GHG.GWP.ToString()).Font(font).FontSize(9d);
                        table.Rows[rowIndex].Cells[7].Paragraphs.First().Append(GHG.Emission.ToString("N4")).Font(font).FontSize(9d);
                    }

                    rowIndex++;
                }
            }
            else
            {
                table.Rows[1].MergeCells(0, 7);
                string switchingGasName = SwitchGasName(gasName);
                var paragraph = table.Rows[1].Cells[0].Paragraphs.First().Append($"無{switchingGasName}{emissionPattern}排放源").Font(font).FontSize(9d);
                paragraph.Alignment = Alignment.center;
            }

            // 原本用不含角括號的 tableName 比對，但 MOEReport.docx/IIIReport.docx 範本內的標記實際是 "<固定CO2>" 這種角括號形式，會導致表格被靜默略過（Word 裡仍留著空白範本標記）。
            doc.ReplaceTextWithObject("<" + tableName + ">", table);
        }
        public string SwitchGasName(string gasName)
        {
            string returnGasName = string.Empty;
            switch (gasName)
            {
                case "CO2":
                    returnGasName = "CO₂";
                    break;
                case "CH4":
                    returnGasName = "CH₄";
                    break;
                case "N2O":
                    returnGasName = "N₂O";
                    break;
                case "HFCS":
                    returnGasName = "HFCs";
                    break;
                case "PFCS":
                    returnGasName = "PFCs";
                    break;
                case "SF6":
                    returnGasName = "SF₆";
                    break;
                case "NF3":
                    returnGasName = "NF₃";
                    break;
            }
            return returnGasName;
        }
        public void replaceImage(DocX doc, string text, string imagePath)
        {
            foreach (var paragraph in doc.Paragraphs)
            {
                if (paragraph.Text.Contains(text))
                {
                    if (imagePath != null)
                    {
                        var image = doc.AddImage(imagePath);
                        var picture = image.CreatePicture();

                        // A4纸的尺寸 (单位：像素)
                        //const float a4Width = (float)(100 * 3.78);  // 210mm * 3.78 = 794像素 (DPI = 96)
                        //const float a4Height = (float)(100 * 3.78); // 297mm * 3.78 = 1123像素 (DPI = 96)
                        const float a4Width = (float)(200);  // 210mm * 3.78 = 794像素 (DPI = 96)
                        const float a4Height = (float)(200); // 297mm * 3.78 = 1123像素 (DPI = 96)

                        double aspectRatio = (double)picture.Width / picture.Height;

                        // 根据长宽比调整图片大小

                        if (aspectRatio > 1) // 宽度大于高度
                        {
                            picture.Width = a4Width;
                            picture.Height = (int)(a4Width / aspectRatio);
                        }
                        else // 高度大于宽度
                        {
                            picture.Height = a4Height;
                            picture.Width = (int)(a4Height * aspectRatio);
                        }


                        paragraph.InsertPicture(picture);
                    }



                    doc.ReplaceText(text, string.Empty);
                }
            }
        }
        public void GenerateReport(DocX doc, List<Device> devices, List<GHG> AllGHGs)
        {
            var font = new Xceed.Document.NET.Font("標楷體");
            Xceed.Document.NET.Table table = doc.AddTable(devices.Count() + 1, 4);
            table.AutoFit = AutoFit.Contents;
            table.Design = TableDesign.MediumShading1Accent3;
            // 填充表格標題
            table.Rows[0].Cells[0].Paragraphs.First().Append("類別").Font(font).FontSize(11d);
            table.Rows[0].Cells[1].Paragraphs.First().Append("型式").Font(font).FontSize(11d);
            table.Rows[0].Cells[2].Paragraphs.First().Append("排放源").Font(font).FontSize(11d);
            table.Rows[0].Cells[3].Paragraphs.First().Append("產生之溫室氣體").Font(font).FontSize(11d);

            for (int x = 0; x < devices.Count(); x++)
            {
                table.Rows[x + 1].Cells[0].Paragraphs.First().Append(devices[x].Scope).Font(font).FontSize(11d);
                table.Rows[x + 1].Cells[1].Paragraphs.First().Append(devices[x].EmissionPattern).Font(font).FontSize(11d);
                string deviceCellString = devices[x].Name
                    + (devices[x].Name != "WD40" ? ("-" + devices[x].Material) : "-二氧化碳")
                    + (devices[x].NameRemark != null && devices[x].NameRemark.Trim() != string.Empty ? "(" + devices[x].NameRemark + ")" : "");
                table.Rows[x + 1].Cells[2].Paragraphs.First().Append(deviceCellString).Font(font).FontSize(11d);
                var GHGs = AllGHGs.Where(ghg => ghg.DeviceId == devices[x].Id);
                var ghgOrder = new Dictionary<string, int>
                {
                    { "CO2", 0 },
                    { "CH4", 1 },
                    { "N2O", 2 },
                    { "HFCS", 3 },
                    { "PFCS", 4 },
                    { "SF6", 5 },
                    { "NF3", 6 }
                };

                // 定義排序方法
                int SortByGHGName(GHG ghg)
                {
                    // 如果 GHG 的名稱在映射中，則返回對應的排序數值，否則返回 int.MaxValue
                    return ghgOrder.ContainsKey(ghg.Name) ? ghgOrder[ghg.Name] : int.MaxValue;
                }

                // 使用排序方法來排序 GHGs
                var sortedGHGs = GHGs.OrderBy(SortByGHGName);

                string text = "";
                foreach (var item in sortedGHGs)
                {
                    text += SwitchGasName(item.Name);

                    text += "、";
                }
                if (text.EndsWith("、"))
                {
                    text = text.Remove(text.Length - 1);
                }
                table.Rows[x + 1].Cells[3].Paragraphs.First().Append(text).Font(font).FontSize(11d);

            }
            // 原本用不含角括號的字串比對，但範本內的標記實際是 "<報告邊界表>"，會導致表格被靜默略過。
            doc.ReplaceTextWithObject("<報告邊界表>", table);
        }
        public void GenerateScopeTable(DocX doc, List<Device> devices, List<GHG> AllGHGs, string scope)
        {
            // 獲取特定類別的設備數量
            int numDevices = devices.Where(x => x.Scope == scope).Count();
            // 創建表格，行數為設備數量 + 1
            Xceed.Document.NET.Table table = doc.AddTable(numDevices + 1, 10);
            table.Design = TableDesign.MediumShading1Accent3;
            table.AutoFit = AutoFit.Contents;
            var font = new Xceed.Document.NET.Font("標楷體");
            // 填充表格標題
            table.Rows[0].Cells[0].Paragraphs.First().Append("類別").Font(font).FontSize(11d);
            table.Rows[0].Cells[1].Paragraphs.First().Append("型式").Font(font).FontSize(11d);
            table.Rows[0].Cells[2].Paragraphs.First().Append("排放源").Font(font).FontSize(11d);
            table.Rows[0].Cells[3].Paragraphs.First().Append("CO₂").Font(font).FontSize(11d);
            table.Rows[0].Cells[4].Paragraphs.First().Append("CH₄").Font(font).FontSize(11d);
            table.Rows[0].Cells[5].Paragraphs.First().Append("N₂O").Font(font).FontSize(11d);
            table.Rows[0].Cells[6].Paragraphs.First().Append("HFCs").Font(font).FontSize(11d);
            table.Rows[0].Cells[7].Paragraphs.First().Append("PFCs").Font(font).FontSize(11d);
            table.Rows[0].Cells[8].Paragraphs.First().Append("SF₆").Font(font).FontSize(11d);
            table.Rows[0].Cells[9].Paragraphs.First().Append("NF₃").Font(font).FontSize(11d);

            // 遍歷設備列表，填充表格內容
            int rowIndex = 1;
            foreach (var item in devices.Where(x => x.Scope == scope))
            {
                // 獲取特定設備的溫室氣體
                var GHGs = AllGHGs.Where(ghg => ghg.DeviceId == item.Id);

                // 填充基本資料
                table.Rows[rowIndex].Cells[0].Paragraphs.First().Append(item.Scope).Font(font).FontSize(11d);
                table.Rows[rowIndex].Cells[1].Paragraphs.First().Append(item.EmissionPattern).Font(font).FontSize(11d);
                string deviceCellString = item.Name + (item.Name != "WD40" ? ("-" + item.Material) : ("-二氧化碳"));
                table.Rows[rowIndex].Cells[2].Paragraphs.First().Append(deviceCellString).Font(font).FontSize(11d);

                // 填充溫室氣體選項欄位
                foreach (var GHG in GHGs)
                {
                    switch (GHG.Name)
                    {
                        case "CO2":
                            table.Rows[rowIndex].Cells[3].Paragraphs.First().Append("v").Font(font).FontSize(11d);
                            break;
                        case "CH4":
                            table.Rows[rowIndex].Cells[4].Paragraphs.First().Append("v").Font(font).FontSize(11d);
                            break;
                        case "N2O":
                            table.Rows[rowIndex].Cells[5].Paragraphs.First().Append("v").Font(font).FontSize(11d);
                            break;
                        case "HFCS":
                            table.Rows[rowIndex].Cells[6].Paragraphs.First().Append("v").Font(font).FontSize(11d);
                            break;
                        case "PFCS":
                            table.Rows[rowIndex].Cells[7].Paragraphs.First().Append("v").Font(font).FontSize(11d);
                            break;
                        case "SF6":
                            table.Rows[rowIndex].Cells[8].Paragraphs.First().Append("v").Font(font).FontSize(11d);
                            break;
                        case "NF3":
                            table.Rows[rowIndex].Cells[9].Paragraphs.First().Append("v").Font(font).FontSize(11d);
                            break;
                    }
                }
                rowIndex++;
            }

            // 原本用不含角括號的字串比對，但範本內的標記實際是 "<類別一Table>" 這種角括號形式，會導致表格被靜默略過。
            doc.ReplaceTextWithObject("<" + scope + "Table>", table);

        }
        //public void GenerateAnalysisTable(DocX doc, Analysis analysis)
        //{
        //    // 創建表格，行數為設備數量 + 1
        //    Xceed.Document.NET.Table table = doc.AddTable(27 + 1, 10);
        //    table.Design = TableDesign.MediumShading1Accent3;
        //    table.AutoFit = AutoFit.Contents;
        //    var font = new Xceed.Document.NET.Font("標楷體");
        //    // 填充表格標題
        //    table.Rows[0].Cells[0].Paragraphs.First().Append("類別").Font(font).FontSize(11d);
        //    table.Rows[0].Cells[1].Paragraphs.First().Append("間接排放活動項目").Font(font).FontSize(11d);
        //    table.Rows[0].Cells[2].Paragraphs.First().Append("活動資料品質(A)").Font(font).FontSize(11d);
        //    table.Rows[0].Cells[3].Paragraphs.First().Append("係數品質(B)").Font(font).FontSize(11d);
        //    table.Rows[0].Cells[4].Paragraphs.First().Append("發生/使用頻率(C)").Font(font).FontSize(11d);
        //    table.Rows[0].Cells[5].Paragraphs.First().Append("重大性(A*B)*C").Font(font).FontSize(11d);
        //    table.Rows[0].Cells[6].Paragraphs.First().Append("量化與否").Font(font).FontSize(11d);

        //    // 遍歷設備列表，填充表格內容
        //    int rowIndex = 1;
        //    for (int i = 3; i <= 6; i++)
        //    {
        //        string scope = string.Empty;
        //        switch (i)
        //        {
        //            case 3:
        //                scope = "類別三";
        //                break;
        //            case 4:
        //                scope = "類別四";
        //                break;
        //            case 5:
        //                scope = "類別五";
        //                break;
        //            case 6:
        //                scope = "類別六";
        //                break;
        //        }

        //        foreach (var item in devices.Where(x => x.Scope == scope))
        //        {
        //            // 獲取特定設備的溫室氣體
        //            var GHGs = AllGHGs.Where(ghg => ghg.DeviceId == item.Id);

        //            // 填充基本資料
        //            table.Rows[rowIndex].Cells[0].Paragraphs.First().Append(item.Scope).Font(font).FontSize(11d);
        //            table.Rows[rowIndex].Cells[1].Paragraphs.First().Append(item.EmissionPattern).Font(font).FontSize(11d);
        //            string deviceCellString = item.Name + (item.Name != "WD40" ? ("-" + item.Material) : ("-二氧化碳"));
        //            table.Rows[rowIndex].Cells[2].Paragraphs.First().Append(deviceCellString).Font(font).FontSize(11d);

        //            // 填充溫室氣體選項欄位
        //            foreach (var GHG in GHGs)
        //            {
        //                switch (GHG.Name)
        //                {
        //                    case "CO2":
        //                        table.Rows[rowIndex].Cells[3].Paragraphs.First().Append("v").Font(font).FontSize(11d);
        //                        break;
        //                    case "CH4":
        //                        table.Rows[rowIndex].Cells[4].Paragraphs.First().Append("v").Font(font).FontSize(11d);
        //                        break;
        //                    case "N2O":
        //                        table.Rows[rowIndex].Cells[5].Paragraphs.First().Append("v").Font(font).FontSize(11d);
        //                        break;
        //                    case "HFCS":
        //                        table.Rows[rowIndex].Cells[6].Paragraphs.First().Append("v").Font(font).FontSize(11d);
        //                        break;
        //                    case "PFCS":
        //                        table.Rows[rowIndex].Cells[7].Paragraphs.First().Append("v").Font(font).FontSize(11d);
        //                        break;
        //                    case "SF6":
        //                        table.Rows[rowIndex].Cells[8].Paragraphs.First().Append("v").Font(font).FontSize(11d);
        //                        break;
        //                    case "NF3":
        //                        table.Rows[rowIndex].Cells[9].Paragraphs.First().Append("v").Font(font).FontSize(11d);
        //                        break;
        //                }
        //            }
        //            rowIndex++;
        //        }
        //    }


        //    doc.ReplaceTextWithObject("顯著性評估表格", table);

        //}
        // 原本 UpdateReplacePattern 改為需要傳入區域字典後，這裡也一併加上 replacePatterns 參數，讓呼叫端的區域字典能傳遞進來。
        public void ScopeDevice(Dictionary<string, string> replacePatterns, List<Device> devices, string scope)
        {
            string Scope = "";
            int i = 0;
            foreach (var item in devices)
            {

                if (item.Scope == scope)
                {
                    i++;
                    Scope += i + "." + item.Name + "(" + item.Material + ")" + "\n";
                }
            }
            UpdateReplacePattern(replacePatterns, "" + scope + "Device", Scope.Trim());
        }
        // 原本 UpdateReplacePattern 改為需要傳入區域字典後，這裡也一併加上 replacePatterns 參數，讓呼叫端的區域字典能傳遞進來。
        public void ScopeDeviceDirections(Dictionary<string, string> replacePatterns, List<Device> devices, string EmissionPattern)
        {
            var ScopeDevices = devices.Where(x => x.EmissionPattern == EmissionPattern).ToList();
            string content = string.Empty;

            if (ScopeDevices.Count != 0)
            {
                string symbol = string.Empty;
                content = "，包含";
                for (int i = 0; i < ScopeDevices.Count; i++)
                {
                    if (i != ScopeDevices.Count - 1)
                    {
                        symbol = "、";
                    }
                    else
                    {
                        symbol = "";
                    }

                    content += ScopeDevices[i].Name + "(" + ScopeDevices[i].Material + ")" + symbol;
                }
            }
            content += "。";

            UpdateReplacePattern(replacePatterns, EmissionPattern + "排放源說明", content.Trim());
        }
    }
}
