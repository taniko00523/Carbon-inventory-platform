using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carbon_inventory_platform.Models
{
    public class Year
    {
        public Guid Id { get; set; }
        public Guid AreaId { get; set; }

        [Display(Name = "公司名稱")]
        public string? Company { get; set; }
        [Range(1,10000)]
        [Display(Name ="盤查年度")]
        [Required(ErrorMessage = "請輸入盤查年度")]
        public int Num { get; set; }
        //類別一各溫室氣體排放量
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Scope1_CO2 { get; set; }= 0;
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Scope1_CH4 { get; set; }= 0;
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Scope1_N2O { get; set; }= 0;
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Scope1_HFCS { get; set; }= 0;
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Scope1_PFCS { get; set; }= 0;
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Scope1_SF6 { get; set; }= 0;
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Scope1_NF3 { get; set; }= 0;

        //類別二各溫室氣體排放量
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Scope2_CO2 { get; set; }= 0;
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Scope2_CH4 { get; set; }= 0;
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Scope2_N2O { get; set; }= 0;
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Scope2_HFCS { get; set; }= 0;
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Scope2_PFCS { get; set; }= 0;
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Scope2_SF6 { get; set; }= 0;
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Scope2_NF3 { get; set; }= 0;

        //各溫室氣體排放量

        [Column(TypeName = "decimal(18, 4)")]
        public decimal CO2 { get; set; }= 0;
        [Column(TypeName = "decimal(18, 4)")]
        public decimal CH4 { get; set; }= 0;
        [Column(TypeName = "decimal(18, 4)")]
        public decimal N2O { get; set; }= 0;
        [Column(TypeName = "decimal(18, 4)")]
        public decimal HFCS { get; set; }= 0;
        [Column(TypeName = "decimal(18, 4)")]
        public decimal PFCS { get; set; }= 0;
        [Column(TypeName = "decimal(18, 4)")]
        public decimal SF6 { get; set; }= 0;
        [Column(TypeName = "decimal(18, 4)")]
        public decimal NF3 { get; set; }= 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal percentage1_CO2 { get; set; }= 0;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal percentage1_CH4 { get; set; }= 0;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal percentage1_N2O { get; set; }= 0;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal percentage1_HFCS { get; set; }= 0;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal percentage1_PFCS { get; set; }= 0;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal percentage1_SF6 { get; set; }= 0;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal percentage1_NF3 { get; set; }= 0;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal percentage2_CO2 { get; set; }= 0;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal percentage2_CH4 { get; set; }= 0;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal percentage2_N2O { get; set; }= 0;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal percentage2_HFCS { get; set; }= 0;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal percentage2_PFCS { get; set; }= 0;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal percentage2_SF6 { get; set; }= 0;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal percentage2_NF3 { get; set; }= 0;
        [Column(TypeName = "decimal(18, 4)")]
        public decimal non_move { get; set; }= 0;
        [Column(TypeName = "decimal(18, 4)")]
        public decimal move { get; set; }= 0;
        [Column(TypeName = "decimal(18, 4)")]
        public decimal process { get; set; }= 0;
        [Column(TypeName = "decimal(18, 4)")]
        public decimal escape { get; set; }= 0;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal percentage_nonMove { get; set; }= 0;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal percentage_Move { get; set; }= 0;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal percentage_Process { get; set; }= 0;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal percentage_Escape { get; set; }= 0;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal percentage_Scope1 { get; set; }= 0;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal percentage_Scope2 { get; set; } = 0;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Scope1 { get; set; }= 0;
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Scope2 { get; set; }= 0;

        [Column(TypeName = "decimal(18, 3)")]
        [Display(Name = "總排放量")]
        public decimal All { get; set; }= 0;
        [Column(TypeName = "decimal(18, 4)")]
        public decimal cal_all { get; set; }= 0;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal percentage_CalAll { get; set; }= 0;

        public int no1_Grade { get; set; }= 0;
        public int no2_Grade { get; set; }= 0;
        public int no3_Grade { get; set; }= 0;
        public float avg_Grade { get; set; }= 0;
        public string all_Grade { get; set; } = "";
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ULL { get; set; }= 0;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal UUL { get; set; }= 0;

        [Display(Name = "是否刪除")]
        public byte isDeleted { get; set; } = 0;

        [Display(Name = "建立時間")]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        [Display(Name = "修改時間")]
        public DateTime? ModifiedTime { get; set; }

        [Display(Name = "刪除時間")]
        public DateTime? DeleteTime { get; set; }

        //Navigation Property
        [ForeignKey("AreaId")]
        public Area? Area { get; set; }
        public ICollection<Device>? Devices { get; set; }

    }
}
