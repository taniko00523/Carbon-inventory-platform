using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Carbon_inventory_platform.Models
{
    public class GWP
    {
        [Key]
        public string Name { get; set; }
        public float? Num { get; set; } = null;
        public int GWP_Year { get; set; }

    }
}
