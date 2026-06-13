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
    public class PlayerService : IPlayerService {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PlayerService(ApplicationDbContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<PlayerDto>> GetPlayersAsync(
    int? leagueId,
    int? teamId,
    PaginationParams pagination) {
            var query = _context.Players
                .AsNoTracking()
                .AsQueryable();

            // Filtrar por liga usando Player → Team
            if (leagueId.HasValue)
                query = query.Where(p => p.Team.LeagueId == leagueId.Value);

            // Filtrar por equipo
            if (teamId.HasValue)
                query = query.Where(p => p.TeamId == teamId.Value);

            // Proyección a DTO
            var dtoQuery = query
                .ProjectTo<PlayerDto>(_mapper.ConfigurationProvider);

            // SIN PAGINADO
            if (pagination.PageSize == 0) {
                var items = await dtoQuery
                    .OrderBy(x => x.JerseyNumber)
                    .ToListAsync();

                return new PagedResult<PlayerDto> {
                    Items = items,
                    PageNumber = 1,
                    PageSize = items.Count == 0 ? 1 : items.Count,
                    TotalCount = items.Count
                };
            }

            // CON PAGINADO
            return await dtoQuery.ToPagedResultAsync(
                q => q.OrderBy(x => x.Id),
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

            await _context.Entry(player)
                .Reference(p => p.Team)
                .LoadAsync();

            return _mapper.Map<PlayerDto>(player);
        }

        public async Task<bool> ImportPlayersAsync(int teamId, ImportPlayersDto dto) {

            var teamExists = await _context.Teams
                .AnyAsync(t => t.Id == teamId);

            if (!teamExists)
                return false;

            using var transaction =
                await _context.Database.BeginTransactionAsync();

            try {
                if (dto?.Players == null || !dto.Players.Any())
                    return false;
                // borrar jugadores actuales
                var existingPlayers =
                    await _context.Players
                        .Where(p => p.TeamId == teamId)
                        .ToListAsync();

                _context.Players.RemoveRange(existingPlayers);
                await _context.SaveChangesAsync();
                // mapear nuevos jugadores

                var newPlayers = _mapper.Map<List<Player>>(dto.Players);

                foreach (var player in newPlayers)
                    player.TeamId = teamId;

                await _context.Players.AddRangeAsync(newPlayers);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<PlayerDto?> UpdatePlayerAsync(int id, UpdatePlayerDto dto) {
            var player = await _context.Players.FindAsync(id);

            if (player == null)
                return null;

            _mapper.Map(dto, player);

            await _context.SaveChangesAsync();

            await _context.Entry(player)
                .Reference(p => p.Team)
                .LoadAsync();

            return _mapper.Map<PlayerDto>(player);
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