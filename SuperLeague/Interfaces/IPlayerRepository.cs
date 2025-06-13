using SuperLeague.Models;

namespace SuperLeague.Interfaces
{
    public interface IPlayerRepository
    {
        Task<IEnumerable<Player>> GetAllAsync(int teamId);

        //Task<Player?> GetBasicByIdAsync(int playerId);
        Task<IEnumerable<PlayerStats>> GetStatsByIdAsync(int playerId);
        Task AddAsync(Player player, int teamId);

    }
}
