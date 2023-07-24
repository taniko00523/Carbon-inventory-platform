using System.ComponentModel.DataAnnotations;

namespace 碳盤查平台.Models
{
    public class Area
    {
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "公司代號")]
        public int CompanyNO { get; set; }

        [Display(Name = "廠區代號")]
        public int AreaNo { get; set; }

        [MaxLength(20)]
        [Display(Name = "廠區名稱")]
        public string Name { get; set; }

        [Display(Name ="郵遞區號")]
        public int PostalCode { get; set; }

        [MaxLength (10)]
        [Display(Name ="縣市別")]
        public string City { get; set; }

        [MaxLength(10)]
        [Display(Name = "鄉鎮區別")]
        public string District { get; set; }

        [MaxLength(50)]
        [Display(Name = "地址")]
        public string Address { get; set; }

        [Display(Name = "基準年")]
        public int Year { get; set; }

        [MaxLength(10)]
        [Display(Name = "產業別")]
        public string Type { get; set; }

        [Display(Name = "是否刪除")]
        public byte isDeleted { get; set; }

        [Display(Name = "建立時間")]
        public int CreateTime { get; set; }

        [Display(Name = "修改時間")]
        public int ModifiedTime { get; set; }

        [Display(Name = "刪除時間")]
        public int DeleteTime { get; set; }




    }
}
