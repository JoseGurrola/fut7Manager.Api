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
    public class GroupService : IGroupService {

        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GroupService(ApplicationDbContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<GroupDto>> GetGroupsAsync(
            int? leagueId,
            PaginationParams pagination) {

            var query = _context.Groups
                .AsNoTracking()
                .AsQueryable();

            if (leagueId.HasValue)
                query = query.Where(g => g.LeagueId == leagueId.Value);

            var dtoQuery = query
                .ProjectTo<GroupDto>(_mapper.ConfigurationProvider);

            return await dtoQuery.ToPagedResultAsync(
                q => q.OrderBy(x => x.Id),
                pagination.PageNumber,
                pagination.PageSize);
        }

        public async Task<GroupDto?> GetGroupByIdAsync(int id) {
            return await _context.Groups
                .AsNoTracking()
                .Where(g => g.Id == id)
                .ProjectTo<GroupDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<GroupDto> CreateGroupAsync(CreateGroupDto dto) {
            var group = _mapper.Map<Group>(dto);

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            return _mapper.Map<GroupDto>(group);
        }

        public async Task<bool> DeleteGroupAsync(int id) {
            var group = await _context.Groups.FindAsync(id);

            if (group == null)
                return false;

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}