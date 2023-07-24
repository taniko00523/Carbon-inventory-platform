using System.ComponentModel.DataAnnotations;

namespace 碳盤查平台.Models
{
    public class Device
    {
        [Display(Name ="ID")]
        public int Id { get; set; }

        [Display(Name = "廠區代號")]
        public int AreaNo { get; set; }

        [Display(Name = "排放源代號")]
        public int DeviceNo { get; set; }

        [MaxLength(20)]
        [Display(Name = "財產邊號")]
        public string AssetNo { get; set; }

        [MaxLength(20)]
        [Display(Name = "排放源名稱")]
        public string Name { get; set; }

        [MaxLength(20)]
        [Display(Name = "製程")]
        public string Provess { get; set; }

        [MaxLength(20)]
        [Display(Name = "原燃物料或產品")]
        public string Material { get; set; }

        [MaxLength(20)]
        [Display(Name = "原燃物料邊號")]
        public string MaterialNo { get; set; }

        

    }
}
