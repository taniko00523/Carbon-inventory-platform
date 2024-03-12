using Carbon_inventory_platform.Data;
using ElectronNET.API;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseElectron(args);

// Is optional, but you can use the Electron.NET API-Classes directly with DI (relevant if you wont more encoupled code)
builder.Services.AddElectron();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false) //Email驗證關閉
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.Configure<IdentityOptions>(options =>
{
    //取消對Email格式的限制
    options.User.RequireUniqueEmail = false;

    // Default Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); //發生鎖定時，使用者遭到鎖定的時長。
    options.Lockout.MaxFailedAccessAttempts = 5; //使用者遭到鎖定之前允許的存取嘗試失敗次數上限 (如果已啟用鎖定)。
    options.Lockout.AllowedForNewUsers = true; //判斷是否可以鎖定新的使用者。

    // 預設的密碼格式
    options.Password.RequireDigit = false; //密碼中需要介於 0-9 之間的數位(數字)。
    options.Password.RequireLowercase = false; //密碼中需要小寫字元。	
    options.Password.RequireNonAlphanumeric = false; //密碼中需要非英數字元。	
    options.Password.RequireUppercase = false; //密碼中需要大寫字元。	
    options.Password.RequiredLength = 6; //密碼長度下限。
    options.Password.RequiredUniqueChars = 1; //需要密碼中的相異字元數。

    // Default SignIn settings.
    options.SignIn.RequireConfirmedAccount = false; 
    options.SignIn.RequireConfirmedEmail = false; //需要確認的電子郵件才能登入。
    options.SignIn.RequireConfirmedPhoneNumber = false; //需要確認的電話號碼才能登入。
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

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

//await app.StartAsync();

//// Open the Electron-Window here
//await Electron.WindowManager.CreateWindowAsync();

//app.WaitForShutdown();

using (var scope = app.Services.CreateScope())
{
    // setting initial data in system, Role, Account...
    // Role:
    var roleManager =
        scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "Manager", "User"};

    foreach (var role in roles)
    {
        if(!await roleManager.RoleExistsAsync(role)) // 如果角色不存在
        {
            // 建立角色
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

using (var scope = app.Services.CreateScope())
{
    // setting initial data in system, Role, User...
    // User:
    var UserManager =
        scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string account = "Admin"; //新增一個預設的管理員帳號
    string password = "!Admin1234";

    if(await UserManager.FindByEmailAsync(account) == null) // 如果Admin 帳號不存在
    {
        // 建立一個Admin用戶
        var user = new IdentityUser();
        user.UserName = account;
        user.Email = account;

        // 新增至資料庫
        await UserManager.CreateAsync(user, password);

        // 賦予Admin身分
        await UserManager.AddToRoleAsync(user, "Admin");    
    }
}

app.Run();

