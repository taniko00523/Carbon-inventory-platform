using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.EntityFrameworkCore;

namespace Carbon_inventory_platform.Services
{
    public class AreaTrendPoint
    {
        public Guid AreaId { get; set; }
        public int Year { get; set; }
        public bool IsBaseYear { get; set; }
        public decimal All { get; set; }
        public decimal Scope1 { get; set; }
        public decimal Scope2 { get; set; }
        public decimal NonMove { get; set; }
        public decimal Move { get; set; }
        public decimal Process { get; set; }
        public decimal Escape { get; set; }
        public float AvgGrade { get; set; }
        public string AllGrade { get; set; } = "";

        /// <summary>相對基準年的增減百分比；找不到基準年廠區或基準年總排放量為 0 時為 null。</summary>
        public decimal? PercentVsBaseYear { get; set; }
    }

    /// <summary>
    /// 廠區歷年排放趨勢的共用查詢與計算，同時供 B2（歷年趨勢頁）與 B3（首頁儀表板）使用，
    /// 避免兩處各自重寫一份「跟基準年比較」的公式。
    /// </summary>
    public class AreaTrendService
    {
        private readonly ApplicationDbContext _context;

        public AreaTrendService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 同一個廠區歷年的排放量趨勢。「同一個廠區」用 CompanyId + FullAddress 判斷是否為同一地址，
        /// 跟 AreasController 判斷「設為基準年時要解除同地址舊基準年」用的是同一套邏輯。
        /// </summary>
        public async Task<List<AreaTrendPoint>> GetTrendAsync(Guid companyId, string fullAddress)
        {
            var sameSiteAreas = await _context.Areas
                .Where(a => a.CompanyId == companyId && a.FullAddress == fullAddress)
                .OrderBy(a => a.Year)
                .ToListAsync();

            var baseYearAll = await GetBaseYearAllAsync(companyId);

            return sameSiteAreas.Select(a => ToPoint(a, baseYearAll)).ToList();
        }

        /// <summary>單一廠區與基準年的比較（首頁儀表板用，不需要整條歷年清單）。</summary>
        public async Task<AreaTrendPoint?> GetSinglePointAsync(Guid areaId)
        {
            var area = await _context.Areas.FirstOrDefaultAsync(a => a.Id == areaId);
            if (area == null)
            {
                return null;
            }
            var baseYearAll = await GetBaseYearAllAsync(area.CompanyId);
            return ToPoint(area, baseYearAll);
        }

        // 跟三份報告書用同一個「基準年」定義：以公司為單位找 BaseYear=true 的廠區（不限定同一個地址），
        // 這樣百分比才會跟報告書上的數字一致。公司內若有多個廠區各自設定基準年，跟報告書一樣只會
        // 取到其中一筆——這是既有的已知限制（見 plan.md B2），未來要修正建議新增明確的「廠區群組」概念。
        private async Task<decimal?> GetBaseYearAllAsync(Guid companyId)
        {
            var baseYearArea = await _context.Areas
                .Where(a => a.CompanyId == companyId && a.BaseYear)
                .FirstOrDefaultAsync();
            return baseYearArea?.All;
        }

        private static AreaTrendPoint ToPoint(Area a, decimal? baseYearAll)
        {
            return new AreaTrendPoint
            {
                AreaId = a.Id,
                Year = a.Year,
                IsBaseYear = a.BaseYear,
                All = a.All,
                Scope1 = a.Scope1,
                Scope2 = a.Scope2,
                NonMove = a.non_move,
                Move = a.move,
                Process = a.process,
                Escape = a.escape,
                AvgGrade = a.avg_Grade,
                AllGrade = a.all_Grade,
                // 跟報告書「比較總排放差異」用同一個公式：(當年 - 基準年) / 基準年 * 100。
                PercentVsBaseYear = (baseYearAll.HasValue && baseYearAll.Value != 0)
                    ? Math.Round((a.All - baseYearAll.Value) / baseYearAll.Value * 100m, 2)
                    : (decimal?)null,
            };
        }
    }
}
