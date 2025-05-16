using Microsoft.AspNetCore.Mvc;
using NBAFantasy.Interfaces;
using NBAFantasy.Models;

namespace NBAFantasy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase
    {

        private readonly IPlayerService _playerService;

        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpGet("GetPlayerByName")]
        public async Task<IActionResult> GetPlayerByName([FromQuery] string playerName)
        {
            Player? player = await _playerService.GetPlayerByNameAsync(playerName);
            if (player == null)
            {
                return NotFound($"Player with name {playerName} not found.");
            }
            return Ok(player);
        }
        
        [HttpGet("GetPlayerIDsByTeam")]
        public async Task<IActionResult> GetPlayerIDsByTeam([FromQuery] string teamName)
        {
            List<Player> players = await _playerService.GetPlayersByTeam(teamName);
            List<String> playerIds = players.Select(p => p.id).ToList();
            return Ok(playerIds);
        }

        [HttpGet("GetPlayersByTeam")]
        public async Task<IActionResult> GetPlayersByTeam([FromQuery] string teamName)
        {
            List<Player> players = await _playerService.GetPlayersByTeam(teamName);
            List<String> playerNames = players.Select(p => p.FullName).ToList();
            return Ok(players);
        }

        [HttpPost("PopulatePlayers")]
        public async Task<IActionResult> PopulatePlayers()
        {
            await _playerService.PopulateActivePlayersAsync();
            return Ok("Players populated Successfully");
        }
    }
}
