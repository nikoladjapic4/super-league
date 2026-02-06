using SuperLeague.Application.ExternalAPI.DTOs;

namespace SuperLeague.Application.Services.Sync
{
    public interface IPlayerSyncService
    {
        Task<Dictionary<string, int>> SyncPlayersAsync(List<SquadApiResponse> squads, int userId);
    }
}
