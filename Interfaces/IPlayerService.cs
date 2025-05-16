using NBAFantasy.Models;

namespace NBAFantasy.Interfaces
{
    public interface IPlayerService
    {
        Task PopulateActivePlayersAsync();
        Task<Player?> GetPlayerByNameAsync(string Player);
        Task<List<Player>> GetPlayersByTeam(string teamName);
    }
}
