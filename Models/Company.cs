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
        //在關聯各端點的實體類型上，導覽屬性為選擇性的屬性。 如果您在關聯其中一個端點的實體類型上定義導覽屬性，就不必在關聯另一個端點的實體類型上定義導覽屬性。

        public Company()
        {
            Id = Guid.NewGuid();
            Areas = new List<Area>();
        }
    }
}
