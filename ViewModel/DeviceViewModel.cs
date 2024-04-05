using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carbon_inventory_platform.ViewModel
{
    public class DeviceViewModel
    {
        [Display(Name = "ID")]
        public Guid Id { get; set; }

        [Display(Name = "盤查年度")]
        public Guid YearId { get; set; }

        [MaxLength(20)]
        [Display(Name = "排放源編號")]
        public string? AssetNo { get; set; }

        [MaxLength(20)]
        [Display(Name = "排放源名稱")]
        public string Name { get; set; }

        [MaxLength(20)]
        [Display(Name = "自訂排放源名稱")]
        public string? OtherName { get; set; }

        [MaxLength(20)]
        [Display(Name = "製程")]
        public string? Provess { get; set; }

        [MaxLength(10)]
        [Display(Name = "類別")]
        public string Scope { get; set; }

        [MaxLength(10)]
        [Display(Name = "排放型式")]
        public string EmissionPattern { get; set; }

        [Display(Name = "原燃物料")]
        public string Material { get; set; }

        [Display(Name = "自訂排放係數")]
        public bool Customize { get; set; } = false;

        [Display(Name = "CO2排放係數")]
        [Column(TypeName = "decimal(18, 10)")]
        public decimal? CO2CEF { get; set; }

        [Display(Name = "CH4排放係數")]
        [Column(TypeName = "decimal(18, 10)")]
        public decimal? CH4CEF { get; set; }

        [Display(Name = "N2O排放係數")]
        [Column(TypeName = "decimal(18, 10)")]
        public decimal? N2OCEF { get; set; }

        [Display(Name = "HFCS排放係數")]
        [Column(TypeName = "decimal(18, 10)")]
        public decimal? HFCSCEF { get; set; }

        [Display(Name = "PFCS排放係數")]
        [Column(TypeName = "decimal(18, 10)")]
        public decimal? PFCSCEF { get; set; }

        [Display(Name = "SF6排放係數")]
        [Column(TypeName = "decimal(18, 10)")]
        public decimal? SF6CEF { get; set; }

        [Display(Name = "NF3排放係數")]
        [Column(TypeName = "decimal(18, 10)")]
        public decimal? NF3CEF { get; set; } 

        //[Display(Name = "排放係數誤差等級")]
        //public int CEF_Correction { get; set; } = 3;
    }
}
