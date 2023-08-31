using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carbon_inventory_platform.Models
{
    public class Device
    {
        [Display(Name = "ID")]
        public Guid Id { get; set; }

        [Display(Name = "廠區名稱")]
        public Guid AreaId { get; set; }

        [MaxLength(20)]
        [Display(Name = "財產編號")]
        public string? AssetNo { get; set; }

        [MaxLength(20)]
        [Display(Name = "排放源名稱")]
        public string Name { get; set; } = ""; //需修改

        [MaxLength(20)]
        [Display(Name = "製程")]
        public string Provess { get; set; } = ""; //需修改

        [Display(Name = "活動數據")] 
        public Guid? ActivityDataId { get; set; }

        [Display(Name = "原燃物料")] 
        public int MaterialId { get; set; }

        [Display(Name = "排放CO2")]
        public Boolean CO2 { get; set; } = false;

        [Display(Name = "排放CH4")]
        public Boolean CH4 { get; set; } = false;

        [Display(Name = "排放N2O")]
        public Boolean N2O { get; set; } = false;

        [Display(Name = "排放HFCS")]
        public Boolean HFCS { get; set; } = false;

        [Display(Name = "排放PFCS")]
        public Boolean PFCS { get; set; } = false;

        [Display(Name = "排放SF6")]
        public Boolean SF6 { get; set; } = false;

        [Display(Name = "排放NF3")]
        public Boolean NF3 { get; set; } = false;

        [Display(Name = "是否刪除")]
        public byte isDeleted { get; set; } = 0;

        [Display(Name = "建立時間")]
        public DateTime CreateTime { get; set; }

        [Display(Name = "修改時間")]
        public DateTime? ModifiedTime { get; set; }

        [Display(Name = "刪除時間")]
         public DateTime? DeleteTime { get; set; }

        //Navigation Property
        [ForeignKey("AreaId")]
        public ICollection<Area> Areas { get; set; } = null!;
        [ForeignKey("MaterialId")]
        public Material? Material { get; set; } //可以為Null 
        public ActivityData? ActivityData { get; set; }

        public Device()
        {
            Id = Guid.NewGuid();
            Areas = new List<Area>();
        }
    }
}
