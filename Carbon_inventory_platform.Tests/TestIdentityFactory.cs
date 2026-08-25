using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Carbon_inventory_platform.Tests;

/// <summary>
/// CompanyOwnershipService 依賴 UserManager&lt;ApplicationUser&gt;.GetUserId(ClaimsPrincipal) 取得使用者 Id，
/// 這個方法本身只是讀取 ClaimTypes.NameIdentifier 這個 claim，但它是 UserManager 的實例方法，
/// 必須真的建立一個 UserManager 才能呼叫——這裡用「指向同一個 InMemory 資料庫的 UserStore」建立一個
/// 最小可用的 UserManager，不需要密碼雜湊、驗證器等其他 Identity 功能。
/// </summary>
internal static class TestIdentityFactory
{
    public static UserManager<ApplicationUser> CreateUserManager(ApplicationDbContext context)
    {
        var store = new UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, string>(context);
        return new UserManager<ApplicationUser>(
            store,
            Options.Create(new IdentityOptions()),
            new PasswordHasher<ApplicationUser>(),
            Array.Empty<IUserValidator<ApplicationUser>>(),
            Array.Empty<IPasswordValidator<ApplicationUser>>(),
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            null!,
            NullLogger<UserManager<ApplicationUser>>.Instance);
    }

    /// <summary>建立一個帶有 NameIdentifier 與（可選）角色 claim 的登入者，供服務層測試模擬 User。</summary>
    public static ClaimsPrincipal MakePrincipal(string userId, params string[] roles)
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
    }
}
