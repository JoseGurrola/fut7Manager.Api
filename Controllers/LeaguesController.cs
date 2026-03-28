using AutoMapper;
using fut7Manager.Api.DTOs.Requests;
using fut7Manager.Api.DTOs.Responses;
using fut7Manager.Api.Extensions;
using fut7Manager.Api.Models;
using fut7Manager.Api.Services;
using fut7Manager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fut7Manager.Api.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LeaguesController : ControllerBase {
        private readonly ILeagueService _leagueService;
        private readonly IScheduleService _scheduleService;
        private readonly IMapper _mapper;

        public LeaguesController(ILeagueService leagueService, IScheduleService scheduleService, IMapper mapper) {
            _leagueService = leagueService;
            _scheduleService = scheduleService;
            _mapper = mapper;
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

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLeague(int id, CreateLeagueDto dto) {
            var result = await _leagueService.UpdateLeagueAsync(id, dto);

            if (!result)
                return NotFound();

            return Ok();
        }


        [HttpPost("{id}/schedule")]
        public async Task<IActionResult> GenerateSchedule(int id, GenerateScheduleDto dto) {
            try {
                var matchdays = await _scheduleService.GenerateScheduleAsync(id, dto.InterGroupMatches);
                return Ok(matchdays);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
