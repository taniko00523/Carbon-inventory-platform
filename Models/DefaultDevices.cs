using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Carbon_inventory_platform.Models
{
    public class DefaultDevices
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {  get; set; }
        public string Name { get; set; }
        public string Material { get; set; }
        public string Scope { get; set; }
        public string EmissionPattern { get; set; }
        public string Type { get; set; } = "";
    }
}
