using System.ComponentModel.DataAnnotations.Schema;

namespace Carbon_inventory_platform.Models
{
    public class UserPermission
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int PermissionId { get; set; }

        // 導航屬性
        [ForeignKey("UserId")]
        public required ICollection<ApplicationUser> Users { get; set; }
        [ForeignKey("PermissionId")]
        public required ICollection<Permission> Permissions { get; set; }
    }
}