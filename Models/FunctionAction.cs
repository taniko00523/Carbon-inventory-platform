using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carbon_inventory_platform.Models
{
    public class FunctionAction // 系統功能Action
    {
        public int Id { get; set; }
        [Display(Name = "Action名稱")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [Display(Name = "Action中文名稱")]
        [MaxLength(100)]
        public string CName { get; set; } = string.Empty;
        [Display(Name = "是否為預設功能")]
        public byte IsDefault { get; set; } = 0;
    }
}
