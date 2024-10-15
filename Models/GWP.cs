using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Carbon_inventory_platform.Models
{
    public class GWP
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18, 10)")]
        public decimal Num { get; set; } = 0;
        public int ARVersion { get; set; }
        public string? Source { get; set; } = "MOE"; //先預設為環境部

        //[ForeignKey("ARVersion")]
        //public GWPVersion? GWPVersion { get; set; }
    }
}
