using System.ComponentModel.DataAnnotations.Schema;

namespace Carbon_inventory_platform.Models
{
    public class RolePermission
    {
        public int Id { get; set; }
        public string RoleId { get; set; } = string.Empty;
        public int PermissionId { get; set; }

        // 導航屬性
        [ForeignKey("RoleId")]
        public ICollection<ApplicationRole>? Roles { get; set; }
        [ForeignKey("PermissionId")]
        public ICollection<Permission>? Permissions { get; set; }
    }
}