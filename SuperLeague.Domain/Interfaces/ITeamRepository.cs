using SuperLeague.Domain.Entities;

namespace SuperLeague.Domain.Interfaces.Repositories
{
    public interface ITeamRepository
    {
        Task<IEnumerable<Team>> GetAllActiveAsync();
        Task<Team?> GetByIdAsync(int teamId);
        Task<int> AddAsync(Team team); // Returns new TeamId
        Task<bool> UpdateAsync(Team team);
        Task<bool> ExistsAsync(string teamName, string city, int? excludeTeamId = null);
        Task<Dictionary<string, int>> GetColumnLengthsAsync();
    }
}