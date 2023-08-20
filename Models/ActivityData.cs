using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Carbon_inventory_platform.Models
{
    public class ActivityData 
    {
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "活動數據")]
        public float Num { get; set; }

        [MaxLength(20)]
        [Display(Name = "單位")]
        public string Unit { get; set; }

        [MaxLength(20)]
        [Display(Name = "數據來源名稱")]
        public string Source { get; set; }

        [MaxLength(20)]
        [Display(Name = "保存單位")]
        public string Dept { get; set; }

        [Display(Name = "活動數據等級")]
        public int Level { get; set; }

        [Display(Name = "儀器校正等級")]
        public int Correction { get; set; }

        [Display(Name = "是否刪除")]
        public byte isDeleted { get; set; }

        [Display(Name = "建立時間")]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "修改時間")]
        public DateTime? ModifiedTime { get; set; }

        [Display(Name = "刪除時間")]
         public DateTime? DeleteTime { get; set; }

        //Navigation Proprty
        public virtual Device Device { get; set; }

    }
}
