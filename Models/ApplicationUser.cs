using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Carbon_inventory_platform.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "使用期限")]
        public DateTime? UserLimitData { get; set; }

        // 添加自訂的屬性
    }
}


