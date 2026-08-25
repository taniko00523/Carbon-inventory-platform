using System.ComponentModel.DataAnnotations;

namespace Carbon_inventory_platform.Models
{
    /// <summary>
    /// B4：稽核軌跡。記錄「誰、何時、對哪個實體做了什麼、改成什麼」。
    /// 這張表本身不可透過任何介面修改或刪除（見 AuditLogsController，只有 Index/Details）。
    /// </summary>
    public class AuditLog
    {
        public long Id { get; set; }

        [MaxLength(450)]
        public string? UserId { get; set; }

        [MaxLength(100)]
        public string? UserName { get; set; }

        [MaxLength(100)]
        public string EntityName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string EntityId { get; set; } = string.Empty;

        /// <summary>Create／Update／Delete。</summary>
        [MaxLength(20)]
        public string Action { get; set; } = string.Empty;

        /// <summary>變更前的欄位值（JSON），新增時為 null。</summary>
        public string? OldValues { get; set; }

        /// <summary>變更後的欄位值（JSON），刪除時為 null。</summary>
        public string? NewValues { get; set; }

        [MaxLength(45)]
        public string? IpAddress { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
