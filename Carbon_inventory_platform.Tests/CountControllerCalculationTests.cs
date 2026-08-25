using Carbon_inventory_platform.Controllers;
using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.EntityFrameworkCore;

namespace Carbon_inventory_platform.Tests;

/// <summary>
/// CopyGHG / CopyDevice 是純函式（不觸碰 _context），可以直接測，不需要資料庫。
/// </summary>
public class CopyMethodsTests
{
    private static CountController CreateController() => new CountController(null!);

    [Fact]
    public void CopyGHG_產生的主鍵不是Guid全零()
    {
        // 原本寫 Id = new Guid()，等於 Guid.Empty。複製一台有兩種以上氣體的排放源時，
        // 每一種氣體都會拿到同一個 Guid.Empty 當主鍵，第二筆存檔就違反主鍵限制而整批失敗。
        var controller = CreateController();
        var original = new GHG { Id = Guid.NewGuid(), DeviceId = Guid.NewGuid(), Name = "CO2", GWP = 1, CEF = 0.5m };

        var copy = controller.CopyGHG(original, Guid.NewGuid());

        Assert.NotEqual(Guid.Empty, copy.Id);
    }

    [Fact]
    public void CopyGHG_複製同一個排放源的多種氣體_每筆主鍵都不同()
    {
        // 直接模擬「複製一台有 CO2/CH4/N2O 三種氣體的排放源」的實際使用情境。
        var controller = CreateController();
        var newDeviceId = Guid.NewGuid();
        var ghgs = new[]
        {
            new GHG { Id = Guid.NewGuid(), DeviceId = Guid.NewGuid(), Name = "CO2", GWP = 1 },
            new GHG { Id = Guid.NewGuid(), DeviceId = Guid.NewGuid(), Name = "CH4", GWP = 27.9m },
            new GHG { Id = Guid.NewGuid(), DeviceId = Guid.NewGuid(), Name = "N2O", GWP = 273 },
        };

        var copies = ghgs.Select(g => controller.CopyGHG(g, newDeviceId)).ToList();

        Assert.Equal(3, copies.Select(c => c.Id).Distinct().Count());
        Assert.All(copies, c => Assert.NotEqual(Guid.Empty, c.Id));
        Assert.All(copies, c => Assert.Equal(newDeviceId, c.DeviceId));
    }

    [Fact]
    public void CopyGHG_保留原始的排放係數與GWP()
    {
        var controller = CreateController();
        var original = new GHG
        {
            Id = Guid.NewGuid(),
            DeviceId = Guid.NewGuid(),
            Name = "CH4",
            GWP = 27.9m,
            CEF = 0.0000246603m,
            Emission = 123.456m,
        };

        var copy = controller.CopyGHG(original, Guid.NewGuid());

        Assert.Equal(original.Name, copy.Name);
        Assert.Equal(original.GWP, copy.GWP);
        Assert.Equal(original.CEF, copy.CEF);
        Assert.Equal(original.Emission, copy.Emission);
        Assert.Equal((byte)0, copy.isDeleted);
    }

    [Fact]
    public void CopyDevice_使用傳入的新Id_而不是原設備的Id()
    {
        var controller = CreateController();
        var original = new Device { Id = Guid.NewGuid(), Name = "冰水主機", Scope = "類別一", EmissionPattern = "逸散", Material = "R-134A" };
        var newAreaId = Guid.NewGuid();
        var newDeviceId = Guid.NewGuid();

        var copy = controller.CopyDevice(original, newAreaId, newDeviceId);

        Assert.Equal(newDeviceId, copy.Id);
        Assert.Equal(newAreaId, copy.AreaId);
        Assert.Equal(original.Name, copy.Name);
        Assert.Equal((byte)0, copy.isDeleted);
    }
}

/// <summary>
/// CountEmissionData / CountEmissionAsync 需要資料庫，用 EF Core InMemory provider 測。
/// </summary>
public class CountEmissionDataTests
{
    private static Device NewDevice(Guid id, Guid areaId, string material = "R-134A", string scope = "類別一", string pattern = "逸散")
        => new Device { Id = id, AreaId = areaId, Name = "測試設備", Scope = scope, EmissionPattern = pattern, Material = material };

    [Fact]
    public async Task 七種氣體都會被加總進設備總排放量_不只前三種()
    {
        // 原本 all_Emission 只在 i==1~3 的分支內累加，第 4~7 種氣體(HFCS/PFCS/SF6/NF3)
        // 的 Emission 算出來、寫回資料庫，卻沒有進到設備總量 Device.Emissions。
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var areaId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();

        context.Areas.Add(new Area { Id = areaId, CompanyId = Guid.NewGuid(), FullAddress = "test", Year = 113 });
        context.Devices.Add(NewDevice(deviceId, areaId));

        // 7 種氣體各排放 1 單位 CEF、GWP=1，活動數據=1000（除以1000後 Num項=1）。
        string[] gasNames = { "CO2", "CH4", "N2O", "HFCS", "PFCS", "SF6", "NF3" };
        var createTime = DateTime.Now;
        foreach (var name in gasNames)
        {
            context.GHGs.Add(new GHG
            {
                Id = Guid.NewGuid(),
                DeviceId = deviceId,
                Name = name,
                CEF = 1m,
                GWP = 1m,
                CreateTime = createTime,
            });
            createTime = createTime.AddMilliseconds(1); // 確保 OrderBy(CreateTime) 的順序穩定
        }
        context.ActivityDatas.Add(new ActivityData { DeviceId = deviceId, Num = 1000m });
        await context.SaveChangesAsync();

        var controller = new CountController(testDb.CreateNew());
        await controller.CountEmissionData(deviceId);

        var device = await testDb.CreateNew().Devices.FindAsync(deviceId);
        // 每種氣體 Emission = CEF(1) * Num(1000) / 1000 * GWP(1) = 1，7 種氣體總和應為 7。
        Assert.Equal(7m, device!.Emissions);
    }

    [Fact]
    public async Task 廢水處理設備的排放量不除以1000()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var areaId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();

        context.Areas.Add(new Area { Id = areaId, CompanyId = Guid.NewGuid(), FullAddress = "test", Year = 113 });
        context.Devices.Add(NewDevice(deviceId, areaId, material: "廢水處理", pattern: "逸散"));
        context.GHGs.Add(new GHG { Id = Guid.NewGuid(), DeviceId = deviceId, Name = "CH4", CEF = 2m, GWP = 1m, CreateTime = DateTime.Now });
        context.ActivityDatas.Add(new ActivityData { DeviceId = deviceId, Num = 10m });
        await context.SaveChangesAsync();

        var controller = new CountController(testDb.CreateNew());
        await controller.CountEmissionData(deviceId);

        var device = await testDb.CreateNew().Devices.FindAsync(deviceId);
        // 廢水處理：Emission = CEF(2) * Num(10) * GWP(1) = 20（不除以1000）。
        Assert.Equal(20m, device!.Emissions);
    }

    [Fact]
    public async Task 設備不存在時直接返回_不拋出例外()
    {
        // 原本沒檢查 Device 是否為 null 就直接用 Device.AreaId，會丟 NullReferenceException。
        using var testDb = new TestDb();
        var controller = new CountController(testDb.CreateNew());

        await controller.CountEmissionData(Guid.NewGuid()); // 不應該拋出任何例外

        Assert.True(true);
    }

    [Fact]
    public async Task 廠區總排放量是重新彙總所有設備_而不是每次疊加()
    {
        // 原本 emission.All += all_Emission，每次呼叫都把同一台設備的排放量再加一次，
        // 導致存幾次活動數據，Area.All 就虛增幾倍。這裡驗證重複呼叫兩次結果不會加倍。
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var areaId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();

        context.Areas.Add(new Area { Id = areaId, CompanyId = Guid.NewGuid(), FullAddress = "test", Year = 113 });
        context.Devices.Add(NewDevice(deviceId, areaId));
        context.GHGs.Add(new GHG { Id = Guid.NewGuid(), DeviceId = deviceId, Name = "CO2", CEF = 1m, GWP = 1m, CreateTime = DateTime.Now });
        context.ActivityDatas.Add(new ActivityData { DeviceId = deviceId, Num = 1000m });
        await context.SaveChangesAsync();

        var controller = new CountController(testDb.CreateNew());
        await controller.CountEmissionData(deviceId);
        var controller2 = new CountController(testDb.CreateNew());
        await controller2.CountEmissionData(deviceId); // 重複呼叫，模擬使用者重複儲存活動數據

        var area = await testDb.CreateNew().Areas.FindAsync(areaId);
        Assert.Equal(1m, area!.All); // 而不是 2
    }
}

public class CountEmissionAsyncTests
{
    private static Device NewDevice(Guid id, Guid areaId, string scope, decimal emissions, int grade = 27)
        => new Device
        {
            Id = id,
            AreaId = areaId,
            Name = "測試設備",
            Scope = scope,
            EmissionPattern = "逸散",
            Material = "R-134A",
            Emissions = emissions,
            Grade = grade,
        };

    [Fact]
    public async Task NF3總量不會混入類別二的SF6()
    {
        // 原本 decimal NF3 = sum1_NF3 + sum2_SF6;（複製貼上打錯變數），
        // 廠區的 NF3 總量會混入類別二 SF6 的排放量，且類別二 NF3 完全沒被算進去。
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var areaId = Guid.NewGuid();
        context.Areas.Add(new Area { Id = areaId, CompanyId = Guid.NewGuid(), FullAddress = "test", Year = 113 });

        var device1 = NewDevice(Guid.NewGuid(), areaId, "類別一", emissions: 10m);
        var device2 = NewDevice(Guid.NewGuid(), areaId, "類別二", emissions: 20m);
        context.Devices.AddRange(device1, device2);
        context.GHGs.AddRange(
            new GHG { Id = Guid.NewGuid(), DeviceId = device1.Id, Name = "NF3", Emission = 10m, CreateTime = DateTime.Now },
            new GHG { Id = Guid.NewGuid(), DeviceId = device2.Id, Name = "SF6", Emission = 20m, CreateTime = DateTime.Now },
            new GHG { Id = Guid.NewGuid(), DeviceId = device2.Id, Name = "NF3", Emission = 5m, CreateTime = DateTime.Now });
        await context.SaveChangesAsync();

        var controller = new CountController(testDb.CreateNew());
        await controller.CountEmissionAsync(areaId);

        var area = await testDb.CreateNew().Areas.FindAsync(areaId);
        // NF3 應該是 類別一NF3(10) + 類別二NF3(5) = 15，不應該混入類別二的 SF6(20)。
        Assert.Equal(15m, area!.NF3);
        Assert.Equal(20m, area.SF6);
    }

    [Fact]
    public async Task 廠區內所有設備排放量皆為零時_不拋出除以零例外()
    {
        // 原本 avgGrade 的計算對 sumAll 做除法且沒有零值防呆，decimal 除以零會直接拋
        // DivideByZeroException，讓剛建立、還沒有任何活動數據的廠區的排放量頁面 500。
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var areaId = Guid.NewGuid();
        context.Areas.Add(new Area { Id = areaId, CompanyId = Guid.NewGuid(), FullAddress = "test", Year = 113 });
        context.Devices.Add(NewDevice(Guid.NewGuid(), areaId, "類別一", emissions: 0m));
        await context.SaveChangesAsync();

        var controller = new CountController(testDb.CreateNew());
        var success = await controller.CountEmissionAsync(areaId);

        Assert.True(success);
        var area = await testDb.CreateNew().Areas.FindAsync(areaId);
        Assert.Equal(0m, area!.All);
        Assert.Equal(0m, area.percentage_CalAll);
    }

    [Fact]
    public async Task 設備等級為零時歸入第一級_而非第二級()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var areaId = Guid.NewGuid();
        context.Areas.Add(new Area { Id = areaId, CompanyId = Guid.NewGuid(), FullAddress = "test", Year = 113 });
        context.Devices.Add(NewDevice(Guid.NewGuid(), areaId, "類別一", emissions: 5m, grade: 0));
        await context.SaveChangesAsync();

        var controller = new CountController(testDb.CreateNew());
        await controller.CountEmissionAsync(areaId);

        var area = await testDb.CreateNew().Areas.FindAsync(areaId);
        Assert.Equal(1, area!.no1_Grade);
        Assert.Equal(0, area.no2_Grade);
    }

    [Fact]
    public async Task 已軟刪除的設備不計入廠區總排放量()
    {
        // 這裡同時驗證 A1（全域軟刪除查詢過濾器）跟計算邏輯銜接是否正確：
        // Device 被標記 isDeleted=1 後，CountEmissionAsync 透過 Include(a => a.Devices)
        // 撈到的清單應該已經不含這台設備。
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var areaId = Guid.NewGuid();
        context.Areas.Add(new Area { Id = areaId, CompanyId = Guid.NewGuid(), FullAddress = "test", Year = 113 });

        var activeDevice = NewDevice(Guid.NewGuid(), areaId, "類別一", emissions: 10m);
        var deletedDevice = NewDevice(Guid.NewGuid(), areaId, "類別一", emissions: 999m);
        deletedDevice.isDeleted = 1;
        context.Devices.AddRange(activeDevice, deletedDevice);
        await context.SaveChangesAsync();

        // 用另一個全新的 context 執行 CountEmissionAsync，模擬正式環境每個請求都是
        // 全新 DbContext 的情形，避免變更追蹤器的導覽屬性修復繞過全域查詢過濾器。
        var controller = new CountController(testDb.CreateNew());
        await controller.CountEmissionAsync(areaId);

        var area = await testDb.CreateNew().Areas.FindAsync(areaId);
        Assert.Equal(10m, area!.All); // 不是 10 + 999
    }

    [Fact]
    public async Task 找不到廠區時回傳false()
    {
        using var testDb = new TestDb();
        var controller = new CountController(testDb.CreateNew());

        var success = await controller.CountEmissionAsync(Guid.NewGuid());

        Assert.False(success);
    }
}
