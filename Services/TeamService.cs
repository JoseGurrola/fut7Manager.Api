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
    public class TeamService : ITeamService {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TeamService(ApplicationDbContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<TeamDto>> GetTeamsAsync(int? leagueId, PaginationParams pagination) {
            var query = _context.Teams
                .AsNoTracking()
                .AsQueryable();

            // Filtrar por liga
            if (leagueId.HasValue)
                query = query.Where(t => t.LeagueId == leagueId.Value);

            // Proyección a DTO después de aplicar filtros
            var dtoQuery = query
                .ProjectTo<TeamDto>(_mapper.ConfigurationProvider);

            return await dtoQuery.ToPagedResultAsync(q => q.OrderBy(x => x.Id),
                pagination.PageNumber,
                pagination.PageSize);
        }

        public async Task<TeamDto?> GetTeamByIdAsync(int id) {
            return await _context.Teams
                .AsNoTracking()
                .Where(t => t.Id == id)
                .ProjectTo<TeamDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<TeamDto> CreateTeamAsync(CreateTeamDto dto) {
            var team = _mapper.Map<Team>(dto);

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            return _mapper.Map<TeamDto>(team);
        }

        public async Task<bool> UpdateTeamAsync(int id, UpdateTeamDto dto) {
            var team = await _context.Teams.FindAsync(id);

            if (team == null)
                return false;

            _mapper.Map(dto, team);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteTeamAsync(int id) {
            var team = await _context.Teams.FindAsync(id);

            if (team == null)
                return false;

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}