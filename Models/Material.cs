using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carbon_inventory_platform.Models
{
    public class Material
    {
        [Display(Name = "ID")]
        public int Id { get; set; }

        [StringLength(20)]
        [Display(Name = "原燃物料或產品")]
        public string Name { get; set; } = "";

        [StringLength(10)]
        [Display(Name = "類別")]
        public string Scope { get; set; } = "";

        [StringLength(10)]
        [Display(Name = "排放型式")]
        public string EmissionPattern { get; set; } = "";

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "CO2排放係數")]
        public decimal CO2CEF { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "CO2不確定性95%信賴區間下限")]
        public decimal CO2ULL { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "CO2不確定性95%信賴區間上限")]
        public decimal CO2UUL { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "CH4排放係數")]
        public decimal CH4CEF { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "CH4不確定性95%信賴區間下限")]
        public decimal CH4ULL { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "CH4不確定性95%信賴區間上限")]
        public decimal CH4UUL { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "N2O排放係數")]
        public decimal N2OCEF { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "N2O不確定性95%信賴區間下限")]
        public decimal N2OULL { get; set; }=0;

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "N2O不確定性95%信賴區間上限")]
        public decimal N2OUUL { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "HFCS排放係數")]
        public decimal HFCSCEF { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "HFCS不確定性95%信賴區間下限")]
        public decimal HFCSULL { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "HFCS不確定性95%信賴區間上限")]
        public decimal HFCSUUL { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "PFCS排放係數")]
        public decimal PFCSCEF { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "PFCS不確定性95%信賴區間下限")]
        public decimal PFCSULL { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "PFCS不確定性95%信賴區間上限")]
        public decimal PFCSUUL { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "SF6排放係數")]
        public decimal SF6CEF { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "SF6不確定性95%信賴區間下限")]
        public decimal SF6ULL { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "NF3不確定性95%信賴區間上限")]
        public decimal NF3UUL { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "NF3排放係數")]
        public decimal NF3CEF { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "NF3不確定性95%信賴區間下限")]
        public decimal NF3ULL { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        // 原本這裡誤標成「N2O不確定性95%信賴區間上限」，畫面上會顯示成錯誤的欄位名稱
        // （Index 表頭、Create/Edit/Details 的欄位標籤都是靠這個 DisplayName 產生）。
        [Display(Name = "SF6不確定性95%信賴區間上限")]
        public decimal SF6UUL { get; set; } = 0;

        [Display(Name = "排放係數誤差等級")]
        public int CEF_Correction { get; set; } = 3;

        [Display(Name = "活動數據不確定性95%信賴區間上限")]
        [Column(TypeName = "decimal(18, 10)")]

        public decimal DataUUL { get; set; } = 0;

        [Display(Name = "活動數據不確定性95%信賴區間下限")]

        [Column(TypeName = "decimal(18, 10)")]

        public decimal DataULL { get; set; } = 0;


        [Display(Name = "年份")]
        public int Year { get; set; }

        [StringLength(10)]
        [Display(Name = "單位")]
        public string Unit { get; set; } = "";
    }
}
