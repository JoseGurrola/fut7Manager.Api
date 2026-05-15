using fut7Manager.Api.DTOs.Requests;
using fut7Manager.Api.DTOs.Responses;
using fut7Manager.Api.Models;

namespace fut7Manager.Api.Services.Interfaces {
    public interface IScheduleService {
        Task<List<MatchdayDto>> PreviewScheduleAsync(int leagueId, GenerateScheduleDto dto);

        Task FinalizeSetupAsync(int leagueId, FinalizeLeagueSetupDto dto);
        //Task<MatchdayDto?> GetNextMatchdayAsync(int leagueId);
        Task<LeagueDashboardDto> GetDashboardAsync(int leagueId);

        Task<StandingsResponseDto> GetStandingsAsync(int leagueId);
    }
}
