using System.ComponentModel.DataAnnotations;

namespace Carbon_inventory_platform.Models
{
    /// <summary>角色與權限的對應（多對多的中介實體）。</summary>
    public class RolePermission
    {
        public int Id { get; set; }

        [Display(Name = "角色")]
        public string RoleId { get; set; } = string.Empty;

        [Display(Name = "權限")]
        public int PermissionId { get; set; }

        // 導航屬性：一筆 RolePermission 對應「一個」角色與「一個」權限。
        public ApplicationRole? Role { get; set; }
        public Permission? Permission { get; set; }
    }
}
