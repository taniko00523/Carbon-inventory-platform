using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Carbon_inventory_platform.Models
{
    public class GWP
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Column(TypeName = "decimal(18, 10)")]
        public decimal Num { get; set; } = 0;
        public int ARCount { get; set; }

    }
}
