using NBAFantasy.Data;
using NBAFantasy.Interfaces;
using NBAFantasy.Models;
using System.Net.Http;
using System.Numerics;
using System.Text.Json;

namespace NBAFantasy.Services
{
    public class StatsService : IStatsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<StatsService> _logger;
        private readonly AppDbContext _context;
        private readonly IPlayerService _playerService;

        public StatsService(HttpClient httpClient, ILogger<StatsService> logger, AppDbContext context, IPlayerService playerService)
        {
            _httpClient = httpClient;
            _logger = logger;
            _context = context;
            _playerService = playerService;
        }

        public async Task<PlayerStats?> GetAvgTeamStatsByTeam(string teamName)
        {
            List<Player> players = await _playerService.GetPlayersByTeam(teamName);
            List<String> playerIDs = players.Select(p => p.id).ToList();

            PlayerStats teamAggregateStats = new PlayerStats
            {
                PlayerID = teamName,
                AvgPoints = 0,
                AvgRebounds = 0,
                AvgAssists = 0,
                FieldGoalPct = 0
            };

            // Batch all player stat calls
            var allPlayerStats = playerIDs.Select(GetPlayerStatsByID).ToList();
            var playerStatsList = await Task.WhenAll(allPlayerStats);

            // Count number of nulls
            var nullCount = playerStatsList.Count(ps => ps == null);
            _logger.LogInformation("Null Count: {nullCount}", nullCount);


            foreach (var playerStats in playerStatsList.Where(ps =>ps != null))
            {
                if (playerStats != null)
                {
                    // Aggregate stats here if needed
                    teamAggregateStats.AvgPoints += playerStats.AvgPoints;
                    teamAggregateStats.AvgRebounds += playerStats.AvgRebounds;
                    teamAggregateStats.AvgAssists += playerStats.AvgAssists;
                    teamAggregateStats.FieldGoalPct += playerStats.FieldGoalPct;
                }
            }
            // Divide by the number of players to get average team stats
            teamAggregateStats.AvgPoints /= (playerIDs.Count - nullCount);
            teamAggregateStats.AvgRebounds /= (playerIDs.Count - nullCount);
            teamAggregateStats.AvgAssists /= (playerIDs.Count - nullCount);
            teamAggregateStats.FieldGoalPct /= (playerIDs.Count - nullCount);
            return teamAggregateStats;
        }

        public async Task<PlayerStats?> GetPlayerStatsByID(string id)
        {
            string athleteAPI = $"https://site.api.espn.com/apis/common/v3/sports/basketball/nba/athletes/{id}";

            var response = await _httpClient.GetAsync(athleteAPI);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to fetch player stats from ESPN API.");
            }

            var content = await response.Content.ReadAsStringAsync();

            var playerStats = JsonDocument.Parse(content);
            JsonElement statsSummary;
            try
            {
                statsSummary = playerStats.RootElement.GetProperty("athlete").GetProperty("statsSummary").GetProperty("statistics");
            } catch
            {
                return null;
            }


            PlayerStats stats = new PlayerStats
            {
                PlayerID = id,
                AvgPoints = statsSummary[0].GetProperty("value").GetSingle(),
                AvgRebounds = statsSummary[1].GetProperty("value").GetSingle(),
                AvgAssists = statsSummary[2].GetProperty("value").GetSingle(),
                FieldGoalPct = statsSummary[3].GetProperty("value").GetSingle()
            };
            return stats;
        }
    }
}
