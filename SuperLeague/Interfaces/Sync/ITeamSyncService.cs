using SuperLeague.ExternalAPI.DTOs;

namespace SuperLeague.Interfaces.Sync
{
    public interface ITeamSyncService
    {
        Task<Dictionary<int, int>> SyncTeamsAsync(List<TeamApiResponse> apiTeams, int userId);
    }
}
