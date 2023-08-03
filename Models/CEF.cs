using System.ComponentModel.DataAnnotations;

namespace 碳盤查平台.Models
{
    public class CEF
    {
        [Display(Name ="ID")]
        public int Id { get; set; }

        [Display(Name="源燃物料編號")]
        public int MaterialNo { get; set; }

        [Display(Name ="碳排放係數")]
        public float Num { get; set; }

        [MaxLength(20)]
        [Display(Name ="單位")]
        public string Unit { get; set; }

        [Display(Name ="係數來源")]
        public string Source { get; set; }
    }
}
