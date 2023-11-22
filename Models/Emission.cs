using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carbon_inventory_platform.Models
{
    public class Emission
    {
        [Key]
        public Guid AreaId { get; set; }
        public string Scope1_CO2 { get; set; }
        public string CO2 { get; set; }
        public string CH4 { get;set; }
        public string N2O { get; set; }
        public string HFCS { get; set; }
        public string PFCS { get; set; }
        public string SF6 { get; set; }
        public string NF3 { get; set; }
        public string percentage1_CO2 { get; set; }
        public string percentage1_CH4 { get; set; }
        public string percentage1_N2O { get; set; }
        public string percentage1_HFCS { get; set; }
        public string percentage1_PFCS { get; set; }
        public string percentage1_SF6 { get; set; }
        public string percentage1_NF3 { get; set; }
        public string percentage2_CO2 { get; set; }
        public string percentage2_CH4 { get; set; }
        public string percentage2_N2O { get; set; }
        public string percentage2_HFCS { get; set; }
        public string percentage2_PFCS { get; set; }
        public string percentage2_SF6 { get; set; }
        public string percentage2_NF3 { get; set; }
        public string non_move { get; set; }
        public string move { get; set; }
        public string process { get; set; }
        public string escape { get; set; }
        public string percentage_nonMove { get; set; }
        public string percentage_Move { get; set; }
        public string percentage_Process { get; set; }
        public string percentage_Escape { get; set; }
        public string percentage_Scope1 { get; set; }
        public string Scope1 { get; set; }
        public string Scope2 { get; set; }
        public string All { get; set; }
        public string percentage_Scope2 { get; set; }
        public string cal_all { get; set; }
        public string percentage_CalAll { get; set; }
        public string no1_Grade { get; set; }
        public string no2_Grade { get; set;}
        public string no3_Grade { get; set; }
        public string avg_Grade { get; set; }
        public string all_Grade { get; set; }
        public string ULL { get; set; }
        public string UUL { get; set; }


        [ForeignKey("AreaId")]
        public Area? Areas { get; set; }
    }
}
