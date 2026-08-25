using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Filters;
using Carbon_inventory_platform.Models;
using Carbon_inventory_platform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'DefaultConnection' not found. " +
        "請以環境變數 ConnectionStrings__DefaultConnection、Azure App Service 連線字串設定，" +
        "或 dotnet user-secrets 提供資料庫連線字串。");
}
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false) //Email驗證關閉
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// B6：用真正會寄信的 SmtpEmailSender 取代 Identity 預設的 no-op 實作（AddDefaultIdentity 內部用
// TryAddTransient 註冊 no-op，這裡的 AddTransient 會覆蓋掉它）。Email:Host 沒設定時安靜地不寄，
// 跟現有 LINE 通知的作法一致。正式環境請用 dotnet user-secrets 或環境變數 Email__Password 等提供密碼。
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();

builder.Services.AddControllersWithViews(options =>
{
    // 所有 POST/PUT/DELETE 一律驗證防偽 Token。
    // 之前是逐個 Action 加 [ValidateAntiForgeryToken]，而最危險的兩個
    // (Devices/ImportDeviceExcel、Feedbacks/SubmitFeedback) 剛好被漏掉。
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

builder.Services.AddAuthorization(options =>
{
    // 預設「拒絕」：沒有標註授權屬性的 Controller 一律需要登入。
    // 之前有 8 個 scaffold 出來的 Controller（含 Permissions / RolePermissions /
    // Materials）完全沒有 [Authorize]，任何人都能讀寫刪除。
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// ApplicationDbContext 的稽核軌跡（B4）需要透過 IHttpContextAccessor 取得目前登入者與 IP。
builder.Services.AddHttpContextAccessor();
// CountController 快取 GWP 清單用（A3），避免匯入排放源時每一筆都重新查一次幾乎不變動的主檔。
builder.Services.AddMemoryCache();
// A9：/healthz 健康檢查，上線後至少能知道資料庫連不連得上。
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();
// A9：結構化日誌／APM。沒有設定連線字串時 SDK 只是安靜地不送遙測（不會拋例外、不影響啟動），
// 跟現有 LINE Token 沒設定就安靜停用的作法一致。正式環境請用
// dotnet user-secrets 或環境變數 ApplicationInsights__ConnectionString 提供連線字串。
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddScoped<RolePermissionService>();
builder.Services.AddScoped<CompanyOwnershipService>();
builder.Services.AddScoped<AreaTrendService>();
builder.Services.AddScoped<AreaLockService>();
// PermissionFilterAttribute 建構子依賴 UserManager/RoleManager/RolePermissionService，
// 必須透過 [ServiceFilter(typeof(PermissionFilterAttribute))] 由 DI 容器建立，故需在此註冊。
builder.Services.AddScoped<PermissionFilterAttribute>();
builder.Services.AddHttpClient();

builder.Services.Configure<IdentityOptions>(options =>
{
    //取消對Email格式的限制
    options.User.RequireUniqueEmail = false;

    // Default Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); //發生鎖定時，使用者遭到鎖定的時長。
    options.Lockout.MaxFailedAccessAttempts = 5; //使用者遭到鎖定之前允許的存取嘗試失敗次數上限 (如果已啟用鎖定)。
    options.Lockout.AllowedForNewUsers = true; //判斷是否可以鎖定新的使用者。

    // 密碼格式：長度可用 Identity:Password:RequiredLength 設定覆寫，
    // 但不再允許 1 個字元的密碼（原設定讓任何帳號都能被瞬間猜中）。
    options.Password.RequireDigit = false; //密碼中需要介於 0-9 之間的數位(數字)。
    options.Password.RequireLowercase = false; //密碼中需要小寫字元。
    options.Password.RequireNonAlphanumeric = false; //密碼中需要非英數字元。
    options.Password.RequireUppercase = false; //密碼中需要大寫字元。
    options.Password.RequiredLength = builder.Configuration.GetValue<int?>("Identity:Password:RequiredLength") ?? 8;
    options.Password.RequiredUniqueChars = 1; //需要密碼中的相異字元數。

    // Default SignIn settings.
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false; //需要確認的電子郵件才能登入。
    options.SignIn.RequireConfirmedPhoneNumber = false; //需要確認的電話號碼才能登入。
});

// 固定使用 zh-TW 格式化與剖析數字/日期。Linux 容器預設是 InvariantCulture，
// 會讓民國年與小數點的顯示在本機與雲端不一致。
var supportedCultures = new[] { new CultureInfo("zh-TW") };
builder.Services.Configure<Microsoft.AspNetCore.Builder.RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("zh-TW");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRequestLocalization();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// A9：全域「預設要登入」的授權原則對 Endpoint Routing 也生效，這裡要明確允許匿名存取，
// 否則監控系統打 /healthz 只會拿到 302 轉去登入頁，而不是真正的健康狀態。
app.MapHealthChecks("/healthz").AllowAnonymous();

// 報表輸出目錄必須存在，否則第一次產生報表會拋 DirectoryNotFoundException
// （wwwroot/output 在 csproj 裡只是個空資料夾宣告，git 不會保留空目錄）。
Directory.CreateDirectory(Path.Combine(app.Environment.WebRootPath, "output"));

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        if (app.Environment.IsDevelopment())
        {
            await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.MigrateAsync();
        }

        await DbSeeder.SeedAsync(scope.ServiceProvider, app.Configuration, logger);
    }
    catch (Exception ex)
    {
        // 種子資料失敗不應該讓整個網站起不來，但一定要留下紀錄。
        logger.LogError(ex, "初始化基礎資料時發生錯誤");
    }
}

app.Run();
