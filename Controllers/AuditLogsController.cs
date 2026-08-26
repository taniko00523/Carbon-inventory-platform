using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Carbon_inventory_platform.Controllers
{
    /// <summary>
    /// B4：稽核軌跡查詢畫面。刻意只有 Index／Details（唯讀），沒有 Create／Edit／Delete——
    /// 稽核紀錄本身不可透過任何介面被竄改，寫入只能透過 ApplicationDbContext.SaveChanges 自動產生。
    /// </summary>
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AuditLogsController : Controller
    {
        private const int PageSize = 30;

        /// <summary>目前實際會被記錄的實體（見 ApplicationDbContext.AuditedEntityTypes），篩選下拉選單用。</summary>
        public static readonly string[] AuditedEntityNames =
        {
            "Material", "GWP", "Device", "ActivityData", "Area", "Company", "RolePermission", "UserPermission",
            "DefaultDevices", "DeviceData",
        };

        private readonly ApplicationDbContext _context;

        public AuditLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? entityName, string? userName, DateTime? from, DateTime? to, int pageNumber = 1)
        {
            var query = _context.AuditLogs.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(entityName))
            {
                query = query.Where(a => a.EntityName == entityName);
            }
            if (!string.IsNullOrWhiteSpace(userName))
            {
                query = query.Where(a => a.UserName != null && a.UserName.Contains(userName));
            }
            if (from.HasValue)
            {
                query = query.Where(a => a.CreatedAt >= from.Value.Date);
            }
            if (to.HasValue)
            {
                // 含結束當天：比對「小於次日 0 時」。
                var toExclusive = to.Value.Date.AddDays(1);
                query = query.Where(a => a.CreatedAt < toExclusive);
            }

            var paged = await PagedResult<Models.AuditLog>.CreateAsync(query.OrderByDescending(a => a.CreatedAt), pageNumber, PageSize);

            ViewBag.EntityNames = AuditedEntityNames;
            ViewBag.EntityName = entityName;
            ViewBag.UserName = userName;
            ViewBag.From = from?.ToString("yyyy-MM-dd");
            ViewBag.To = to?.ToString("yyyy-MM-dd");
            ViewBag.PageNumber = paged.PageNumber;
            ViewBag.TotalPages = paged.TotalPages;
            ViewBag.TotalCount = paged.TotalCount;

            return View(paged.Items);
        }

        public async Task<IActionResult> Details(long id)
        {
            var log = await _context.AuditLogs.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            if (log == null)
            {
                return NotFound();
            }

            var oldDict = ParseValues(log.OldValues);
            var newDict = ParseValues(log.NewValues);
            var fields = oldDict.Keys.Union(newDict.Keys).OrderBy(f => f);

            var model = new AuditLogDetailViewModel
            {
                Log = log,
                Changes = fields
                    .Select(f => (
                        Field: f,
                        OldValue: oldDict.TryGetValue(f, out var ov) ? ov : null,
                        NewValue: newDict.TryGetValue(f, out var nv) ? nv : null))
                    .ToList(),
            };

            return View(model);
        }

        private static Dictionary<string, string?> ParseValues(string? json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return new Dictionary<string, string?>();
            }

            var raw = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json) ?? new();
            return raw.ToDictionary(kv => kv.Key, kv => (string?)kv.Value.ToString());
        }
    }
}
