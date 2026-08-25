using System.ComponentModel.DataAnnotations;

namespace Carbon_inventory_platform.Models
{
    /// <summary>使用者的個別權限（覆寫或補充角色權限）。</summary>
    public class UserPermission
    {
        public int Id { get; set; }

        [Display(Name = "使用者")]
        public string UserId { get; set; } = string.Empty;

        [Display(Name = "權限")]
        public int PermissionId { get; set; }

        // 導航屬性：一筆 UserPermission 對應「一個」使用者與「一個」權限。
        public ApplicationUser? User { get; set; }
        public Permission? Permission { get; set; }
    }
}
