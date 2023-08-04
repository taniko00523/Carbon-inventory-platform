using System.ComponentModel.DataAnnotations;

namespace 碳盤查平台.Models
{
    public class Material
    {
        [Display(Name ="ID")]
        public int Id { get; set; }

        [Display(Name ="原燃物料或產品")]
        public string Name { get; set; }

        [Display(Name="碳排放係數")]
        public float CEF { get; set; }

        [MaxLength(20)]
        [Display(Name="單位")]
        public string Unit { get; set; }

        [Display(Name="係數來源")]
        public int Source { get; set; }

        //Navigation Property
        public virtual Device Device { get; set; }
    }
}
