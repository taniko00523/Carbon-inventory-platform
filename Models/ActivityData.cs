using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Carbon_inventory_platform.Models
{
    public class ActivityData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id {  get; set; }
        public Guid DeviceId { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        [Display(Name = "活動數據")]
        [Required(ErrorMessage = "請填寫活動數據")]
        public decimal Num { get; set; }
        public DateTime? Time{ get; set; }

        public string? remark { get; set; }

        //Navigation Property
        [ForeignKey("DeviceId")]
        public Device? Device { get; set; }

    }
}
