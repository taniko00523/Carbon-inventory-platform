using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Carbon_inventory_platform.Data
{
    /// <summary>
    /// 啟動時建立系統必要的基礎資料：角色、管理員帳號、以及權限設定用的
    /// 畫面 (Function) / 功能 (FunctionAction) 參考資料。
    /// 原本這段程式碼在 Program.cs 中被註解掉，導致全新的資料庫沒有任何角色與帳號，
    /// 而 Register 頁面本身需要 Admin 身分，形成「誰都無法登入」的死結。
    /// </summary>
    public static class DbSeeder
    {
        public static readonly string[] Roles = { "SuperAdmin", "Admin", "PowerUser", "User", "Guest" };

        /// <summary>後台畫面清單：Name 必須與 Controller 名稱一致，PermissionFilter 才能比對得到。</summary>
        private static readonly (string Name, string CName, int Sort, int Level, int Class)[] Functions =
        {
            ("Home",            "首頁",           10, 1, 2),
            ("Companies",       "公司總覽",       20, 1, 1),
            ("Areas",           "邊界總覽",       30, 1, 2),
            ("Devices",         "排放源",         40, 1, 2),
            ("Emission",        "排放量統計",     50, 1, 2),
            ("GWP",             "GWP總覽",        60, 1, 1),
            ("Materials",       "排放係數設定",   70, 1, 1),
            ("DefaultDevices",  "預設排放源",     80, 1, 1),
            ("DeviceDatas",     "排放源設定",     90, 1, 1),
            ("User",            "用戶總覽",      100, 1, 1),
            ("Functions",       "畫面總覽",      110, 1, 1),
            ("FunctionActions", "功能總覽",      120, 1, 1),
            ("Permissions",     "權限總覽",      130, 1, 1),
            ("RolePermissions", "角色權限總覽",  140, 1, 1),
            ("Feedbacks",       "回報問題總覽",  150, 1, 1),
        };

        /// <summary>動作清單：Name 必須與 Action 名稱一致（大小寫依 MVC 比對規則不敏感，但保持一致較清楚）。</summary>
        private static readonly (string Name, string CName, byte IsDefault)[] Actions =
        {
            ("Index",   "檢視", 1),
            ("Details", "明細", 0),
            ("Create",  "新增", 0),
            ("Edit",    "修改", 0),
            ("Delete",  "刪除", 0),
        };

        /// <summary>
        /// 少數畫面的實際 Action 名稱跟標準 CRUD 五動作不一樣（例如 GWPController 的新增動作
        /// 叫 Add 不是 Create），這裡個別補上「畫面 + 專屬動作」的權限，不放進上面的標準網格，
        /// 避免其他 14 個畫面也一起長出用不到的「Add」選項。
        /// </summary>
        private static readonly (string FunctionName, string ActionName, string ActionCName)[] ExtraActions =
        {
            // CName 特意跟標準網格的「新增」做出區別（標準網格是給 Create 動作用的，
            // 但 GWPController 沒有 Create 這個 action，只有 Add，兩者不能共用同一個顯示名稱，
            // 否則角色權限總覽的下拉選單會出現兩個看起來一樣但實際指向不同動作的選項）。
            ("GWP", "Add", "新增(Add)"),
        };

        public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration, ILogger logger)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            await SeedRolesAsync(roleManager, logger);
            await SeedReferenceDataAsync(context, logger);
            await SeedExtraPermissionsAsync(context, logger);
            await SeedAdminAsync(userManager, configuration, logger);
            await GrantAdminAllPermissionsAsync(context, roleManager, logger);
        }

        private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager, ILogger logger)
        {
            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new ApplicationRole { Name = role });
                    if (result.Succeeded)
                    {
                        logger.LogInformation("已建立角色 {Role}", role);
                    }
                    else
                    {
                        logger.LogError("建立角色 {Role} 失敗: {Errors}", role,
                            string.Join("; ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
        }

        private static async Task SeedReferenceDataAsync(ApplicationDbContext context, ILogger logger)
        {
            var existingFunctions = await context.Functions.Select(f => f.Name).ToListAsync();
            var newFunctions = Functions
                .Where(f => !existingFunctions.Contains(f.Name))
                .Select(f => new Function
                {
                    Name = f.Name,
                    CName = f.CName,
                    Sort = f.Sort,
                    FLevel = f.Level,
                    UpperFunction = 0,
                    IsDefault = 1,
                    IsShow = 1,
                    Class = f.Class,
                })
                .ToList();

            var existingActions = await context.FunctionActions.Select(a => a.Name).ToListAsync();
            var newActions = Actions
                .Where(a => !existingActions.Contains(a.Name))
                .Select(a => new FunctionAction { Name = a.Name, CName = a.CName, IsDefault = a.IsDefault })
                .ToList();

            if (newFunctions.Count > 0)
            {
                context.Functions.AddRange(newFunctions);
            }

            if (newActions.Count > 0)
            {
                context.FunctionActions.AddRange(newActions);
            }

            if (newFunctions.Count > 0 || newActions.Count > 0)
            {
                await context.SaveChangesAsync();
                logger.LogInformation("已新增 {Functions} 個畫面與 {Actions} 個功能動作", newFunctions.Count, newActions.Count);
            }

            // 為每個「畫面 x 動作」組合建立權限，權限總覽/角色權限總覽才有東西可以選。
            var functionIds = await context.Functions.ToDictionaryAsync(f => f.Name, f => f);
            var actionIds = await context.FunctionActions.ToDictionaryAsync(a => a.Name, a => a);
            var existingPairs = await context.Permissions
                .Select(p => new { p.FunctionId, p.FunctionActionId })
                .ToListAsync();
            var existingSet = existingPairs.Select(p => (p.FunctionId, p.FunctionActionId)).ToHashSet();

            var newPermissions = new List<Permission>();
            foreach (var (fname, cname, _, _, _) in Functions)
            {
                if (!functionIds.TryGetValue(fname, out var function))
                {
                    continue;
                }

                foreach (var (aname, acname, _) in Actions)
                {
                    if (!actionIds.TryGetValue(aname, out var action) ||
                        existingSet.Contains((function.Id, action.Id)))
                    {
                        continue;
                    }

                    newPermissions.Add(new Permission
                    {
                        Name = $"{cname}-{acname}",
                        FunctionId = function.Id,
                        FunctionActionId = action.Id,
                        CreateTime = DateTime.Now,
                    });
                }
            }

            if (newPermissions.Count > 0)
            {
                context.Permissions.AddRange(newPermissions);
                await context.SaveChangesAsync();
                logger.LogInformation("已新增 {Count} 筆權限定義", newPermissions.Count);
            }
        }

        private static async Task SeedExtraPermissionsAsync(ApplicationDbContext context, ILogger logger)
        {
            if (ExtraActions.Length == 0)
            {
                return;
            }

            var existingActionNames = await context.FunctionActions.Select(a => a.Name).ToListAsync();
            var newActions = ExtraActions
                .Select(e => e.ActionName)
                .Distinct()
                .Where(name => !existingActionNames.Contains(name))
                .Select(name => new FunctionAction
                {
                    Name = name,
                    CName = ExtraActions.First(e => e.ActionName == name).ActionCName,
                    IsDefault = 0,
                })
                .ToList();

            if (newActions.Count > 0)
            {
                context.FunctionActions.AddRange(newActions);
                await context.SaveChangesAsync();
                logger.LogInformation("已新增 {Count} 個專屬功能動作", newActions.Count);
            }

            var functionIds = await context.Functions.ToDictionaryAsync(f => f.Name, f => f);
            var actionIdsByName = await context.FunctionActions.ToDictionaryAsync(a => a.Name, a => a);
            var existingPairs = (await context.Permissions
                .Select(p => new { p.FunctionId, p.FunctionActionId })
                .ToListAsync())
                .Select(p => (p.FunctionId, p.FunctionActionId))
                .ToHashSet();

            var newPermissions = new List<Permission>();
            foreach (var (functionName, actionName, actionCName) in ExtraActions)
            {
                if (!functionIds.TryGetValue(functionName, out var function) ||
                    !actionIdsByName.TryGetValue(actionName, out var action) ||
                    existingPairs.Contains((function.Id, action.Id)))
                {
                    continue;
                }

                newPermissions.Add(new Permission
                {
                    Name = $"{function.CName}-{actionCName}",
                    FunctionId = function.Id,
                    FunctionActionId = action.Id,
                    CreateTime = DateTime.Now,
                });
            }

            if (newPermissions.Count > 0)
            {
                context.Permissions.AddRange(newPermissions);
                await context.SaveChangesAsync();
                logger.LogInformation("已新增 {Count} 筆專屬動作的權限定義", newPermissions.Count);
            }
        }

        private static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager, IConfiguration configuration, ILogger logger)
        {
            var account = configuration["Seed:AdminUserName"] ?? "Admin";
            var password = configuration["Seed:AdminPassword"];

            if (await userManager.FindByNameAsync(account) != null)
            {
                return;
            }

            // 沒有設定密碼就不建立帳號 —— 絕不使用內建的預設密碼，
            // 否則正式環境會出現一個眾所皆知的管理員帳號。
            if (string.IsNullOrWhiteSpace(password))
            {
                logger.LogWarning(
                    "找不到管理員帳號 {Account}，且未設定 Seed:AdminPassword，因此略過建立。" +
                    "請以 dotnet user-secrets 或環境變數 Seed__AdminPassword 設定初始密碼後重新啟動。", account);
                return;
            }

            var user = new ApplicationUser
            {
                UserName = account,
                Email = account,
                EmailConfirmed = true,
                UserLimitData = DateTime.Now.AddYears(100),
            };

            var created = await userManager.CreateAsync(user, password);
            if (!created.Succeeded)
            {
                logger.LogError("建立管理員帳號失敗: {Errors}",
                    string.Join("; ", created.Errors.Select(e => e.Description)));
                return;
            }

            var assigned = await userManager.AddToRoleAsync(user, "Admin");
            if (!assigned.Succeeded)
            {
                logger.LogError("指派 Admin 角色失敗: {Errors}",
                    string.Join("; ", assigned.Errors.Select(e => e.Description)));
                return;
            }

            logger.LogInformation("已建立管理員帳號 {Account}", account);
        }

        private static async Task GrantAdminAllPermissionsAsync(ApplicationDbContext context, RoleManager<ApplicationRole> roleManager, ILogger logger)
        {
            var admin = await roleManager.FindByNameAsync("Admin");
            if (admin == null)
            {
                return;
            }

            var granted = await context.RolePermissions
                .Where(rp => rp.RoleId == admin.Id)
                .Select(rp => rp.PermissionId)
                .ToListAsync();
            var grantedSet = granted.ToHashSet();

            var missing = await context.Permissions
                .Where(p => p.IsDeleted == 0)
                .Select(p => p.Id)
                .ToListAsync();

            var toAdd = missing
                .Where(id => !grantedSet.Contains(id))
                .Select(id => new RolePermission { RoleId = admin.Id, PermissionId = id })
                .ToList();

            if (toAdd.Count > 0)
            {
                context.RolePermissions.AddRange(toAdd);
                await context.SaveChangesAsync();
                logger.LogInformation("已授予 Admin 角色 {Count} 筆權限", toAdd.Count);
            }
        }
    }
}
