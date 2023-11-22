using System.ComponentModel.DataAnnotations;

namespace Carbon_inventory_platform.Models
{
    public class Company
    {
        [Display(Name = "ID")]
        public Guid Id { get; set; }

        [MaxLength(20)]
        [Display(Name = "公司名稱")]
        public string Name { get; set; } = "";

        [MaxLength(20)]
        [Display(Name = "負責人")]
        public string Owner { get; set; } = "";

        [MaxLength(50)]
        [Display(Name = "電子信箱")]
        public string Email { get; set; } = "";

        [MaxLength(20)]
        [Display(Name = "電話號碼")]
        public string Phone { get; set; } =null!;

        [Display(Name = "公司簡介")]
        public string Information { get; set; } = "";

        [Display(Name = "是否刪除")]
        public byte isDeleted { get; set; } = 0;

        [Display(Name = "建立時間")]
        public DateTime CreateTime { get; set; }

        [Display(Name = "修改時間")]
        public DateTime? ModifiedTime { get; set; }

        [Display(Name = "刪除時間")]
         public DateTime? DeleteTime { get; set; }

        //Navigation Property
        public ICollection<Area> Areas { get; set; } = null!;

        public Company()
        {
            Id = Guid.NewGuid();
            Areas = new List<Area>();
        }
    }
}
