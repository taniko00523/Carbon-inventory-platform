using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carbon_inventory_platform.Models
{
    public class Device
    {
        [Display(Name = "ID")]
        public Guid Id { get; set; }

        [Display(Name = "廠區名稱")]
        public Guid AreaId { get; set; } 
       
        [MaxLength(20)]
        [Display(Name = "財產編號")]
        public string? AssetNo { get; set; }

        [MaxLength(10)]
        [Display(Name = "類別")]
        public string Scope { get; set; } = "";

        [MaxLength(10)]
        [Display(Name = "排放型式")]
        public string EmissionPattern { get; set; } = "";

        [MaxLength(20)]
        [Display(Name = "排放源名稱")]
        public string Name { get; set; } = ""; //需修改

        [MaxLength(20)]
        [Display(Name = "製程")]
        public string Provess { get; set; } = ""; //需修改

        [Display(Name = "原燃物料")] 
        public string Material { get; set; }

        [Display(Name = "排放CO2")]
        public Boolean CO2_Emission { get; set; } = false;

        [Display(Name = "排放CH4")]
        public Boolean CH4_Emission { get; set; } = false;

        [Display(Name = "排放N2O")]
        public Boolean N2O_Emission { get; set; } = false;

        [Display(Name = "排放HFCS")]
        public Boolean HFCS_Emission { get; set; } = false;

        [Display(Name = "排放PFCS")]
        public Boolean PFCS_Emission { get; set; } = false;

        [Display(Name = "排放SF6")]
        public Boolean SF6_Emission { get; set; } = false;

        [Display(Name = "排放NF3")]
        public Boolean NF3_Emission { get; set; } = false;

        [Display(Name = "CO2排放量")]
        public float CO2 { get; set; } = 0;

        [Display(Name = "CH4排放量")]
        public float CH4 { get; set; } = 0;

        [Display(Name = "N2O排放量")]
        public float N2O { get; set; } = 0;

        [Display(Name = "HFCS排放量")]
        public float HFCS { get; set; } = 0;

        [Display(Name = "PFCS排放量")]
        public float PFCS { get; set; } = 0;

        [Display(Name = "SF6排放量")]
        public float SF6 { get; set; } = 0;

        [Display(Name = "NF3排放量")]
        public float NF3 { get; set; } = 0;

        [Display(Name = "排放量")]
        public float Emissions { get; set; } = 0;

        [Display(Name = "活動數據")]
        public float Num { get; set; }

        [MaxLength(20)]
        [Display(Name = "單位")]
        public string Unit { get; set; } = "";

        [MaxLength(20)]
        [Display(Name = "數據來源名稱")]
        public string Source { get; set; } = "";

        [MaxLength(20)]
        [Display(Name = "保存單位")]
        public string Dept { get; set; } = "";

        [Display(Name = "活動數據等級")]
        public int Level { get; set; }

        [Display(Name = "儀器校正等級")]
        public int Correction { get; set; }

        [Display(Name = "是否刪除")]
        public byte isDeleted { get; set; } = 0;

        [Display(Name = "建立時間")]
        public DateTime CreateTime { get; set; }

        [Display(Name = "修改時間")]
        public DateTime? ModifiedTime { get; set; }

        [Display(Name = "刪除時間")]
         public DateTime? DeleteTime { get; set; }

        //Navigation Property
        [ForeignKey("AreaId")]
        public Area? Areas { get; set; }
        //public ICollection<Area> Areas { get; set; } *廠區名稱顯示bug
        //[Display(Name = "使用物料")]
        //[ForeignKey("MaterialId")]
        //public Material Material { get; set; } //可以為Null 
      
        public Device()
        {
            Id = Guid.NewGuid();
            //Area= new Area(); *廠區名稱顯示bug
        }
    }
}
