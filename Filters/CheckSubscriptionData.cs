using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Carbon_inventory_platform.Filters
{
    public class CheckSubscriptionData : Attribute, IAuthorizationFilter
    {
        private readonly string _renewalUrl = "/Home/Index"; // 續費頁面的 URL

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // 獲取用戶管理器
            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();

            // 獲取當前用戶
            var user = userManager.GetUserAsync(context.HttpContext.User).Result;

            // 檢查使用期限
            if (user != null && user.Email != "Admin")
            {
                if (user.UserLimitData.HasValue)
                {
                    if (user.UserLimitData < DateTime.UtcNow)
                    {
                        // 如果使用期限已過，則重定向到續費頁面
                        context.Result = new RedirectResult(_renewalUrl);
                    }
                }
                else
                {
                    // 如果沒有設定使用期限，則重定向到續費頁面
                    context.Result = new RedirectResult(_renewalUrl);
                }
            }
        }
    }
}
