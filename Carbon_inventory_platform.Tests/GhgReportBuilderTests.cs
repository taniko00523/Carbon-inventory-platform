using Carbon_inventory_platform.Models;
using Carbon_inventory_platform.Services;

namespace Carbon_inventory_platform.Tests;

/// <summary>
/// A8（報告書重構）：把原本在三個 action 裡各寫一份的「直接溫室氣體排放說明」敘述組裝邏輯
/// 抽成單一方法之後，用這些測試把既有行為釘住——包含幾個看起來像 bug、但三份報告書行為一致
/// 而刻意保留的細節（製程後面不補「，」、同名排放源只列一次）。
/// </summary>
public class GhgReportBuilderTests
{
    private static Device Dev(string name, string pattern)
        => new Device { Id = Guid.NewGuid(), Name = name, EmissionPattern = pattern, Scope = "類別一", Material = "柴油" };

    [Fact]
    public void BuildScope1Narrative_四種排放源都有時依固定移動逸散製程順序列出()
    {
        var devices = new List<Device>
        {
            Dev("緊急發電機", "固定"),
            Dev("公務車", "移動"),
            Dev("冰水主機", "逸散"),
            Dev("製程廢氣處理設備", "製程"),
        };

        var text = GhgReportBuilder.BuildScope1Narrative(devices);

        Assert.Equal(
            "包含固定源燃燒的直接排放，例如：緊急發電機，"
            + "移動源燃燒的直接排放，例如：公務車，"
            + "人為活動產生的逸散排放，例如：冰水主機，"
            + "產生溫室氣體排放製程，例如：製程廢氣處理設備"
            + "。此外，本次盤查範圍無土地利用變化，也無生質燃料直接排放。",
            text);
    }

    [Fact]
    public void BuildScope1Narrative_完全沒有排放源時四段都顯示無()
    {
        var text = GhgReportBuilder.BuildScope1Narrative(new List<Device>());

        Assert.Equal(
            "包含本公司無固定式排放源本公司無移動式排放源本公司無逸散式排放源本公司無製程排放源"
            + "。此外，本次盤查範圍無土地利用變化，也無生質燃料直接排放。",
            text);
    }

    [Fact]
    public void BuildScope1Narrative_同一段內多個排放源用頓號分隔_最後一個才補逗號()
    {
        var devices = new List<Device>
        {
            Dev("緊急發電機", "固定"),
            Dev("鍋爐", "固定"),
        };

        var text = GhgReportBuilder.BuildScope1Narrative(devices);

        Assert.Contains("例如：緊急發電機、鍋爐，", text);
    }

    [Fact]
    public void BuildScope1Narrative_製程段落結尾刻意不補逗號()
    {
        // 固定／移動／逸散在最後一個排放源後面會補「，」，製程沒有。
        // 這個不一致在原本三份報告書裡完全一樣，屬於既有行為，重構時原樣保留。
        var devices = new List<Device>
        {
            Dev("乙炔焊接", "製程"),
            Dev("熱處理爐", "製程"),
        };

        var text = GhgReportBuilder.BuildScope1Narrative(devices);

        Assert.Contains("例如：乙炔焊接、熱處理爐。此外", text);
        Assert.DoesNotContain("熱處理爐，。", text);
    }

    [Fact]
    public void BuildScope1Narrative_同名排放源只列出一次()
    {
        var devices = new List<Device>
        {
            Dev("公務車", "移動"),
            Dev("公務車", "移動"),
            Dev("公務車", "移動"),
        };

        var text = GhgReportBuilder.BuildScope1Narrative(devices);

        // GroupBy(Name).Select(First) 之後只會出現一次，且因為只剩一筆所以直接補「，」。
        Assert.Contains("移動源燃燒的直接排放，例如：公務車，", text);
        Assert.DoesNotContain("公務車、公務車", text);
    }

    // ---- 三份報告書的差異描述（GhgReportSpec）----------------------------

    [Fact]
    public void ReportSpec_三份報告書各自使用不同的範本檔()
    {
        Assert.Equal("ISOReport.docx", GhgReportSpec.Iso.TemplateFileName);
        Assert.Equal("MOEReport.docx", GhgReportSpec.Moe.TemplateFileName);
        // 範本檔名是三個 I，不是 IIS。
        Assert.Equal("IIIReport.docx", GhgReportSpec.Iis.TemplateFileName);
    }

    [Fact]
    public void ReportSpec_只有ISO需要基準年比較與即時加總的類別小計()
    {
        Assert.True(GhgReportSpec.Iso.EmitBaseYearComparison);
        Assert.False(GhgReportSpec.Moe.EmitBaseYearComparison);
        Assert.False(GhgReportSpec.Iis.EmitBaseYearComparison);

        // Area 只有 Scope1/Scope2 兩個欄位，ISO 需要的類別三~六只能即時從排放源加總。
        Assert.Equal(ScopeTotalsSource.DeviceSums, GhgReportSpec.Iso.Totals);
        Assert.Equal(ScopeTotalsSource.AreaColumns, GhgReportSpec.Moe.Totals);
        Assert.Equal(ScopeTotalsSource.AreaColumns, GhgReportSpec.Iis.Totals);
    }

    [Fact]
    public void ReportSpec_資策會版本多一張公司組織圖且順序在廠區圖與地理位置圖之間()
    {
        Assert.Equal(new[] { "廠區圖", "地理位置圖" }, GhgReportSpec.Iso.Images.Select(i => i.Key));
        Assert.Equal(new[] { "廠區圖", "地理位置圖" }, GhgReportSpec.Moe.Images.Select(i => i.Key));
        Assert.Equal(new[] { "廠區圖", "公司組織圖", "地理位置圖" }, GhgReportSpec.Iis.Images.Select(i => i.Key));
    }

    [Fact]
    public void ReportSpec_ISO的圖片標記沒有角括號其餘兩份有()
    {
        // 這是既有行為：ISO 傳不含角括號的字串給 replaceImage，範本會留下多餘的 "<>"。
        // 保留原樣以免這次重構動到輸出內容。
        Assert.Equal("廠區圖", GhgReportSpec.Iso.Images[0].Marker);
        Assert.Equal("<廠區圖>", GhgReportSpec.Moe.Images[0].Marker);
        Assert.Equal("<廠區圖>", GhgReportSpec.Iis.Images[0].Marker);
    }

    [Fact]
    public void ReportSpec_資策會範本沒有獨立的盤查年標記所以不寫這個鍵()
    {
        Assert.True(GhgReportSpec.Iso.EmitInventoryYear);
        Assert.True(GhgReportSpec.Moe.EmitInventoryYear);
        Assert.False(GhgReportSpec.Iis.EmitInventoryYear);
    }

    [Fact]
    public void ReportSpec_前言與預期用途只有環境部與資策會版本有()
    {
        Assert.False(GhgReportSpec.Iso.EmitForewordAndPurpose);
        Assert.True(GhgReportSpec.Moe.EmitForewordAndPurpose);
        Assert.True(GhgReportSpec.Iis.EmitForewordAndPurpose);

        // 公司英文名稱反過來只有 ISO 範本有標記。
        Assert.True(GhgReportSpec.Iso.EmitEnglishCompanyName);
        Assert.False(GhgReportSpec.Moe.EmitEnglishCompanyName);
        Assert.False(GhgReportSpec.Iis.EmitEnglishCompanyName);
    }

    [Fact]
    public void ReportSpec_要做基準年比較就必須用IncludeGraph載入()
    {
        // 基準年比較是把「基準年廠區的排放源」逐筆加總算出來的，
        // 只有 IncludeGraph 這種載入方式會把基準年廠區的 Devices 一起載進來。
        // 如果哪天有人把某個 spec 改成「要基準年比較 + 投影查詢」，
        // 基準年那組數字不會報錯，而是會安靜地全部變成 0——這個測試就是為了擋住那種改法。
        foreach (var spec in new[] { GhgReportSpec.Iso, GhgReportSpec.Moe, GhgReportSpec.Iis })
        {
            if (spec.EmitBaseYearComparison)
            {
                Assert.Equal(ReportDataLoad.IncludeGraph, spec.Load);
            }
        }
    }

    [Fact]
    public void ReportSpec_ISO載入整棵物件樹其餘兩份用投影查詢並明確排序()
    {
        // 兩種載入方式產生的排放源順序不同，而下游表格與敘述都吃這個順序，所以不能統一。
        Assert.Equal(ReportDataLoad.IncludeGraph, GhgReportSpec.Iso.Load);
        Assert.Equal(ReportDataLoad.ProjectedOrdered, GhgReportSpec.Moe.Load);
        Assert.Equal(ReportDataLoad.ProjectedOrdered, GhgReportSpec.Iis.Load);
    }
}
