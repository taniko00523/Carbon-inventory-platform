using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Carbon_inventory_platform.Services;

namespace Carbon_inventory_platform.Tests;

/// <summary>
/// B1（接上權限系統）階段 1 的回歸測試：CompanyOwnershipService（原本分散在
/// AreasController／DevicesController／EmissionController 三份不一致的擁有權檢查，
/// 已合併成一個共用服務）與 RolePermissionService（選單可見性 + 權限判斷）。
/// </summary>
public class CompanyOwnershipServiceTests
{
    private static Company NewCompany(string? userId) => new Company
    {
        UserId = userId,
        Name = "測試公司",
        EnglishName = "Test Co",
        ContactName = "測試",
        Email = "test@example.com",
        Phone = "0000000000",
    };

    private static CompanyOwnershipService NewService(TestDb testDb)
        => new CompanyOwnershipService(testDb.CreateNew(), TestIdentityFactory.CreateUserManager(testDb.CreateNew()));

    [Fact]
    public async Task Admin可存取任何公司_即使不是自己名下的()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var company = NewCompany(userId: "someone-else");
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        var service = NewService(testDb);
        var admin = TestIdentityFactory.MakePrincipal("admin-user", "Admin");

        Assert.True(await service.CanAccessCompanyAsync(admin, company.Id));
    }

    [Fact]
    public async Task SuperAdmin可存取任何廠區()
    {
        // 原本 AreasController.CanAccessCompanyAsync 只放行 Admin，沒有放行 SuperAdmin，
        // 跟 DevicesController／EmissionController 的版本不一致，合併後統一以較完整的版本為準。
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var company = NewCompany(userId: "someone-else");
        context.Companies.Add(company);
        var area = new Area { Id = Guid.NewGuid(), CompanyId = company.Id, FullAddress = "test", Year = 113 };
        context.Areas.Add(area);
        await context.SaveChangesAsync();

        var service = NewService(testDb);
        var superAdmin = TestIdentityFactory.MakePrincipal("super-admin-user", "SuperAdmin");

        Assert.True(await service.CanAccessAreaAsync(superAdmin, area.Id));
    }

    [Fact]
    public async Task 一般使用者可存取自己的公司_不可存取別人的公司()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var ownCompany = NewCompany(userId: "user-1");
        var otherCompany = NewCompany(userId: "user-2");
        context.Companies.AddRange(ownCompany, otherCompany);
        await context.SaveChangesAsync();

        var service = NewService(testDb);
        var user1 = TestIdentityFactory.MakePrincipal("user-1", "User");

        Assert.True(await service.CanAccessCompanyAsync(user1, ownCompany.Id));
        Assert.False(await service.CanAccessCompanyAsync(user1, otherCompany.Id));
    }

    [Fact]
    public async Task 使用者名下有兩家公司時_兩家都可存取()
    {
        // 原本 AreasController 版本用 GetCallerCompanyIdAsync（FirstOrDefaultAsync）只認得第一家公司，
        // 同一使用者名下的第二家公司會被誤判為沒有權限存取；合併後改用「是否包含在名下所有公司」判斷。
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var companyA = NewCompany(userId: "multi-user");
        var companyB = NewCompany(userId: "multi-user");
        context.Companies.AddRange(companyA, companyB);
        await context.SaveChangesAsync();

        var service = NewService(testDb);
        var user = TestIdentityFactory.MakePrincipal("multi-user", "User");

        Assert.True(await service.CanAccessCompanyAsync(user, companyA.Id));
        Assert.True(await service.CanAccessCompanyAsync(user, companyB.Id));
    }

    [Fact]
    public async Task 一般使用者可存取自己公司的廠區_不可存取別家公司的廠區()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var ownCompany = NewCompany(userId: "user-1");
        var otherCompany = NewCompany(userId: "user-2");
        context.Companies.AddRange(ownCompany, otherCompany);
        var ownArea = new Area { Id = Guid.NewGuid(), CompanyId = ownCompany.Id, FullAddress = "own", Year = 113 };
        var otherArea = new Area { Id = Guid.NewGuid(), CompanyId = otherCompany.Id, FullAddress = "other", Year = 113 };
        context.Areas.AddRange(ownArea, otherArea);
        await context.SaveChangesAsync();

        var service = NewService(testDb);
        var user1 = TestIdentityFactory.MakePrincipal("user-1", "User");

        Assert.True(await service.CanAccessAreaAsync(user1, ownArea.Id));
        Assert.False(await service.CanAccessAreaAsync(user1, otherArea.Id));
    }

    [Fact]
    public async Task CompanyId為null時一律回傳false_即使是Admin()
    {
        using var testDb = new TestDb();
        var service = NewService(testDb);
        var admin = TestIdentityFactory.MakePrincipal("admin-user", "Admin");

        Assert.False(await service.CanAccessCompanyAsync(admin, null));
    }

    [Fact]
    public async Task AreaId為GuidEmpty時一律回傳false_即使是Admin()
    {
        using var testDb = new TestDb();
        var service = NewService(testDb);
        var admin = TestIdentityFactory.MakePrincipal("admin-user", "Admin");

        Assert.False(await service.CanAccessAreaAsync(admin, Guid.Empty));
    }
}

public class RolePermissionServiceTests
{
    private static Function NewFunction(int id, string name, int sort = 10, byte isShow = 1)
        => new Function { Id = id, Name = name, CName = name, Sort = sort, FLevel = 1, UpperFunction = 0, IsDefault = 1, IsShow = isShow, Class = 1 };

    private static FunctionAction NewAction(int id, string name)
        => new FunctionAction { Id = id, Name = name, CName = name, IsDefault = 0 };

    private static Permission NewPermission(int id, int functionId, int actionId)
        => new Permission { Id = id, Name = "test", FunctionId = functionId, FunctionActionId = actionId, CreateTime = DateTime.Now };

    [Fact]
    public async Task HasPermissionAsync_UserPermission命中時直接放行_即使角色沒有授權()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var function = NewFunction(1, "Materials");
        var indexAction = NewAction(1, "Index");
        var permission = NewPermission(1, function.Id, indexAction.Id);
        context.Functions.Add(function);
        context.FunctionActions.Add(indexAction);
        context.Permissions.Add(permission);
        context.UserPermissions.Add(new UserPermission { Id = 1, UserId = "user-1", PermissionId = permission.Id });
        await context.SaveChangesAsync();

        var service = new RolePermissionService(testDb.CreateNew());
        var result = await service.HasPermissionAsync("user-1", roleIds: Array.Empty<string>(), controller: "Materials", action: "Index");

        Assert.True(result);
    }

    [Fact]
    public async Task HasPermissionAsync_UserPermission沒有但角色有授權時放行()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var function = NewFunction(1, "Materials");
        var indexAction = NewAction(1, "Index");
        var permission = NewPermission(1, function.Id, indexAction.Id);
        context.Functions.Add(function);
        context.FunctionActions.Add(indexAction);
        context.Permissions.Add(permission);
        context.RolePermissions.Add(new RolePermission { Id = 1, RoleId = "role-1", PermissionId = permission.Id });
        await context.SaveChangesAsync();

        var service = new RolePermissionService(testDb.CreateNew());
        var result = await service.HasPermissionAsync("user-1", roleIds: new[] { "role-1" }, controller: "Materials", action: "Index");

        Assert.True(result);
    }

    [Fact]
    public async Task HasPermissionAsync_UserPermission與角色皆無授權時拒絕()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var function = NewFunction(1, "Materials");
        var indexAction = NewAction(1, "Index");
        var permission = NewPermission(1, function.Id, indexAction.Id);
        context.Functions.Add(function);
        context.FunctionActions.Add(indexAction);
        context.Permissions.Add(permission);
        await context.SaveChangesAsync();

        var service = new RolePermissionService(testDb.CreateNew());
        var result = await service.HasPermissionAsync("user-1", roleIds: new[] { "role-1" }, controller: "Materials", action: "Index");

        Assert.False(result);
    }

    [Fact]
    public async Task HasPermissionAsync_權限表沒有這個畫面動作組合時拒絕()
    {
        // 例如控制器的 action 名稱跟權限表的 FunctionAction 名稱對不上（未定義的組合），
        // 應該視為沒有授權，而不是拋例外或誤放行。
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var function = NewFunction(1, "Materials");
        var indexAction = NewAction(1, "Index");
        context.Functions.Add(function);
        context.FunctionActions.Add(indexAction);
        await context.SaveChangesAsync();

        var service = new RolePermissionService(testDb.CreateNew());
        var result = await service.HasPermissionAsync("user-1", roleIds: new[] { "role-1" }, controller: "Materials", action: "Index");

        Assert.False(result);
    }

    [Fact]
    public async Task GetVisibleFunctionsAsync_只靠角色權限也能看到選單項目()
    {
        // 修掉的 bug：GetVisibleFunctionsAsync 原本只讀 UserPermission，完全沒看 RolePermission，
        // 而 UserPermission 目前沒有任何維護介面、資料表永遠是空的 —— 等於選單邏輯必定回傳空清單。
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var function = NewFunction(1, "Materials");
        var indexAction = NewAction(1, "Index");
        var permission = NewPermission(1, function.Id, indexAction.Id);
        context.Functions.Add(function);
        context.FunctionActions.Add(indexAction);
        context.Permissions.Add(permission);
        context.RolePermissions.Add(new RolePermission { Id = 1, RoleId = "role-1", PermissionId = permission.Id });
        await context.SaveChangesAsync();

        var service = new RolePermissionService(testDb.CreateNew());
        var visible = await service.GetVisibleFunctionsAsync("user-1", new[] { "role-1" });

        Assert.Contains(visible, f => f.Id == function.Id);
    }

    [Fact]
    public async Task GetVisibleFunctionsAsync_個別UserPermission也能看到_不需要角色授權()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var function = NewFunction(1, "Materials");
        var indexAction = NewAction(1, "Index");
        var permission = NewPermission(1, function.Id, indexAction.Id);
        context.Functions.Add(function);
        context.FunctionActions.Add(indexAction);
        context.Permissions.Add(permission);
        context.UserPermissions.Add(new UserPermission { Id = 1, UserId = "user-1", PermissionId = permission.Id });
        await context.SaveChangesAsync();

        var service = new RolePermissionService(testDb.CreateNew());
        var visible = await service.GetVisibleFunctionsAsync("user-1", Array.Empty<string>());

        Assert.Contains(visible, f => f.Id == function.Id);
    }

    [Fact]
    public async Task GetVisibleFunctionsAsync_IsShow為0的畫面不會出現()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var function = NewFunction(1, "Hidden", isShow: 0);
        var indexAction = NewAction(1, "Index");
        var permission = NewPermission(1, function.Id, indexAction.Id);
        context.Functions.Add(function);
        context.FunctionActions.Add(indexAction);
        context.Permissions.Add(permission);
        context.RolePermissions.Add(new RolePermission { Id = 1, RoleId = "role-1", PermissionId = permission.Id });
        await context.SaveChangesAsync();

        var service = new RolePermissionService(testDb.CreateNew());
        var visible = await service.GetVisibleFunctionsAsync("user-1", new[] { "role-1" });

        Assert.Empty(visible);
    }

    [Fact]
    public async Task GetVisibleFunctionsAsync_結果依Sort排序()
    {
        using var testDb = new TestDb();
        var context = testDb.Seed;
        var functionB = NewFunction(1, "B畫面", sort: 20);
        var functionA = NewFunction(2, "A畫面", sort: 10);
        var indexAction = NewAction(1, "Index");
        var permB = NewPermission(1, functionB.Id, indexAction.Id);
        var permA = NewPermission(2, functionA.Id, indexAction.Id);
        context.Functions.AddRange(functionB, functionA);
        context.FunctionActions.Add(indexAction);
        context.Permissions.AddRange(permB, permA);
        context.RolePermissions.AddRange(
            new RolePermission { Id = 1, RoleId = "role-1", PermissionId = permB.Id },
            new RolePermission { Id = 2, RoleId = "role-1", PermissionId = permA.Id });
        await context.SaveChangesAsync();

        var service = new RolePermissionService(testDb.CreateNew());
        var visible = await service.GetVisibleFunctionsAsync("user-1", new[] { "role-1" });

        Assert.Equal(new[] { functionA.Id, functionB.Id }, visible.Select(f => f.Id));
    }

    [Fact]
    public async Task GetVisibleFunctionsAsync_沒有UserId時回傳空清單()
    {
        using var testDb = new TestDb();
        var service = new RolePermissionService(testDb.CreateNew());

        var visible = await service.GetVisibleFunctionsAsync(null, new[] { "role-1" });

        Assert.Empty(visible);
    }
}
