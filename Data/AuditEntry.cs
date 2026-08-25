using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Carbon_inventory_platform.Data
{
    /// <summary>
    /// SaveChanges 攔截時的暫存資料。EF Core 對於新增的實體（Added），主鍵是資料庫產生的，
    /// 存檔前根本不知道值是多少，所以需要先記下「哪些屬性要等存檔後才能填」（TemporaryProperties），
    /// 存檔完成後再回填、補寫一筆 AuditLog。
    /// </summary>
    internal class AuditEntry
    {
        public AuditEntry(EntityEntry entry)
        {
            Entry = entry;
            // 存檔成功後 EF Core 會呼叫 ChangeTracker.AcceptAllChanges()，把 Added/Modified/Deleted
            // 都轉成 Unchanged/Detached——如果 Action 是動態讀 Entry.State 算出來的，
            // 對「主鍵是資料庫產生」而延後到存檔後才補寫的那批（見 TemporaryProperties），
            // 讀到的就已經是存檔後的狀態，Create 會被誤判成 Update。這裡在狀態還沒被
            // AcceptAllChanges 動過之前就把它定住。
            Action = entry.State switch
            {
                EntityState.Added => "Create",
                EntityState.Deleted => "Delete",
                _ => "Update",
            };
        }

        public EntityEntry Entry { get; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? IpAddress { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public string Action { get; }
        public Dictionary<string, object?> KeyValues { get; } = new();
        public Dictionary<string, object?> OldValues { get; } = new();
        public Dictionary<string, object?> NewValues { get; } = new();
        public List<PropertyEntry> TemporaryProperties { get; } = new();

        public bool HasTemporaryProperties => TemporaryProperties.Count > 0;

        // System.Text.Json 預設會把中文等非 ASCII 字元轉成 \uXXXX，稽核紀錄存的是要給人看的
        // 異動內容，用 UnsafeRelaxedJsonEscaping 讓中文直接可讀（這裡不是要嵌進 HTML/<script>，
        // 不會有 XSS 顧慮）。
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        public Models.AuditLog ToAuditLog()
        {
            return new Models.AuditLog
            {
                UserId = UserId,
                UserName = UserName,
                IpAddress = IpAddress,
                EntityName = EntityName,
                EntityId = string.Join(",", KeyValues.Values.Select(v => v?.ToString() ?? "")),
                Action = Action,
                OldValues = OldValues.Count > 0 ? JsonSerializer.Serialize(OldValues, JsonOptions) : null,
                NewValues = NewValues.Count > 0 ? JsonSerializer.Serialize(NewValues, JsonOptions) : null,
                CreatedAt = DateTime.Now,
            };
        }
    }
}
