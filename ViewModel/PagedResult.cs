using Microsoft.EntityFrameworkCore;

namespace Carbon_inventory_platform.ViewModel
{
    /// <summary>
    /// A5：共用的分頁結果，避免每個清單頁各自手刻一份 Skip/Take + 分頁資訊
    /// （原本 Companies/Index 就是這樣，之後每加一個分頁頁面就會多一份幾乎一樣的程式碼）。
    /// </summary>
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; } = 1;

        public static async Task<PagedResult<T>> CreateAsync(IQueryable<T> query, int pageNumber, int pageSize)
        {
            pageNumber = Math.Max(1, pageNumber);
            var totalCount = await query.CountAsync();
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
            pageNumber = Math.Min(pageNumber, totalPages);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<T>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
            };
        }
    }

    /// <summary>供 Views/Shared/_Pagination.cshtml 使用；PageUrl 由呼叫端決定要保留哪些查詢參數。</summary>
    public class PaginationViewModel
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public Func<int, string> PageUrl { get; set; } = _ => "#";
    }
}
