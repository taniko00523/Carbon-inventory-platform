using System.ComponentModel.DataAnnotations;

namespace Carbon_inventory_platform.Models
{
    public class Analysis //Materiality Analysis 重大性評估
    {
        public Guid Id { get; set; }
        public Guid AreaId { get; set; }
        public int Target { get; set; } = 10;
        public int _21A { get; set; } = 3;
        public int _21B { get; set; } = 3;
        public int _21C { get; set; } = 3;
        public int _21D { get; set; } = 3;
        public int _22A { get; set; } = 3;
        public int _22B { get; set; } = 3;
        public int _22C { get; set; } = 3;
        public int _22D { get; set; } = 3;
        public int _31A { get; set; } = 3;
        public int _31B { get; set; } = 3;
        public int _31C { get; set; } = 3;
        public int _31D { get; set; } = 3;
        public int _32A { get; set; } = 3;
        public int _32B { get; set; } = 3;
        public int _32C { get; set; } = 3;
        public int _32D { get; set; } = 3;
        public int _33A { get; set; } = 3;
        public int _33B { get; set; } = 3;
        public int _33C { get; set; } = 3;
        public int _33D { get; set; } = 3;
        public int _34A { get; set; } = 3;
        public int _34B { get; set; } = 3;
        public int _34C { get; set; } = 3;
        public int _34D { get; set; } = 3;
        public int _35A { get; set; } = 3;
        public int _35B { get; set; } = 3;
        public int _35C { get; set; } = 3;
        public int _35D { get; set; } = 3;
        public int _41A { get; set; } = 3;
        public int _41B { get; set; } = 3;
        public int _41C { get; set; } = 3;
        public int _41D { get; set; } = 3;
        public int _42A { get; set; } = 3;
        public int _42B { get; set; } = 3;
        public int _42C { get; set; } = 3;
        public int _42D { get; set; } = 3;
        public int _43A { get; set; } = 3;
        public int _43B { get; set; } = 3;
        public int _43C { get; set; } = 3;
        public int _43D { get; set; } = 3;
        public int _44A { get; set; } = 3;
        public int _44B { get; set; } = 3;
        public int _44C { get; set; } = 3;
        public int _44D { get; set; } = 3;
        public int _45A { get; set; } = 3;
        public int _45B { get; set; } = 3;
        public int _45C { get; set; } = 3;
        public int _45D { get; set; } = 3;
        public int _46A { get; set; } = 3;
        public int _46B { get; set; } = 3;
        public int _46C { get; set; } = 3;
        public int _46D { get; set; } = 3;
        public int _47A { get; set; } = 3;
        public int _47B { get; set; } = 3;
        public int _47C { get; set; } = 3;
        public int _47D { get; set; } = 3;
        public int _48A { get; set; } = 3;
        public int _48B { get; set; } = 3;
        public int _48C { get; set; } = 3;
        public int _48D { get; set; } = 3;
        public int _49A { get; set; } = 3;
        public int _49B { get; set; } = 3;
        public int _49C { get; set; } = 3;
        public int _49D { get; set; } = 3;
        public int _410A { get; set; } = 3;
        public int _410B { get; set; } = 3;
        public int _410C { get; set; } = 3;
        public int _410D { get; set; } = 3;
        public int _411A { get; set; } = 3;
        public int _411B { get; set; } = 3;
        public int _411C { get; set; } = 3;
        public int _411D { get; set; } = 3;
        public int _51A { get; set; } = 3;
        public int _51B { get; set; } = 3;
        public int _51C { get; set; } = 3;
        public int _51D { get; set; } = 3;
        public int _52A { get; set; } = 3;
        public int _52B { get; set; } = 3;
        public int _52C { get; set; } = 3;
        public int _52D { get; set; } = 3;
        public int _53A { get; set; } = 3;
        public int _53B { get; set; } = 3;
        public int _53C { get; set; } = 3;
        public int _53D { get; set; } = 3;
        public int _54A { get; set; } = 3;
        public int _54B { get; set; } = 3;
        public int _54C { get; set; } = 3;
        public int _54D { get; set; } = 3;
        public int _55A { get; set; } = 3;
        public int _55B { get; set; } = 3;
        public int _55C { get; set; } = 3;
        public int _55D { get; set; } = 3;
        public int _61A { get; set; } = 3;
        public int _61B { get; set; } = 3;
        public int _61C { get; set; } = 3;
        public int _61D { get; set; } = 3;
        public int _21 { get; set; } = 9;
        public int _22 { get; set; }= 9;
        public int _31 { get; set; }= 9;
        public int _32 { get; set; }= 9;
        public int _33 { get; set; }= 9;
        public int _34 { get; set; }= 9;
        public int _35 { get; set; }= 9;
        public int _41 { get; set; }= 9;
        public int _42 { get; set; }= 9;
        public int _43 { get; set; }= 9;
        public int _44 { get; set; }= 9;
        public int _45 { get; set; }= 9;
        public int _46 { get; set; }= 9;
        public int _47 { get; set; }= 9;
        public int _48 { get; set; }= 9;
        public int _49 { get; set; }= 9;
        public int _410 { get; set; }= 9;
        public int _411 { get; set; }= 9;
        public int _51 { get; set; }= 9;
        public int _52 { get; set; }= 9;
        public int _53 { get; set; }= 9;
        public int _54 { get; set; }= 9;
        public int _55 { get; set; }= 9;
        public int _61 { get; set; }= 9;

        public int _21isCal { get; set; }
        public int _22isCal { get; set; }
        public int _31isCal { get; set; }
        public int _32isCal { get; set; }
        public int _33isCal { get; set; }
        public int _34isCal { get; set; }
        public int _35isCal { get; set; }
        public int _41isCal { get; set; }
        public int _42isCal { get; set; }
        public int _43isCal { get; set; }
        public int _44isCal { get; set; }
        public int _45isCal { get; set; }
        public int _46isCal { get; set; }
        public int _47isCal { get; set; }
        public int _48isCal { get; set; }
        public int _49isCal { get; set; }
        public int _410isCal { get; set; }
        public int _411isCal { get; set; }
        public int _51isCal { get; set; }
        public int _52isCal { get; set; }
        public int _53isCal { get; set; }
        public int _54isCal { get; set; }
        public int _55isCal { get; set; }
        public int _61isCal { get; set; }

        public string _21Remark { get; set; }
        public string _22Remark { get; set; }
        public string _31Remark { get; set; }
        public string _32Remark { get; set; }
        public string _33Remark { get; set; }
        public string _34Remark { get; set; }
        public string _35Remark { get; set; }
        public string _41Remark { get; set; }
        public string _42Remark { get; set; }
        public string _43Remark { get; set; }
        public string _44Remark { get; set; }
        public string _45Remark { get; set; }
        public string _46Remark { get; set; }
        public string _47Remark { get; set; }
        public string _48Remark { get; set; }
        public string _49Remark { get; set; }
        public string _410Remark { get; set; }
        public string _411Remark { get; set; }
        public string _51Remark { get; set; }
        public string _52Remark { get; set; }
        public string _53Remark { get; set; }
        public string _54Remark { get; set; }
        public string _55Remark { get; set; }
        public string _61Remark { get; set; }
        [Display(Name = "是否刪除")]
        public byte isDeleted { get; set; } = 0;

        [Display(Name = "建立時間")]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        [Display(Name = "修改時間")]
        public DateTime? ModifiedTime { get; set; }

        [Display(Name = "刪除時間")]
        public DateTime? DeleteTime { get; set; }
        public Area? Area { get; set; }

    }
}
