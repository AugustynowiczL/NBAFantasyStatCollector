using Microsoft.AspNetCore.Mvc;
using NBAFantasy.Interfaces;
using NBAFantasy.Models;

namespace NBAFantasy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatsController : ControllerBase
    {

        private readonly IPlayerService _playerService;
        private readonly IStatsService _statsService;

        public StatsController(IPlayerService playerService, IStatsService statsService)
        {
            _playerService = playerService;
            _statsService = statsService;
        }

        [HttpGet("GetPlayerStatsByID")]
        public async Task<IActionResult> GetPlayerStatsByID([FromQuery] string id)
        {
            PlayerStats? playerStats = await _statsService.GetPlayerStatsByID(id);
            if (playerStats == null)
            {
                return NotFound($"Player with ID {id} not found.");
            }
            return Ok(playerStats);
        }

        [HttpGet("GetAvgTeamStatsByTeam")]
        public async Task<IActionResult> GetAvgTeamStatsByTeam([FromQuery] string teamName)
        {
            PlayerStats? teamAggregateStats = await _statsService.GetAvgTeamStatsByTeam(teamName);
            if (teamAggregateStats == null)
            {
                return NotFound($"Unable to get team stats for team {teamName}.");
            }
            return Ok(teamAggregateStats);
        }

    }
}
