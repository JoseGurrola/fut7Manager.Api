using AutoMapper;
using AutoMapper.QueryableExtensions;
using fut7Manager.Api.DTOs.Requests;
using fut7Manager.Api.DTOs.Responses;
using fut7Manager.Api.Extensions;
using fut7Manager.Api.Helpers;
using fut7Manager.Api.Models;
using fut7Manager.Api.Services.Interfaces;
using fut7Manager.Data;
using Microsoft.EntityFrameworkCore;

namespace fut7Manager.Api.Services {
    public class Fut7MatchService : IFut7MatchService {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public Fut7MatchService(ApplicationDbContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<Fut7MatchDto>> GetMatchesAsync(int? leagueId, int? teamId, PaginationParams pagination) {
            var query = _context.Matches
                .AsNoTracking()
                .Include(m => m.Matchday)
                 .Include(m => m.HomeTeam)    
                 .Include(m => m.AwayTeam)
                .AsQueryable();

            if (leagueId.HasValue) //filtro por liga
                query = query.Where(m => m.LeagueId == leagueId.Value);

            if (teamId.HasValue) //filtro por team
                query = query.Where(m => m.HomeTeamId == teamId || m.AwayTeamId == teamId);

            var dtoQuery = query.Select(m => new Fut7MatchDto {
                Id = m.Id,
                HomeTeamId = m.HomeTeamId,
                AwayTeamId = m.AwayTeamId,

                HomeTeamName = m.HomeTeam.Name,
                AwayTeamName = m.AwayTeam.Name,

                HomeTeamLogo = m.HomeTeam.LogoUrl,
                AwayTeamLogo = m.AwayTeam.LogoUrl,

                HomeTeamPrimaryColor = m.HomeTeam.TeamPrimaryColor,
                AwayTeamPrimaryColor = m.AwayTeam.TeamPrimaryColor,

                HomeGoals = m.HomeGoals,
                AwayGoals = m.AwayGoals,
                MatchDate = m.MatchDate,
                Location = m.Location,

                HomePenaltyGoals = m.HomePenaltyGoals,
                AwayPenaltyGoals = m.AwayPenaltyGoals,

                MatchdayId = m.MatchdayId,
                MatchdayNumber = m.Matchday != null ? m.Matchday.Number : 0,

                GroupId = m.GroupId,
                LeagueId = m.LeagueId
            });

            if (pagination.PageSize == 0) {

                var items = await dtoQuery
                    .OrderBy(x => x.MatchDate == null)
                    .ThenBy(x => x.MatchDate)
                    .ThenBy(x => x.MatchdayNumber)
                    .ThenBy(x => x.Id)
                    .ToListAsync();

                return new PagedResult<Fut7MatchDto> {
                    Items = items,
                    PageNumber = 1,
                    PageSize = items.Count == 0 ? 1 : items.Count,
                    TotalCount = items.Count
                };
            } else {

                return await dtoQuery.ToPagedResultAsync(
                    q => q.OrderBy(x => x.MatchDate == null)
                          .ThenBy(x => x.MatchDate)
                          .ThenBy(x => x.MatchdayNumber)
                          .ThenBy(x => x.Id),
                    pagination.PageNumber,
                    pagination.PageSize);
            }
        }

        public async Task<Fut7MatchDto?> GetMatchByIdAsync(int id) {
            return await _context.Matches
                .Where(m => m.Id == id)
                .ProjectTo<Fut7MatchDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<Fut7MatchDto> CreateMatchAsync(CreateFut7MatchDto dto) {
            var match = _mapper.Map<Fut7Match>(dto);

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();

            return _mapper.Map<Fut7MatchDto>(match);
        }

        public async Task<Fut7MatchDto?> UpdateMatchAsync(int id, UpdateFut7MatchDto dto) {
            var match = await _context.Matches.FindAsync(id);

            if (match == null)
                return null;

            _mapper.Map(dto, match);

            await _context.SaveChangesAsync();

            return _mapper.Map<Fut7MatchDto>(match);
        }

        public async Task<bool> DeleteMatchAsync(int id) {
            var match = await _context.Matches.FindAsync(id);

            if (match == null)
                return false;

            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<MatchdayDto>> GetMatchdaysAsync(int leagueId) {
            var matchdays = await _context.Matchdays
                .AsNoTracking()
                .Include(md => md.Matches)
                    .ThenInclude(m => m.HomeTeam)
                .Include(md => md.Matches)
                    .ThenInclude(m => m.AwayTeam)
                .Where(md => md.LeagueId == leagueId)
                .OrderBy(md => md.Number)
                .ToListAsync();

            var allTeams = await _context.Teams
                .Where(t => t.LeagueId == leagueId)
                .ToListAsync();

            var result = matchdays.Select(md =>
            {
                // calcular descansos dinámicamente
                var restingTeams = allTeams
                    .Where(t => !md.Matches.Any(m =>
                        m.HomeTeamId == t.Id ||
                        m.AwayTeamId == t.Id))
                    .Select(t => t.Name)
                    .ToList();

                return new MatchdayDto {
                    Id = md.Id,
                    Number = md.Number,
                    RestingTeamNames = restingTeams,

                    Matches = md.Matches
                        .OrderBy(m => m.MatchDate == null)
                        .ThenBy(m => m.MatchDate)
                        .ThenBy(m => m.Id)
                        .Select(m => new Fut7MatchDto {
                            Id = m.Id,

                            HomeTeamId = m.HomeTeamId,
                            AwayTeamId = m.AwayTeamId,

                            HomeTeamName = m.HomeTeam.Name,
                            AwayTeamName = m.AwayTeam.Name,

                            HomeTeamLogo = m.HomeTeam.LogoUrl,
                            AwayTeamLogo = m.AwayTeam.LogoUrl,

                            HomeTeamPrimaryColor = m.HomeTeam.TeamPrimaryColor,
                            AwayTeamPrimaryColor = m.AwayTeam.TeamPrimaryColor,

                            HomeGoals = m.HomeGoals,
                            AwayGoals = m.AwayGoals,

                            HomePenaltyGoals = m.HomePenaltyGoals,
                            AwayPenaltyGoals = m.AwayPenaltyGoals,

                            MatchDate = m.MatchDate,
                            Location = m.Location,

                            MatchdayId = m.MatchdayId,
                            MatchdayNumber = md.Number,

                            GroupId = m.GroupId,
                            LeagueId = m.LeagueId
                        })
                        .ToList()
                };
            }).ToList();

            return result;
        }
    }
}