using SuperLeague.Models;

namespace SuperLeague.Interfaces
{
    public interface ITeamRepository
    {
        Task<IEnumerable<Team>> GetAllAsync();
        Task<Team?> GetByIdAsync(int teamId);
        Task AddAsync(Team team);
        Task<bool> UpdateAsync(Team team);
        Task<bool> SoftDeleteAsync(int teamId, byte[] versionRow, int deletedBy);
        Task<bool> RestoreAsync(int teamId);
        Task<bool> TeamExistingAsync(string teamName, string city);

    }
}
