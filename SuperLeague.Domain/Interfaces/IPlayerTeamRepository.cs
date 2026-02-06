using SuperLeague.Domain.Entities;
namespace SuperLeague.Domain.Interfaces.Repositories
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
