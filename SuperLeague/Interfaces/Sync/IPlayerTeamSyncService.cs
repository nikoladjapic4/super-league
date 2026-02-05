using SuperLeague.ExternalAPI.DTOs;

namespace SuperLeague.Interfaces.Sync
{
    public interface IPlayerTeamSyncService
    {
        Task SyncPlayerTeamLinksAsync(
            List<SquadApiResponse> squads,Dictionary<int, int> teamMapping,Dictionary<string, int> playerMapping,
            int season,int userId);
    }
}
