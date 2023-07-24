using System.ComponentModel.DataAnnotations;

namespace 碳盤查平台.Models
{
    public class Areaa
    {
        [Display(Name = "ID")]
        [DisplayFormat(DataFormatString = "{0:0000}", ApplyFormatInEditMode = false)]
        public int Id { get; set; }

        [Display(Name = "公司代號")]
        public int CompanyNo { get; set; }

        [Display(Name = "廠區代號")]
        public int AreaNo { get; set; }

        [Display(Name = "公司名稱")]
        [MaxLength(20)]
        public string Name { get; set; }

        [Display(Name = "郵遞區號")]
        public int PostalCode { get; set; }

        [Display(Name = "縣市別")]
        [MaxLength(10)]
        public string City { get; set; }

        [Display(Name = "鄉鎮區別")]
        [MaxLength(10)]
        public string Distict { get; set; }

        [Display(Name = "地址")]
        [MaxLength(50)]
        public string Address { get; set; }

        [Display(Name = "基準年")]
        public int Year { get; set; }

        [Display(Name = "產業別")]
        [MaxLength(10)]
        public string Type { get; set; }

        [Display(Name = "是否刪除")]
        public byte isDeleted { get; set; }

        [Display(Name = "建立時間")]
        public DateTime CreateTime { get; set; }

        [Display(Name = "修改時間")]
        public DateTime ModifiedTime { get; set; }

        [Display(Name = "刪除時間")]
        public DateTime DeleteTime { get; set; }
    }
}
