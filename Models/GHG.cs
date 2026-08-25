using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carbon_inventory_platform.Models
{
    public class GHG
    {
        public Guid Id {  get; set; }

        public Guid DeviceId { get; set; }

        [MaxLength(20)]
        [Display(Name = "溫室氣體種類")]
        public string Name { get; set; } = string.Empty;

        // 與來源欄位 GWP.Num 一致；原本是 decimal(18,2)，會把小數位截掉。
        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "GWP值")]
        public decimal GWP { get; set; }

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "排放係數")]
        public decimal CEF { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "單一溫室氣體信賴區間上限")]
        public decimal all_UUL { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "單一溫室氣體信賴區間下限")]
        public decimal all_ULL { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "溫室氣體排放係數信賴區間上限")]
        public decimal CEF_UUL { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "溫室氣體排放係數信賴區間下限")]
        public decimal CEF_ULL { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "排放量")]
        public decimal Emission { get; set; } = 0;

        // 生命週期紀錄
        [Display(Name = "是否刪除")]
        public byte isDeleted { get; set; } = 0;

        [Display(Name = "建立時間")]
        public DateTime CreateTime { get; set; }

        [Display(Name = "修改時間")]
        public DateTime? ModifiedTime { get; set; }

        [Display(Name = "刪除時間")]
        public DateTime? DeleteTime { get; set; }

        //Navigation Property
        [ForeignKey("DeviceId")]
        public Device? Device{ get; set; }
    }
}
