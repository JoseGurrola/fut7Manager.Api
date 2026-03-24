using fut7Manager.Api.DTOs.Requests;
using fut7Manager.Api.DTOs.Responses;
using fut7Manager.Api.Extensions;
using fut7Manager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fut7Manager.Api.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GroupsController : ControllerBase {

        private readonly IGroupService _groupService;

        public GroupsController(IGroupService groupService) {
            _groupService = groupService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupDto>>> GetGroups(
            [FromQuery] int? leagueId,
            [FromQuery] PaginationParams pagination) {

            var result = await _groupService.GetGroupsAsync(leagueId, pagination);

            Response.AddPaginationHeader(result);

            return Ok(result.Items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GroupDto>> GetGroup(int id) {
            var group = await _groupService.GetGroupByIdAsync(id);

            if (group == null)
                return NotFound();

            return Ok(group);
        }

        [HttpPost]
        public async Task<ActionResult<GroupDto>> CreateGroup(CreateGroupDto dto) {
            var group = await _groupService.CreateGroupAsync(dto);

            return CreatedAtAction(nameof(GetGroup), new { id = group.Id }, group);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id) {
            var result = await _groupService.DeleteGroupAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}