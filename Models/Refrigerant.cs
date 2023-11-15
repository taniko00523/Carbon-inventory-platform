using System.ComponentModel.DataAnnotations;

namespace Carbon_inventory_platform.Models
{
    public class Refrigerant
    {
        [Key]
        public string Name { get; set; }
        public double Num { get; set; }

    }
}
