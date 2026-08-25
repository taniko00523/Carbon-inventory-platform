using Carbon_inventory_platform.Data;
using Microsoft.EntityFrameworkCore;

namespace Carbon_inventory_platform.Tests;

/// <summary>
/// 每個測試都要拿到一份完全獨立、乾淨的資料庫，避免測試互相污染。
/// 用 EF Core InMemory provider（不是真的 SQL Server），每次給一個新的資料庫名稱即可。
///
/// 用法：seed 資料用一個 context，實際呼叫 Controller 方法時用「另一個指向同一個
/// 資料庫名稱」的 context（見 CreateAdditional）。這是刻意的：EF Core 的變更追蹤器
/// 在同一個 context 內會用「導覽屬性修復」直接把已追蹤的實體接上導覽屬性，
/// 這個過程不會套用全域查詢過濾器，會讓測試「看起來」漏掉過濾條件，
/// 但正式環境每個請求都是全新的 DbContext（Scoped 生命週期），不會有這個假象。
/// 用兩個 context 模擬正式環境的行為，才能真正驗證過濾器有沒有生效。
/// </summary>
internal sealed class TestDb : IDisposable
{
    private readonly string _databaseName = Guid.NewGuid().ToString();
    private readonly List<ApplicationDbContext> _contexts = new();

    /// <summary>建立資料時用的 context。</summary>
    public ApplicationDbContext Seed => _contexts.Count > 0 ? _contexts[0] : CreateNew();

    /// <summary>模擬「下一個請求」的全新 context，指向同一份 InMemory 資料庫。</summary>
    public ApplicationDbContext CreateNew()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;
        var context = new ApplicationDbContext(options);
        _contexts.Add(context);
        return context;
    }

    public void Dispose()
    {
        foreach (var context in _contexts)
        {
            context.Dispose();
        }
    }
}
