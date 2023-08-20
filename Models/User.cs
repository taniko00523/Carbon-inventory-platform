using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Carbon_inventory_platform.Models
{
    public class User
    {
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "帳號")]
        [MaxLength(30)]
        public string Accounts { get; set; }

        [MaxLength(255)]
        public string Password { get; set; }

        [Display(Name = "員工ID")]
        public int StaffId { get; set; }

        [Display(Name = "連絡電話")]
        [MaxLength(50)]
        public string Phone { get; set; }

        [Display(Name = "Email")]
        [MaxLength(100)]
        public string Email { get; set; }

        [Display(Name = "備註")]
        [MaxLength]
        public string Remark { get; set; }

        [Display(Name = "所屬群組")]
        public int GroupsId { get; set; }

        [Display(Name = "是否啟用")]
        public byte isEnable { get; set; }

        [Display(Name = "是否刪除")]
        public byte isDeleted { get; set; }

        [Display(Name = "建立時間")]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "修改時間")]
        public DateTime? ModifiedTime { get; set; }

        [Display(Name = "刪除時間")]
         public DateTime? DeleteTime { get; set; }

        [MaxLength(255)]
        [Display(Name = "Token")]
        public string Token { get; set; }

        [Display(Name = "Token建立時間")]
        public DateTime TokenCreateTime { get; set; }

        [Display(Name = "Token更新時間")]
        public DateTime TokenUpdateTime { get; set; }

        //Navigation Property
        public virtual Staff Staff { get; set; }
        public virtual Groups Groups { get; set; }
    }
}
