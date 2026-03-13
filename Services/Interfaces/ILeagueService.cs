using fut7Manager.Api.DTOs.Requests;
using fut7Manager.Api.DTOs.Responses;
using fut7Manager.Api.Helpers;

namespace fut7Manager.Api.Services.Interfaces {
    public interface ILeagueService {
        Task<PagedResult<LeagueDto>> GetLeaguesAsync(PaginationParams pagination);

        Task<LeagueDto?> GetLeagueByIdAsync(int id);

        Task<LeagueDto> CreateLeagueAsync(CreateLeagueDto dto);

        Task<bool> DeleteLeagueAsync(int id);
    }
}
