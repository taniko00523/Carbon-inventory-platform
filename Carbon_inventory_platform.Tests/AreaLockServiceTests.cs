using Carbon_inventory_platform.Models;
using Carbon_inventory_platform.Services;
using Microsoft.EntityFrameworkCore;

namespace Carbon_inventory_platform.Tests;

/// <summary>
/// B5（盤查年度鎖定與簽核）：鎖定/解鎖的狀態切換與快照記錄。
/// </summary>
public class AreaLockServiceTests
{
    private static Area NewArea(Guid id, decimal all = 100m)
        => new Area { Id = id, CompanyId = Guid.NewGuid(), FullAddress = "test", Year = 114, All = all };

    [Fact]
    public async Task LockAsync_設定鎖定狀態並記錄快照()
    {
        using var testDb = new TestDb();
        var areaId = Guid.NewGuid();
        testDb.Seed.Areas.Add(NewArea(areaId, all: 1652.4m));
        await testDb.Seed.SaveChangesAsync();

        var service = new AreaLockService(testDb.CreateNew());
        var result = await service.LockAsync(areaId, "user-1", "測試使用者");

        Assert.True(result);
        var area = await testDb.CreateNew().Areas.AsNoTracking().FirstAsync(a => a.Id == areaId);
        Assert.True(area.IsLocked);
        Assert.Equal("user-1", area.LockedByUserId);
        Assert.Equal("測試使用者", area.LockedByUserName);
        Assert.Equal(1652.4m, area.LockedSnapshotAll);
        Assert.NotNull(area.LockedAt);
    }

    [Fact]
    public async Task LockAsync_已經鎖定時不會覆寫快照_回傳false()
    {
        using var testDb = new TestDb();
        var areaId = Guid.NewGuid();
        var seedArea = NewArea(areaId, all: 100m);
        seedArea.IsLocked = true;
        seedArea.LockedSnapshotAll = 100m;
        testDb.Seed.Areas.Add(seedArea);
        await testDb.Seed.SaveChangesAsync();

        var service = new AreaLockService(testDb.CreateNew());
        // 鎖定後總排放量因為其他原因變動（理論上不該再變，但這裡驗證 LockAsync 對已鎖定的廠區是無害的 no-op）。
        var result = await service.LockAsync(areaId, "user-2", "另一個使用者");

        Assert.False(result);
        var area = await testDb.CreateNew().Areas.AsNoTracking().FirstAsync(a => a.Id == areaId);
        Assert.Equal(100m, area.LockedSnapshotAll);
    }

    [Fact]
    public async Task UnlockAsync_清除鎖定狀態與快照()
    {
        using var testDb = new TestDb();
        var areaId = Guid.NewGuid();
        var seedArea = NewArea(areaId);
        seedArea.IsLocked = true;
        seedArea.LockedAt = DateTime.Now;
        seedArea.LockedByUserId = "user-1";
        seedArea.LockedByUserName = "測試使用者";
        seedArea.LockedSnapshotAll = 100m;
        testDb.Seed.Areas.Add(seedArea);
        await testDb.Seed.SaveChangesAsync();

        var service = new AreaLockService(testDb.CreateNew());
        var result = await service.UnlockAsync(areaId);

        Assert.True(result);
        var area = await testDb.CreateNew().Areas.AsNoTracking().FirstAsync(a => a.Id == areaId);
        Assert.False(area.IsLocked);
        Assert.Null(area.LockedAt);
        Assert.Null(area.LockedByUserId);
        Assert.Null(area.LockedSnapshotAll);
    }

    [Fact]
    public async Task IsAreaLockedAsync_對null或空Guid回傳false()
    {
        using var testDb = new TestDb();
        var service = new AreaLockService(testDb.CreateNew());

        Assert.False(await service.IsAreaLockedAsync(null));
        Assert.False(await service.IsAreaLockedAsync(Guid.Empty));
    }

    [Fact]
    public async Task IsAreaLockedByDeviceIdAsync_跟著所屬廠區的鎖定狀態()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var areaId = Guid.NewGuid();
        var area = NewArea(areaId);
        area.IsLocked = true;
        context.Areas.Add(area);
        var deviceId = Guid.NewGuid();
        context.Devices.Add(new Device { Id = deviceId, AreaId = areaId, Name = "測試設備", Scope = "類別一", EmissionPattern = "固定", Material = "柴油" });
        await context.SaveChangesAsync();

        var service = new AreaLockService(testDb.CreateNew());
        var locked = await service.IsAreaLockedByDeviceIdAsync(deviceId);

        Assert.True(locked);
    }
}
