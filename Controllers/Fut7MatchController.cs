using fut7Manager.Api.DTOs.Requests;
using fut7Manager.Api.DTOs.Responses;
using fut7Manager.Api.Models;
using fut7Manager.Api.Services;
using fut7Manager.Api.Services.Interfaces;
using fut7Manager.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using fut7Manager.Api.Extensions;

namespace fut7Manager.Api.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class Fut7MatchesController : ControllerBase {
        private readonly IFut7MatchService _matchService;

        public Fut7MatchesController(IFut7MatchService matchService) {
            _matchService = matchService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Fut7MatchDto>>> GetMatches(
    [FromQuery] PaginationParams pagination) {
            var result = await _matchService.GetMatchesAsync(pagination);

            Response.AddPaginationHeader(result);

            return Ok(result.Items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Fut7MatchDto>> GetMatch(int id) {
            var match = await _matchService.GetMatchByIdAsync(id);

            if (match == null)
                return NotFound();

            return Ok(match);
        }

        [HttpPost]
        public async Task<ActionResult<Fut7MatchDto>> CreateMatch(CreateFut7MatchDto dto) {
            var match = await _matchService.CreateMatchAsync(dto);

            return CreatedAtAction(nameof(GetMatch), new { id = match.Id }, match);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMatch(int id, UpdateFut7MatchDto dto) {
            var result = await _matchService.UpdateMatchAsync(id, dto);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMatch(int id) {
            var result = await _matchService.DeleteMatchAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}