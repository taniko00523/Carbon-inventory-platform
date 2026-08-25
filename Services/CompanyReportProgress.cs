using Carbon_inventory_platform.Models;

namespace Carbon_inventory_platform.Services
{
    /// <summary>
    /// 公司「報告書內容填寫進度」的定義：8 個報告書會用到的文字欄位是否已填寫。
    /// 原本這份清單只寫在 Views/Companies/Index.cshtml 裡，B3 首頁儀表板需要用同一套
    /// 定義統計「本年度已完成盤查數」，抽成共用類別避免兩處各自維護一份、之後改一邊漏一邊。
    /// </summary>
    public static class CompanyReportProgress
    {
        public static readonly (string Label, Func<Company, bool> Filled)[] ReportFields =
        {
            ("公司簡介", c => !string.IsNullOrEmpty(c.CompanyInformation)),
            ("前言", c => !string.IsNullOrEmpty(c.ReportOpening)),
            ("報告用途", c => !string.IsNullOrEmpty(c.ReportingPurposes)),
            ("組織邊界", c => !string.IsNullOrEmpty(c.AddressInformation)),
            ("報告邊界", c => !string.IsNullOrEmpty(c.ReportingInformation)),
            ("溫室氣體", c => !string.IsNullOrEmpty(c.GHGInformation)),
            ("直接溫室氣體", c => !string.IsNullOrEmpty(c.Scope1Information)),
            ("間接溫室氣體", c => !string.IsNullOrEmpty(c.Scope2Information)),
        };

        public static int FilledCount(Company company) => ReportFields.Count(f => f.Filled(company));

        public static bool IsComplete(Company company) => FilledCount(company) == ReportFields.Length;
    }
}
