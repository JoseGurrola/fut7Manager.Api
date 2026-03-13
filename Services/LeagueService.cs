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

            return await query.ToPagedResultAsync(
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

            _context.Leagues.Add(league);
            await _context.SaveChangesAsync();

            return _mapper.Map<LeagueDto>(league);
        }

        public async Task<bool> DeleteLeagueAsync(int id) {
            var league = await _context.Leagues.FindAsync(id);

            if (league == null)
                return false;

            _context.Leagues.Remove(league);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}