using SuperLeague.Domain.Models;
namespace SuperLeague.Interfaces.Repository
{
    public interface IPlayerTeamRepository
    {
        Task<int> AddAsync (PlayerTeam playerTeam);
        Task<bool> ExistsAsync(int playerId, int teamId);
        Task<PlayerTeam?> GetActiveByPlayerIdAsync(int playerId);
        Task UpdateAsync(PlayerTeam playerTeam);
        Task<List<PlayerTeam>> GetAllActiveLinksAsync();
        Task BulkAddAsync(List<PlayerTeam> playerTeams);
    }
}
