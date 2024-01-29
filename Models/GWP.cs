using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Carbon_inventory_platform.Models
{
    public class GWP
    {
        [Key]
        public string Name { get; set; }

        [Column(TypeName = "decimal(18, 10)")]
        public decimal Num { get; set; } = 0;
        public int GWP_Year { get; set; }

    }
}
