using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Filters;
using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Xceed.Document.NET;
using Xceed.Words.NET;
using static System.Net.Mime.MediaTypeNames;

namespace Carbon_inventory_platform.Controllers
{
    [CheckSubscriptionData]
    [Authorize]
    public class EmissionController : CountController
    {
        private readonly ApplicationDbContext _context;
        public EmissionController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IActionResult> IndexAsync(Guid? id)
        {
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
        public async Task<IActionResult> ChartAsync(Guid? id)
        {
            TempData["yearId"] = id;
            bool success = await CountEmissionAsync(id);
            if (!success)
            {
                return NotFound();
            }

            var emissions = await _context.Areas.Include(y => y.Company).Where(x => x.Id == id).FirstOrDefaultAsync();
            return View(emissions);

        }
        public async Task<bool> CountEmissionAsync(Guid? id)
        {
            // 合并数据库查询
            var areaData = await _context.Areas
    .Include(a => a.Devices.Where(d => d.isDeleted == 0))  // 只包含 isDeleted == 0 的 Devices
    .ThenInclude(d => d.GHGs)
    .Where(a => a.Id == id && a.isDeleted == 0)  // Area 的 Id 等于 id，且 IsDeleted == 0
    .FirstOrDefaultAsync();


            if (areaData == null) return false;
            if (areaData.Devices == null) return false;
            // 初始化所有需要的变量
            decimal sumHardlyMove = 0, sumMove = 0, sumEscape = 0, sumProcess = 0, sumElectricity = 0;
            decimal sum1_CO2 = 0, sum2_CO2 = 0, sum1_CH4 = 0, sum2_CH4 = 0, sum1_N2O = 0, sum2_N2O = 0;
            decimal sum1_HFCS = 0, sum2_HFCS = 0, sum1_PFCS = 0, sum2_PFCS = 0, sum1_SF6 = 0, sum2_SF6 = 0, sum1_NF3 = 0, sum2_NF3 = 0;
            decimal sumScope1 = 0, sumScope2 = 0, sumAll = 0;
            int no1Grade = 0, no2Grade = 0, no3Grade = 0;
            float avgGrade = 0;
            decimal all_UUL = 0, all_ULL = 0, allCountUUL = 0, allCountULL = 0, sumUncertainty = 0;

            // 計算排放量及
            foreach (var device in areaData.Devices)
            {
                if (device.GHGs != null)
                {
                    foreach (var GHG in device.GHGs)
                    {
                        if (device.Scope == "類別一")
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
                        else if (device.Scope == "類別二")
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
                }
               
                switch (device.EmissionPattern)
                {
                    case "固定": sumHardlyMove += device.Emissions; break;
                    case "移動": sumMove += device.Emissions; break;
                    case "逸散": sumEscape += device.Emissions; break;
                    case "製程": sumProcess += device.Emissions; break;
                    case "外購電力": sumElectricity += device.Emissions; break;
                }

                if (device.Grade < 10 && device.Grade > 0) no1Grade++;
                else if (device.Grade < 19) no2Grade++;
                else no3Grade++;

                allCountULL += device.count_ULL;
                allCountUUL += device.count_UUL;

                if (device.all_ULL != 0)
                {
                    sumUncertainty += device.Emissions;
                }
            }

            decimal CO2 = sum1_CO2 + sum2_CO2;
            decimal CH4 = sum1_CH4 + sum2_CH4;
            decimal N2O = sum1_N2O + sum2_N2O;
            decimal HFCS = sum1_HFCS + sum2_HFCS;
            decimal PFCS = sum1_PFCS + sum2_PFCS;
            decimal SF6 = sum1_SF6 + sum2_SF6;
            decimal NF3 = sum1_NF3 + sum2_SF6;

            sumScope1 = sum1_CO2 + sum1_CH4 + sum1_N2O + sum1_HFCS + sum1_PFCS + sum1_NF3 + sum1_SF6;
            sumScope2 = sum2_CO2 + sum2_CH4 + sum2_N2O + sum2_HFCS + sum2_PFCS + sum2_NF3 + sum2_SF6;
            sumAll = sumScope1 + sumScope2;

            foreach (var device in areaData.Devices)
            {
                avgGrade += (float)Math.Round((float)(Math.Round(device.Emissions / sumAll, 4) * device.Grade), 2);
            }

            if (sumUncertainty != 0)
            {
                all_UUL = DecimalSqrt(allCountUUL) / sumUncertainty;
                all_ULL = DecimalSqrt(allCountULL) / sumUncertainty;
            }

            bool sumMatch = areaData.All != Math.Round(sumAll, 3);
            bool uncertaintyMatch = areaData.ULL != Math.Round(all_ULL, 2);

            if (sumMatch || uncertaintyMatch)
            {
                // 更新資料庫中的資料
                UpdateEmissionData(areaData, sum1_CO2, sum1_CH4, sum1_N2O, sum1_HFCS, sum1_PFCS, sum1_SF6, sum1_NF3,
                    sum2_CO2, sum2_CH4, sum2_N2O, sum2_HFCS, sum2_PFCS, sum2_SF6, sum2_NF3,
                    CO2, CH4, N2O, HFCS, PFCS, SF6, NF3,
                    sumScope1, sumScope2, sumAll,
                    sumHardlyMove, sumMove, sumEscape, sumProcess,
                    all_UUL, all_ULL, sumUncertainty,
                    no1Grade, no2Grade, no3Grade, avgGrade);

                await _context.SaveChangesAsync();
            }

            return true;
        }
        private void UpdateEmissionData(Area areaData,
            decimal sum1_CO2, decimal sum1_CH4, decimal sum1_N2O, decimal sum1_HFCS, decimal sum1_PFCS, decimal sum1_SF6, decimal sum1_NF3,
            decimal sum2_CO2, decimal sum2_CH4, decimal sum2_N2O, decimal sum2_HFCS, decimal sum2_PFCS, decimal sum2_SF6, decimal sum2_NF3,
            decimal CO2, decimal CH4, decimal N2O, decimal HFCS, decimal PFCS, decimal SF6, decimal NF3,
            decimal sumScope1, decimal sumScope2, decimal sumAll,
            decimal sumHardlyMove, decimal sumMove, decimal sumEscape, decimal sumProcess,
            decimal all_UUL, decimal all_ULL, decimal sumUncertainty,
            int no1Grade, int no2Grade, int no3Grade, float avgGrade)
        {
            areaData.Scope1_CO2 = sum1_CO2;
            areaData.Scope1_CH4 = sum1_CH4;
            areaData.Scope1_N2O = sum1_N2O;
            areaData.Scope1_HFCS = sum1_HFCS;
            areaData.Scope1_PFCS = sum1_PFCS;
            areaData.Scope1_SF6 = sum1_SF6;
            areaData.Scope1_NF3 = sum1_NF3;

            areaData.Scope2_CO2 = sum2_CO2;
            areaData.Scope2_CH4 = sum2_CH4;
            areaData.Scope2_N2O = sum2_N2O;
            areaData.Scope2_HFCS = sum2_HFCS;
            areaData.Scope2_PFCS = sum2_PFCS;
            areaData.Scope2_SF6 = sum2_SF6;
            areaData.Scope2_NF3 = sum2_NF3;

            areaData.CO2 = CO2;
            areaData.CH4 = CH4;
            areaData.N2O = N2O;
            areaData.HFCS = HFCS;
            areaData.PFCS = PFCS;
            areaData.SF6 = SF6;
            areaData.NF3 = NF3;

            areaData.Scope1 = sumScope1;
            areaData.Scope2 = sumScope2;
            areaData.All = sumAll;

            areaData.non_move = sumHardlyMove;
            areaData.move = sumMove;
            areaData.escape = sumEscape;
            areaData.process = sumProcess;

            if (sumScope1 != 0)
            {
                areaData.percentage1_CO2 = (sum1_CO2 / sumScope1 * 100);
                areaData.percentage1_CH4 = (sum1_CH4 / sumScope1 * 100);
                areaData.percentage1_N2O = (sum1_N2O / sumScope1 * 100);
                areaData.percentage1_HFCS = (sum1_HFCS / sumScope1 * 100);
                areaData.percentage1_PFCS = (sum1_PFCS / sumScope1 * 100);
                areaData.percentage1_SF6 = (sum1_SF6 / sumScope1 * 100);
                areaData.percentage1_NF3 = (sum1_NF3 / sumScope1 * 100);
            }
            else
            {
                areaData.percentage1_CO2 = 0;
                areaData.percentage1_CH4 = 0;
                areaData.percentage1_N2O = 0;
                areaData.percentage1_HFCS = 0;
                areaData.percentage1_PFCS = 0;
                areaData.percentage1_SF6 = 0;
                areaData.percentage1_NF3 = 0;
            }

            if (sumAll != 0)
            {
                areaData.percentage2_CO2 = (CO2 / sumAll * 100);
                areaData.percentage2_CH4 = (CH4 / sumAll * 100);
                areaData.percentage2_N2O = (N2O / sumAll * 100);
                areaData.percentage2_HFCS = (HFCS / sumAll * 100);
                areaData.percentage2_PFCS = (PFCS / sumAll * 100);
                areaData.percentage2_SF6 = (SF6 / sumAll * 100);
                areaData.percentage2_NF3 = (NF3 / sumAll * 100);

                areaData.percentage_nonMove = (sumHardlyMove / sumAll * 100);
                areaData.percentage_Move = (sumMove / sumAll * 100);
                areaData.percentage_Escape = (sumEscape / sumAll * 100);
                areaData.percentage_Process = (sumProcess / sumAll * 100);
                areaData.percentage_Scope1 = (sumScope1 / sumAll * 100);
                areaData.percentage_Scope2 = (sumScope2 / sumAll * 100);
            }
            else
            {
                areaData.percentage2_CO2 = 0;
                areaData.percentage2_CH4 = 0;
                areaData.percentage2_N2O = 0;
                areaData.percentage2_HFCS = 0;
                areaData.percentage2_PFCS = 0;
                areaData.percentage2_SF6 = 0;
                areaData.percentage2_NF3 = 0;

                areaData.percentage_nonMove = 0;
                areaData.percentage_Move = 0;
                areaData.percentage_Escape = 0;
                areaData.percentage_Process = 0;
                areaData.percentage_Scope1 = 0;
                areaData.percentage_Scope2 = 0;
            }

            areaData.cal_all = sumUncertainty;
            areaData.no1_Grade = no1Grade;
            areaData.no2_Grade = no2Grade;
            areaData.no3_Grade = no3Grade;
            areaData.avg_Grade = avgGrade;
            areaData.all_Grade = avgGrade < 10 ? "第一級" : (avgGrade < 19 ? "第二級" : "第三級");
            areaData.percentage_CalAll = (sumUncertainty / sumAll * 100);
            areaData.ULL = all_ULL;
            areaData.UUL = all_UUL;
        }
        public async Task<IActionResult> Word3Async(Guid id)
        {
            bool success = await CountEmissionAsync(id);
            if (!success)
            {
                return NotFound();
            }

            var dataQuery = _context.Areas
                .Where(x => x.Id == id && x.isDeleted == 0)
                .Include(x => x.Company)
                .Select(area => new
                {
                    Area = area,
                    Devices = _context.Devices.Where(d => d.AreaId == area.Id && d.isDeleted == 0)
                                               .OrderBy(d => d.Scope)
                                               .ThenBy(d => d.EmissionPattern).ToList(),
                    AllGHGs = _context.Devices.Where(d => d.AreaId == area.Id && d.isDeleted == 0)
                                               .SelectMany(d => d.GHGs).ToList(),
                    AllActivityData = _context.ActivityDatas.ToList()

                });

            var dataResult = await dataQuery.FirstOrDefaultAsync();
            if (dataResult == null) return NotFound();
            var AllActivityData = dataResult.AllActivityData;
            var data = dataResult.Area;
            if (data.Company == null)
            {
                return NotFound();
            }
            var device = dataResult.Devices;
            var AllGHGs = dataResult.AllGHGs;

            //-----------------檔案設定
            string currentDirectory = Directory.GetCurrentDirectory();
            string filePath = Path.Combine(currentDirectory, "wwwroot\\doc\\", "溫盤報告書範本3.docx");
            string newFilePath = Path.Combine(currentDirectory, "wwwroot\\output\\");
            string fileName = data.Year + "年度" + "-" + data.Company.Name + (data.Name != null ? ("-" + data.Name) : "") + "-溫室氣體盤查報告書.docx";
            string newFile = Path.Combine(newFilePath, fileName);

            if (!Directory.Exists(newFilePath))
            {
                Directory.CreateDirectory(newFilePath);
            }
            //-----------------檔案設定

            using (DocX doc = DocX.Load(filePath))
            {
                //-----------------替換的文本
                UpdateReplacePattern("聯絡人姓名", data.Company.ContactName);
                UpdateReplacePattern("聯絡人電話", data.Company.Phone);
                UpdateReplacePattern("聯絡人電子信箱", data.Company.Email);

                UpdateReplacePattern("公司中文名稱", data.Company.Name);
                UpdateReplacePattern("西元盤查年份", (data.Year + 1911).ToString());
                UpdateReplacePattern("民國盤查年份", (data.Year).ToString());
                if (data.Name != null)
                {
                    if (data.Name.Trim() != "")
                    {
                        UpdateReplacePattern("廠區名稱", data.Name);
                    }
                }
                UpdateReplacePattern("進行評估排放當量", data.cal_all.ToString("N4"));
                UpdateReplacePattern("不確定性評估占比", data.percentage_CalAll.ToString("N2") + "%");
                UpdateReplacePattern("第1級評分", data.no1_Grade.ToString());
                UpdateReplacePattern("第2級評分", data.no2_Grade.ToString());
                UpdateReplacePattern("第3級評分", data.no3_Grade.ToString());
                UpdateReplacePattern("清冊等級分數補充", data.avg_Grade.ToString());
                UpdateReplacePattern("清冊級別補充", data.all_Grade.ToString());
                UpdateReplacePattern("95上", "-" + data.UUL.ToString("N2") + "%");
                UpdateReplacePattern("95下", "+" + data.ULL.ToString("N2") + "%");

                if (data.Company.ReportOpening != null)
                {
                    string trimRO = data.Company.ReportOpening.Replace(" ", "");
                    trimRO = trimRO.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern("前言", trimRO);
                }
                else
                {
                    UpdateReplacePattern("前言", "自1997年12月第三次締約國大會(COP3)簽署京都議定書後，全球先進國家均研擬因應溫室氣體減量的方法與措施，2005年2月京都議定書正式生效後，全球各國更積極建立了溫室氣體排放管制的共識，2007年12月巴里路線圖進一步強調開發中國家應推動可量測(Measurable)、報告(Reportable)及可供查證(Verifiable)之適當減緩行動，2009年12月丹麥哥本哈根會議更針對後京都世界各國溫室氣體減量提出可行方案。\r\n    聯合國環境規劃署在2014年「溫室氣體排放缺口報告」指出，全球碳中和應於2055年至2070年間達成，否則地球將面臨氣候變遷災難。2015年聯合國發布「2030年永續發展方針」，公布17項永續發展目標(SDGs)，為人類和地球的「和平與繁榮」提供了現在與未來的共享藍圖。基於全球減碳趨勢，我國響應聯合國氣候變遷的目標，亦提出「2050淨零路徑里程碑」，隨後於112年1月10通過「氣候變遷因應法」，以達成節能減碳之永續發展目。\r\n    " + data.Company.Name + "(以下簡稱本公司)為因應全球永續發展趨勢於 2015年底《聯合國氣候變化綱要公約》第21屆締約國(COP21)後「巴黎協議」產生，加上環境部《溫室氣體減量及管理法》於 2015年7月正式公布實施，配合國家整體溫室氣體減量策略發展，以達成節能減碳之永續發展目標，特配合政府政策，持續進行公司內部溫室氣體盤查，以瞭解溫室氣體排放實況，進而訂定改善措施，以求達成二氧化碳排放減量之目標。 \r\n    本公司基於關心全球氣候變遷、善用資源及善盡企業的責任，根據ISO/IEC 14064-1： 2018要求，對溫室氣體管制發展趨勢及因應未來溫室氣體減量之要求，進行系統化的溫室氣體排放盤查 與清冊建置及查證程序等推動計畫，提供日後實施有效的減量改善方案作參考。今後除將持續推動溫室氣體排放管制以降低成本外，並期盼能達成兼顧資源效率、能源節約、環境保護的永續能源發展，共同為產業朝向低碳型經濟社會來努力。");
                }
                if (data.Company.CompanyInformation != null)
                {
                    string trimCM = data.Company.CompanyInformation.Replace(" ", "");
                    trimCM = trimCM.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern("公司基本資料", trimCM);
                }
                else
                {

                    UpdateReplacePattern("公司基本資料", " ");
                }
                if (data.Company.ReportingPurposes != null)
                {
                    string trimRP = data.Company.ReportingPurposes.Replace(" ", "");
                    trimRP = trimRP.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern("預期用途", trimRP);
                }
                else
                {
                    UpdateReplacePattern("預期用途", "為接軌國際議題，提升競爭力，以及實施社會責任，進行本報告書撰寫，以展現本公司溫室氣體盤查結果及推動減碳的決心。");
                }

                if (data.Company.AddressInformation != null)
                {
                    string trimAI = data.Company.AddressInformation.Replace(" ", "");
                    trimAI = trimAI.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern("組織邊界設定", trimAI);
                }
                else
                {
                    UpdateReplacePattern("組織邊界設定", "本次組織邊界之設定，遵循ISO 14064-1:2018標準，採用營運控制權法。");
                }
                if (data.Company.ReportingInformation != null)
                {
                    string trimRI = data.Company.ReportingInformation.Replace(" ", "");
                    trimRI = trimRI.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern("報告邊界", trimRI);
                }
                else
                {
                    UpdateReplacePattern("報告邊界", "");

                }
                if (data.Company.GHGInformation != null)
                {
                    string trimGI = data.Company.GHGInformation.Replace(" ", "");
                    trimGI = trimGI.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern("溫室氣體排放類型與排放量說明", trimGI);
                }
                else
                {
                    UpdateReplacePattern("溫室氣體排放類型與排放量說明", "溫室氣體之種類係指上述標準定義之溫室氣體，包括二氧化碳(CO₂)、甲烷(CH₄)、氧化亞氮(N₂O)、氫氟碳化物(HFCs)、全氟碳化物(PFCs)、六氟化硫(SF₆)、三氟化氮(NF₃)以及其他經環境部公告者，但不包含蒙特婁議定書規範之氟氯碳化物(CFCs)。");
                }
                if (data.Company.Scope1Information != null)
                {
                    string trimS1I = data.Company.Scope1Information.Replace(" ", "");
                    trimS1I = trimS1I.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern("直接溫室氣體排放說明", trimS1I);
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
                    UpdateReplacePattern("直接溫室氣體排放說明", text);

                }
                if (data.Company.Scope2Information != null)
                {
                    string trimS2I = data.Company.Scope2Information.Replace(" ", "");
                    trimS2I = trimS2I.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern("能源間接溫室氣體排放說明", trimS2I);
                }
                else
                {
                    UpdateReplacePattern("能源間接溫室氣體排放說明", "本廠類別二能源間接溫室氣體排放皆來自於外購電力部份。");
                }
                if (device.Find(x => x.Name == "WD40") != null)
                {
                    UpdateReplacePattern("WD40排放係數描述", "依WD-40之安全資料表(SDS)，可知其組成包含2~3 Wt%之CO₂，取其平均值2.5%；因CO₂係作為WD-40之推進劑，當使用WD-40時，亦將造成CO₂逸散，故假設每使用1單位重量之WD-40時，會有2.5%之單位重量CO₂隨之逸散。");
                }
                else
                {
                    UpdateReplacePattern("WD40排放係數描述", "");
                }
                UpdateReplacePattern("盤查月", DateTime.Now.Month.ToString());
                UpdateReplacePattern("盤查日", DateTime.Now.Day.ToString());
                UpdateReplacePattern("地址", data.FullAddress);
                UpdateReplacePattern("民國基準年", data.Year.ToString());

                ScopeDevice(doc, device, "類別一");
                ScopeDevice(doc, device, "類別二");
                if (device.Find(x => x.Name == "電力") != null)
                {
                    Guid deviceId = device.Find(x => x.Name == "電力").Id;
                    var deviceActivityData = AllActivityData.Where(x => x.DeviceId == deviceId).ToList();


                    if (deviceActivityData != null)
                    {
                        UpdateReplacePattern("電力使用量", (deviceActivityData.Sum(ad => ad.Num) / 1000).ToString());
                    }
                    else
                    {
                        UpdateReplacePattern("電力使用量", "0");
                    }
                }


                UpdateReplacePattern("類別一CO2排放", data.Scope1_CO2.ToString("N4"));
                UpdateReplacePattern("類別一CH4排放", data.Scope1_CH4.ToString("N4"));
                UpdateReplacePattern("類別一N2O排放", data.Scope1_N2O.ToString("N4"));
                UpdateReplacePattern("類別一HFCS排放", data.Scope1_HFCS.ToString("N4"));
                UpdateReplacePattern("類別一PFCS排放", data.Scope1_PFCS.ToString("N4"));
                UpdateReplacePattern("類別一SF6排放", data.Scope1_SF6.ToString("N4"));
                UpdateReplacePattern("類別一NF3排放", data.Scope1_NF3.ToString("N4"));

                UpdateReplacePattern("類別一CO2占比", data.percentage1_CO2.ToString("N2") + "%");
                UpdateReplacePattern("類別一CH4占比", data.percentage1_CH4.ToString("N2") + "%");
                UpdateReplacePattern("類別一N2O占比", data.percentage1_N2O.ToString("N2") + "%");
                UpdateReplacePattern("類別一HFCS占比", data.percentage1_HFCS.ToString("N2") + "%");
                UpdateReplacePattern("類別一PFCS占比", data.percentage1_PFCS.ToString("N2") + "%");
                UpdateReplacePattern("類別一SF6占比", data.percentage1_SF6.ToString("N2") + "%");
                UpdateReplacePattern("類別一NF3占比", data.percentage2_NF3.ToString("N2") + "%");

                UpdateReplacePattern("CO2排放", data.CO2.ToString("N4"));
                UpdateReplacePattern("CH4排放", data.CH4.ToString("N4"));
                UpdateReplacePattern("N2O排放", data.N2O.ToString("N4"));
                UpdateReplacePattern("HFCS排放", data.HFCS.ToString("N4"));
                UpdateReplacePattern("PFCS排放", data.PFCS.ToString("N4"));
                UpdateReplacePattern("SF6排放", data.SF6.ToString("N4"));
                UpdateReplacePattern("NF3排放", data.NF3.ToString("N4"));

                UpdateReplacePattern("CO2占比", data.percentage2_CO2.ToString("N2") + "%");
                UpdateReplacePattern("CH4占比", data.percentage2_CH4.ToString("N2") + "%");
                UpdateReplacePattern("N2O占比", data.percentage2_N2O.ToString("N2") + "%");
                UpdateReplacePattern("HFCS占比", data.percentage2_HFCS.ToString("N2") + "%");
                UpdateReplacePattern("PFCS占比", data.percentage2_PFCS.ToString("N2") + "%");
                UpdateReplacePattern("SF6占比", data.percentage2_SF6.ToString("N2") + "%");
                UpdateReplacePattern("NF3占比", data.percentage2_NF3.ToString("N2") + "%");
                UpdateReplacePattern("總排放當量", data.All.ToString("N3"));
                UpdateReplacePattern("固定排放量", data.non_move.ToString("N4"));
                UpdateReplacePattern("移動排放量", data.move.ToString("N4"));
                UpdateReplacePattern("製程排放量", data.process.ToString("N4"));
                UpdateReplacePattern("逸散排放量", data.escape.ToString("N4"));
                UpdateReplacePattern("固定排放比例", data.percentage_nonMove.ToString("N2") + "%");
                UpdateReplacePattern("製程排放比例", data.percentage_Process.ToString("N2") + "%");
                UpdateReplacePattern("移動排放比例", data.percentage_Move.ToString("N2") + "%");
                UpdateReplacePattern("逸散排放比例", data.percentage_Escape.ToString("N2") + "%");
                UpdateReplacePattern("類別一占比", data.percentage_Scope1.ToString("N2") + "%");
                UpdateReplacePattern("類別二占比", data.percentage_Scope2.ToString("N2") + "%");
                UpdateReplacePattern("類別一總排放", data.Scope1.ToString("N4"));
                UpdateReplacePattern("類別二總排放", data.Scope2.ToString("N4"));
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
                    UpdateReplacePattern("廠區圖", "");
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
                    UpdateReplacePattern("地理位置圖", "");
                }
                //--------------------------------圖片
                // Check if all the replace patterns are used in the loaded document.
                if (doc.FindUniqueByPattern(@"<[\w \=]{4,}>", RegexOptions.IgnoreCase).Count > 0)
                {
                    // Do the replacement of all the found tags and with green bold strings.
                    var replaceTextOptions = new FunctionReplaceTextOptions()
                    {
                        FindPattern = "<(.*?)>",
                        RegexMatchHandler = ReplaceFunc,
                        RegExOptions = RegexOptions.IgnoreCase,
                    };
                    doc.ReplaceText(replaceTextOptions);
                }

                doc.SaveAs(newFile);
            }

            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(newFile);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
        }
        public async Task<IActionResult> IISReportAsync(Guid id)
        {
            bool success = await CountEmissionAsync(id);
            if (!success)
            {
                return NotFound();
            }

            var dataQuery = _context.Areas
                .Where(x => x.Id == id && x.isDeleted == 0)
                .Include(x => x.Company)
                .Select(area => new
                {
                    Area = area,
                    Devices = _context.Devices.Where(d => d.AreaId == area.Id && d.isDeleted == 0)
                                               .OrderBy(d => d.Scope)
                                               .ThenBy(d => d.EmissionPattern).ToList(),
                    AllGHGs = _context.Devices.Where(d => d.AreaId == area.Id && d.isDeleted == 0)
                                               .SelectMany(d => d.GHGs).ToList(),
                    AllActivityData = _context.ActivityDatas.ToList()

                });

            var dataResult = await dataQuery.FirstOrDefaultAsync();
            if (dataResult == null) return NotFound();
            var AllActivityData = dataResult.AllActivityData;
            var data = dataResult.Area;
            if (data.Company == null)
            {
                return NotFound();
            }
            var device = dataResult.Devices;
            var AllGHGs = dataResult.AllGHGs;

            //-----------------檔案設定
            string currentDirectory = Directory.GetCurrentDirectory();
            string filePath = Path.Combine(currentDirectory, "wwwroot\\doc\\", "資策會版.docx");
            string newFilePath = Path.Combine(currentDirectory, "wwwroot\\output\\");
            string fileName = data.Year + "年度" + "-" + data.Company.Name + (data.Name != null ? ("-" + data.Name) : "") + "-溫室氣體盤查報告書.docx";
            string newFile = Path.Combine(newFilePath, fileName);
            if (!Directory.Exists(newFilePath))
            {
                Directory.CreateDirectory(newFilePath);
            }
            //-----------------檔案設定

            using (DocX doc = DocX.Load(filePath))
            {
                //-----------------替換的文本
                UpdateReplacePattern("聯絡人姓名", data.Company.ContactName);
                UpdateReplacePattern("聯絡人電話", data.Company.Phone);
                UpdateReplacePattern("聯絡人電子信箱", data.Company.Email);

                UpdateReplacePattern("公司中文名稱", data.Company.Name);
                UpdateReplacePattern("西元盤查年份", (data.Year + 1911).ToString());
                UpdateReplacePattern("民國盤查年份", (data.Year).ToString());
                if (data.Name != null)
                {
                    if (data.Name.Trim() != "")
                    {
                        UpdateReplacePattern("廠區名稱", data.Name);
                    }
                }
                UpdateReplacePattern("進行評估排放當量", data.cal_all.ToString("N4"));
                UpdateReplacePattern("不確定性評估占比", data.percentage_CalAll.ToString("N2") + "%");
                UpdateReplacePattern("第1級評分", data.no1_Grade.ToString());
                UpdateReplacePattern("第2級評分", data.no2_Grade.ToString());
                UpdateReplacePattern("第3級評分", data.no3_Grade.ToString());
                UpdateReplacePattern("清冊等級分數補充", data.avg_Grade.ToString());
                UpdateReplacePattern("清冊級別補充", data.all_Grade.ToString());
                UpdateReplacePattern("95上", "-" + data.UUL.ToString("N2") + "%");
                UpdateReplacePattern("95下", "+" + data.ULL.ToString("N2") + "%");

                if (data.Company.ReportOpening != null)
                {
                    string trimRO = data.Company.ReportOpening.Replace(" ", "");
                    trimRO = trimRO.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern("前言", trimRO);
                }
                else
                {
                    UpdateReplacePattern("前言", "自1997年12月第三次締約國大會(COP3)簽署京都議定書後，全球先進國家均研擬因應溫室氣體減量的方法與措施，2005年2月京都議定書正式生效後，全球各國更積極建立了溫室氣體排放管制的共識，2007年12月巴里路線圖進一步強調開發中國家應推動可量測(Measurable)、報告(Reportable)及可供查證(Verifiable)之適當減緩行動，2009年12月丹麥哥本哈根會議更針對後京都世界各國溫室氣體減量提出可行方案。\r\n    聯合國環境規劃署在2014年「溫室氣體排放缺口報告」指出，全球碳中和應於2055年至2070年間達成，否則地球將面臨氣候變遷災難。2015年聯合國發布「2030年永續發展方針」，公布17項永續發展目標(SDGs)，為人類和地球的「和平與繁榮」提供了現在與未來的共享藍圖。基於全球減碳趨勢，我國響應聯合國氣候變遷的目標，亦提出「2050淨零路徑里程碑」，隨後於112年1月10通過「氣候變遷因應法」，以達成節能減碳之永續發展目。\r\n    " + data.Company.Name + "(以下簡稱本公司)為因應全球永續發展趨勢於 2015年底《聯合國氣候變化綱要公約》第21屆締約國(COP21)後「巴黎協議」產生，加上環境部《溫室氣體減量及管理法》於 2015年7月正式公布實施，配合國家整體溫室氣體減量策略發展，以達成節能減碳之永續發展目標，特配合政府政策，持續進行公司內部溫室氣體盤查，以瞭解溫室氣體排放實況，進而訂定改善措施，以求達成二氧化碳排放減量之目標。 \r\n    本公司基於關心全球氣候變遷、善用資源及善盡企業的責任，根據ISO/IEC 14064-1： 2018要求，對溫室氣體管制發展趨勢及因應未來溫室氣體減量之要求，進行系統化的溫室氣體排放盤查 與清冊建置及查證程序等推動計畫，提供日後實施有效的減量改善方案作參考。今後除將持續推動溫室氣體排放管制以降低成本外，並期盼能達成兼顧資源效率、能源節約、環境保護的永續能源發展，共同為產業朝向低碳型經濟社會來努力。");
                }
                if (data.Company.CompanyInformation != null)
                {
                    string trimCM = data.Company.CompanyInformation.Replace(" ", "");
                    trimCM = trimCM.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern("公司基本資料", trimCM);
                }
                else
                {

                    UpdateReplacePattern("公司基本資料", " ");
                }
                if (data.Company.ReportingPurposes != null)
                {
                    string trimRP = data.Company.ReportingPurposes.Replace(" ", "");
                    trimRP = trimRP.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern("預期用途", trimRP);
                }
                else
                {
                    UpdateReplacePattern("預期用途", "為接軌國際議題，提升競爭力，以及實施社會責任，進行本報告書撰寫，以展現本公司溫室氣體盤查結果及推動減碳的決心。");
                }

                if (data.Company.AddressInformation != null)
                {
                    string trimAI = data.Company.AddressInformation.Replace(" ", "");
                    trimAI = trimAI.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern("組織邊界設定", trimAI);
                }
                else
                {
                    UpdateReplacePattern("組織邊界設定", "本次組織邊界之設定，遵循ISO 14064-1:2018標準，採用營運控制權法。");
                }
                if (data.Company.ReportingInformation != null)
                {
                    string trimRI = data.Company.ReportingInformation.Replace(" ", "");
                    trimRI = trimRI.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern("報告邊界", trimRI);
                }
                else
                {
                    UpdateReplacePattern("報告邊界", "");

                }
                if (data.Company.GHGInformation != null)
                {
                    string trimGI = data.Company.GHGInformation.Replace(" ", "");
                    trimGI = trimGI.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern("溫室氣體排放類型與排放量說明", trimGI);
                }
                else
                {
                    UpdateReplacePattern("溫室氣體排放類型與排放量說明", "溫室氣體之種類係指上述標準定義之溫室氣體，包括二氧化碳(CO₂)、甲烷(CH₄)、氧化亞氮(N₂O)、氫氟碳化物(HFCs)、全氟碳化物(PFCs)、六氟化硫(SF₆)、三氟化氮(NF₃)以及其他經環境部公告者，但不包含蒙特婁議定書規範之氟氯碳化物(CFCs)。");
                }
                if (data.Company.Scope1Information != null)
                {
                    string trimS1I = data.Company.Scope1Information.Replace(" ", "");
                    trimS1I = trimS1I.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern("直接溫室氣體排放說明", trimS1I);
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
                    UpdateReplacePattern("直接溫室氣體排放說明", text);

                }
                if (data.Company.Scope2Information != null)
                {
                    string trimS2I = data.Company.Scope2Information.Replace(" ", "");
                    trimS2I = trimS2I.Replace("\r\n", "\r\n    ");
                    UpdateReplacePattern("能源間接溫室氣體排放說明", trimS2I);
                }
                else
                {
                    UpdateReplacePattern("能源間接溫室氣體排放說明", "本廠類別二能源間接溫室氣體排放皆來自於外購電力部份。");
                }
                if (device.Find(x => x.Name == "WD40") != null)
                {
                    UpdateReplacePattern("WD40排放係數描述", "依WD-40之安全資料表(SDS)，可知其組成包含2~3 Wt%之CO₂，取其平均值2.5%；因CO₂係作為WD-40之推進劑，當使用WD-40時，亦將造成CO₂逸散，故假設每使用1單位重量之WD-40時，會有2.5%之單位重量CO₂隨之逸散。");
                }
                else
                {
                    UpdateReplacePattern("WD40排放係數描述", "");
                }
                UpdateReplacePattern("盤查月", DateTime.Now.Month.ToString());
                UpdateReplacePattern("盤查日", DateTime.Now.Day.ToString());
                UpdateReplacePattern("地址", data.FullAddress);
                UpdateReplacePattern("民國基準年", data.Year.ToString());

                ScopeDevice(doc, device, "類別一");
                ScopeDevice(doc, device, "類別二");
                if (device.Find(x => x.Name == "電力") != null)
                {
                    Guid deviceId = device.Find(x => x.Name == "電力").Id;
                    var deviceActivityData = AllActivityData.Where(x => x.DeviceId == deviceId).ToList();


                    if (deviceActivityData != null)
                    {
                        UpdateReplacePattern("電力使用量", (deviceActivityData.Sum(ad => ad.Num) / 1000).ToString());
                    }
                    else
                    {
                        UpdateReplacePattern("電力使用量", "0");
                    }
                }


                UpdateReplacePattern("類別一CO2排放", data.Scope1_CO2.ToString("N4"));
                UpdateReplacePattern("類別一CH4排放", data.Scope1_CH4.ToString("N4"));
                UpdateReplacePattern("類別一N2O排放", data.Scope1_N2O.ToString("N4"));
                UpdateReplacePattern("類別一HFCS排放", data.Scope1_HFCS.ToString("N4"));
                UpdateReplacePattern("類別一PFCS排放", data.Scope1_PFCS.ToString("N4"));
                UpdateReplacePattern("類別一SF6排放", data.Scope1_SF6.ToString("N4"));
                UpdateReplacePattern("類別一NF3排放", data.Scope1_NF3.ToString("N4"));

                UpdateReplacePattern("類別一CO2占比", data.percentage1_CO2.ToString("N2") + "%");
                UpdateReplacePattern("類別一CH4占比", data.percentage1_CH4.ToString("N2") + "%");
                UpdateReplacePattern("類別一N2O占比", data.percentage1_N2O.ToString("N2") + "%");
                UpdateReplacePattern("類別一HFCS占比", data.percentage1_HFCS.ToString("N2") + "%");
                UpdateReplacePattern("類別一PFCS占比", data.percentage1_PFCS.ToString("N2") + "%");
                UpdateReplacePattern("類別一SF6占比", data.percentage1_SF6.ToString("N2") + "%");
                UpdateReplacePattern("類別一NF3占比", data.percentage2_NF3.ToString("N2") + "%");

                UpdateReplacePattern("CO2排放", data.CO2.ToString("N4"));
                UpdateReplacePattern("CH4排放", data.CH4.ToString("N4"));
                UpdateReplacePattern("N2O排放", data.N2O.ToString("N4"));
                UpdateReplacePattern("HFCS排放", data.HFCS.ToString("N4"));
                UpdateReplacePattern("PFCS排放", data.PFCS.ToString("N4"));
                UpdateReplacePattern("SF6排放", data.SF6.ToString("N4"));
                UpdateReplacePattern("NF3排放", data.NF3.ToString("N4"));

                UpdateReplacePattern("CO2占比", data.percentage2_CO2.ToString("N2") + "%");
                UpdateReplacePattern("CH4占比", data.percentage2_CH4.ToString("N2") + "%");
                UpdateReplacePattern("N2O占比", data.percentage2_N2O.ToString("N2") + "%");
                UpdateReplacePattern("HFCS占比", data.percentage2_HFCS.ToString("N2") + "%");
                UpdateReplacePattern("PFCS占比", data.percentage2_PFCS.ToString("N2") + "%");
                UpdateReplacePattern("SF6占比", data.percentage2_SF6.ToString("N2") + "%");
                UpdateReplacePattern("NF3占比", data.percentage2_NF3.ToString("N2") + "%");
                UpdateReplacePattern("總排放當量", data.All.ToString("N3"));
                UpdateReplacePattern("固定排放量", data.non_move.ToString("N4"));
                UpdateReplacePattern("移動排放量", data.move.ToString("N4"));
                UpdateReplacePattern("製程排放量", data.process.ToString("N4"));
                UpdateReplacePattern("逸散排放量", data.escape.ToString("N4"));
                UpdateReplacePattern("固定排放比例", data.percentage_nonMove.ToString("N2") + "%");
                UpdateReplacePattern("製程排放比例", data.percentage_Process.ToString("N2") + "%");
                UpdateReplacePattern("移動排放比例", data.percentage_Move.ToString("N2") + "%");
                UpdateReplacePattern("逸散排放比例", data.percentage_Escape.ToString("N2") + "%");
                UpdateReplacePattern("類別一占比", data.percentage_Scope1.ToString("N2") + "%");
                UpdateReplacePattern("類別二占比", data.percentage_Scope2.ToString("N2") + "%");
                UpdateReplacePattern("類別一總排放", data.Scope1.ToString("N4"));
                UpdateReplacePattern("類別二總排放", data.Scope2.ToString("N4"));
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
                    UpdateReplacePattern("廠區圖", "");
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
                    UpdateReplacePattern("公司組織圖", "");
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
                    UpdateReplacePattern("地理位置圖", "");
                }
                //--------------------------------圖片
                // Check if all the replace patterns are used in the loaded document.
                if (doc.FindUniqueByPattern(@"<[\w \=]{4,}>", RegexOptions.IgnoreCase).Count > 0)
                {
                    // Do the replacement of all the found tags and with green bold strings.
                    var replaceTextOptions = new FunctionReplaceTextOptions()
                    {
                        FindPattern = "<(.*?)>",
                        RegexMatchHandler = ReplaceFunc,
                        RegExOptions = RegexOptions.IgnoreCase,

                    };
                    doc.ReplaceText(replaceTextOptions);
                }
                doc.SaveAs(newFile);
            }

            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(newFile);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
        }
        private static Dictionary<string, string> _replacePatterns = new Dictionary<string, string>() { };
        private static string ReplaceFunc(string findStr)
        {
            if (_replacePatterns.ContainsKey(findStr))
            {
                return _replacePatterns[findStr];
            }
            return string.Empty;
        }
        public static void UpdateReplacePattern(string key, string value)
        {
            if (_replacePatterns.ContainsKey(key))
            {
                _replacePatterns[key] = value;
            }
            else
            {
                _replacePatterns.Add(key, value);
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

            doc.ReplaceTextWithObject(tableName, table);
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
            doc.ReplaceTextWithObject("報告邊界表", table);
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

            doc.ReplaceTextWithObject("" + scope + "Table", table);

        }
        public void ScopeDevice(DocX doc, List<Device> devices, string scope)
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
            UpdateReplacePattern("" + scope + "Device", Scope.Trim());
        }
    }
}
