using System.ComponentModel.DataAnnotations;

namespace Carbon_inventory_platform.Models
{
    public class N2O
    {
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "排放係數")]
        public float CEF { get; set; }

        [Display(Name = "不確定性95%信賴區間下限")]
        public float UncertaintyLowerLimit { get; set; }

        [Display(Name = "不確定性95%信賴區間上限")]
        public float UncertaintyUpperLimit { get; set; }

        //Nagivation Property
        public virtual Material Material { get; set; }
    }
}
