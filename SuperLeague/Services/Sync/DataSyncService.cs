// Services/Sync/DataSyncService.cs
using SuperLeague.ExternalAPI.DTOs;
using SuperLeague.ExternalAPI.Interfaces;
using SuperLeague.Interfaces.Sync;
using SuperLeague.Services.Sync;
using System.Text.Json;

public class DataSyncService : IDataSyncService
{
    private readonly IFootballApiService _footballApiService;
    private readonly ITeamSyncService _teamSyncService;
    private readonly IPlayerSyncService _playerSyncService;
    private readonly IPlayerTeamSyncService _playerTeamSyncService;
    private readonly ILogger<DataSyncService> _logger;

    public DataSyncService(
        IFootballApiService footballApiService,
        ITeamSyncService teamSyncService,
        IPlayerSyncService playerSyncService,
        IPlayerTeamSyncService playerTeamSyncService,
        ILogger<DataSyncService> logger)
    {
        _footballApiService = footballApiService;
        _teamSyncService = teamSyncService;
        _playerSyncService = playerSyncService;
        _playerTeamSyncService = playerTeamSyncService;
        _logger = logger;
    }

    public async Task SyncLeagueDataAsync(int leagueId, int season, int userId)
    {
        _logger.LogInformation("Starting data sync for league {LeagueId}, season {Season}",
            leagueId, season);

        try
        {
            var cacheFile = $"cache/squads_{leagueId}_{season}.json";
            List<SquadApiResponse> squads;

            if (File.Exists(cacheFile))
            {
                _logger.LogInformation("Loading squads from cache file");
                var json = await File.ReadAllTextAsync(cacheFile);
                squads = JsonSerializer.Deserialize<List<SquadApiResponse>>(json) ?? new List<SquadApiResponse>();

            }
            else
            {
                _logger.LogInformation("Fetching squads from API (this will take time due to rate limits)");
                squads = await _footballApiService.GetAllSquadsByLeagueAsync(leagueId, season);

                Directory.CreateDirectory("Cache");

                var json = JsonSerializer.Serialize(squads, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                await File.WriteAllTextAsync(cacheFile, json);
                _logger.LogInformation("Saved squads to cache file");
            }


            // 1. Fetch sve podatke odjednom
            var apiTeams = await _footballApiService.GetTeamsByLeagueAsync(leagueId, season);

            // 2. Sync timove i dobij mapping
            var teamMapping = await _teamSyncService.SyncTeamsAsync(apiTeams, userId);

            // 3. Sync igrače i dobij mapping
            var playerMapping = await _playerSyncService.SyncPlayersAsync(squads, userId);

            // 4. Sync Player-Team veze
            await _playerTeamSyncService.SyncPlayerTeamLinksAsync(
                squads, teamMapping, playerMapping, season, userId);

            _logger.LogInformation("Data sync completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Data sync failed");
            throw;
        }
    }
}