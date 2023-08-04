using System.ComponentModel.DataAnnotations;

namespace 碳盤查平台.Models
{
    public class Company
    {
        [Display(Name ="ID")]
        public int Id { get; set; }

        [MaxLength(20)]
        [Display(Name="公司名稱")]
        public string Name { get; set; }

        [MaxLength(20)]
        [Display(Name ="負責人")]
        public string Owner { get; set; }

        [MaxLength(50)]
        [Display(Name ="電子信箱")]
        public string Email { get; set; }

        [MaxLength(20)]
        [Display(Name = "電話號碼")]    
        public string Phone { get; set; }

        [Display(Name = "是否刪除")]
        public byte isDeleted { get; set; }

        [Display(Name = "建立時間")]
        public DateTime CreateTime { get; set; }

        [Display(Name = "修改時間")]
        public DateTime ModifiedTime { get; set; }

        [Display(Name = "刪除時間")]
        public DateTime DeleteTime { get; set; }

        //Navigation Property
        public ICollection<Area> Areas { get; set; }


    }
}
