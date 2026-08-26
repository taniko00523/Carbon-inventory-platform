using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Carbon_inventory_platform.Models
{
    public class GWP
    {
        public int Id { get; set; }

        // 必須有長度限制，否則會被對應成 nvarchar(max)，SQL Server 無法建立索引。
        [StringLength(20)]
        [Display(Name = "溫室氣體種類")]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "GWP值")]
        // 畫面上的 min="0" 只是前端提示，原本沒有對應的伺服器端限制，直接組 POST 還是能存負值。
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "GWP值不能是負數。")]
        public decimal Num { get; set; } = 0;
        [Display(Name = "AR 版本")]
        public int ARVersion { get; set; }
        [StringLength(50)]
        [Display(Name = "資料來源")]
        public string? Source { get; set; } = "MOE"; //先預設為環境部

        //[ForeignKey("ARVersion")]
        //public GWPVersion? GWPVersion { get; set; }
    }
}
