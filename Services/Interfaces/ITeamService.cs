using fut7Manager.Api.DTOs.Requests;
using fut7Manager.Api.DTOs.Responses;
using fut7Manager.Api.Helpers;

namespace fut7Manager.Api.Services.Interfaces {
    public interface ITeamService {
        Task<PagedResult<TeamDto>> GetTeamsAsync(int? leagueId, PaginationParams pagination);

        Task<TeamDto?> GetTeamByIdAsync(int id);

        Task<TeamDto> CreateTeamAsync(CreateTeamDto dto);

        Task<bool> UpdateTeamAsync(int id, UpdateTeamDto dto);

        Task<bool> DeleteTeamAsync(int id);
    }
}
