using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carbon_inventory_platform.Models
{
    public class Device
    {
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "廠區代號")]
        public int AreaId { get; set; }

        [MaxLength(20)]
        [Display(Name = "財產邊號")]
        public string AssetNo { get; set; }

        [MaxLength(20)]
        [Display(Name = "排放源名稱")]
        public string Name { get; set; }

        [MaxLength(20)]
        [Display(Name = "製程")]
        public string Provess { get; set; }

        [Display(Name = "活動數據編號")] //不顯示
        public int DataId { get; set; }

        [Display(Name = "原燃物料編號")] //不顯示
        public int MaterialId { get; set; }

        [Display(Name = "排放CO2")]
        public byte CO2 { get; set; }

        [Display(Name = "排放CH4")]
        public byte CH4 { get; set; }

        [Display(Name = "排放N2O")]
        public byte N2O { get; set; }

        [Display(Name = "排放HFCS")]
        public byte HFCS { get; set; }

        [Display(Name = "排放PFCS")]
        public byte PFCS { get; set; }

        [Display(Name = "排放SF6")]
        public byte SF6 { get; set; }

        [Display(Name = "排放NF3")]
        public byte NF3 { get; set; }

        [Display(Name = "是否刪除")]
        public byte isDeleted { get; set; }

        [Display(Name = "建立時間")]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "修改時間")]
        public DateTime? ModifiedTime { get; set; }

        [Display(Name = "刪除時間")]
         public DateTime? DeleteTime { get; set; }

        //Navigation Property
        [ForeignKey("AreaId")]
        public virtual Area Area { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
        public virtual ActivityData Data { get; set; }
    }
}
