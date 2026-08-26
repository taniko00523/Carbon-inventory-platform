using Carbon_inventory_platform.Models;
using Carbon_inventory_platform.Services;
using Carbon_inventory_platform.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace Carbon_inventory_platform.Tests;

/// <summary>
/// B1 階段 3／4：角色×權限、使用者個別權限的矩陣勾選畫面，背後的資料比對／存檔邏輯。
/// 取代原本一次一筆的 RolePermission CRUD，以及原本完全沒有維護介面的 UserPermission。
/// </summary>
public class PermissionMatrixServiceTests
{
    private static Function NewFunction(int id, string name, int sort = 10)
        => new Function { Id = id, Name = name, CName = name, Sort = sort, FLevel = 1, UpperFunction = 0, IsDefault = 1, IsShow = 1, Class = 1 };

    private static FunctionAction NewAction(int id, string name)
        => new FunctionAction { Id = id, Name = name, CName = name, IsDefault = 0 };

    private static Permission NewPermission(int id, int functionId, int actionId)
        => new Permission { Id = id, Name = $"perm-{id}", FunctionId = functionId, FunctionActionId = actionId, CreateTime = DateTime.Now };

    [Fact]
    public async Task BuildRoleMatrixAsync_矩陣的列與欄依權限目錄建立()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var materials = NewFunction(1, "Materials", sort: 10);
        var devices = NewFunction(2, "Devices", sort: 20);
        var index = NewAction(1, "Index");
        var edit = NewAction(2, "Edit");
        context.Functions.AddRange(materials, devices);
        context.FunctionActions.AddRange(index, edit);
        context.Permissions.AddRange(
            NewPermission(1, materials.Id, index.Id),
            NewPermission(2, materials.Id, edit.Id),
            NewPermission(3, devices.Id, index.Id));
            // 故意不建立 devices-edit 的權限，驗證矩陣會把該格顯示成「沒有定義」。
        await context.SaveChangesAsync();

        var service = new PermissionMatrixService(testDb.CreateNew());
        var matrix = await service.BuildRoleMatrixAsync("role-1");

        Assert.Equal(new[] { "Materials", "Devices" }, matrix.Rows.Select(r => r.CName));
        Assert.Equal(new[] { "Index", "Edit" }, matrix.Columns.Select(c => c.CName));

        var devicesRow = matrix.Rows.Single(r => r.CName == "Devices");
        Assert.NotNull(devicesRow.Cells[0]); // Index 有定義
        Assert.Null(devicesRow.Cells[1]);    // Edit 沒有定義
    }

    [Fact]
    public async Task BuildRoleMatrixAsync_已授權的格子顯示為勾選()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var function = NewFunction(1, "Materials");
        var index = NewAction(1, "Index");
        var permission = NewPermission(1, function.Id, index.Id);
        context.Functions.Add(function);
        context.FunctionActions.Add(index);
        context.Permissions.Add(permission);
        context.RolePermissions.Add(new RolePermission { RoleId = "role-1", PermissionId = permission.Id });
        await context.SaveChangesAsync();

        var service = new PermissionMatrixService(testDb.CreateNew());
        var matrix = await service.BuildRoleMatrixAsync("role-1");

        Assert.True(matrix.Rows[0].Cells[0]!.Checked);
    }

    [Fact]
    public async Task BuildRoleMatrixAsync_不同角色的授權互不影響()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var function = NewFunction(1, "Materials");
        var index = NewAction(1, "Index");
        var permission = NewPermission(1, function.Id, index.Id);
        context.Functions.Add(function);
        context.FunctionActions.Add(index);
        context.Permissions.Add(permission);
        context.RolePermissions.Add(new RolePermission { RoleId = "role-1", PermissionId = permission.Id });
        await context.SaveChangesAsync();

        var service = new PermissionMatrixService(testDb.CreateNew());
        var matrixForOtherRole = await service.BuildRoleMatrixAsync("role-2");

        Assert.False(matrixForOtherRole.Rows[0].Cells[0]!.Checked);
    }

    [Fact]
    public async Task SaveRoleMatrixAsync_勾選新的權限會新增RolePermission()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var function = NewFunction(1, "Materials");
        var index = NewAction(1, "Index");
        var permission = NewPermission(1, function.Id, index.Id);
        context.Functions.Add(function);
        context.FunctionActions.Add(index);
        context.Permissions.Add(permission);
        await context.SaveChangesAsync();

        var service = new PermissionMatrixService(testDb.CreateNew());
        await service.SaveRoleMatrixAsync("role-1", new[] { permission.Id });

        var stored = await testDb.CreateNew().RolePermissions.ToListAsync();
        Assert.Single(stored);
        Assert.Equal("role-1", stored[0].RoleId);
        Assert.Equal(permission.Id, stored[0].PermissionId);
    }

    [Fact]
    public async Task SaveRoleMatrixAsync_取消勾選會刪除原本的RolePermission()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var function = NewFunction(1, "Materials");
        var index = NewAction(1, "Index");
        var permission = NewPermission(1, function.Id, index.Id);
        context.Functions.Add(function);
        context.FunctionActions.Add(index);
        context.Permissions.Add(permission);
        context.RolePermissions.Add(new RolePermission { RoleId = "role-1", PermissionId = permission.Id });
        await context.SaveChangesAsync();

        var service = new PermissionMatrixService(testDb.CreateNew());
        // 送出空白清單＝全部取消勾選。
        await service.SaveRoleMatrixAsync("role-1", Array.Empty<int>());

        var stored = await testDb.CreateNew().RolePermissions.ToListAsync();
        Assert.Empty(stored);
    }

    [Fact]
    public async Task SaveRoleMatrixAsync_只異動這個角色_不影響其他角色的授權()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var function = NewFunction(1, "Materials");
        var index = NewAction(1, "Index");
        var permission = NewPermission(1, function.Id, index.Id);
        context.Functions.Add(function);
        context.FunctionActions.Add(index);
        context.Permissions.Add(permission);
        context.RolePermissions.Add(new RolePermission { RoleId = "role-2", PermissionId = permission.Id });
        await context.SaveChangesAsync();

        var service = new PermissionMatrixService(testDb.CreateNew());
        await service.SaveRoleMatrixAsync("role-1", new[] { permission.Id });

        var stored = await testDb.CreateNew().RolePermissions.ToListAsync();
        Assert.Equal(2, stored.Count);
        Assert.Contains(stored, rp => rp.RoleId == "role-1");
        Assert.Contains(stored, rp => rp.RoleId == "role-2");
    }

    [Fact]
    public async Task SaveRoleMatrixAsync_不存在的權限Id會被忽略_不會產生違反外鍵的資料()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var function = NewFunction(1, "Materials");
        var index = NewAction(1, "Index");
        var permission = NewPermission(1, function.Id, index.Id);
        context.Functions.Add(function);
        context.FunctionActions.Add(index);
        context.Permissions.Add(permission);
        await context.SaveChangesAsync();

        var service = new PermissionMatrixService(testDb.CreateNew());
        // 9999 不是任何一筆權限的 Id（例如表單被竄改夾帶進來的值）。
        await service.SaveRoleMatrixAsync("role-1", new[] { permission.Id, 9999 });

        var stored = await testDb.CreateNew().RolePermissions.ToListAsync();
        Assert.Single(stored);
        Assert.Equal(permission.Id, stored[0].PermissionId);
    }

    [Fact]
    public async Task BuildUserMatrixAsync_角色授予的權限顯示為勾選且鎖住()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var function = NewFunction(1, "Materials");
        var index = NewAction(1, "Index");
        var permission = NewPermission(1, function.Id, index.Id);
        context.Functions.Add(function);
        context.FunctionActions.Add(index);
        context.Permissions.Add(permission);
        context.RolePermissions.Add(new RolePermission { RoleId = "role-1", PermissionId = permission.Id });
        await context.SaveChangesAsync();

        var service = new PermissionMatrixService(testDb.CreateNew());
        var matrix = await service.BuildUserMatrixAsync("user-1", new[] { "role-1" });

        var cell = matrix.Rows[0].Cells[0]!;
        Assert.True(cell.Checked);
        Assert.True(cell.Locked);
    }

    [Fact]
    public async Task BuildUserMatrixAsync_個別授予的權限顯示為勾選但不鎖住()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var function = NewFunction(1, "Materials");
        var index = NewAction(1, "Index");
        var permission = NewPermission(1, function.Id, index.Id);
        context.Functions.Add(function);
        context.FunctionActions.Add(index);
        context.Permissions.Add(permission);
        context.UserPermissions.Add(new UserPermission { UserId = "user-1", PermissionId = permission.Id });
        await context.SaveChangesAsync();

        var service = new PermissionMatrixService(testDb.CreateNew());
        var matrix = await service.BuildUserMatrixAsync("user-1", Array.Empty<string>());

        var cell = matrix.Rows[0].Cells[0]!;
        Assert.True(cell.Checked);
        Assert.False(cell.Locked);
    }

    [Fact]
    public async Task BuildUserMatrixAsync_都沒有授權時顯示未勾選且不鎖住()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var function = NewFunction(1, "Materials");
        var index = NewAction(1, "Index");
        var permission = NewPermission(1, function.Id, index.Id);
        context.Functions.Add(function);
        context.FunctionActions.Add(index);
        context.Permissions.Add(permission);
        await context.SaveChangesAsync();

        var service = new PermissionMatrixService(testDb.CreateNew());
        var matrix = await service.BuildUserMatrixAsync("user-1", Array.Empty<string>());

        var cell = matrix.Rows[0].Cells[0]!;
        Assert.False(cell.Checked);
        Assert.False(cell.Locked);
    }

    [Fact]
    public async Task SaveUserMatrixAsync_勾選新權限會新增UserPermission()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var function = NewFunction(1, "Materials");
        var index = NewAction(1, "Index");
        var permission = NewPermission(1, function.Id, index.Id);
        context.Functions.Add(function);
        context.FunctionActions.Add(index);
        context.Permissions.Add(permission);
        await context.SaveChangesAsync();

        var service = new PermissionMatrixService(testDb.CreateNew());
        await service.SaveUserMatrixAsync("user-1", Array.Empty<string>(), new[] { permission.Id });

        var stored = await testDb.CreateNew().UserPermissions.ToListAsync();
        Assert.Single(stored);
        Assert.Equal("user-1", stored[0].UserId);
    }

    [Fact]
    public async Task SaveUserMatrixAsync_取消勾選會刪除原本的個別授權()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var function = NewFunction(1, "Materials");
        var index = NewAction(1, "Index");
        var permission = NewPermission(1, function.Id, index.Id);
        context.Functions.Add(function);
        context.FunctionActions.Add(index);
        context.Permissions.Add(permission);
        context.UserPermissions.Add(new UserPermission { UserId = "user-1", PermissionId = permission.Id });
        await context.SaveChangesAsync();

        var service = new PermissionMatrixService(testDb.CreateNew());
        await service.SaveUserMatrixAsync("user-1", Array.Empty<string>(), Array.Empty<int>());

        var stored = await testDb.CreateNew().UserPermissions.ToListAsync();
        Assert.Empty(stored);
    }

    [Fact]
    public async Task SaveUserMatrixAsync_角色已授予的權限即使被送出也不會多存一筆個別授權()
    {
        // 畫面上角色授予的格子是 disabled，瀏覽器不會把它送出；但這裡故意模擬「被竄改的表單」
        // 把角色已授予的 PermissionId 也塞進 checkedPermissionIds，驗證伺服器端仍然會濾掉，
        // 不會因此多出一筆看似合法、實際上完全多餘的 UserPermission。
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var function = NewFunction(1, "Materials");
        var index = NewAction(1, "Index");
        var permission = NewPermission(1, function.Id, index.Id);
        context.Functions.Add(function);
        context.FunctionActions.Add(index);
        context.Permissions.Add(permission);
        context.RolePermissions.Add(new RolePermission { RoleId = "role-1", PermissionId = permission.Id });
        await context.SaveChangesAsync();

        var service = new PermissionMatrixService(testDb.CreateNew());
        await service.SaveUserMatrixAsync("user-1", new[] { "role-1" }, new[] { permission.Id });

        var stored = await testDb.CreateNew().UserPermissions.ToListAsync();
        Assert.Empty(stored);
    }

    [Fact]
    public async Task SaveUserMatrixAsync_角色授予的格子沒有送出時不會誤刪既有的個別授權()
    {
        // 這是矩陣設計裡最容易踩到的地雷：角色已授予的格子在畫面上被鎖住（disabled），
        // 表單送出時完全不會帶到這個 PermissionId。如果重設邏輯天真地把「沒被勾選」
        // 都當成「要移除」，就會把使用者原本就有的個別授權（即使跟角色重複）一併刪掉。
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var function = NewFunction(1, "Materials");
        var index = NewAction(1, "Index");
        var permission = NewPermission(1, function.Id, index.Id);
        context.Functions.Add(function);
        context.FunctionActions.Add(index);
        context.Permissions.Add(permission);
        context.RolePermissions.Add(new RolePermission { RoleId = "role-1", PermissionId = permission.Id });
        context.UserPermissions.Add(new UserPermission { UserId = "user-1", PermissionId = permission.Id });
        await context.SaveChangesAsync();

        var service = new PermissionMatrixService(testDb.CreateNew());
        // 表單完全沒有帶這個 PermissionId（畫面上是鎖住的格子），模擬真實送出內容。
        await service.SaveUserMatrixAsync("user-1", new[] { "role-1" }, Array.Empty<int>());

        var stored = await testDb.CreateNew().UserPermissions.ToListAsync();
        Assert.Single(stored);
        Assert.Equal(permission.Id, stored[0].PermissionId);
    }

    [Fact]
    public async Task SaveUserMatrixAsync_只異動這個使用者_不影響其他使用者的個別授權()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var function = NewFunction(1, "Materials");
        var index = NewAction(1, "Index");
        var permission = NewPermission(1, function.Id, index.Id);
        context.Functions.Add(function);
        context.FunctionActions.Add(index);
        context.Permissions.Add(permission);
        context.UserPermissions.Add(new UserPermission { UserId = "user-2", PermissionId = permission.Id });
        await context.SaveChangesAsync();

        var service = new PermissionMatrixService(testDb.CreateNew());
        await service.SaveUserMatrixAsync("user-1", Array.Empty<string>(), new[] { permission.Id });

        var stored = await testDb.CreateNew().UserPermissions.ToListAsync();
        Assert.Equal(2, stored.Count);
        Assert.Contains(stored, up => up.UserId == "user-1");
        Assert.Contains(stored, up => up.UserId == "user-2");
    }

    // ---- 併發保護：偵測「存檔前有別的管理員也改過」（見 PermissionMatrixService 的 review 修正）--------

    [Fact]
    public async Task BuildRoleMatrixAsync_Baseline反映目前的授權集合()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var function = NewFunction(1, "Materials");
        var index = NewAction(1, "Index");
        var edit = NewAction(2, "Edit");
        var permIndex = NewPermission(1, function.Id, index.Id);
        var permEdit = NewPermission(2, function.Id, edit.Id);
        context.Functions.Add(function);
        context.FunctionActions.AddRange(index, edit);
        context.Permissions.AddRange(permIndex, permEdit);
        context.RolePermissions.Add(new RolePermission { RoleId = "role-1", PermissionId = permIndex.Id });
        await context.SaveChangesAsync();

        var service = new PermissionMatrixService(testDb.CreateNew());
        var matrix = await service.BuildRoleMatrixAsync("role-1");

        Assert.Equal("1", matrix.Baseline);
    }

    [Fact]
    public async Task SaveRoleMatrixAsync_Baseline與資料庫現況相符時正常存檔()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var function = NewFunction(1, "Materials");
        var index = NewAction(1, "Index");
        var permission = NewPermission(1, function.Id, index.Id);
        context.Functions.Add(function);
        context.FunctionActions.Add(index);
        context.Permissions.Add(permission);
        await context.SaveChangesAsync();

        var service = new PermissionMatrixService(testDb.CreateNew());
        // 目前資料庫是空的（沒有任何 RolePermission），所以正確的 baseline 是空字串。
        var result = await service.SaveRoleMatrixAsync("role-1", new[] { permission.Id }, expectedBaseline: "");

        Assert.Equal(PermissionMatrixSaveResult.Saved, result);
        var stored = await testDb.CreateNew().RolePermissions.ToListAsync();
        Assert.Single(stored);
    }

    [Fact]
    public async Task SaveRoleMatrixAsync_Baseline與資料庫現況不符時拒絕存檔且不異動資料()
    {
        // 模擬：管理員 A 打開矩陣畫面時（baseline 是空字串），管理員 B 搶先勾選了同一個角色的某個權限並存檔；
        // A 送出表單時附上的還是舊的（空字串）baseline，跟資料庫當下的實際狀態對不起來，必須拒絕存檔，
        // 否則 A 的表單（完全不知道 B 剛新增的那筆）送出後會被誤判成「B 新增的那筆要被刪掉」。
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var function = NewFunction(1, "Materials");
        var index = NewAction(1, "Index");
        var edit = NewAction(2, "Edit");
        var permIndex = NewPermission(1, function.Id, index.Id);
        var permEdit = NewPermission(2, function.Id, edit.Id);
        context.Functions.Add(function);
        context.FunctionActions.AddRange(index, edit);
        context.Permissions.AddRange(permIndex, permEdit);
        // 管理員 B 搶先新增的授權。
        context.RolePermissions.Add(new RolePermission { RoleId = "role-1", PermissionId = permIndex.Id });
        await context.SaveChangesAsync();

        var service = new PermissionMatrixService(testDb.CreateNew());
        // 管理員 A 用「載入畫面時」的舊 baseline（空字串）送出，想額外加上 permEdit。
        var result = await service.SaveRoleMatrixAsync("role-1", new[] { permEdit.Id }, expectedBaseline: "");

        Assert.Equal(PermissionMatrixSaveResult.Conflict, result);
        // 資料庫必須維持 B 存的那筆不動，A 這次完全沒有寫入任何東西（沒有多存 permEdit，也沒有刪掉 permIndex）。
        var stored = await testDb.CreateNew().RolePermissions.ToListAsync();
        Assert.Single(stored);
        Assert.Equal(permIndex.Id, stored[0].PermissionId);
    }

    [Fact]
    public async Task SaveRoleMatrixAsync_沒有帶Baseline時維持舊行為_不檢查直接覆蓋()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var function = NewFunction(1, "Materials");
        var index = NewAction(1, "Index");
        var permission = NewPermission(1, function.Id, index.Id);
        context.Functions.Add(function);
        context.FunctionActions.Add(index);
        context.Permissions.Add(permission);
        context.RolePermissions.Add(new RolePermission { RoleId = "role-1", PermissionId = permission.Id });
        await context.SaveChangesAsync();

        var service = new PermissionMatrixService(testDb.CreateNew());
        // 不傳 expectedBaseline（沿用預設值 null）＝不檢查，直接依 checkedPermissionIds 覆蓋。
        var result = await service.SaveRoleMatrixAsync("role-1", Array.Empty<int>());

        Assert.Equal(PermissionMatrixSaveResult.Saved, result);
        var stored = await testDb.CreateNew().RolePermissions.ToListAsync();
        Assert.Empty(stored);
    }

    [Fact]
    public async Task BuildUserMatrixAsync_Baseline只涵蓋非角色授予的個別權限()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var function = NewFunction(1, "Materials");
        var index = NewAction(1, "Index");
        var edit = NewAction(2, "Edit");
        var permIndex = NewPermission(1, function.Id, index.Id);
        var permEdit = NewPermission(2, function.Id, edit.Id);
        context.Functions.Add(function);
        context.FunctionActions.AddRange(index, edit);
        context.Permissions.AddRange(permIndex, permEdit);
        // permIndex 由角色授予，permEdit 是個別授予。
        context.RolePermissions.Add(new RolePermission { RoleId = "role-1", PermissionId = permIndex.Id });
        context.UserPermissions.Add(new UserPermission { UserId = "user-1", PermissionId = permEdit.Id });
        await context.SaveChangesAsync();

        var service = new PermissionMatrixService(testDb.CreateNew());
        var matrix = await service.BuildUserMatrixAsync("user-1", new[] { "role-1" });

        // Baseline 只能是 permEdit，角色授予的 permIndex 不在「可異動範圍」內，不該算進去。
        Assert.Equal(permEdit.Id.ToString(), matrix.Baseline);
    }

    [Fact]
    public async Task SaveUserMatrixAsync_Baseline不符時拒絕存檔且不異動個別授權()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var function = NewFunction(1, "Materials");
        var index = NewAction(1, "Index");
        var edit = NewAction(2, "Edit");
        var permIndex = NewPermission(1, function.Id, index.Id);
        var permEdit = NewPermission(2, function.Id, edit.Id);
        context.Functions.Add(function);
        context.FunctionActions.AddRange(index, edit);
        context.Permissions.AddRange(permIndex, permEdit);
        // 管理員 B 搶先幫這個使用者加了 permIndex 的個別授權。
        context.UserPermissions.Add(new UserPermission { UserId = "user-1", PermissionId = permIndex.Id });
        await context.SaveChangesAsync();

        var service = new PermissionMatrixService(testDb.CreateNew());
        // 管理員 A 用載入畫面時的舊 baseline（空字串，當時還沒有任何個別授權）送出。
        var result = await service.SaveUserMatrixAsync("user-1", Array.Empty<string>(), new[] { permEdit.Id }, expectedBaseline: "");

        Assert.Equal(PermissionMatrixSaveResult.Conflict, result);
        var stored = await testDb.CreateNew().UserPermissions.ToListAsync();
        Assert.Single(stored);
        Assert.Equal(permIndex.Id, stored[0].PermissionId);
    }

    // ---- EncodedBaseline／DecodeExpectedBaseline：表單欄位編碼 -----------------------------
    // ASP.NET Core 的表單繫結會把「值是空字串」的欄位直接繫結成 null（視同完全沒有這個欄位），
    // 這對大多數欄位是合理的行為，但 Baseline 空字串是「這個角色／使用者當下完全沒有任何
    // 授權」的合法狀態，如果被誤判成 null＝「不用檢查併發」，併發保護在這個情境下會整個失效
    // （這是實機測試時真的踩到的 bug，不是憑空想像的邊界案例）。

    [Fact]
    public void EncodedBaseline_空白基準值編碼後解碼回來仍然是空字串_不是null()
    {
        var vm = new PermissionMatrixViewModel { Baseline = "" };

        var decoded = PermissionMatrixViewModel.DecodeExpectedBaseline(vm.EncodedBaseline);

        Assert.NotNull(decoded);
        Assert.Equal("", decoded);
    }

    [Fact]
    public void EncodedBaseline_非空白基準值可以正確編碼解碼()
    {
        var vm = new PermissionMatrixViewModel { Baseline = "1,6,31" };

        var decoded = PermissionMatrixViewModel.DecodeExpectedBaseline(vm.EncodedBaseline);

        Assert.Equal("1,6,31", decoded);
    }

    [Fact]
    public void DecodeExpectedBaseline_欄位整個缺席時回傳null_視同不檢查()
    {
        Assert.Null(PermissionMatrixViewModel.DecodeExpectedBaseline(null));
    }

    [Fact]
    public async Task SaveRoleMatrixAsync_角色目前沒有任何授權時_空白Baseline走完整個表單編碼流程仍然能偵測衝突()
    {
        // 對應實機踩到的情境：管理員 A 打開一個目前完全沒有授權的角色（Baseline=""），
        // 這段時間管理員 B 搶先新增了一筆授權，A 才送出表單——這裡完整模擬「View 編碼、
        // Controller 解碼」的路徑（不是直接塞 ""），確認空白 Baseline 真的能擋下這次存檔。
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var function = NewFunction(1, "Materials");
        var index = NewAction(1, "Index");
        var permission = NewPermission(1, function.Id, index.Id);
        context.Functions.Add(function);
        context.FunctionActions.Add(index);
        context.Permissions.Add(permission);
        await context.SaveChangesAsync();

        var service = new PermissionMatrixService(testDb.CreateNew());
        // 管理員 A 打開畫面時，角色還沒有任何授權。
        var matrixAtLoadTime = await service.BuildRoleMatrixAsync("role-1");
        Assert.Equal("", matrixAtLoadTime.Baseline);
        var postedFieldValue = matrixAtLoadTime.EncodedBaseline; // View 送出時 hidden input 的值

        // 管理員 B 搶先存了一筆。
        var seedContext = testDb.CreateNew();
        seedContext.RolePermissions.Add(new RolePermission { RoleId = "role-1", PermissionId = permission.Id });
        await seedContext.SaveChangesAsync();

        // 管理員 A 的表單這時候才送到伺服器：Controller 端解碼 postedFieldValue 再呼叫存檔。
        var decoded = PermissionMatrixViewModel.DecodeExpectedBaseline(postedFieldValue);
        var result = await service.SaveRoleMatrixAsync("role-1", Array.Empty<int>(), decoded);

        Assert.Equal(PermissionMatrixSaveResult.Conflict, result);
        var stored = await testDb.CreateNew().RolePermissions.ToListAsync();
        Assert.Single(stored); // B 存的那筆還在，沒有被 A 的（過期的）空白提交清空。
    }
}
