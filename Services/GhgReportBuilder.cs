using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace Carbon_inventory_platform.Services
{
    /// <summary>
    /// A8：三份溫室氣體盤查報告書的共用產生流程。
    /// 原本 ISOReportAsync／MOEReportAsync／IISReportAsync 各自 450 行、彼此高度重複，
    /// 現在流程只有一份，各標準的差異全部由 <see cref="GhgReportSpec"/> 描述。
    ///
    /// 幾個不能動的順序約束（動了就會安靜地產出錯誤的報告書）：
    ///   1. CountEmissionAsync 必須在載入 Area 之前跑完（由 Controller 負責），否則所有 data.* 數字會是上一次的舊值。
    ///   2. 表格必須在最後那次 doc.ReplaceText 掃描之前插入——表格是插在範本標記的位置上，
    ///      掃描會把未知標記換成空字串，順序顛倒的話表格會整批消失而且不會報錯。
    ///   3. 插圖順序就是文件裡的圖片順序（replaceImage 是 InsertPicture）。
    ///   4. 數字格式刻意保留原本的 ToString("N4"/"N3"/"N2") 不加 IFormatProvider，
    ///      加上 InvariantCulture 會改變報告書裡每一個數字。
    /// </summary>
    public class GhgReportBuilder
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public GhgReportBuilder(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>載入一次產生報告書所需要的全部資料。</summary>
        private sealed class LoadedData
        {
            public Area Area = null!;
            public List<Device> Devices = new();
            public List<GHG> AllGHGs = new();
            public List<ActivityData> AllActivityData = new();
            public Area? BaseYearArea;
        }

        /// <summary>
        /// 產生報告書。呼叫端（Controller）必須已經做過權限檢查並跑過 CountEmissionAsync。
        /// </summary>
        public async Task<GhgReportResult> BuildAsync(GhgReportSpec spec, Guid areaId)
        {
            LoadedData? loaded = spec.Load == ReportDataLoad.IncludeGraph
                ? await LoadWithIncludeGraphAsync(areaId)
                : await LoadProjectedOrderedAsync(areaId);

            if (loaded == null)
            {
                return GhgReportResult.NotFound();
            }
            if (loaded.BaseYearArea == null)
            {
                return GhgReportResult.Error("請先設定基準年");
            }

            Area data = loaded.Area;
            List<Device> device = loaded.Devices;
            List<GHG> AllGHGs = loaded.AllGHGs;
            List<ActivityData> AllActivityData = loaded.AllActivityData;
            int baseYear = loaded.BaseYearArea.Year;

            //-----------------檔案設定
            // 原本用 Directory.GetCurrentDirectory() 加上 "wwwroot\\doc\\" 硬編碼反斜線，IIS/Linux 部署時會導致找不到範本檔。
            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "doc", spec.TemplateFileName);
            // 原本把產生的報告書存到 wwwroot\output\，該目錄由 UseStaticFiles 匿名對外提供，會導致任何人猜檔名就下載別家公司的報告書。
            string fileName = BuildReportFileName(data.Year, data.Company!.Name, data.Name);
            //-----------------檔案設定

            // 原本 _replacePatterns 是跨請求共用的 static 欄位，多人同時產生報表時會互相覆蓋彼此公司的替換文本；改為每次請求各自建立區域字典。
            var replacePatterns = new Dictionary<string, string>();
            string LocalReplaceFunc(string findStr) => replacePatterns.TryGetValue(findStr, out var replaceValue) ? replaceValue : string.Empty;

            byte[] fileBytes;
            using (MemoryStream outputStream = new MemoryStream())
            using (DocX doc = DocX.Load(filePath))
            {
                //-----------------替換的文本
                WriteCompanyAndContact(replacePatterns, spec, data);
                WriteNarratives(replacePatterns, spec, data, device);
                WriteInventoryDates(replacePatterns, spec, data, baseYear);
                WriteGasTotals(replacePatterns, data);
                WritePatternTotals(replacePatterns, data);
                WriteElectricityUsage(replacePatterns, device, AllActivityData);
                WriteScopeTotals(replacePatterns, spec, data, device);

                switch (spec.Narrative)
                {
                    case ScopeNarrativeStyle.PatternDirections:
                        ScopeDeviceDirections(replacePatterns, device, "固定");
                        ScopeDeviceDirections(replacePatterns, device, "移動");
                        ScopeDeviceDirections(replacePatterns, device, "逸散");
                        break;
                    case ScopeNarrativeStyle.NumberedDeviceList:
                        ScopeDevice(replacePatterns, device, "類別一");
                        ScopeDevice(replacePatterns, device, "類別二");
                        break;
                }

                if (spec.EmitBaseYearComparison)
                {
                    WriteBaseYearComparison(replacePatterns, data, device, loaded.BaseYearArea);
                }
                //-----------------替換的文本

                EmitTables(doc, device, AllGHGs, AllActivityData);

                //--------------------------------圖片
                foreach (var slot in spec.Images)
                {
                    string? imageName = slot.Path(data);
                    if (!string.IsNullOrEmpty(imageName))
                    {
                        try
                        {
                            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", data.CompanyId.ToString(), data.Id.ToString(), imageName);
                            replaceImage(doc, slot.Marker, imagePath);
                        }
                        catch (Exception)
                        {
                            // 原本三個 action 各自在 catch 裡直接 RedirectToAction，還用未檢查的 (Guid)TempData.Peek("areaId")
                            // 取值（那個鍵不一定存在，錯誤處理自己也可能再丟一次例外）。改為回報失敗、由 Controller 決定怎麼導頁。
                            return GhgReportResult.Error("請上傳圖片檔案");
                        }
                    }
                    else
                    {
                        UpdateReplacePattern(replacePatterns, slot.Key, "");
                    }
                }
                //--------------------------------圖片

                // Check if all the replace patterns are used in the loaded document.
                // 這個探測用的 regex 只決定「要不要跑替換」，實際比對用的是下面的 "<(.*?)>"。
                // CJK 在 .NET 的 \w 裡算字元，所以 4 個字以上的中文標記能通過探測；不要簡化或收緊這個條件。
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

            return GhgReportResult.Ok(fileBytes, fileName);
        }

        // ===================== 資料載入 =====================

        /// <summary>
        /// ISO 用的載入方式：Include 整棵物件樹。
        /// 排序沿用原本的 deviceOrder 對照表——注意那張表的鍵是「類別一~六」（Scope 值）卻是用 device.Name 去查，
        /// 所以實際上每個排放源都落到 int.MaxValue、OrderBy 是穩定的無效排序，等於維持 EF 回傳的順序。
        /// 這個「無效排序」會影響下游所有表格與敘述的排放源順序，所以刻意原樣保留，不在這次重構裡修。
        /// </summary>
        private async Task<LoadedData?> LoadWithIncludeGraphAsync(Guid id)
        {
            // 原本只過濾 GHGs 沒有過濾 Devices，已軟刪除的排放源仍會被加總，會導致報告書數字與畫面不一致。
            Area? data = await _context.Areas
                                        .AsNoTracking()
                                        .Where(a => a.Id == id) // isDeleted 由全域查詢過濾器處理（Area/Device/GHG 皆已套用）
                                        .Include(a => a.Company)
                                        .Include(a => a.Devices)
                                            .ThenInclude(d => d.GHGs)
                                        .FirstOrDefaultAsync();

            if (data == null || data.Company == null)
            {
                return null;
            }

            var deviceOrder = new Dictionary<string, int>
                {
                    { "類別一", 1 },
                    { "類別二", 2 },
                    { "類別三", 3 },
                    { "類別四", 4 },
                    { "類別五", 5 },
                    { "類別六", 6 },
                };

            int SortByDeviceScope(Device device)
            {
                return deviceOrder.ContainsKey(device.Name) ? deviceOrder[device.Name] : int.MaxValue;
            }

            var loaded = new LoadedData
            {
                Area = data,
                Devices = data.Devices.OrderBy(SortByDeviceScope).ToList(),
                AllActivityData = _context.ActivityDatas.AsNoTracking().ToList(),
            };
            loaded.AllGHGs = loaded.Devices.SelectMany(d => d.GHGs).ToList();

            // ISO 需要基準年廠區的整棵物件樹（要拿它的排放源來算基準年小計）。
            if (data.BaseYear)
            {
                loaded.BaseYearArea = data;
            }
            else
            {
                loaded.BaseYearArea = await _context.Areas // isDeleted 由全域查詢過濾器處理
                                        .AsNoTracking()
                                        .Where(a => a.CompanyId == data.CompanyId && a.BaseYear)
                                        .Include(a => a.Company)
                                        .Include(a => a.Devices)
                                            .ThenInclude(d => d.GHGs)
                                        .FirstOrDefaultAsync();
            }

            return loaded;
        }

        /// <summary>
        /// 環境部／資策會用的載入方式：投影查詢，排放源明確依 Scope、EmissionPattern 排序。
        /// </summary>
        private async Task<LoadedData?> LoadProjectedOrderedAsync(Guid id)
        {
            // isDeleted 由全域查詢過濾器處理（Area/Device/GHG 皆已套用），不需要重複寫。
            var dataResult = await _context.Areas
                                        .AsNoTracking()
                                        .Where(x => x.Id == id)
                                        .Include(x => x.Company)
                                        .Select(area => new
                                        {
                                            Area = area,
                                            Devices = _context.Devices
                                                              .AsNoTracking()
                                                              .Where(d => d.AreaId == area.Id)
                                                              .OrderBy(d => d.Scope)
                                                              .ThenBy(d => d.EmissionPattern)
                                                              .ToList(),
                                            // 原本用 _context.Devices.Where(...).SelectMany(d => d.GHGs) 撈 GHG，
                                            // GenerateGHGsTable 內部要用 x.Device.EmissionPattern 篩選，
                                            // 但這種寫法不會帶出 GHG.Device 反向導覽屬性——沒加 AsNoTracking 時，
                                            // 是靠同一個 DbContext 裡剛好也查過 Device 讓變更追蹤器順便接上，
                                            // 一旦改成 NoTracking（或哪天 Devices 那份查詢被拿掉）就會是 null，
                                            // 報告書產生時整頁 500。改成直接查 GHG 並明確 Include(Device)。
                                            AllGHGs = _context.GHGs
                                                              .AsNoTracking()
                                                              .Where(g => g.Device!.AreaId == area.Id)
                                                              .Include(g => g.Device)
                                                              .ToList(),
                                            AllActivityData = _context.ActivityDatas.AsNoTracking().ToList()
                                        })
                                        .FirstOrDefaultAsync();

            if (dataResult == null || dataResult.Area.Company == null)
            {
                return null;
            }

            var data = dataResult.Area;

            // 原本用 .Select(x => x.Year) 取得不可為 null 的 int，「baseYear == null」永遠不成立，公司尚未設定基準年時會直接把 0 當作基準年印進報告書。
            var baseYearArea = await _context.Areas // isDeleted 由全域查詢過濾器處理
                .AsNoTracking()
                .Where(x => x.CompanyId == data.Company!.Id && x.BaseYear)
                .FirstOrDefaultAsync();

            return new LoadedData
            {
                Area = data,
                Devices = dataResult.Devices,
                AllGHGs = dataResult.AllGHGs,
                AllActivityData = dataResult.AllActivityData,
                BaseYearArea = baseYearArea,
            };
        }

        // ===================== 替換文本 =====================

        /// <summary>公司自填欄位：有填就把空白去掉、換行後補四格縮排；沒填就用預設文字。原本三份報告書各自重複這段判斷。</summary>
        private static void Para(Dictionary<string, string> patterns, string key, string? value, string fallback)
        {
            if (value != null)
            {
                string trimmed = value.Replace(" ", "");
                trimmed = trimmed.Replace("\r\n", "\r\n    ");
                UpdateReplacePattern(patterns, key, trimmed);
            }
            else
            {
                UpdateReplacePattern(patterns, key, fallback);
            }
        }

        private static void WriteCompanyAndContact(Dictionary<string, string> patterns, GhgReportSpec spec, Area data)
        {
            Company company = data.Company!;

            UpdateReplacePattern(patterns, "聯絡人姓名", company.ContactName);
            UpdateReplacePattern(patterns, "聯絡人電話", company.Phone);
            UpdateReplacePattern(patterns, "聯絡人電子信箱", company.Email);

            UpdateReplacePattern(patterns, "公司中文名稱", company.Name);
            if (spec.EmitEnglishCompanyName)
            {
                UpdateReplacePattern(patterns, "公司英文名稱", company.EnglishName);
            }
            UpdateReplacePattern(patterns, "西元盤查年份", (data.Year + 1911).ToString());
            UpdateReplacePattern(patterns, "民國盤查年份", (data.Year).ToString());
            // 原本只有 data.Name 有值時才設定「廠區名稱」，未設定的鍵會沿用先前的殘值，會導致別家廠區名稱出現在報告書中。
            // 環境部／資策會原本是「空白就不寫這個鍵」，因為找不到鍵時替換函式回傳空字串，兩種寫法印出來完全一樣，這裡統一成一律寫。
            UpdateReplacePattern(patterns, "廠區名稱", string.IsNullOrWhiteSpace(data.Name) ? "" : data.Name);

            UpdateReplacePattern(patterns, "進行評估排放當量", data.cal_all.ToString("N4"));
            UpdateReplacePattern(patterns, "第1級評分", data.no1_Grade.ToString());
            UpdateReplacePattern(patterns, "第2級評分", data.no2_Grade.ToString());
            UpdateReplacePattern(patterns, "第3級評分", data.no3_Grade.ToString());
            UpdateReplacePattern(patterns, "清冊等級分數補充", data.avg_Grade.ToString());
            UpdateReplacePattern(patterns, "清冊級別補充", data.all_Grade.ToString());
            // 原本上限印成負號、下限印成正號（與畫面相反），會導致報告書出現負的 95% 信賴區間上限。
            UpdateReplacePattern(patterns, "95上", "+" + data.UUL.ToString("N2") + "%");
            UpdateReplacePattern(patterns, "95下", "-" + data.ULL.ToString("N2") + "%");
        }

        private static void WriteNarratives(Dictionary<string, string> patterns, GhgReportSpec spec, Area data, List<Device> device)
        {
            Company company = data.Company!;

            if (spec.EmitForewordAndPurpose)
            {
                Para(patterns, "前言", company.ReportOpening,
                    "自1997年12月第三次締約國大會(COP3)簽署京都議定書後，全球先進國家均研擬因應溫室氣體減量的方法與措施，2005年2月京都議定書正式生效後，全球各國更積極建立了溫室氣體排放管制的共識，2007年12月巴里路線圖進一步強調開發中國家應推動可量測(Measurable)、報告(Reportable)及可供查證(Verifiable)之適當減緩行動，2009年12月丹麥哥本哈根會議更針對後京都世界各國溫室氣體減量提出可行方案。\r\n    聯合國環境規劃署在2014年「溫室氣體排放缺口報告」指出，全球碳中和應於2055年至2070年間達成，否則地球將面臨氣候變遷災難。2015年聯合國發布「2030年永續發展方針」，公布17項永續發展目標(SDGs)，為人類和地球的「和平與繁榮」提供了現在與未來的共享藍圖。基於全球減碳趨勢，我國響應聯合國氣候變遷的目標，亦提出「2050淨零路徑里程碑」，隨後於112年1月10通過「氣候變遷因應法」，以達成節能減碳之永續發展目。\r\n    " + company.Name + "(以下簡稱本公司)為因應全球永續發展趨勢於 2015年底《聯合國氣候變化綱要公約》第21屆締約國(COP21)後「巴黎協議」產生，加上環境部《溫室氣體減量及管理法》於 2015年7月正式公布實施，配合國家整體溫室氣體減量策略發展，以達成節能減碳之永續發展目標，特配合政府政策，持續進行公司內部溫室氣體盤查，以瞭解溫室氣體排放實況，進而訂定改善措施，以求達成二氧化碳排放減量之目標。 \r\n    本公司基於關心全球氣候變遷、善用資源及善盡企業的責任，根據ISO/IEC 14064-1： 2018要求，對溫室氣體管制發展趨勢及因應未來溫室氣體減量之要求，進行系統化的溫室氣體排放盤查 與清冊建置及查證程序等推動計畫，提供日後實施有效的減量改善方案作參考。今後除將持續推動溫室氣體排放管制以降低成本外，並期盼能達成兼顧資源效率、能源節約、環境保護的永續能源發展，共同為產業朝向低碳型經濟社會來努力。");
            }

            Para(patterns, "公司簡介", company.CompanyInformation, " ");

            if (spec.EmitForewordAndPurpose)
            {
                Para(patterns, "預期用途", company.ReportingPurposes,
                    "為接軌國際議題，提升競爭力，以及實施社會責任，進行本報告書撰寫，以展現本公司溫室氣體盤查結果及推動減碳的決心。");
            }

            Para(patterns, "組織邊界設定", company.AddressInformation,
                "本次組織邊界之設定，遵循ISO 14064-1:2018標準，採用營運控制權法。");
            Para(patterns, "報告邊界", company.ReportingInformation, "");
            Para(patterns, "溫室氣體排放類型與排放量說明", company.GHGInformation,
                "溫室氣體之種類係指上述標準定義之溫室氣體，包括二氧化碳(CO₂)、甲烷(CH₄)、氧化亞氮(N₂O)、氫氟碳化物(HFCs)、全氟碳化物(PFCs)、六氟化硫(SF₆)、三氟化氮(NF₃)以及其他經環境部公告者，但不包含蒙特婁議定書規範之氟氯碳化物(CFCs)。");

            // 「直接溫室氣體排放說明」沒填時要自動用排放源清單組一段敘述，原本這段 104 行在三份報告書裡一字不差地各寫一次。
            if (company.Scope1Information != null)
            {
                Para(patterns, "直接溫室氣體排放說明", company.Scope1Information, "");
            }
            else
            {
                UpdateReplacePattern(patterns, "直接溫室氣體排放說明", BuildScope1Narrative(device));
            }

            Para(patterns, "能源間接溫室氣體排放說明", company.Scope2Information,
                "本廠類別二能源間接溫室氣體排放皆來自於外購電力部份。");

            if (device.Find(x => x.Name == "WD40") != null)
            {
                UpdateReplacePattern(patterns, "WD40排放係數描述", "依WD-40之安全資料表(SDS)，可知其組成包含2~3 Wt%之CO₂，取其平均值2.5%；因CO₂係作為WD-40之推進劑，當使用WD-40時，亦將造成CO₂逸散，故假設每使用1單位重量之WD-40時，會有2.5%之單位重量CO₂隨之逸散。");
            }
            else
            {
                UpdateReplacePattern(patterns, "WD40排放係數描述", "");
            }
        }

        /// <summary>
        /// 依排放源組出「直接溫室氣體排放說明」。原本這段在三個 action 裡各有一份完全相同的副本。
        /// 注意：固定／移動／逸散在最後一個排放源後面會補一個「，」，製程沒有——這個不一致三份都一樣，
        /// 屬於既有行為，這次重構原樣保留。
        /// </summary>
        public static string BuildScope1Narrative(List<Device> device)
        {
            string text = "包含";

            text += PatternSentence(device, "固定", "固定源燃燒的直接排放，例如：", "本公司無固定式排放源", trailingComma: true);
            text += PatternSentence(device, "移動", "移動源燃燒的直接排放，例如：", "本公司無移動式排放源", trailingComma: true);
            text += PatternSentence(device, "逸散", "人為活動產生的逸散排放，例如：", "本公司無逸散式排放源", trailingComma: true);
            text += PatternSentence(device, "製程", "產生溫室氣體排放製程，例如：", "本公司無製程排放源", trailingComma: false);

            text += "。此外，本次盤查範圍無土地利用變化，也無生質燃料直接排放。";
            return text;
        }

        private static string PatternSentence(List<Device> device, string emissionPattern, string lead, string emptyText, bool trailingComma)
        {
            var deviceList = device.Where(x => x.EmissionPattern == emissionPattern)
                                   .GroupBy(x => x.Name)
                                   .Select(group => group.First())
                                   .ToList();

            if (deviceList.Count == 0)
            {
                return emptyText;
            }

            string text = lead;
            for (int i = 0; i < deviceList.Count; i++)
            {
                text += deviceList[i].Name;
                if (i < deviceList.Count - 1)
                {
                    text += "、";
                }
                else if (trailingComma)
                {
                    text += "，";
                }
            }
            return text;
        }

        private static void WriteInventoryDates(Dictionary<string, string> patterns, GhgReportSpec spec, Area data, int baseYear)
        {
            // 原本每個欄位各讀一次 DateTime.Now（資策會少寫「盤查年」），這裡只取一次，
            // 避免跨午夜/跨年時三個欄位對不起來。
            DateTime now = DateTime.Now;
            if (spec.EmitInventoryYear)
            {
                UpdateReplacePattern(patterns, "盤查年", now.Year.ToString());
            }
            UpdateReplacePattern(patterns, "盤查月", now.Month.ToString());
            UpdateReplacePattern(patterns, "盤查日", now.Day.ToString());
            UpdateReplacePattern(patterns, "地址", data.FullAddress);
            UpdateReplacePattern(patterns, "民國基準年", baseYear.ToString());
            UpdateReplacePattern(patterns, "西元基準年", (baseYear + 1911).ToString());
        }

        private static void WriteGasTotals(Dictionary<string, string> patterns, Area data)
        {
            UpdateReplacePattern(patterns, "類別一CO2排放", data.Scope1_CO2.ToString("N4"));
            UpdateReplacePattern(patterns, "類別一CH4排放", data.Scope1_CH4.ToString("N4"));
            UpdateReplacePattern(patterns, "類別一N2O排放", data.Scope1_N2O.ToString("N4"));
            UpdateReplacePattern(patterns, "類別一HFCS排放", data.Scope1_HFCS.ToString("N4"));
            UpdateReplacePattern(patterns, "類別一PFCS排放", data.Scope1_PFCS.ToString("N4"));
            UpdateReplacePattern(patterns, "類別一SF6排放", data.Scope1_SF6.ToString("N4"));
            UpdateReplacePattern(patterns, "類別一NF3排放", data.Scope1_NF3.ToString("N4"));

            UpdateReplacePattern(patterns, "類別一CO2占比", data.percentage1_CO2.ToString("N2") + "%");
            UpdateReplacePattern(patterns, "類別一CH4占比", data.percentage1_CH4.ToString("N2") + "%");
            UpdateReplacePattern(patterns, "類別一N2O占比", data.percentage1_N2O.ToString("N2") + "%");
            UpdateReplacePattern(patterns, "類別一HFCS占比", data.percentage1_HFCS.ToString("N2") + "%");
            UpdateReplacePattern(patterns, "類別一PFCS占比", data.percentage1_PFCS.ToString("N2") + "%");
            UpdateReplacePattern(patterns, "類別一SF6占比", data.percentage1_SF6.ToString("N2") + "%");
            // 這裡讀的是 percentage2_NF3（類別二的比例）而不是 percentage1_NF3，看起來是筆誤；
            // 但三份報告書都一樣，屬於既有行為，這次重構不動，避免混進輸出差異。
            UpdateReplacePattern(patterns, "類別一NF3占比", data.percentage2_NF3.ToString("N2") + "%");

            UpdateReplacePattern(patterns, "CO2排放", data.CO2.ToString("N4"));
            UpdateReplacePattern(patterns, "CH4排放", data.CH4.ToString("N4"));
            UpdateReplacePattern(patterns, "N2O排放", data.N2O.ToString("N4"));
            UpdateReplacePattern(patterns, "HFCS排放", data.HFCS.ToString("N4"));
            UpdateReplacePattern(patterns, "PFCS排放", data.PFCS.ToString("N4"));
            UpdateReplacePattern(patterns, "SF6排放", data.SF6.ToString("N4"));
            UpdateReplacePattern(patterns, "NF3排放", data.NF3.ToString("N4"));

            UpdateReplacePattern(patterns, "CO2占比", data.percentage2_CO2.ToString("N2") + "%");
            UpdateReplacePattern(patterns, "CH4占比", data.percentage2_CH4.ToString("N2") + "%");
            UpdateReplacePattern(patterns, "N2O占比", data.percentage2_N2O.ToString("N2") + "%");
            UpdateReplacePattern(patterns, "HFCS占比", data.percentage2_HFCS.ToString("N2") + "%");
            UpdateReplacePattern(patterns, "PFCS占比", data.percentage2_PFCS.ToString("N2") + "%");
            UpdateReplacePattern(patterns, "SF6占比", data.percentage2_SF6.ToString("N2") + "%");
            UpdateReplacePattern(patterns, "NF3占比", data.percentage2_NF3.ToString("N2") + "%");
        }

        private static void WritePatternTotals(Dictionary<string, string> patterns, Area data)
        {
            UpdateReplacePattern(patterns, "固定排放量", data.non_move.ToString("N4"));
            UpdateReplacePattern(patterns, "移動排放量", data.move.ToString("N4"));
            UpdateReplacePattern(patterns, "製程排放量", data.process.ToString("N4"));
            UpdateReplacePattern(patterns, "逸散排放量", data.escape.ToString("N4"));
            UpdateReplacePattern(patterns, "固定排放比例", data.percentage_nonMove.ToString("N2") + "%");
            UpdateReplacePattern(patterns, "製程排放比例", data.percentage_Process.ToString("N2") + "%");
            UpdateReplacePattern(patterns, "移動排放比例", data.percentage_Move.ToString("N2") + "%");
            UpdateReplacePattern(patterns, "逸散排放比例", data.percentage_Escape.ToString("N2") + "%");
        }

        private static void WriteElectricityUsage(Dictionary<string, string> patterns, List<Device> device, List<ActivityData> allActivityData)
        {
            var electricity = device.Find(x => x.Name == "電力");
            if (electricity == null)
            {
                return;
            }

            var deviceActivityData = allActivityData.Where(x => x.DeviceId == electricity.Id).ToList();
            if (deviceActivityData != null)
            {
                UpdateReplacePattern(patterns, "電力使用量", (deviceActivityData.Sum(ad => ad.Num) / 1000).ToString());
            }
            else
            {
                UpdateReplacePattern(patterns, "電力使用量", "0");
            }
        }

        /// <summary>
        /// 類別小計與占比。ISO 即時把 Device.Emissions 加總（而且需要類別三~六，Area 沒有這些欄位），
        /// 環境部／資策會直接用 Area 上已存的欄位。兩者目前數值相同，但來源不同，刻意不統一（見 ScopeTotalsSource 的說明）。
        /// </summary>
        private static void WriteScopeTotals(Dictionary<string, string> patterns, GhgReportSpec spec, Area data, List<Device> device)
        {
            if (spec.Totals == ScopeTotalsSource.AreaColumns)
            {
                UpdateReplacePattern(patterns, "不確定性評估占比", data.percentage_CalAll.ToString("N2") + "%");
                UpdateReplacePattern(patterns, "總排放當量", data.All.ToString("N3"));
                UpdateReplacePattern(patterns, "類別一占比", data.percentage_Scope1.ToString("N2") + "%");
                UpdateReplacePattern(patterns, "類別二占比", data.percentage_Scope2.ToString("N2") + "%");
                UpdateReplacePattern(patterns, "類別一總排放", data.Scope1.ToString("N4"));
                UpdateReplacePattern(patterns, "類別二總排放", data.Scope2.ToString("N4"));
                return;
            }

            decimal sumAll = device.Sum(x => x.Emissions);
            UpdateReplacePattern(patterns, "不確定性評估占比", (sumAll != 0 ? (data.cal_all / sumAll) * 100 : 0).ToString("N2") + "%");
            UpdateReplacePattern(patterns, "總排放當量", sumAll.ToString("N3"));

            for (int i = 0; i < ScopeNames.Length; i++)
            {
                string scope = ScopeNames[i];
                decimal sumScope = device.Where(x => x.Scope == scope).Sum(x => x.Emissions);
                // 範本用的是「占比」（不是「佔比」，基準年那組才是「佔」），不要統一寫法，否則對不到標記。
                UpdateReplacePattern(patterns, scope + "占比", PercentText(sumScope, sumAll));
                UpdateReplacePattern(patterns, scope + "總排放", sumScope.ToString("N4"));
            }
        }

        private static readonly string[] ScopeNames = { "類別一", "類別二", "類別三", "類別四", "類別五", "類別六" };

        /// <summary>ISO 專屬：基準年小計、與基準年的差異百分比。</summary>
        private static void WriteBaseYearComparison(Dictionary<string, string> patterns, Area data, List<Device> device, Area baseYearArea)
        {
            List<Device> baseYearDevice = baseYearArea.Devices.ToList();
            decimal sumAll = device.Sum(x => x.Emissions);
            decimal sumBaseAll = baseYearDevice.Sum(x => x.Emissions);

            foreach (string scope in ScopeNames)
            {
                decimal sumScope = device.Where(x => x.Scope == scope).Sum(x => x.Emissions);
                decimal sumBaseScope = baseYearDevice.Where(x => x.Scope == scope).Sum(x => x.Emissions);

                UpdateReplacePattern(patterns, "基準年" + scope + "總排放", sumBaseScope.ToString("N4"));
                UpdateReplacePattern(patterns, "比較" + scope + "排放差異",
                    sumBaseScope != 0 ? ((sumScope - sumBaseScope) / sumBaseScope * 100).ToString("N2") + "%" : "0.00%");
                // 基準年這組範本用的是「佔比」（當年度那組是「占比」），兩個字不同，不能統一。
                UpdateReplacePattern(patterns, "基準年" + scope + "佔比", PercentText(sumBaseScope, sumBaseAll));
            }

            // 原本這一行漏了 * 100（上面類別一~六的差異都有乘），總排放差異在報告書上會顯示成正確值的 1/100。
            UpdateReplacePattern(patterns, "比較總排放差異",
                sumBaseAll != 0 ? ((sumAll - sumBaseAll) / sumBaseAll * 100).ToString("N2") + "%" : "0.00%");
            UpdateReplacePattern(patterns, "基準年總排放當量", sumBaseAll.ToString("N3"));

            if (data.BaseYear) //如果當年為基準年
            {
                UpdateReplacePattern(patterns, "當年度與基準年排放比較敘述", "因本年度為基準年，故無法進行差異性比較。");
            }
            else
            {
                UpdateReplacePattern(patterns, "當年度與基準年排放比較敘述", "");
            }
        }

        // ===================== 表格 =====================

        /// <summary>
        /// 插入所有表格。順序等同文件版面順序，而且必須在最後那次 ReplaceText 之前執行。
        /// 三份報告書原本這一段一字不差。
        /// </summary>
        private void EmitTables(DocX doc, List<Device> device, List<GHG> AllGHGs, List<ActivityData> AllActivityData)
        {
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
        }

        // ===================== 其他共用小工具 =====================

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
