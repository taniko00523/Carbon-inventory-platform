using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carbon_inventory_platform.Models
{
    public class Permission // 單一權限
    {
        public int Id { get; set; }
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


        // Navigation property
        public ICollection<RolePermission>? RolePermissions { get; set; }
        [ForeignKey("FunctionId")]
        public required Function Function { get; set; }
        [ForeignKey("FunctionActionId")]
        public required FunctionAction FunctionAction { get; set; }
    }

}