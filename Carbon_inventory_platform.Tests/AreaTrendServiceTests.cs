using Carbon_inventory_platform.Models;
using Carbon_inventory_platform.Services;

namespace Carbon_inventory_platform.Tests;

/// <summary>
/// B2（跨年度趨勢）：驗證趨勢查詢的分組邏輯與「相對基準年增減百分比」跟報告書用同一個公式。
/// </summary>
public class AreaTrendServiceTests
{
    private static Area NewArea(Guid companyId, string fullAddress, int year, bool baseYear, decimal all)
        => new Area
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            FullAddress = fullAddress,
            Year = year,
            BaseYear = baseYear,
            All = all,
        };

    [Fact]
    public async Task GetTrendAsync_只回傳同一個公司同一個地址的廠區_依年度排序()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var companyId = Guid.NewGuid();

        var y113 = NewArea(companyId, "台南市永康區", 113, baseYear: true, all: 100m);
        var y114 = NewArea(companyId, "台南市永康區", 114, baseYear: false, all: 90m);
        var otherAddress = NewArea(companyId, "高雄市三民區", 114, baseYear: false, all: 999m);
        var otherCompany = NewArea(Guid.NewGuid(), "台南市永康區", 114, baseYear: false, all: 888m);
        context.Areas.AddRange(y114, y113, otherAddress, otherCompany); // 故意打亂新增順序
        await context.SaveChangesAsync();

        var service = new AreaTrendService(testDb.CreateNew());
        var trend = await service.GetTrendAsync(companyId, "台南市永康區");

        Assert.Equal(2, trend.Count);
        Assert.Equal(113, trend[0].Year);
        Assert.Equal(114, trend[1].Year);
    }

    [Fact]
    public async Task PercentVsBaseYear_跟報告書比較總排放差異用同一個公式()
    {
        // 報告書（ISOReportAsync）算法：(當年 - 基準年) / 基準年 * 100。
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var companyId = Guid.NewGuid();
        context.Areas.AddRange(
            NewArea(companyId, "台南市永康區", 113, baseYear: true, all: 100m),
            NewArea(companyId, "台南市永康區", 114, baseYear: false, all: 80m));
        await context.SaveChangesAsync();

        var service = new AreaTrendService(testDb.CreateNew());
        var trend = await service.GetTrendAsync(companyId, "台南市永康區");

        var y114 = trend.Single(p => p.Year == 114);
        Assert.Equal(-20.00m, y114.PercentVsBaseYear);

        var y113 = trend.Single(p => p.Year == 113);
        Assert.Equal(0.00m, y113.PercentVsBaseYear); // 基準年跟自己比較，增減為 0
    }

    [Fact]
    public async Task 找不到基準年廠區時_PercentVsBaseYear為null_而不是拋例外()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var companyId = Guid.NewGuid();
        context.Areas.Add(NewArea(companyId, "台南市永康區", 114, baseYear: false, all: 80m));
        await context.SaveChangesAsync();

        var service = new AreaTrendService(testDb.CreateNew());
        var trend = await service.GetTrendAsync(companyId, "台南市永康區");

        Assert.Null(trend.Single().PercentVsBaseYear);
    }

    [Fact]
    public async Task 基準年總排放量為零時_PercentVsBaseYear為null_不會除以零()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var companyId = Guid.NewGuid();
        context.Areas.AddRange(
            NewArea(companyId, "台南市永康區", 113, baseYear: true, all: 0m),
            NewArea(companyId, "台南市永康區", 114, baseYear: false, all: 50m));
        await context.SaveChangesAsync();

        var service = new AreaTrendService(testDb.CreateNew());
        var trend = await service.GetTrendAsync(companyId, "台南市永康區");

        Assert.Null(trend.Single(p => p.Year == 114).PercentVsBaseYear);
    }

    [Fact]
    public async Task GetSinglePointAsync_找不到廠區時回傳null()
    {
        using var testDb = new TestDb();
        var service = new AreaTrendService(testDb.CreateNew());

        var point = await service.GetSinglePointAsync(Guid.NewGuid());

        Assert.Null(point);
    }

    [Fact]
    public async Task GetSinglePointAsync_不需要同一個地址也能算出跟基準年的比較()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var companyId = Guid.NewGuid();
        var baseYearArea = NewArea(companyId, "台南市永康區", 113, baseYear: true, all: 200m);
        var otherSiteArea = NewArea(companyId, "高雄市三民區", 114, baseYear: false, all: 250m);
        context.Areas.AddRange(baseYearArea, otherSiteArea);
        await context.SaveChangesAsync();

        var service = new AreaTrendService(testDb.CreateNew());
        var point = await service.GetSinglePointAsync(otherSiteArea.Id);

        Assert.NotNull(point);
        Assert.Equal(25.00m, point!.PercentVsBaseYear);
    }
}
