using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Carbon_inventory_platform.Models
{
    public class GWP
    {
        public int Id { get; set; }

        // 必須有長度限制，否則會被對應成 nvarchar(max)，SQL Server 無法建立索引。
        [MaxLength(20)]
        [Display(Name = "溫室氣體種類")]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18, 10)")]
        public decimal Num { get; set; } = 0;
        public int ARVersion { get; set; }
        [MaxLength(50)]
        [Display(Name = "資料來源")]
        public string? Source { get; set; } = "MOE"; //先預設為環境部

        //[ForeignKey("ARVersion")]
        //public GWPVersion? GWPVersion { get; set; }
    }
}
