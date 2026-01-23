using SuperLeague.Models;

namespace SuperLeague.Interfaces
{
    public interface ILeagueRepository
    {
        Task<IEnumerable<LeagueTable_Team>> GetLeagueTableAsync(int seasonId);
        Task<IEnumerable<PlayerStats>> GetTopScorersAsync(int seasonId, int count);
        Task<IEnumerable<PlayerStats>> GetTopAssistersAsync(int seasonId, int count);
    }
}
