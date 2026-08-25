using Carbon_inventory_platform.Models;

namespace Carbon_inventory_platform.ViewModel
{
    public class AuditLogDetailViewModel
    {
        public AuditLog Log { get; set; } = null!;
        public List<(string Field, string? OldValue, string? NewValue)> Changes { get; set; } = new();
    }
}
