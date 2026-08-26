using Carbon_inventory_platform.Models;
using Microsoft.EntityFrameworkCore;

namespace Carbon_inventory_platform.Tests;

/// <summary>
/// B4（稽核軌跡）：驗證 ApplicationDbContext.SaveChanges/SaveChangesAsync 攔截後
/// 正確寫入 AuditLog，且只記錄清單內的實體、不會誤記其他資料表。
/// </summary>
public class AuditTrailTests
{
    [Fact]
    public async Task 新增一筆Material_會寫入一筆Create稽核紀錄_含資料庫產生的Id()
    {
        using var testDb = new TestDb();
        var context = testDb.CreateNew();

        // 不指定 Id，讓 EF 存檔時自動產生——這是稽核攔截最容易漏掉的情境。
        var material = new Material { Name = "測試物料", Scope = "類別一", EmissionPattern = "固定", Unit = "Kg" };
        context.Materials.Add(material);
        await context.SaveChangesAsync();

        var log = await testDb.CreateNew().AuditLogs.SingleAsync(a => a.EntityName == "Material");
        Assert.Equal("Create", log.Action);
        Assert.Equal(material.Id.ToString(), log.EntityId);
        Assert.Contains("測試物料", log.NewValues);
        Assert.Null(log.OldValues);
    }

    [Fact]
    public async Task 修改一筆Material的排放係數_稽核紀錄同時記錄新舊值_且未變更欄位不出現()
    {
        using var testDb = new TestDb();
        var seedContext = testDb.Seed;
        var material = new Material { Id = 501, Name = "測試物料", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 1.0m, Unit = "Kg" };
        seedContext.Materials.Add(material);
        await seedContext.SaveChangesAsync();

        var actContext = testDb.CreateNew();
        var toUpdate = await actContext.Materials.FindAsync(501);
        toUpdate!.CO2CEF = 2.5m;
        await actContext.SaveChangesAsync();

        // 種子（seedContext.SaveChangesAsync）本身也會產生一筆 Create 稽核紀錄，這裡只看 Update 那一筆。
        var log = await testDb.CreateNew().AuditLogs.SingleAsync(a => a.EntityName == "Material" && a.EntityId == "501" && a.Action == "Update");
        Assert.Contains("1.0", log.OldValues);
        Assert.Contains("2.5", log.NewValues);
        // Name 沒有被改動，不應該出現在變更紀錄裡。
        Assert.DoesNotContain("測試物料", log.OldValues!);
        Assert.DoesNotContain("測試物料", log.NewValues!);
    }

    [Fact]
    public async Task 用ContextUpdate整筆覆蓋更新時_也能正確算出真正改動的欄位()
    {
        // MaterialsController.Edit 用的是 _context.Update(model)：整個從表單綁出來的物件
        // 直接標成 Modified，這個 DbContext 從沒真正查過資料庫的原始值。原本的實作依賴
        // property.OriginalValue，這種情境下 EF 會把 OriginalValue 誤設成跟 CurrentValue
        // 一樣，稽核紀錄會變成「改成什麼」跟「改之前」顯示成同一個值，等於白記，
        // 而且每個欄位都會被誤判成「有變更」。改成直接查一次資料庫目前的值來比較。
        using var testDb = new TestDb();
        var seedContext = testDb.Seed;
        var material = new Material { Id = 502, Name = "測試物料", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 1.0m, Unit = "Kg" };
        seedContext.Materials.Add(material);
        await seedContext.SaveChangesAsync();

        var actContext = testDb.CreateNew();
        var posted = new Material { Id = 502, Name = "測試物料", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 9.9999m, Unit = "Kg" };
        actContext.Update(posted); // 整個物件覆蓋更新，跟 MaterialsController.Edit 一樣，不是先查再改。
        await actContext.SaveChangesAsync();

        var log = await testDb.CreateNew().AuditLogs.SingleAsync(a => a.EntityName == "Material" && a.EntityId == "502" && a.Action == "Update");
        Assert.Contains("1.0", log.OldValues);
        Assert.Contains("9.9999", log.NewValues);
        // Name／Scope／EmissionPattern／Unit 都沒有真的改變，即使 _context.Update 把它們全部
        // 標成 IsModified=true，也不應該出現在變更紀錄裡。
        Assert.DoesNotContain("測試物料", log.OldValues!);
        Assert.DoesNotContain("測試物料", log.NewValues!);
    }

    [Fact]
    public async Task 用ContextUpdate送出但完全沒有真正改動欄位時_不產生Update稽核紀錄()
    {
        using var testDb = new TestDb();
        var seedContext = testDb.Seed;
        var material = new Material { Id = 503, Name = "測試物料", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 1.0m, Unit = "Kg" };
        seedContext.Materials.Add(material);
        await seedContext.SaveChangesAsync();

        var actContext = testDb.CreateNew();
        var posted = new Material { Id = 503, Name = "測試物料", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 1.0m, Unit = "Kg" };
        actContext.Update(posted); // 表單原封不動送出，_context.Update 仍會把所有欄位標成 IsModified。
        await actContext.SaveChangesAsync();

        var updateCount = await testDb.CreateNew().AuditLogs
            .CountAsync(a => a.EntityName == "Material" && a.EntityId == "503" && a.Action == "Update");
        Assert.Equal(0, updateCount);
    }

    [Fact]
    public async Task 刪除一筆GWP_稽核紀錄記錄刪除前的資料_NewValues為null()
    {
        using var testDb = new TestDb();
        var seedContext = testDb.Seed;
        var gwp = new GWP { Id = 501, Name = "測試氣體", Num = 100m, ARVersion = 6 };
        seedContext.GWPs.Add(gwp);
        await seedContext.SaveChangesAsync();

        var actContext = testDb.CreateNew();
        var toDelete = await actContext.GWPs.FindAsync(501);
        actContext.GWPs.Remove(toDelete!);
        await actContext.SaveChangesAsync();

        // 種子（seedContext.SaveChangesAsync）本身也會產生一筆 Create 稽核紀錄，這裡只看 Delete 那一筆。
        var log = await testDb.CreateNew().AuditLogs.SingleAsync(a => a.EntityName == "GWP" && a.EntityId == "501" && a.Action == "Delete");
        Assert.Contains("測試氣體", log.OldValues);
        Assert.Null(log.NewValues);
    }

    [Fact]
    public async Task 不在稽核清單內的實體異動_不會產生稽核紀錄()
    {
        using var testDb = new TestDb();
        var context = testDb.CreateNew();

        // Feedback 是使用者意見回報，不是會影響計算結果或授權的共用主檔，不需要留稽核紀錄。
        context.Feedbacks.Add(new Feedback { UserName = "測試", Email = "test@example.com", Message = "測試訊息" });
        await context.SaveChangesAsync();

        var count = await testDb.CreateNew().AuditLogs.CountAsync();
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task 新增一筆DeviceData_會產生稽核紀錄()
    {
        using var testDb = new TestDb();
        var context = testDb.CreateNew();

        // DefaultDevices／DeviceData 原本跟 Material／GWP 一樣是共用主檔（會影響計算結果與下拉選單），
        // 卻沒被列進稽核清單，改壞了排放係數/GWP名稱對不上也查不出是誰、何時改的。
        context.deviceDatas.Add(new DeviceData { Id = 502, Name = "測試設備", Scope = "類別一", EmissionPattern = "固定", Material = "柴油" });
        await context.SaveChangesAsync();

        var count = await testDb.CreateNew().AuditLogs
            .CountAsync(a => a.EntityName == "DeviceData" && a.EntityId == "502" && a.Action == "Create");
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task 沒有HttpContext時UserId與UserName為null_不拋例外()
    {
        // TestDb 建立的 context 沒有 IHttpContextAccessor（等同背景工作、種子資料等情境）。
        using var testDb = new TestDb();
        var context = testDb.CreateNew();
        context.Materials.Add(new Material { Name = "測試物料2", Scope = "類別一", EmissionPattern = "固定", Unit = "Kg" });
        await context.SaveChangesAsync();

        var log = await testDb.CreateNew().AuditLogs.SingleAsync();
        Assert.Null(log.UserId);
        Assert.Null(log.UserName);
        Assert.Null(log.IpAddress);
    }
}
