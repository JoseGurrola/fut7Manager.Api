using System.Text.Json;
using fut7Manager.Api.Helpers;
using Microsoft.AspNetCore.Http;

namespace fut7Manager.Api.Extensions {
    public static class HttpResponseExtensions {
        public static void AddPaginationHeader<T>(
            this HttpResponse response,
            PagedResult<T> pagedResult) {
            var metadata = new {
                pagedResult.PageNumber,
                pagedResult.PageSize,
                pagedResult.TotalCount,
                pagedResult.TotalPages
            };

            response.Headers.Append(
                "X-Pagination",
                JsonSerializer.Serialize(metadata));
        }
    }
}