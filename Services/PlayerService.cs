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
    public class PlayerService : IPlayerService {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PlayerService(ApplicationDbContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<PlayerDto>> GetPlayersAsync(PaginationParams pagination) {
            var query = _context.Players
                .AsNoTracking()
                .ProjectTo<PlayerDto>(_mapper.ConfigurationProvider);

            return await query.ToPagedResultAsync(
                pagination.PageNumber,
                pagination.PageSize);
        }

        public async Task<PlayerDto?> GetPlayerByIdAsync(int id) {
            return await _context.Players
                .Where(p => p.Id == id)
                .ProjectTo<PlayerDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<PlayerDto> CreatePlayerAsync(CreatePlayerDto dto) {
            var player = _mapper.Map<Player>(dto);

            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            return _mapper.Map<PlayerDto>(player);
        }

        public async Task<bool> UpdatePlayerAsync(int id, UpdatePlayerDto dto) {
            var player = await _context.Players.FindAsync(id);

            if (player == null)
                return false;

            _mapper.Map(dto, player);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeletePlayerAsync(int id) {
            var player = await _context.Players.FindAsync(id);

            if (player == null)
                return false;

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}