using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carbon_inventory_platform.Models
{
    public class Device
    {
        [Display(Name = "ID")]
        public Guid Id { get; set; }

        // 排放源基本資料：

        [Display(Name = "盤查年度")]
        public Guid YearId { get; set; }


        [MaxLength(20)]
        [Display(Name = "排放源名稱")]
        public required string Name { get; set; } = "";

        [MaxLength(10)]
        [Display(Name = "類別")]
        public required string Scope { get; set; } = "";

        [MaxLength(10)]
        [Display(Name = "排放型式")]
        public required string EmissionPattern { get; set; } = "";

        [Display(Name = "原燃物料")]
        public required string Material { get; set; } = "";

        // 不一定有的基本資料：

        [MaxLength(20)]
        [Display(Name = "排放源編號")]
        public string? AssetNo { get; set; }

        [MaxLength(20)]
        [Display(Name = "製程")]
        public string? Provess { get; set; }

        [MaxLength(20)]
        [Display(Name = "數據來源名稱")]
        public string? Source { get; set; }

        [MaxLength(20)]
        [Display(Name = "保存單位")]
        public string? Dept { get; set; }

        // 排放源計算過程
        [Column(TypeName = "decimal(18, 4)")]
        [Display(Name = "活動數據")]
        [Required(ErrorMessage = "請填寫活動數據")]
        public decimal Num { get; set; } = 0;

        [MaxLength(20)]
        [Display(Name = "單位")]
        public string? Unit { get; set; }

        [Display(Name = "活動數據誤差等級")]
        public int Data_Correction { get; set; } = 3;

        [Display(Name = "儀器校正等級")]
        public int Device_Correction { get; set; } = 3;

        [Display(Name = "排放係數誤差等級")]
        public int CEF_Correction { get; set; } = 3;

        [Display(Name = "數據等級評分")]
        public int Grade { get; set; } = 27;

        [Column(TypeName = "decimal(18, 4)")]
        [Display(Name = "排放量")]
        public decimal Emissions { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "活動數據信賴區間上限")]
        public decimal data_UUL { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        [Display(Name = "活動數據信賴區間下限")]
        public decimal data_ULL { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        public decimal all_UUL { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        public decimal all_ULL { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        public decimal count_UUL { get; set; } = 0;

        [Column(TypeName = "decimal(18, 10)")]
        public decimal count_ULL { get; set; } = 0;

        // 生命週期紀錄
        [Display(Name = "是否刪除")]
        public byte isDeleted { get; set; } = 0;

        [Display(Name = "建立時間")]
        public DateTime CreateTime { get; set; }

        [Display(Name = "修改時間")]
        public DateTime? ModifiedTime { get; set; }

        [Display(Name = "刪除時間")]
         public DateTime? DeleteTime { get; set; }

        //Navigation Property
        [ForeignKey("YearId")]
        public Year? Year { get; set; }
        public ICollection<GHG>? GHGs { get; set; } = null!;

    }
}
