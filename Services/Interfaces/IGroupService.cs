using fut7Manager.Api.DTOs.Requests;
using fut7Manager.Api.DTOs.Responses;
using fut7Manager.Api.Helpers;

namespace fut7Manager.Api.Services.Interfaces {
    public interface IGroupService {
        Task<PagedResult<GroupDto>> GetGroupsAsync(int? leagueId, PaginationParams pagination);
        Task<GroupDto?> GetGroupByIdAsync(int id);
        Task<GroupDto> CreateGroupAsync(CreateGroupDto dto);
        Task<bool> DeleteGroupAsync(int id);
    }
}
