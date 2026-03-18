using fut7Manager.Api.Helpers;
using Microsoft.EntityFrameworkCore;

namespace fut7Manager.Api.Extensions {
    public static class QueryableExtensions {
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
    this IQueryable<T> query,
    Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
    int pageNumber,
    int pageSize) {
            var totalCount = await query.CountAsync();

            var items = await orderBy(query)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<T> {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}