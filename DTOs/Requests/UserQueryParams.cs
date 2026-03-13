using fut7Manager.Api.DTOs.Requests;

namespace fut7Manager.DTOs.Requests {
    public class UserQueryParams : PaginationParams {
        public string? Search { get; set; }

        public string? Email { get; set; }
    }
}
