using Carbon_inventory_platform.Data;
using Microsoft.EntityFrameworkCore;

namespace Carbon_inventory_platform.Services
{
    /// <summary>
    /// DefaultDevices／DeviceData 的「原燃物料」「類別」「排放型式」都是對照
    /// Materials／GWPs 表的名稱字串（沒有真正的外鍵）。畫面上用下拉選單限制輸入，
    /// 避免打錯字讓後續計算靜靜算出 0；這裡是伺服器端的對應防線，避免有人繞過
    /// 下拉選單直接送出不存在的值。
    ///
    /// 原本這段邏輯（取得名單、驗證）在 DefaultDevicesController／DeviceDatasController
    /// 各自重複一份，且都只驗證了 Material、沒有驗證 Scope／EmissionPattern，抽成共用服務
    /// 順便補齊。
    /// </summary>
    public class MaterialCatalogService
    {
        private readonly ApplicationDbContext _context;

        public MaterialCatalogService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>冷媒類的原燃物料（R-410A、FM200…）只存在 GWPs，所以兩張表都要取。</summary>
        public async Task<List<string>> GetMaterialNamesAsync()
        {
            var fromMaterials = await _context.Materials.AsNoTracking().Select(m => m.Name).Distinct().ToListAsync();
            var fromGwps = await _context.GWPs.AsNoTracking().Select(g => g.Name).Distinct().ToListAsync();
            return fromMaterials.Union(fromGwps).OrderBy(n => n).ToList();
        }

        public async Task<List<string>> GetScopesAsync()
            => await _context.Materials.AsNoTracking().Select(m => m.Scope).Distinct().OrderBy(s => s).ToListAsync();

        public async Task<List<string>> GetEmissionPatternsAsync()
            => await _context.Materials.AsNoTracking().Select(m => m.EmissionPattern).Distinct().OrderBy(e => e).ToListAsync();

        /// <summary>
        /// 驗證 Material／Scope／EmissionPattern 是否都在下拉選單的合法範圍內。
        /// 回傳值不依賴 MVC（不直接碰 ModelState），呼叫端自行把 Errors 轉成 ModelState 錯誤，方便單元測試。
        /// </summary>
        public async Task<MaterialCatalogValidationResult> ValidateAsync(string material, string scope, string emissionPattern)
        {
            var result = new MaterialCatalogValidationResult();

            var names = await GetMaterialNamesAsync();
            if (!names.Contains(material))
            {
                result.Errors.Add(("Material", "原燃物料必須是排放係數表或 GWP 表中已存在的名稱，請重新選擇。"));
            }

            var scopes = await GetScopesAsync();
            if (!scopes.Contains(scope))
            {
                result.Errors.Add(("Scope", "類別必須是排放係數表中已存在的選項，請重新選擇。"));
            }

            var patterns = await GetEmissionPatternsAsync();
            if (!patterns.Contains(emissionPattern))
            {
                result.Errors.Add(("EmissionPattern", "排放型式必須是排放係數表中已存在的選項，請重新選擇。"));
            }

            return result;
        }
    }

    public class MaterialCatalogValidationResult
    {
        public List<(string Field, string Message)> Errors { get; } = new();
        public bool IsValid => Errors.Count == 0;
    }
}
