namespace SuperLeague.Application.Services.Sync;

public interface IDataSyncService
{
    Task SyncLeagueDataAsync(int leagueId, int season, int userId);
}
