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
    public class LeaguesController : ControllerBase {
        private readonly ILeagueService _leagueService;

        public LeaguesController(ILeagueService leagueService) {
            _leagueService = leagueService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeagueDto>>> GetLeagues(
    [FromQuery] PaginationParams pagination) {
            var result = await _leagueService.GetLeaguesAsync(pagination);

            Response.AddPaginationHeader(result);

            return Ok(result.Items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LeagueDto>> GetLeague(int id) {
            var league = await _leagueService.GetLeagueByIdAsync(id);

            if (league == null)
                return NotFound();

            return Ok(league);
        }

        [HttpPost]
        public async Task<ActionResult<LeagueDto>> CreateLeague(CreateLeagueDto dto) {
            var league = await _leagueService.CreateLeagueAsync(dto);

            return CreatedAtAction(nameof(GetLeague), new { id = league.Id }, league);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeague(int id) {
            var result = await _leagueService.DeleteLeagueAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
