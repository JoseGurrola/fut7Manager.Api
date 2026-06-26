using fut7Manager.Api.DTOs.Responses;
using fut7Manager.Api.Models;

namespace fut7Manager.Api.Services.Interfaces {
    public interface IStandingsService {
        LeagueDashboardDto BuildDashboard(
            League league,
            List<Team> teams,
            List<Fut7Match> matches,
            List<Group> groups,
            int playersStatsCount);
    }
}

