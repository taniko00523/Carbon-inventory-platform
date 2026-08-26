using Carbon_inventory_platform.Controllers;
using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Carbon_inventory_platform.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Carbon_inventory_platform.Tests;

/// <summary>
/// 測試用的 TempData 提供者：實際請求會由 MVC 框架的 SessionStateTempDataProvider 處理，
/// 但那需要完整的 Session middleware。這裡直接手動組 Controller（不透過真正的 HTTP 請求管線），
/// 只需要一個「讀空、寫丟棄」的假提供者，讓 controller.TempData[...] = ... 不會因為
/// TempData 是 null 而丟 NullReferenceException。
/// </summary>
internal sealed class NullTempDataProvider : ITempDataProvider
{
    public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
    public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
}

internal static class ControllerTestHelpers
{
    public static void AttachTempData(Controller controller)
    {
        controller.TempData = new TempDataDictionary(new DefaultHttpContext(), new NullTempDataProvider());
    }
}

public class MaterialCatalogServiceTests
{
    private static async Task<TestDb> SeedCatalogAsync()
    {
        var testDb = new TestDb();
        var context = testDb.Seed;
        context.Materials.Add(new Material { Name = "柴油", Scope = "類別一", EmissionPattern = "固定", Unit = "公升" });
        context.GWPs.Add(new GWP { Name = "R-410A", Num = 2255.5m, ARVersion = 6 });
        await context.SaveChangesAsync();
        return testDb;
    }

    [Fact]
    public async Task GetMaterialNamesAsync_同時包含Materials與GWPs來源且不重複()
    {
        using var testDb = await SeedCatalogAsync();
        var service = new MaterialCatalogService(testDb.CreateNew());

        var names = await service.GetMaterialNamesAsync();

        // 冷媒類（R-410A）只存在 GWPs 表，不會出現在 Materials，兩邊都要能查得到。
        Assert.Contains("柴油", names);
        Assert.Contains("R-410A", names);
        Assert.Equal(names.Count, names.Distinct().Count());
    }

    [Fact]
    public async Task ValidateAsync_物料類別排放型式都存在時_視為合法()
    {
        using var testDb = await SeedCatalogAsync();
        var service = new MaterialCatalogService(testDb.CreateNew());

        var result = await service.ValidateAsync("柴油", "類別一", "固定");

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task ValidateAsync_物料不存在於MaterialsOrGWPs時_回傳Material欄位錯誤()
    {
        // 原本 DefaultDevicesController／DeviceDatasController 只驗證 Material，這裡確認
        // 抽出來的共用服務仍然擋得住不存在的物料名稱，避免後續計算對照不到係數而靜靜算出 0。
        using var testDb = await SeedCatalogAsync();
        var service = new MaterialCatalogService(testDb.CreateNew());

        var result = await service.ValidateAsync("不存在的物料", "類別一", "固定");

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Field == "Material");
    }

    [Fact]
    public async Task ValidateAsync_類別不存在於Materials時_回傳Scope欄位錯誤()
    {
        // 這是抽成共用服務時順便補上的驗證，原本兩個 Controller 都沒檢查 Scope/EmissionPattern。
        using var testDb = await SeedCatalogAsync();
        var service = new MaterialCatalogService(testDb.CreateNew());

        var result = await service.ValidateAsync("柴油", "不存在的類別", "固定");

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Field == "Scope");
    }

    [Fact]
    public async Task ValidateAsync_排放型式不存在於Materials時_回傳EmissionPattern欄位錯誤()
    {
        using var testDb = await SeedCatalogAsync();
        var service = new MaterialCatalogService(testDb.CreateNew());

        var result = await service.ValidateAsync("柴油", "類別一", "不存在的型式");

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Field == "EmissionPattern");
    }
}

public class MaterialsControllerDuplicateValidationTests
{
    private static Material NewMaterial(string name = "測試物料", string scope = "類別一", string pattern = "固定", int year = 113)
        => new Material { Name = name, Scope = scope, EmissionPattern = pattern, Year = year, Unit = "公升" };

    [Fact]
    public async Task Create_相同名稱類別型式年份已存在時_擋下不新增第二筆()
    {
        // (Name, Scope, EmissionPattern, Year) 原本只有非唯一索引，重複資料會讓
        // CountController.GHGCheckAsync 的 .FirstOrDefault() 取到哪一筆變成不確定。
        using var testDb = new TestDb();
        var seedContext = testDb.Seed;
        seedContext.Materials.Add(NewMaterial());
        await seedContext.SaveChangesAsync();

        var actContext = testDb.CreateNew();
        var controller = new MaterialsController(actContext, new MaterialCatalogService(actContext));
        ControllerTestHelpers.AttachTempData(controller);

        var result = await controller.Create(NewMaterial());

        Assert.False(controller.ModelState.IsValid);
        Assert.IsType<ViewResult>(result);
        var count = await testDb.CreateNew().Materials.CountAsync(m => m.Name == "測試物料");
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task Create_年份不同時_不視為重複可以新增()
    {
        using var testDb = new TestDb();
        var seedContext = testDb.Seed;
        seedContext.Materials.Add(NewMaterial(year: 112));
        await seedContext.SaveChangesAsync();

        var actContext = testDb.CreateNew();
        var controller = new MaterialsController(actContext, new MaterialCatalogService(actContext));
        ControllerTestHelpers.AttachTempData(controller);

        var result = await controller.Create(NewMaterial(year: 113));

        Assert.True(controller.ModelState.IsValid);
        Assert.IsType<RedirectToActionResult>(result);
        var count = await testDb.CreateNew().Materials.CountAsync(m => m.Name == "測試物料");
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task Edit_儲存自己不變更關鍵欄位時_不會被誤判為重複()
    {
        // ValidateNotDuplicateAsync 用 excludeId 排除自己，這裡確認編輯既有資料（關鍵欄位不變）
        // 不會被自己跟自己比對出「重複」而卡住存檔。
        using var testDb = new TestDb();
        var seedContext = testDb.Seed;
        var material = NewMaterial();
        seedContext.Materials.Add(material);
        await seedContext.SaveChangesAsync();

        var actContext = testDb.CreateNew();
        var controller = new MaterialsController(actContext, new MaterialCatalogService(actContext));
        ControllerTestHelpers.AttachTempData(controller);
        material.Unit = "公斤"; // 改一個非關鍵欄位

        var result = await controller.Edit(material.Id, material);

        Assert.True(controller.ModelState.IsValid);
        Assert.IsType<RedirectToActionResult>(result);
        var updated = await testDb.CreateNew().Materials.FindAsync(material.Id);
        Assert.Equal("公斤", updated!.Unit);
    }
}

public class GWPControllerTests
{
    private static GWPController CreateController(ApplicationDbContext context)
    {
        var controller = new GWPController(context, new MemoryCache(new MemoryCacheOptions()));
        ControllerTestHelpers.AttachTempData(controller);
        return controller;
    }

    [Fact]
    public async Task Index_未指定版本時_預設選最新版本()
    {
        // 原本一定要先手動輸入 AR 版本才會顯示資料，改成預設選最新（數字最大）的既有版本。
        using var testDb = new TestDb();
        var seedContext = testDb.Seed;
        seedContext.GWPs.Add(new GWP { Name = "CO2", Num = 1m, ARVersion = 5 });
        seedContext.GWPs.Add(new GWP { Name = "CO2", Num = 1m, ARVersion = 6 });
        await seedContext.SaveChangesAsync();

        var controller = CreateController(testDb.CreateNew());
        var result = await controller.Index(year: null);

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<GWP>>(view.Model);
        Assert.All(model, g => Assert.Equal(6, g.ARVersion));
    }

    [Fact]
    public async Task Index_完全沒有GWP資料時_不預設任何版本()
    {
        using var testDb = new TestDb();
        var controller = CreateController(testDb.CreateNew());

        await controller.Index(year: null);

        Assert.Null(controller.ViewBag.SearchARVersion);
    }

    [Fact]
    public async Task Add_相同名稱與AR版本已存在時_擋下不新增第二筆()
    {
        // (Name, ARVersion) 原本只有非唯一索引，CountController.CEFAddAsync 查詢 HFCS 的
        // GWP 值時，重複資料會讓 .FirstOrDefault() 取到哪一筆變成不確定。
        using var testDb = new TestDb();
        var seedContext = testDb.Seed;
        seedContext.GWPs.Add(new GWP { Name = "CH4", Num = 27.9m, ARVersion = 6 });
        await seedContext.SaveChangesAsync();

        var actContext = testDb.CreateNew();
        var controller = CreateController(actContext);

        await controller.Add(new GWP { Name = "CH4", Num = 99m }, searchARVersion: 6);

        var count = await testDb.CreateNew().GWPs.CountAsync(g => g.Name == "CH4" && g.ARVersion == 6);
        Assert.Equal(1, count);
        Assert.NotNull(controller.TempData["GWPError"]);
    }

    [Fact]
    public async Task Edit_編輯自己不變更名稱時_不會被誤判為重複()
    {
        using var testDb = new TestDb();
        var seedContext = testDb.Seed;
        var gwp = new GWP { Name = "CH4", Num = 27.9m, ARVersion = 6 };
        seedContext.GWPs.Add(gwp);
        await seedContext.SaveChangesAsync();

        var actContext = testDb.CreateNew();
        var controller = CreateController(actContext);

        await controller.Edit(new GWP { Id = gwp.Id, Name = "CH4", Num = 50m }, searchARVersion: 6);

        var updated = await testDb.CreateNew().GWPs.FindAsync(gwp.Id);
        Assert.Equal(50m, updated!.Num);
        Assert.Null(controller.TempData["GWPError"]);
    }
}
