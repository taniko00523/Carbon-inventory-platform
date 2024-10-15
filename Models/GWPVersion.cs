using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carbon_inventory_platform.Models
{
    public class GWPVersion
    {
        public int Id { get; set; }
        public int Version { get; set; }
        public int Year { get; set; }
        public ICollection<GWP>? GWPs { get; set; }

    }
}
