using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carbon_inventory_platform.Models
{
    public class DefaultDevices
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(20)]
        [Display(Name = "排放源名稱")]
        public string Name { get; set; } = "";

        [MaxLength(20)]
        [Display(Name = "原燃物料")]
        public string Material { get; set; } = "";

        [MaxLength(10)]
        [Display(Name = "類別")]
        public string Scope { get; set; } = "";

        [MaxLength(10)]
        [Display(Name = "排放型式")]
        public string EmissionPattern { get; set; } = "";

        [MaxLength(10)]
        [Display(Name = "產業別")]
        public string Type { get; set; } = "";
    }
}
