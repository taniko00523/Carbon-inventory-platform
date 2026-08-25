using System.ComponentModel.DataAnnotations;

namespace Carbon_inventory_platform.Models
{
    public class Permission // 單一權限
    {
        public int Id { get; set; }

        [MaxLength(100)]
        [Display(Name = "權限名稱")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "畫面")]
        public int FunctionId { get; set; } // 系統功能

        [Display(Name = "功能")]
        public int FunctionActionId { get; set; }  // 系統功能Action

        public byte IsDeleted { get; set; } = 0;
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public DateTime? ModifyTime { get; set; }
        public DateTime? DeleteTime { get; set; }

        // Navigation property - 必須可為 null，否則 MVC 會把它當成 [Required]，
        // 而表單只會送出 FunctionId / FunctionActionId，導致 ModelState 永遠無效。
        public Function? Function { get; set; }
        public FunctionAction? FunctionAction { get; set; }
        public ICollection<RolePermission>? RolePermissions { get; set; }
        public ICollection<UserPermission>? UserPermissions { get; set; }
    }
}
