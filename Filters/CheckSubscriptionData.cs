using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Carbon_inventory_platform.Filters
{
    /// <summary>
    /// 檢查登入使用者的使用期限；已過期就導向「訂閱已過期」頁面。
    ///
    /// 修正的問題：
    /// 1. 原本導向 /Home/Index，而 HomeController 本身也掛著這個 Filter，
    ///    造成任何沒有設定使用期限的使用者一進站就無限重導 (ERR_TOO_MANY_REDIRECTS)。
    ///    正確的目標是 Home/OutOfLimitTime（該 View 一直存在，只是從來沒被用到）。
    /// 2. 原本用 .Result 同步等待非同步呼叫，在 ASP.NET Core 上有死結風險，
    ///    且每個請求都會多打一次資料庫。改用 IAsyncAuthorizationFilter。
    /// 3. 原本用 user.Email != "Admin" 這個魔術字串判斷管理員；Email 是使用者自己
    ///    可以改的欄位，等於讓使用者自行繞過檢查。改用 Admin 角色。
    /// 4. 原本沒有設定使用期限 (null) 就直接視為過期，導致新建立的帳號完全無法使用。
    ///    改為 null 視為「未設限」。
    /// </summary>
    public class CheckSubscriptionData : Attribute, IAsyncAuthorizationFilter
    {
        private const string FallbackController = "Home";
        private const string FallbackAction = "OutOfLimitTime";

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // 匿名頁面不檢查。
            if (context.HttpContext.User?.Identity?.IsAuthenticated != true)
            {
                return;
            }

            // 已經在目標頁（或錯誤頁）就不要再導一次，否則就是無限迴圈。
            var routeValues = context.ActionDescriptor.RouteValues;
            if (string.Equals(routeValues["controller"], FallbackController, StringComparison.OrdinalIgnoreCase) &&
                (string.Equals(routeValues["action"], FallbackAction, StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(routeValues["action"], "Error", StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.GetUserAsync(context.HttpContext.User);
            if (user == null)
            {
                return;
            }

            // 管理員不受使用期限限制。
            if (await userManager.IsInRoleAsync(user, "Admin"))
            {
                return;
            }

            // 沒有設定期限 = 不限制；有設定就比對（UserLimitData 以本地時間寫入，
            // 因此要跟 DateTime.Now 比，原本跟 UtcNow 比在台灣會提早 8 小時失效）。
            if (user.UserLimitData.HasValue && user.UserLimitData.Value < DateTime.Now)
            {
                context.Result = new RedirectToActionResult(FallbackAction, FallbackController, null);
            }
        }
    }
}
