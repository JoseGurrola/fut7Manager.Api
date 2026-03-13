using fut7Manager.Api.DTOs.Requests;
using fut7Manager.Api.DTOs.Responses;
using fut7Manager.Api.Extensions;
using fut7Manager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fut7Manager.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PlayersController : ControllerBase {
        private readonly IPlayerService _playerService;

        public PlayersController(IPlayerService playerService) {
            _playerService = playerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> GetPlayers(
    [FromQuery] PaginationParams pagination) {
            var result = await _playerService.GetPlayersAsync(pagination);

            Response.AddPaginationHeader(result);

            return Ok(result.Items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerDto>> GetPlayer(int id) {
            var player = await _playerService.GetPlayerByIdAsync(id);

            if (player == null)
                return NotFound();

            return Ok(player);
        }

        [HttpPost]
        public async Task<ActionResult<PlayerDto>> CreatePlayer(CreatePlayerDto dto) {
            var player = await _playerService.CreatePlayerAsync(dto);

            return CreatedAtAction(
                nameof(GetPlayer),
                new { id = player.Id },
                player);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlayer(int id, UpdatePlayerDto dto) {
            var result = await _playerService.UpdatePlayerAsync(id, dto);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(int id) {
            var result = await _playerService.DeletePlayerAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}