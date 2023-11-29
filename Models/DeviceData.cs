using System.ComponentModel.DataAnnotations;

namespace Carbon_inventory_platform.Models
{
    public class DeviceData
    {
        public int Id { get; set; }

        [MaxLength(20)]
        [Display(Name = "排放源名稱")]
        public string Name { get; set; } = ""; [MaxLength(10)]

        [Display(Name = "類別")]
        public string Scope { get; set; } = ""; [MaxLength(10)]

        [Display(Name = "排放型式")]
        public string EmissionPattern { get; set; } = "";

        [Display(Name = "原燃物料")]
        public string Material { get; set; }
        [Display(Name = "活動數據等級")]
        public int Level { get; set; }

        [Display(Name = "儀器校正等級")]
        public int Correction { get; set; }
    }
}
