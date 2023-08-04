using System.ComponentModel.DataAnnotations;

namespace 碳盤查平台.Models
{
    public class Groups
    {
        [Display(Name= "ID")]
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name ="預設群組")]
        public byte isDefault { get; set; }

        [Display(Name = "是否刪除")]
        public byte isDeleted { get; set; }

        [Display(Name = "建立時間")]
        public DateTime CreateTime { get; set; }

        [Display(Name = "修改時間")]
        public DateTime ModifiedTime { get; set; }

        [Display(Name = "刪除時間")]
        public DateTime DeleteTime { get; set; }

        //Navigation Property
        public virtual User User { get; set; }
    }
}
