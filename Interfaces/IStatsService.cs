using NBAFantasy.Models;

namespace NBAFantasy.Interfaces
{
    public interface IStatsService
    {
        Task<PlayerStats?> GetPlayerStatsByID(string id);

        Task<PlayerStats?> GetAvgTeamStatsByTeam(string teamName);
    }
}
