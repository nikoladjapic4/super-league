using SuperLeague.ExternalAPI.DTOs;

namespace SuperLeague.Interfaces.Sync
{
    public interface IPlayerSyncService
    {
        Task<Dictionary<string, int>> SyncPlayersAsync(List<SquadApiResponse> squads, int userId);
    }
}
