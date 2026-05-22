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
using System.Numerics;

namespace fut7Manager.Api.Services {
    public class LeagueService : ILeagueService {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public LeagueService(ApplicationDbContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<LeagueDto>> GetLeaguesAsync(PaginationParams pagination) {

            var query = _context.Leagues
                .AsNoTracking()
                .ProjectTo<LeagueDto>(_mapper.ConfigurationProvider);

            // 🔹 Sin paginado
            if (pagination == null || pagination.PageSize == 0) {

                var items = await query
                    .OrderByDescending(x => x.CreationDate)
                    .ToListAsync();

                return new PagedResult<LeagueDto> {
                    Items = items,
                    PageNumber = 1,
                    PageSize = items.Count == 0 ? 1 : items.Count,
                    TotalCount = items.Count
                };
            }

            // 🔹 Con paginado
            return await query.ToPagedResultAsync(
                q => q.OrderByDescending(x => x.CreationDate),
                pagination.PageNumber,
                pagination.PageSize);
        }

        public async Task<LeagueDto?> GetLeagueByIdAsync(int id) {
            return await _context.Leagues
                .AsNoTracking()
                .Where(l => l.Id == id)
                .ProjectTo<LeagueDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<LeagueDto> CreateLeagueAsync(CreateLeagueDto dto) {
            var league = _mapper.Map<League>(dto);

            league.CreationDate = DateTime.UtcNow;

            _context.Leagues.Add(league);
            await _context.SaveChangesAsync();

            return _mapper.Map<LeagueDto>(league);
        }

        public async Task<bool> UpdateLeagueAsync(int id, CreateLeagueDto dto){
            var league = await _context.Leagues.FindAsync(id);

            if (league == null)
                return false;

            var isValid =await IsQualifiedTeamsPerGroupValid(id, dto.QualifiedTeamsPerGroup);

            if (!isValid)
                return false;

            _mapper.Map(dto, league);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteLeagueAsync(int id) {
            var league = await _context.Leagues.FindAsync(id);

            if (league == null)
                return false;

            _context.Leagues.Remove(league);
            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<bool> IsQualifiedTeamsPerGroupValid(int leagueId, int qualifiedTeamsPerGroup) {
            var totalTeams = await _context.Teams
                .CountAsync(t => t.LeagueId == leagueId);

            var totalGroups = await _context.Groups
                .CountAsync(g => g.LeagueId == leagueId);

            if (totalGroups <= 0)
                return false;

            // grupo más pequeño
            var minTeamsPerGroup = totalTeams / totalGroups;

            if (minTeamsPerGroup <= 0)
                return false;

            return qualifiedTeamsPerGroup <= minTeamsPerGroup;
        }
    }
}