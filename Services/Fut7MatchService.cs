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

                HomeGoals = m.HomeGoals,
                AwayGoals = m.AwayGoals,
                MatchDate = m.MatchDate,
                Location = m.Location,

                MatchdayId = m.MatchdayId,
                MatchdayNumber = m.Matchday != null ? m.Matchday.Number : 0,

                GroupId = m.GroupId,
                LeagueId = m.LeagueId
            });

            if (pagination.PageSize == 0) {
                //SIN PAGINADO
                var items = await dtoQuery
                    .OrderBy(x => x.MatchdayNumber)
                    .ThenBy(x => x.Id)
                    .ToListAsync();

                return new PagedResult<Fut7MatchDto> {
                    Items = items,
                    PageNumber = 1,
                    PageSize = items.Count == 0 ? 1 : items.Count, // evita división entre 0
                    TotalCount = items.Count
                };
            } else {
                //CON PAGINADO
                return await dtoQuery.ToPagedResultAsync(
                    q => q.OrderBy(x => x.MatchdayNumber).ThenBy(x => x.Id),
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

        public async Task<bool> UpdateMatchAsync(int id, UpdateFut7MatchDto dto) {
            var match = await _context.Matches.FindAsync(id);

            if (match == null)
                return false;

            _mapper.Map(dto, match);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteMatchAsync(int id) {
            var match = await _context.Matches.FindAsync(id);

            if (match == null)
                return false;

            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}