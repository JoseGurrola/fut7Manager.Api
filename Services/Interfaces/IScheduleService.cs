using fut7Manager.Api.DTOs.Responses;
using fut7Manager.Api.Models;

namespace fut7Manager.Api.Services.Interfaces {
    public interface IScheduleService {
        Task<List<MatchdayDto>> GenerateScheduleAsync(int leagueId, bool interGroupMatches);
    }
}
