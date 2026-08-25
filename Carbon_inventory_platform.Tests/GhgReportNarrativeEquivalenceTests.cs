using Carbon_inventory_platform.Models;
using Carbon_inventory_platform.Services;

namespace Carbon_inventory_platform.Tests;

/// <summary>
/// A8：「直接溫室氣體排放說明」的差異測試。
///
/// 重構前這段敘述組裝邏輯在 ISOReportAsync／MOEReportAsync／IISReportAsync 裡各有一份
/// 一字不差的 104 行副本；重構後只剩 GhgReportBuilder.BuildScope1Narrative 一份。
///
/// 這個檔案把「重構前的原始寫法」原封不動抄成 <see cref="LegacyScope1Narrative"/>，
/// 然後窮舉四種排放源存在與否的 16 種組合（外加多筆、同名、其他排放源等情境），
/// 逐一比對新舊兩種寫法的輸出完全相同。
///
/// 之所以需要這個測試：實機用的公司資料把 Company.Scope1Information 填滿了，
/// 所以重構前後的報告書黃金檔比對根本沒有走進這個 else 分支，光靠檔案比對證明不了它。
/// </summary>
public class GhgReportNarrativeEquivalenceTests
{
    /// <summary>重構前的原始實作，逐行照抄（含製程段落少一個 else 的既有不一致）。不要「順手修正」這裡。</summary>
    private static string LegacyScope1Narrative(List<Device> device)
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

        return text;
    }

    private static Device Dev(string name, string pattern)
        => new Device { Id = Guid.NewGuid(), Name = name, EmissionPattern = pattern, Scope = "類別一", Material = "柴油" };

    private static readonly string[] Patterns = { "固定", "移動", "逸散", "製程" };

    /// <summary>四種排放源「有／沒有」的 16 種組合，每一種都要新舊一致。</summary>
    [Fact]
    public void 窮舉四種排放源的16種有無組合_新舊實作輸出完全相同()
    {
        for (int mask = 0; mask < 16; mask++)
        {
            var devices = new List<Device>();
            for (int i = 0; i < Patterns.Length; i++)
            {
                if ((mask & (1 << i)) != 0)
                {
                    devices.Add(Dev("設備" + Patterns[i], Patterns[i]));
                }
            }

            var legacy = LegacyScope1Narrative(devices);
            var refactored = GhgReportBuilder.BuildScope1Narrative(devices);

            Assert.Equal(legacy, refactored);
        }
    }

    /// <summary>每一段各放 1~4 個排放源，檢查頓號/逗號的分隔行為（含製程段落的既有不一致）。</summary>
    [Fact]
    public void 每段一到四個排放源_新舊實作輸出完全相同()
    {
        for (int count = 1; count <= 4; count++)
        {
            foreach (var pattern in Patterns)
            {
                var devices = new List<Device>();
                for (int i = 1; i <= count; i++)
                {
                    devices.Add(Dev($"{pattern}設備{i}", pattern));
                }

                var legacy = LegacyScope1Narrative(devices);
                var refactored = GhgReportBuilder.BuildScope1Narrative(devices);

                Assert.Equal(legacy, refactored);
            }
        }
    }

    [Fact]
    public void 同名排放源去重與跨段落混合_新舊實作輸出完全相同()
    {
        var cases = new List<List<Device>>
        {
            // 同名重複（GroupBy(Name) 只取第一筆）
            new List<Device> { Dev("公務車", "移動"), Dev("公務車", "移動"), Dev("公務車", "移動") },
            // 同名但落在不同排放型態，兩段都該出現
            new List<Device> { Dev("共用設備", "固定"), Dev("共用設備", "製程") },
            // 四段都有、且各有重複
            new List<Device>
            {
                Dev("鍋爐", "固定"), Dev("鍋爐", "固定"), Dev("緊急發電機", "固定"),
                Dev("公務車", "移動"), Dev("堆高機", "移動"),
                Dev("冰水主機", "逸散"), Dev("冰水主機", "逸散"), Dev("冷氣機", "逸散"),
                Dev("乙炔焊接", "製程"), Dev("熱處理爐", "製程"),
            },
            // 含系統不認識的排放型態，應該完全不影響四段內容
            new List<Device> { Dev("外購電力", "外購電力"), Dev("鍋爐", "固定") },
            // 只有不認識的排放型態 → 四段都是「無」
            new List<Device> { Dev("外購電力", "外購電力") },
        };

        foreach (var devices in cases)
        {
            Assert.Equal(LegacyScope1Narrative(devices), GhgReportBuilder.BuildScope1Narrative(devices));
        }
    }
}
