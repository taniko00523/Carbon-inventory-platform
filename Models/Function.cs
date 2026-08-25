using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carbon_inventory_platform.Models
{
    public class Function // 系統功能
    {
        public int Id { get; set; }
        [Display(Name = "Controller名稱")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [Display(Name = "Controller中文名稱")]
        [MaxLength(100)]
        public string CName { get; set; } = string.Empty;
        [Display(Name = "選單的階層")]
        public int? FLevel { get; set; }  // 選單的階層-主選單為1，第二層選單為2
        [Display(Name = "上一層選單")]
        public int UpperFunction { get; set; }  // 上一層選單的FunctionSN，預設為0
        [Display(Name = "功能選單排序")]
        public int Sort { get; set; }  // 功能選單排序
        [Display(Name = "是否為系統預設權限")]
        public byte? IsDefault { get; set; } = 0;  // 是否為系統預設權限-0:否,1:是
        [Display(Name = "是否為顯示")]
        public byte? IsShow { get; set; } = 1; // 是否為顯示-0:否,1:是
        [Display(Name = "功能類型")]
        public int? Class { get; set; }  // 功能類型-1:後台,2:前台
    }
}
