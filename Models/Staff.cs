using System.ComponentModel.DataAnnotations;

namespace Carbon_inventory_platform.Models
{
    public class Staff
    {
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "廠區ID")]
        public int AreaId { get; set; }

        [MaxLength(20)]
        [Display(Name = "人員編號")]
        public string StaffNo { get; set; }

        [MaxLength(20)]
        [Display(Name = "人員姓名")]
        public string Name { get; set; }

        [MaxLength(50)]
        [Display(Name = "連絡電話")]
        public string Phone { get; set; }

        [MaxLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [MaxLength]
        [Display(Name = "備註")]
        public string Remark { get; set; }

        [Display(Name = "是否刪除")]
        public byte isDeleted { get; set; }

        [Display(Name = "建立時間")]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "修改時間")]
        public DateTime? ModifiedTime { get; set; }

        [Display(Name = "刪除時間")]
         public DateTime? DeleteTime { get; set; }

        //Navigation Property
        public virtual User User { get; set; }
        public virtual Area Area { get; set; }
    }
}
