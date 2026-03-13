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

        public async Task<PagedResult<Fut7MatchDto>> GetMatchesAsync(PaginationParams pagination) {
            var query = _context.Matches
                .AsNoTracking()
                .ProjectTo<Fut7MatchDto>(_mapper.ConfigurationProvider);

            return await query.ToPagedResultAsync(
                pagination.PageNumber,
                pagination.PageSize);
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