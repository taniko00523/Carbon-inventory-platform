using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Carbon_inventory_platform.Filters
{
    public class CheckSubscriptionData : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // 獲取用戶管理器
            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();

            // 獲取當前用戶
            var user = userManager.GetUserAsync(context.HttpContext.User).Result;

            // 檢查使用期限
            if(user != null && user.Email != "Admin")
            {
                if (user.UserLimitData.HasValue)
                {
                    if (user.UserLimitData < DateTime.UtcNow)
                    {
                        //如果小於存取時間，則禁止存取
                        context.Result = new ForbidResult();
                    }
                }
                else // 如果沒有使用時間限制，則直接禁止存取
                {
                    context.Result = new ForbidResult();
                }
            }

        }
    }
}
