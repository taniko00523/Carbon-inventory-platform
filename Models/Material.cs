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

        [Display(Name = "CO2排放係數")]
        public float? CO2CEF { get; set; }

        [Display(Name = "CO2不確定性95%信賴區間下限")]
        public float? CO2ULL { get; set; }

        [Display(Name = "CO2不確定性95%信賴區間上限")]
        public float? CO2UUL { get; set; }

        [Display(Name = "CH4排放係數")]
        public float? CH4CEF { get; set; }

        [Display(Name = "CH4不確定性95%信賴區間下限")]
        public float? CH4ULL { get; set; }

        [Display(Name = "CH4不確定性95%信賴區間上限")]
        public float? CH4UUL { get; set; }

        [Display(Name = "N2O排放係數")]
        public float? N2OCEF { get; set; }

        [Display(Name = "N2O不確定性95%信賴區間下限")]
        public float? N2OULL { get; set; }

        [Display(Name = "N2O不確定性95%信賴區間上限")]
        public float? N2OUUL { get; set; }

        [Display(Name = "年份")]
        public int Year { get; set; }

        [MaxLength(10)]
        [Display(Name = "單位")]
        public string Unit { get; set; } = "";

        //Navigation Property
        public ICollection<Device> Devices { get; set; } = null!;
    }
}
