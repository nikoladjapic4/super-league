using SuperLeague.Domain.Models;

namespace SuperLeague.Interfaces
{
    public class IPlayerTeamRepository
    {
        Task<int> AddAsync (PlayerTeam playerTeam);
        Task<bool> ExistsAsync(int playerId, int teamId);
        Task<PlayerTeam?> GetActiveByPlayerIdAsync(int playerId);
        Task UpdateAsync(PlayerTeam playerTeam);
    }
}
