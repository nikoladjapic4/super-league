using SuperLeague.Domain.Entities;

namespace SuperLeague.Domain.Interfaces.Repositories
{
    public interface IPlayerRepository
    {
        Task<IEnumerable<Player>> GetAllActiveAsync();
        Task<Player?> GetByIdAsync(int playerId);
        Task<int> AddAsync(Player player);
        Task<bool> UpdateAsync(Player player);
        Task<bool> ExistsAsync(string firstName, string lastName, DateTime birthDate, int? excludePlayerId = null);
    }
}