using fut7Manager.Api.DTOs.Requests;
using fut7Manager.Api.DTOs.Responses;
using fut7Manager.Api.Helpers;

namespace fut7Manager.Api.Services.Interfaces {
    public interface IPlayerService {
        Task<PagedResult<PlayerDto>> GetPlayersAsync(int? leagueId, int? teamId, PaginationParams pagination);
        Task<PlayerDto?> GetPlayerByIdAsync(int id);

        Task<PlayerDto> CreatePlayerAsync(CreatePlayerDto dto);

        Task<bool> UpdatePlayerAsync(int id, UpdatePlayerDto dto);

        Task<bool> DeletePlayerAsync(int id);
    }
}
