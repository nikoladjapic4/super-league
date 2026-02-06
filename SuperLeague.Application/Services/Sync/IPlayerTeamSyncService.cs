using SuperLeague.Application.ExternalAPI.DTOs;

namespace SuperLeague.Application.Services.Sync
{
    public interface IPlayerTeamSyncService
    {
        Task SyncPlayerTeamLinksAsync(
            List<SquadApiResponse> squads,Dictionary<int, int> teamMapping,Dictionary<string, int> playerMapping,
            int season,int userId);
    }
}
