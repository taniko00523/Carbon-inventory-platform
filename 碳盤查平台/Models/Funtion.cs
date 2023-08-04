using System.ComponentModel.DataAnnotations;

namespace 碳盤查平台.Models
{
    public class Funtion
    {
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "Controller")]
        public string MainFunction { get; set; }

        [Display(Name = "Controller中文功能名稱")]
        public string CMainFunction { get; set; }

        [Display(Name = "Action")]
        public string SubFunction { get; set; }

        [Display(Name = "Action 中文功能名稱")]
        public string CSubFunction { get; set; }

        [Display(Name = "0:後台,1:前台")]
        public byte Device { get; set; }

        [Display(Name = "是否顯示於介面")]
        public byte isOption { get; set; }

        [Display(Name = "大項功能編號")]
        public int MainFunctionNo { get; set; }

        [Display(Name = "細項功能編號")]
        public int SubFunctionNo { get; set;}

        //Navigation Property
        public ICollection<Groups> Groups { get; set; }
    }
}
