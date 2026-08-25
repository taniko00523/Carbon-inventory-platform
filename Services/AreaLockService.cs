using Carbon_inventory_platform.Data;
using Microsoft.EntityFrameworkCore;

namespace Carbon_inventory_platform.Services
{
    /// <summary>
    /// B5：盤查年度鎖定與簽核。填報單位自己可以鎖定（確認資料填完了、不想再被誤改），
    /// 但只有 Admin 能解鎖，避免鎖定形同虛設。鎖定時記錄一份「當下數字」的快照
    /// （時間／鎖定者／總排放量），之後即使 Area.All 因為其他原因變動，仍能對照
    /// 鎖定當時的樣子。Area 已經在 ApplicationDbContext 的稽核實體清單裡，
    /// 鎖定/解鎖這裡的欄位變化會自動被 B4 的稽核軌跡記錄下來，不用另外處理。
    /// </summary>
    public class AreaLockService
    {
        private readonly ApplicationDbContext _context;

        public AreaLockService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsAreaLockedAsync(Guid? areaId)
        {
            if (areaId == null || areaId == Guid.Empty)
            {
                return false;
            }
            return await _context.Areas.AsNoTracking()
                .Where(a => a.Id == areaId)
                .Select(a => a.IsLocked)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsAreaLockedByDeviceIdAsync(Guid? deviceId)
        {
            if (deviceId == null || deviceId == Guid.Empty)
            {
                return false;
            }
            return await _context.Devices.AsNoTracking()
                .Where(d => d.Id == deviceId)
                .Select(d => d.Area!.IsLocked)
                .FirstOrDefaultAsync();
        }

        /// <summary>回傳 false 表示廠區不存在或已經是鎖定狀態（沒有任何變化）。</summary>
        public async Task<bool> LockAsync(Guid areaId, string? userId, string? userName)
        {
            var area = await _context.Areas.FirstOrDefaultAsync(a => a.Id == areaId);
            if (area == null || area.IsLocked)
            {
                return false;
            }

            area.IsLocked = true;
            area.LockedAt = DateTime.Now;
            area.LockedByUserId = userId;
            area.LockedByUserName = userName;
            area.LockedSnapshotAll = area.All;
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>回傳 false 表示廠區不存在或本來就沒有鎖定（沒有任何變化）。</summary>
        public async Task<bool> UnlockAsync(Guid areaId)
        {
            var area = await _context.Areas.FirstOrDefaultAsync(a => a.Id == areaId);
            if (area == null || !area.IsLocked)
            {
                return false;
            }

            area.IsLocked = false;
            area.LockedAt = null;
            area.LockedByUserId = null;
            area.LockedByUserName = null;
            area.LockedSnapshotAll = null;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
