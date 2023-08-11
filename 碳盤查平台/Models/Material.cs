using System.ComponentModel.DataAnnotations;

namespace 碳盤查平台.Models
{
    public class Material
    {
        [Display(Name ="ID")]
        public int Id { get; set; }

        [MaxLength(20)]
        [Display(Name ="原燃物料或產品")]
        public string Name { get; set; }

        [MaxLength(10)]
        [Display(Name="範疇別")]
        public string Scope { get; set; }

        [MaxLength(10)]
        [Display(Name="排放型式")]
        public string EmissionPattern { get; set; }

        [Display(Name ="CH4排放係數編號")]
        public int CH4Id { get; set; }

        [Display(Name = "CO2排放係數編號")]
        public int CO2Id { get; set; }

        [Display(Name = "N2O排放係數編號")]
        public int N2OId { get; set; }

        [MaxLength(10)]
        [Display(Name = "單位")]
        public string Unit { get; set;}

        //Navigation Property
        public virtual Device Device { get; set; }
        public virtual CH4 CH4 { get; set; }
        public virtual CO2 CO2 { get; set; }
        public virtual N2O N2O { get; set; }
    }
}
