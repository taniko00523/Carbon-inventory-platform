using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carbon_inventory_platform.Models
{
    public class Refrigerant
    {
        [Key]
        public string Name { get; set; }

        [Column(TypeName = "decimal(18, 10)")]
        public decimal Num { get; set; }

    }
}
