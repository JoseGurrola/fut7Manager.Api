using fut7Manager.Api.DTOs.Requests;
using fut7Manager.Api.DTOs.Responses;
using fut7Manager.Api.Helpers;

namespace fut7Manager.Api.Services.Interfaces {
    public interface IFut7MatchService {
        Task<PagedResult<Fut7MatchDto>> GetMatchesAsync(int? leagueId, int? teamId, PaginationParams pagination);

        Task<Fut7MatchDto?> GetMatchByIdAsync(int id);

        Task<Fut7MatchDto> CreateMatchAsync(CreateFut7MatchDto dto);

        Task<bool> UpdateMatchAsync(int id, UpdateFut7MatchDto dto);

        Task<bool> DeleteMatchAsync(int id);
    }
}
