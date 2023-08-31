using System.ComponentModel.DataAnnotations;

namespace Carbon_inventory_platform.Models
{
    public class Material
    {
        [Display(Name = "ID")]
        public int Id { get; set; }

        [MaxLength(20)]
        [Display(Name = "原燃物料或產品")]
        public string Name { get; set; } = "";

        [MaxLength(10)]
        [Display(Name = "範疇別")]
        public string Scope { get; set; } = "";

        [MaxLength(10)]
        [Display(Name = "排放型式")]
        public string EmissionPattern { get; set; } = "";

        [Display(Name = "CH4排放係數編號")]
        public int? CH4Id { get; set; }

        [Display(Name = "CO2排放係數編號")]
        public int? CO2Id { get; set; }

        [Display(Name = "N2O排放係數編號")]
        public int? N2OId { get; set; }

        [MaxLength(10)]
        [Display(Name = "單位")]
        public string Unit { get; set; } = "";

        //Navigation Property
        public ICollection<Device> Devices { get; set; } = null!;
        public CH4 CH4 { get; set; } = null!;
        public CO2 CO2 { get; set; } = null!;
        public N2O N2O { get; set; } = null!;
    }
}
