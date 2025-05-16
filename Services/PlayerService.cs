using Microsoft.EntityFrameworkCore;
using NBAFantasy.Data;
using NBAFantasy.Interfaces;
using NBAFantasy.Models;
using System.Text.Json;

namespace NBAFantasy.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PlayerService> _logger;
        private readonly AppDbContext _context;

        public PlayerService(HttpClient httpClient, ILogger<PlayerService> logger, AppDbContext context)
        {
            _httpClient = httpClient;
            _logger = logger;
            _context = context; 
        }

        public async Task<Player?> GetPlayerByNameAsync(string playerName)
        {
            // Split player first and last name
            var nameParts = playerName.Split('-');
            if (nameParts.Length != 2)
            {
                _logger.LogWarning("Invalid player name format: {PlayerName}", playerName);
                return null;
            }
            var firstName = nameParts[0].Trim().ToLower();
            var lastName = nameParts[1].Trim().ToLower();

            // Get player by name from the database
            Player? player = await _context.Players
                .Where(p => p.FirstName.ToLower() == firstName.ToLower() && p.LastName.ToLower() == lastName.ToLower())
                .FirstOrDefaultAsync();

            // Query player by ID from ESPN API
            _logger.LogInformation("Player: {PlayerName}, PlayerID: {PlayerNumber}", player.FullName, player.id);
            return player;
        }
        
        public async Task<List<Player>> GetPlayersByTeam(string teamName)
        {
            _logger.LogInformation("Team: {TeamName}", teamName);
            // Get Player Ids from the DB
            List<Player> players = await _context.Players
                .Where(p => p.TeamName.ToLower() == teamName.ToLower())
                .ToListAsync();
            _logger.LogInformation("Players: {Players}", players);
            return players;
        }
        
        public async Task PopulateActivePlayersAsync()
        {
            // ESPN Get Teams API
            string teamAPI = "https://site.api.espn.com/apis/site/v2/sports/basketball/nba/teams";

            var response = await _httpClient.GetAsync(teamAPI);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to fetch teams from ESPN API.");
            }

            var teamContent = await response.Content.ReadAsStringAsync();

            var jsonDoc = JsonDocument.Parse(teamContent);

            // Json of each team
            var teamsJson = jsonDoc.RootElement.GetProperty("sports")[0].GetProperty("leagues")[0].GetProperty("teams");
            
            foreach (var teamWrapper in teamsJson.EnumerateArray())
            {
                // Extract NBA team info
                var team = teamWrapper.GetProperty("team");
                string teamId = team.GetProperty("id").GetString() ?? "Missing Team Name";
                string teamName = team.GetProperty("displayName").GetString() ?? "Missing Team Name";
                _logger.LogInformation("Team: {TeamName}", teamName);

                // Get Team Roster
                string rosterAPI = $"https://site.api.espn.com/apis/site/v2/sports/basketball/nba/teams/{teamId}?enable=roster";
                response = await _httpClient.GetAsync(rosterAPI);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to fetch roster from ESPN API.");
                }
                var rosterContent = await response.Content.ReadAsStringAsync();
                var rosterDoc = JsonDocument.Parse(rosterContent);
                // Extract Roster JSON
                var rosterJson = rosterDoc.RootElement.GetProperty("team").GetProperty("athletes");

                var playersToInsert = new List<Player>();
                foreach (var rosterWrapper in rosterJson.EnumerateArray())
                {                     
                    // Extract Player info
                    var playerID = rosterWrapper.GetProperty("id").GetString() ?? "Missing Player ID";
                    var playerFirstName = rosterWrapper.GetProperty("firstName").GetString()?.ToLower() ?? "Missing Player First Name";
                    var playerLastName = rosterWrapper.GetProperty("lastName").GetString()?.ToLower() ?? "Missing Player Last Name";
                    var playerFullName = rosterWrapper.GetProperty("displayName").GetString()?.ToLower() ?? "Missing Player Name";
                    var playerTeamName = rosterWrapper.GetProperty("displayName").GetString()?.ToLower() ?? "Missing Player Name";
                    var newPlayer = new Player { id = playerID, FirstName = playerFirstName, LastName = playerLastName, FullName = playerFullName, TeamName = teamName};
                    playersToInsert.Add(newPlayer);
                    _logger.LogInformation("Player: {PlayerName}, Number: {PlayerNumber}", playerFullName, playerID);
                }
                // Batch insert all players on current team into DB
                _context.Players.AddRange(playersToInsert);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Inserted {Count} players.", playersToInsert.Count);
            }
        }
    }
}
