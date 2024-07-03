using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Filters;
using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace Carbon_inventory_platform.Controllers
{
    [CheckSubscriptionData]
    [Authorize]
    public class EmissionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmissionController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> IndexAsync(Guid? id)
        {
            TempData["yearId"] = id;
            await CountEmissionAsync(id);

            var emissions = await _context.Areas.Include(y => y.Company).Where(x => x.Id == id).FirstOrDefaultAsync();
            return View(emissions);

        }
        public async Task<Area> CountEmissionAsync(Guid? id)
        {

            var Devices = await _context.Devices
                .Where(x => x.isDeleted == 0 && x.AreaId == id)
                .ToListAsync();
            var GHGs = await _context.GHGs
                .Include(x => x.Device)
                .Where(x => x.Device.isDeleted == 0 && x.Device.AreaId == id)
                .ToListAsync();
            var Emission = await _context.Areas.Where(x => x.Id == id).FirstOrDefaultAsync();

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

                all_countULL += device.count_ULL;
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


            bool sumMatch = Emission.All != Math.Round(sum_all, 3);
            bool uncertaintyMatch = Emission.ULL != Math.Round(all_ULL, 2);
            if (sumMatch || uncertaintyMatch) //如果總量和不確定性上限有差，則修正進資料庫。
            {

                Emission.Scope1_CO2 = sum1_CO2;
                Emission.Scope1_CH4 = sum1_CH4;
                Emission.Scope1_N2O = sum1_N2O;
                Emission.Scope1_HFCS = sum1_HFCS;
                Emission.Scope1_PFCS = sum1_PFCS;
                Emission.Scope1_SF6 = sum1_SF6;
                Emission.Scope1_NF3 = sum1_NF3;

                Emission.Scope2_CO2 = sum2_CO2;
                Emission.Scope2_CH4 = sum2_CH4;
                Emission.Scope2_N2O = sum2_N2O;
                Emission.Scope2_HFCS = sum2_HFCS;
                Emission.Scope2_PFCS = sum2_PFCS;
                Emission.Scope2_SF6 = sum2_SF6;
                Emission.Scope2_NF3 = sum2_NF3;

                Emission.CO2 = CO2;
                Emission.CH4 = CH4;
                Emission.N2O = N2O;
                Emission.HFCS = HFCS;
                Emission.PFCS = PFCS;
                Emission.SF6 = SF6;
                Emission.NF3 = NF3;

                Emission.Scope1 = sum_Scope1;
                Emission.Scope2 = sum_Scope2;

                Emission.All = sum_all;

                Emission.non_move = sum_hardlymove;
                Emission.move = sum_move;
                Emission.escape = sum_escape;
                Emission.process = sum_process;
                if (sum_Scope1 != 0)
                {
                    Emission.percentage1_CO2 = (sum1_CO2 / sum_Scope1 * 100);
                    Emission.percentage1_CH4 = (sum1_CH4 / sum_Scope1 * 100);
                    Emission.percentage1_N2O = (sum1_N2O / sum_Scope1 * 100);
                    Emission.percentage1_HFCS = (sum1_HFCS / sum_Scope1 * 100);
                    Emission.percentage1_PFCS = (sum1_PFCS / sum_Scope1 * 100);
                    Emission.percentage1_SF6 = (sum1_SF6 / sum_Scope1 * 100);
                    Emission.percentage1_NF3 = (sum1_NF3 / sum_Scope1 * 100);
                }
                else
                {
                    Emission.percentage1_CO2 = 0;
                    Emission.percentage1_CH4 = 0;
                    Emission.percentage1_N2O = 0;
                    Emission.percentage1_HFCS = 0;
                    Emission.percentage1_PFCS = 0;
                    Emission.percentage1_SF6 = 0;
                    Emission.percentage1_NF3 = 0;
                }

                if (sum_all != 0)
                {
                    Emission.percentage2_CO2 = (CO2 / sum_all * 100);
                    Emission.percentage2_CH4 = (CH4 / sum_all * 100);
                    Emission.percentage2_N2O = (N2O / sum_all * 100);
                    Emission.percentage2_HFCS = (HFCS / sum_all * 100);
                    Emission.percentage2_PFCS = (PFCS / sum_all * 100);
                    Emission.percentage2_SF6 = (SF6 / sum_all * 100);
                    Emission.percentage2_NF3 = (NF3 / sum_all * 100);

                    Emission.percentage_nonMove = (sum_hardlymove / sum_all * 100);
                    Emission.percentage_Move = (sum_move / sum_all * 100);
                    Emission.percentage_Escape = (sum_escape / sum_all * 100);
                    Emission.percentage_Process = (sum_process / sum_all * 100);
                    Emission.percentage_Scope1 = (sum_Scope1 / sum_all * 100);
                    Emission.percentage_Scope2 = (sum_Scope2 / sum_all * 100);
                }
                else
                {
                    Emission.percentage2_CO2 = 0;
                    Emission.percentage2_CH4 = 0;
                    Emission.percentage2_N2O = 0;
                    Emission.percentage2_HFCS = 0;
                    Emission.percentage2_PFCS = 0;
                    Emission.percentage2_SF6 = 0;
                    Emission.percentage2_NF3 = 0;

                    Emission.percentage_nonMove = 0;
                    Emission.percentage_Move = 0;
                    Emission.percentage_Escape = 0;
                    Emission.percentage_Process = 0;
                    Emission.percentage_Scope1 = 0;
                    Emission.percentage_Scope2 = 0;
                }




                Emission.cal_all = sum_Uncertainty;
                Emission.no1_Grade = no1_Grade;
                Emission.no2_Grade = no2_Grade;
                Emission.no3_Grade = no3_Grade;
                Emission.avg_Grade = avg_Grade;
                Emission.all_Grade = avg_Grade < 10 ? "第一級" : (avg_Grade < 19 ? "第二級" : "第三級");
                Emission.percentage_CalAll = (sum_Uncertainty / sum_all * 100);
                Emission.ULL = all_ULL;
                Emission.UUL = all_UUL;
            }



            await _context.SaveChangesAsync();
            return null;
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


        public async Task<IActionResult> Word3Async(Guid id)
        {
            await CountEmissionAsync(id);
            var data = await _context.Areas.Where(x => x.Id == id && x.isDeleted == 0).Include(x => x.Company).FirstOrDefaultAsync();
            var device = await _context.Devices.Where(x => x.AreaId == id && x.isDeleted == 0).OrderBy(x => x.Scope).ThenBy(x => x.EmissionPattern).ToListAsync();
            var ManyGHGs = await _context.Devices.Where(x => x.AreaId == id && x.isDeleted == 0).SelectMany(x => x.GHGs).ToListAsync();

            //-----------------檔案設定
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\doc\\", "溫盤報告書範本3.docx");
            string newFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\output\\");
            string newFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\output\\", data.Year + "年度" + "-" + data.Company.Name + (data.Name != null ? ("-" + data.Name) : "") + "-溫室氣體盤查報告書.docx");
            if (!Directory.Exists(newFilePath))
            { Directory.CreateDirectory(newFilePath); }
            //-----------------檔案設定
            // 複製文件
            using (DocX doc = DocX.Load(filePath))
            {
                //-----------------替換的文本
                doc.ReplaceText("[聯絡人姓名]", data.Company.ContactName);
                doc.ReplaceText("[聯絡人電話]", data.Company.Phone);
                doc.ReplaceText("[聯絡人電子信箱]", data.Company.Email);

                doc.ReplaceText("[公司中文名稱]", data.Company.Name);
                doc.ReplaceText("[西元盤查年份]", (data.Year + 1911).ToString());
                doc.ReplaceText("[民國盤查年份]", (data.Year).ToString());
                if (data.Name != null)
                {
                    if (data.Name.Trim() != "")
                    {
                        doc.ReplaceText("[廠區名稱]", data.Name.ToString());
                    }
                }
                doc.ReplaceText("[進行評估排放當量]", data.cal_all.ToString("N4"));
                doc.ReplaceText("[不確定性評估占比]", data.percentage_CalAll.ToString("N2") + "%");
                doc.ReplaceText("[第1級評分]", data.no1_Grade.ToString());
                doc.ReplaceText("[第2級評分]", data.no2_Grade.ToString());
                doc.ReplaceText("[第3級評分]", data.no3_Grade.ToString());
                doc.ReplaceText("[清冊等級分數補充]", data.avg_Grade.ToString());
                doc.ReplaceText("[清冊級別補充]", data.all_Grade.ToString());
                doc.ReplaceText("[95上]", "-" + data.UUL.ToString("N2") + "%");
                doc.ReplaceText("[95下]", "+" + data.ULL.ToString("N2") + "%");
                
                if (data.Company.ReportOpening != null)
                {
                    string trimRO = data.Company.ReportOpening.Replace(" ", "");
                    trimRO = trimRO.Replace("\r\n", "\r\n    ");
                    doc.ReplaceText("[前言]", trimRO);
                }
                else
                {
                    doc.ReplaceText("[前言]", "自1997年12月第三次締約國大會(COP3)簽署京都議定書後，全球先進國家均研擬因應溫室氣體減量的方法與措施，2005年2月京都議定書正式生效後，全球各國更積極建立了溫室氣體排放管制的共識，2007年12月巴里路線圖進一步強調開發中國家應推動可量測(Measurable)、報告(Reportable)及可供查證(Verifiable)之適當減緩行動，2009年12月丹麥哥本哈根會議更針對後京都世界各國溫室氣體減量提出可行方案。\r\n    聯合國環境規劃署在2014年「溫室氣體排放缺口報告」指出，全球碳中和應於2055年至2070年間達成，否則地球將面臨氣候變遷災難。2015年聯合國發布「2030年永續發展方針」，公布17項永續發展目標(SDGs)，為人類和地球的「和平與繁榮」提供了現在與未來的共享藍圖。基於全球減碳趨勢，我國響應聯合國氣候變遷的目標，亦提出「2050淨零路徑里程碑」，隨後於112年1月10通過「氣候變遷因應法」，以達成節能減碳之永續發展目。\r\n    " + data.Company.Name + "(以下簡稱本公司)為因應全球永續發展趨勢於 2015年底《聯合國氣候變化綱要公約》第21屆締約國(COP21)後「巴黎協議」產生，加上環境部《溫室氣體減量及管理法》於 2015年7月正式公布實施，配合國家整體溫室氣體減量策略發展，以達成節能減碳之永續發展目標，特配合政府政策，持續進行公司內部溫室氣體盤查，以瞭解溫室氣體排放實況，進而訂定改善措施，以求達成二氧化碳排放減量之目標。 \r\n    本公司基於關心全球氣候變遷、善用資源及善盡企業的責任，根據ISO/IEC 14064-1： 2018要求，對溫室氣體管制發展趨勢及因應未來溫室氣體減量之要求，進行系統化的溫室氣體排放盤查 與清冊建置及查證程序等推動計畫，提供日後實施有效的減量改善方案作參考。今後除將持續推動溫室氣體排放管制以降低成本外，並期盼能達成兼顧資源效率、能源節約、環境保護的永續能源發展，共同為產業朝向低碳型經濟社會來努力。");
                }
                if (data.Company.CompanyInformation != null)
                {
                    string trimCM = data.Company.CompanyInformation.Replace(" ","");
                    trimCM = trimCM.Replace("\r\n", "\r\n    ");
                    doc.ReplaceText("[公司基本資料]", trimCM);
                }
                else
                {　
        
                    doc.ReplaceText("[公司基本資料]", " ");
                }
                if (data.Company.ReportingPurposes != null)
                {
                    string trimRP = data.Company.ReportingPurposes.Replace(" ", "");
                    trimRP = trimRP.Replace("\r\n", "\r\n    ");
                    doc.ReplaceText("[預期用途]", trimRP);
                }
                else
                {
                    doc.ReplaceText("[預期用途]", "為接軌國際議題，提升競爭力，以及實施社會責任，進行本報告書撰寫，以展現本公司溫室氣體盤查結果及推動減碳的決心。");
                }

                if (data.Company.AddressInformation != null)
                {
                    string trimAI = data.Company.AddressInformation.Replace(" ", "");
                    trimAI = trimAI.Replace("\r\n", "\r\n    ");
                    doc.ReplaceText("[組織邊界設定]", trimAI);
                }
                else
                {
                    doc.ReplaceText("[組織邊界設定]", "本次組織邊界之設定，遵循ISO 14064-1:2018標準，採用營運控制權法。");
                }
                if (data.Company.ReportingInformation != null)
                {
                    string trimRI = data.Company.ReportingInformation.Replace(" ", "");
                    trimRI = trimRI.Replace("\r\n", "\r\n    ");
                    doc.ReplaceText("[報告邊界]", trimRI);
                }
                else
                {
                    doc.ReplaceText("[報告邊界]", "");

                }
                if (data.Company.GHGInformation != null)
                {
                    string trimGI = data.Company.GHGInformation.Replace(" ", "");
                    trimGI = trimGI.Replace("\r\n", "\r\n    ");
                    doc.ReplaceText("[溫室氣體排放類型與排放量說明]", trimGI);
                }
                else
                {
                    doc.ReplaceText("[溫室氣體排放類型與排放量說明]", "溫室氣體之種類係指上述標準定義之溫室氣體，包括二氧化碳(CO₂)、甲烷(CH₄)、氧化亞氮(N₂O)、氫氟碳化物(HFCs)、全氟碳化物(PFCs)、六氟化硫(SF₆)、三氟化氮(NF₃)以及其他經環境部公告者，但不包含蒙特婁議定書規範之氟氯碳化物(CFCs)。");
                }
                if (data.Company.Scope1Information != null)
                {
                    string trimS1I = data.Company.Scope1Information.Replace(" ", "");
                    trimS1I = trimS1I.Replace("\r\n", "\r\n    ");
                    doc.ReplaceText("[直接溫室氣體排放說明]", trimS1I);
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
                            if (item.Name != "其他")
                            {
                                text += item.Name;
                            }
                            else
                            {
                                text += item.OtherName;
                            }
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
                            if (item.Name != "其他")
                            {
                                text += item.Name;
                            }
                            else
                            {
                                text += item.OtherName;
                            }
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
                            if (item.Name != "其他")
                            {
                                text += item.Name;
                            }
                            else
                            {
                                text += item.OtherName;
                            }
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
                            if (item.Name != "其他")
                            {
                                text += item.Name;
                            }
                            else
                            {
                                text += item.OtherName;
                            }
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
                    doc.ReplaceText("[直接溫室氣體排放說明]", text);

                }
                if (data.Company.Scope2Information != null)
                {
                    string trimS2I = data.Company.Scope2Information.Replace(" ", "");
                    trimS2I = trimS2I.Replace("\r\n", "\r\n    ");
                    doc.ReplaceText("[能源間接溫室氣體排放說明]", trimS2I);
                }
                else
                {
                    doc.ReplaceText("[能源間接溫室氣體排放說明]", "本廠類別二能源間接溫室氣體排放皆來自於外購電力部份。");
                }
                if (device.Find(x => x.Name == "WD40") != null)
                {
                    doc.ReplaceText("[WD40排放係數描述]", "依WD-40之安全資料表(SDS)，可知其組成包含2~3 Wt%之CO₂，取其平均值2.5%；因CO₂係作為WD-40之推進劑，當使用WD-40時，亦將造成CO₂逸散，故假設每使用1單位重量之WD-40時，會有2.5%之單位重量CO₂隨之逸散。");
                }
                else
                {
                    doc.ReplaceText("[WD40排放係數描述]", "");
                }
                doc.ReplaceText("[盤查月]", DateTime.Now.Month.ToString());
                doc.ReplaceText("[盤查日]", DateTime.Now.Day.ToString());
                doc.ReplaceText("[地址]", data.FullAddress);
                doc.ReplaceText("[民國基準年]", data.Year.ToString());

                ScopeDevice(doc, device, "類別一");
                ScopeDevice(doc, device, "類別二");
                if (device.Find(x => x.Name == "電力") != null)
                {
                    Guid deviceId = device.Find(x => x.Name == "電力").Id;
                    var ActivityData = await _context.ActivityDatas.Where(x => x.DeviceId == deviceId).ToListAsync();
                    if (ActivityData != null)
                    {
                        doc.ReplaceText("[電力使用量]", (ActivityData.Sum(ad => ad.Num) / 1000).ToString());
                    }
                    else
                    {
                        doc.ReplaceText("[電力使用量]", "0");
                    }
                }


                doc.ReplaceText("[類別一CO2排放]", data.Scope1_CO2.ToString("N4"));
                doc.ReplaceText("[類別一CH4排放]", data.Scope1_CH4.ToString("N4"));
                doc.ReplaceText("[類別一N2O排放]", data.Scope1_N2O.ToString("N4"));
                doc.ReplaceText("[類別一HFCS排放]", data.Scope1_HFCS.ToString("N4"));
                doc.ReplaceText("[類別一PFCS排放]", data.Scope1_PFCS.ToString("N4"));
                doc.ReplaceText("[類別一SF6排放]", data.Scope1_SF6.ToString("N4"));
                doc.ReplaceText("[類別一NF3排放]", data.Scope1_NF3.ToString("N4"));

                doc.ReplaceText("[類別一CO2占比]", data.percentage1_CO2.ToString("N2") + "%");
                doc.ReplaceText("[類別一CH4占比]", data.percentage1_CH4.ToString("N2") + "%");
                doc.ReplaceText("[類別一N2O占比]", data.percentage1_N2O.ToString("N2") + "%");
                doc.ReplaceText("[類別一HFCS占比]", data.percentage1_HFCS.ToString("N2") + "%");
                doc.ReplaceText("[類別一PFCS占比]", data.percentage1_PFCS.ToString("N2") + "%");
                doc.ReplaceText("[類別一SF6占比]", data.percentage1_SF6.ToString("N2") + "%");
                doc.ReplaceText("[類別一NF3占比]", data.percentage2_NF3.ToString("N2") + "%");

                doc.ReplaceText("[CO2排放]", data.CO2.ToString("N4"));
                doc.ReplaceText("[CH4排放]", data.CH4.ToString("N4"));
                doc.ReplaceText("[N2O排放]", data.N2O.ToString("N4"));
                doc.ReplaceText("[HFCS排放]", data.HFCS.ToString("N4"));
                doc.ReplaceText("[PFCS排放]", data.PFCS.ToString("N4"));
                doc.ReplaceText("[SF6排放]", data.SF6.ToString("N4"));
                doc.ReplaceText("[NF3排放]", data.NF3.ToString("N4"));

                doc.ReplaceText("[CO2占比]", data.percentage2_CO2.ToString("N2") + "%");
                doc.ReplaceText("[CH4占比]", data.percentage2_CH4.ToString("N2") + "%");
                doc.ReplaceText("[N2O占比]", data.percentage2_N2O.ToString("N2") + "%");
                doc.ReplaceText("[HFCS占比]", data.percentage2_HFCS.ToString("N2") + "%");
                doc.ReplaceText("[PFCS占比]", data.percentage2_PFCS.ToString("N2") + "%");
                doc.ReplaceText("[SF6占比]", data.percentage2_SF6.ToString("N2") + "%");
                doc.ReplaceText("[NF3占比]", data.percentage2_NF3.ToString("N2") + "%");
                doc.ReplaceText("[總排放當量]", data.All.ToString("N3"));
                doc.ReplaceText("[固定排放量]", data.non_move.ToString("N4"));
                doc.ReplaceText("[移動排放量]", data.move.ToString("N4"));
                doc.ReplaceText("[製程排放量]", data.process.ToString("N4"));
                doc.ReplaceText("[逸散排放量]", data.escape.ToString("N4"));
                doc.ReplaceText("[固定排放比例]", data.percentage_nonMove.ToString("N2") + "%");
                doc.ReplaceText("[製程排放比例]", data.percentage_Process.ToString("N2") + "%");
                doc.ReplaceText("[移動排放比例]", data.percentage_Move.ToString("N2") + "%");
                doc.ReplaceText("[逸散排放比例]", data.percentage_Escape.ToString("N2") + "%");
                doc.ReplaceText("[類別一占比]", data.percentage_Scope1.ToString("N2") + "%");
                doc.ReplaceText("[類別二占比]", data.percentage_Scope2.ToString("N2") + "%");
                doc.ReplaceText("[類別一總排放]", data.Scope1.ToString("N4"));
                doc.ReplaceText("[類別二總排放]", data.Scope2.ToString("N4"));
                //-----------------替換的文本
                //--------------------------------排放源溫室氣體表表
                GenerateGHGsTable(doc, ManyGHGs, "固定", "CO2");
                GenerateGHGsTable(doc, ManyGHGs, "固定", "CH4");
                GenerateGHGsTable(doc, ManyGHGs, "固定", "N2O");
                GenerateGHGsTable(doc, ManyGHGs, "移動", "CO2");
                GenerateGHGsTable(doc, ManyGHGs, "移動", "CH4");
                GenerateGHGsTable(doc, ManyGHGs, "移動", "N2O");
                GenerateGHGsTable(doc, ManyGHGs, "逸散", "CO2");
                GenerateGHGsTable(doc, ManyGHGs, "逸散", "CH4");
                GenerateGHGsTable(doc, ManyGHGs, "逸散", "HFCS");
                GenerateGHGsTable(doc, ManyGHGs, "製程", "CO2");
                GenerateGHGsTable(doc, ManyGHGs, "外購電力", "CO2");
                //--------------------------------排放源溫室氣體表表
                //--------------------------------類別表
                if (device.Any(x => x.Scope == "類別一"))
                {
                    GenerateScopeTable(doc, device, "類別一");
                }
                // 檢查是否有類別二設備
                if (device.Any(x => x.Scope == "類別二"))
                {
                    GenerateScopeTable(doc, device, "類別二");
                }
                //--------------------------------類別表
                //--------------------------------報告邊界
                GenerateReport(doc, device);
                //--------------------------------報告邊界
                GenerateActivityDataTable(doc, device);

                //--------------------------------圖片                               
                if (data.ShopDrawingsPath != "")
                {
                    try
                    {
                        string showDrawingPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", data.CompanyId.ToString(), data.Id.ToString(), data.ShopDrawingsPath);
                        replaceImage(doc, "[廠區圖]", showDrawingPath);
                    }
                    catch (Exception e)
                    {
                        ViewData["showDrawingError"] = e.Message;
                        throw;
                    }                    
                }
                else
                {
                    doc.ReplaceText("[廠區圖]", "");
                }

                if (data.OrganizationImagePath != "")
                {
                    try
                    {
                        string organiztionPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", data.CompanyId.ToString(), data.Id.ToString(), data.OrganizationImagePath);
                        replaceImage(doc, "[公司組織圖]", organiztionPath);
                    }
                    catch (Exception e)
                    {
                        ViewData["organiztionError"] = e.Message;
                        throw;
                    }
                    
                }
                else
                {
                    doc.ReplaceText("[公司組織圖]", "");
                }
                if (data.MapImagePath != "")
                {
                    try
                    {
                        string mapPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", data.CompanyId.ToString(), data.Id.ToString(), data.MapImagePath);
                        replaceImage(doc, "[地理位置圖]", mapPath);
                    }
                    catch (Exception e)
                    {
                        ViewData["mapError"] = e.Message;
                        throw;
                    }
                    
                }
                else
                {
                    doc.ReplaceText("[地理位置圖]", "");
                }
                //--------------------------------圖片

                // 保存新文檔
                doc.SaveAs(newFile);
            }
            var fileBytes = System.IO.File.ReadAllBytes(newFile);
            var fileName = data.Year + "年度" + "-" + data.Company.Name + (data.Name != null ? ("-" + data.Name) : "") + "-溫室氣體盤查報告書.docx"; // 可以自行定義檔名
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
        }

        public void GenerateActivityDataTable(DocX doc, List<Device> devices)
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
                string deviceCellString = (devices[x].Name != "WD40" ? devices[x].Material : "二氧化碳")
                    + (devices[x].Name == "其他" ? ("(" + devices[x].OtherName + ")") : ("(" + devices[x].Name + ")"));
                table.Rows[x + 1].Cells[2].Paragraphs.First().Append(deviceCellString).Font(font).FontSize(11d);

                var DeviceId = devices[x].Id;
                var activityData = _context.ActivityDatas.Where(x => x.DeviceId == DeviceId).ToList();
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
            doc.ReplaceTextWithObject("[排放源活動數據表]", table);

        }
        public double CountWidth(double cmWidth)
        {
            double widthInPoints = cmWidth * 0.393701 * 72; // 1英寸 ≈ 2.54厘米，1磅 ≈ 0.0353厘米
            return widthInPoints;
        }

        public void GenerateGHGsTable(DocX doc, List<GHG> manyGHGs, string emissionPattern, string gasName)
        {
            int num = manyGHGs.Count(x => x.Device.EmissionPattern == emissionPattern && x.Name == gasName);
            string tableName = $"[{emissionPattern}{gasName}]";
            var font = new Xceed.Document.NET.Font("標楷體");
            Xceed.Document.NET.Table table = doc.AddTable(num != 0 ? (num + 1) : (num + 2), 8);
            table.AutoFit = AutoFit.Contents;
            table.Design = TableDesign.MediumShading1Accent3;

            table.Rows[0].Cells[0].Paragraphs.First().Append("排放源名稱").Font(font).FontSize(9d);
            table.Rows[0].Cells[1].Paragraphs.First().Append("原燃物料或產品").Font(font).FontSize(9d);
            table.Rows[0].Cells[2].Paragraphs.First().Append("活動數據").Font(font).FontSize(9d);
            table.Rows[0].Cells[3].Paragraphs.First().Append("排放係數").Font(font).FontSize(9d);
            table.Rows[0].Cells[4].Paragraphs.First().Append("係數來源").Font(font).FontSize(9d);
            table.Rows[0].Cells[5].Paragraphs.First().Append("排放量\n(公噸/年)").Font(font).FontSize(9d);
            table.Rows[0].Cells[6].Paragraphs.First().Append("GWP").Font(font).FontSize(9d);
            table.Rows[0].Cells[7].Paragraphs.First().Append("CO₂排放當量\n(公噸CO₂e/年)").Font(font).FontSize(9d);

            int rowIndex = 1;
            if (num > 0)
            {
                foreach (var GHG in manyGHGs.Where(x => x.Device.EmissionPattern == emissionPattern && x.Name == gasName))
                {
                    string deviceCellString = (GHG.Device.Name == "其他" ? GHG.Device.OtherName : GHG.Device.Name)
                                             + (GHG.Device.NameRemark != null && GHG.Device.NameRemark.Trim() != string.Empty ? "(" + GHG.Device.NameRemark + ")" : "");

                    table.Rows[rowIndex].Cells[0].Paragraphs.First().Append(deviceCellString).Font(font).FontSize(9d);
                    table.Rows[rowIndex].Cells[1].Paragraphs.First().Append(GHG.Device.Name != "WD40" ? GHG.Device.Material : "二氧化碳").Font(font).FontSize(9d);
                    var ActivityData = _context.ActivityDatas.Where(x => x.DeviceId == GHG.DeviceId).ToList();

                    if (ActivityData != null)
                    {
                        table.Rows[rowIndex].Cells[2].Paragraphs.First().Append(ActivityData.Sum(ad => ad.Num) + GHG.Device.Unit).Font(font).FontSize(9d);
                    }
                    else
                    {
                        table.Rows[rowIndex].Cells[2].Paragraphs.First().Append(0 + GHG.Device.Unit).Font(font).FontSize(9d);
                    }
                    table.Rows[rowIndex].Cells[3].Paragraphs.First().Append(GHG.CEF.ToString() + "公斤/" + GHG.Device.Unit).Font(font).FontSize(9d);
                    string CEF_Source = string.Empty;
                    if (GHG.Device.CEF_Correction == 1)
                    {
                        CEF_Source = "自廠發展係數";
                    }
                    else
                    {
                        CEF_Source = "溫室氣體排放係數管理表 6.0.4 版";
                    }
                    table.Rows[rowIndex].Cells[4].Paragraphs.First().Append(CEF_Source).Font(font).FontSize(9d);
                    table.Rows[rowIndex].Cells[5].Paragraphs.First().Append((GHG.Emission / GHG.GWP).ToString("N4")).Font(font).FontSize(9d);
                    table.Rows[rowIndex].Cells[6].Paragraphs.First().Append(GHG.GWP.ToString()).Font(font).FontSize(9d);
                    table.Rows[rowIndex].Cells[7].Paragraphs.First().Append(GHG.Emission.ToString("N4")).Font(font).FontSize(9d);
                    rowIndex++;
                }




            }
            else
            {
                table.Rows[1].MergeCells(0, 7);
                string switchingGasName = SwitchGasName(gasName);
                var paragraph = table.Rows[1].Cells[0].Paragraphs.First().Append($"{"無"}{switchingGasName}{emissionPattern}{"排放源"}").Font(font).FontSize(9d);
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
                    // 插入圖片
                    if (imagePath != null)
                    {
                        var image = doc.AddImage(imagePath);
                        var picture = image.CreatePicture(200, 200);
                        paragraph.InsertPicture(picture);
                    }

                    // 移除包含替換文字的原始內容
                    doc.ReplaceText(text, string.Empty);
                }
            }
        }
        public void GenerateReport(DocX doc, List<Device> devices)
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
                string deviceCellString = (devices[x].Name == "其他" ? devices[x].OtherName : devices[x].Name)
                    + (devices[x].Name != "WD40" ? ("-" + devices[x].Material) : "-二氧化碳")
                    + (devices[x].NameRemark != null && devices[x].NameRemark.Trim() != string.Empty ? "(" + devices[x].NameRemark + ")" : "");
                table.Rows[x + 1].Cells[2].Paragraphs.First().Append(deviceCellString).Font(font).FontSize(11d);
                var GHGs = _context.GHGs.Where(ghg => ghg.DeviceId == devices[x].Id);
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
            doc.ReplaceTextWithObject("[報告邊界表]", table);
        }
        public void GenerateScopeTable(DocX doc, List<Device> devices, string scope)
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
                var GHGs = _context.GHGs.Where(ghg => ghg.DeviceId == item.Id);

                // 填充基本資料
                table.Rows[rowIndex].Cells[0].Paragraphs.First().Append(item.Scope).Font(font).FontSize(11d);
                table.Rows[rowIndex].Cells[1].Paragraphs.First().Append(item.EmissionPattern).Font(font).FontSize(11d);
                string deviceCellString = (item.Name == "其他" ? item.OtherName : item.Name)
                    + (item.Name != "WD40" ? ("-" + item.Material) : ("-二氧化碳"));
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

            doc.ReplaceTextWithObject("[" + scope + "Table]", table);

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
            doc.ReplaceText("[" + scope + "Device]", Scope.Trim());
        }
    }
}
