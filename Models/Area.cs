using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carbon_inventory_platform.Models
{
    public class Area
    {
        [Display(Name = "ID")]
        public Guid Id { get; set; }

        [Display(Name = "公司別")]
        public Guid CompanyId { get; set; } //使用者公司代號自動帶入 不須選擇

        [MaxLength(20)]
        [Display(Name = "廠區名稱")]
        public string Name { get; set; } = "";

        [Display(Name = "郵遞區號")]
        public int PostalCode { get; set; }

        [MaxLength(10)]
        [Display(Name = "縣市別")]
        public string City { get; set; } = "";

        [MaxLength(10)]
        [Display(Name = "鄉鎮區別")]
        public string District { get; set; } = "";

        [MaxLength(50)]
        [Display(Name = "地址")]
        public string Address { get; set; } = "";

        [Display(Name = "基準年")]
        public int Year { get; set; }

        [MaxLength(10)]
        [Display(Name = "產業別")]
        public string? Type { get; set; } = "";

        [Display(Name = "是否刪除")]
        public byte isDeleted { get; set; } = 0;

        [Display(Name = "建立時間")]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        [Display(Name = "修改時間")]
        public DateTime? ModifiedTime { get; set; }

        [Display(Name = "刪除時間")]
        public DateTime? DeleteTime { get; set; }

        //Navigation導覽屬性
        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }
        public ICollection<Device> Devices { get; set; } = null!;

        public Area()
        {
            Id = Guid.NewGuid();
            Devices = new List<Device>();
        }
    }
}
