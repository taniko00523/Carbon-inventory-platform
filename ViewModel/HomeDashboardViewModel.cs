using Carbon_inventory_platform.Models;
using Carbon_inventory_platform.Services;

namespace Carbon_inventory_platform.ViewModel
{
    /// <summary>B3：登入後首頁依角色顯示的摘要資訊。</summary>
    public class HomeDashboardViewModel
    {
        public bool IsAdmin { get; set; }

        // ---- 管理員視角 ----
        public int TotalCompanies { get; set; }
        public int CompletedReportCount { get; set; }
        public List<(Guid CompanyId, string Name)> IncompleteCompanies { get; set; } = new();
        public List<Feedback> RecentFeedbacks { get; set; } = new();
        public List<(string UserName, DateTime Limit)> ExpiringSoonAccounts { get; set; } = new();

        // ---- 一般使用者視角：我的盤查進度 ----
        public bool HasCompany { get; set; }
        public bool HasArea { get; set; }
        public bool HasDevice { get; set; }
        public bool HasActivityData { get; set; }
        public Guid? CurrentAreaId { get; set; }
        public int CurrentAreaYear { get; set; }
        public AreaTrendPoint? CurrentAreaTrend { get; set; }
        public int MissingActivityDataDeviceCount { get; set; }
    }
}
