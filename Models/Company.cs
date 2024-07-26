using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carbon_inventory_platform.Models
{
    public class Company
    {
        [Display(Name = "ID")]
        public Guid Id { get; set; }

        public string UserId { get; set; } // 外來鍵屬性

        [MaxLength(100)]
        [Display(Name = "公司名稱")]
        [Required(ErrorMessage = "請填寫公司名稱")]
        public string Name { get; set; } = "";

        [MaxLength(100)]
        [Display(Name = "公司簡稱")]
        public string? EasyName { get; set; } = "";

        [MaxLength(100)]
        [Display(Name = "英文公司名稱")]
        [Required(ErrorMessage = "請填寫英文公司名稱")]
        public string EnglishName { get; set; } = "";

        [MaxLength(100)]
        [Display(Name = "英文公司簡稱")]
        public string? EasyEnglishName { get; set; } = "";

        [MaxLength(20)]
        [Display(Name = "聯絡人")]
        [Required(ErrorMessage = "請填寫聯絡人")]
        public string ContactName { get; set; } = "";

        [EmailAddress(ErrorMessage = "電子信箱格式錯誤")]
        [MaxLength(50)]
        [Display(Name = "電子信箱")]
        [Required(ErrorMessage = "請填寫電子信箱")]
        public string Email { get; set; } = "";

        [MaxLength(20)]
        [Display(Name = "電話號碼")]
        [Required(ErrorMessage = "請填寫手機號碼")]
        public string Phone { get; set; } = "";

        [Display(Name = "公司簡介")]
        public string? CompanyInformation { get; set; }

        [Display(Name = "組織邊界")]
        public string? AddressInformation { get; set; }

        [Display(Name = "報告邊界")]
        public string? ReportingInformation { get; set; }

        [Display(Name = "溫室氣體")]
        public string? GHGInformation { get; set; }

        [Display(Name = "直接溫室氣體")]
        public string? Scope1Information { get; set; }

        [Display(Name = "間接溫室氣體")]
        public string? Scope2Information { get; set; }
        [Display(Name = "前言")]
        public string? ReportOpening { get; set; }

        [Display(Name = "報告用途")]
        public string? ReportingPurposes { get; set; }
        [Display(Name = "是否刪除")]
        public byte isDeleted { get; set; } = 0;

        [Display(Name = "建立時間")] 
        public DateTime CreateTime { get; set; }

        [Display(Name = "修改時間")]
        public DateTime? ModifiedTime { get; set; }

        [Display(Name = "刪除時間")]
        public DateTime? DeleteTime { get; set; }

        //Navigation導覽屬性
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } // 外來鍵導覽屬性
        public ICollection<Area> Areas { get; set; }
        public Company()
        {
            Id = Guid.NewGuid();
            Areas = new List<Area>();
        }
    }
}
