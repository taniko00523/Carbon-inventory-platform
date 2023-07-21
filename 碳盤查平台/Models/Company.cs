using System.ComponentModel.DataAnnotations;

namespace 碳盤查平台.Models
{
    public class Company
    {
        [Display(Name ="ID")]
        [DisplayFormat(DataFormatString = "{0:0000}", ApplyFormatInEditMode = false)]
        public int Id { get; set; }
        [Display(Name="公司代號")]
        public int CompanyNo { get; set; }
        [Display(Name="公司名稱")]
        public string Name { get; set; }
        [Display(Name ="負責人")]
        public string Owner { get; set; }
        [Display(Name ="電子信箱")]
        public string Email { get; set; }
        [Display(Name = "電話號碼")]    
        public int Phone { get; set; }
        [Display(Name = "是否刪除")]
        public int isDeleted { get; set; }
        [Display(Name = "建立時間")]
        public int CreateTime { get; set; }
        [Display(Name = "修改時間")]
        public int ModifiedTime { get; set; }
        [Display(Name = "刪除時間")]
        public int DeleteTime { get; set; }


    }
}
