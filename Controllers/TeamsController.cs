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
    public class TeamsController : ControllerBase {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService) {
            _teamService = teamService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamDto>>> GetTeams(
    [FromQuery] PaginationParams pagination) {
            var result = await _teamService.GetTeamsAsync(pagination);

            Response.AddPaginationHeader(result);

            return Ok(result.Items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDto>> GetTeam(int id) {
            var team = await _teamService.GetTeamByIdAsync(id);

            if (team == null)
                return NotFound();

            return Ok(team);
        }

        [HttpPost]
        public async Task<ActionResult<TeamDto>> CreateTeam(CreateTeamDto dto) {
            var team = await _teamService.CreateTeamAsync(dto);

            return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, team);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeam(int id, UpdateTeamDto dto) {
            var result = await _teamService.UpdateTeamAsync(id, dto);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id) {
            var result = await _teamService.DeleteTeamAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
