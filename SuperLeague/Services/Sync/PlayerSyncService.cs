// Services/Sync/PlayerSyncService.cs
using SuperLeague.DTOs.Player;
using SuperLeague.ExternalAPI.DTOs;
using SuperLeague.ExternalAPI.Mappers;
using SuperLeague.Interfaces.Service;
using SuperLeague.Interfaces.Sync;

public class PlayerSyncService : IPlayerSyncService
{
    private readonly IPlayerService _playerService;
    private readonly ILogger<PlayerSyncService> _logger;

    public PlayerSyncService(IPlayerService playerService, ILogger<PlayerSyncService> logger)
    {
        _playerService = playerService;
        _logger = logger;
    }

    public async Task<Dictionary<string, int>> SyncPlayersAsync(
        List<SquadApiResponse> squads,
        int userId)
    {
        // ✅ UČITAJ SVE IGRAČE JEDNOM
        var existingPlayers = await _playerService.GetAllPlayersAsync();
        var existingPlayersDict = existingPlayers
            .ToDictionary(
                p => $"{p.FirstName}|{p.LastName}|{p.BirthDate:yyyy-MM-dd}",
                p => p.PlayerId);

        var playerMapping = new Dictionary<string, int>(); // Key: API player unique identifier

        foreach (var squad in squads)
        {
            foreach (var apiPlayer in squad.Players)
            {
                try
                {
                    var player = PlayerMapper.MapToDomain(apiPlayer, userId);
                    var key = $"{player.PlayerFirstName}|{player.PlayerLastName}|{player.BirthDate:yyyy-MM-dd}";

                    int dbPlayerId;

                    if (existingPlayersDict.TryGetValue(key, out var existingPlayerId))
                    {
                        dbPlayerId = existingPlayerId;
                        _logger.LogInformation("Player already exists: {Name} (ID: {PlayerId})",
                            $"{player.PlayerFirstName} {player.PlayerLastName}", dbPlayerId);
                    }
                    else
                    {
                        var createDto = new CreatePlayerDto
                        {
                            FirstName = player.PlayerFirstName,
                            LastName = player.PlayerLastName,
                            JerseyNumber = player.JerseyNumber,
                            Position = player.Position,
                            Nationality = player.Nationality,
                            BirthDate = player.BirthDate
                        };

                        var createdPlayer = await _playerService.CreatePlayerAsync(createDto, userId);
                        dbPlayerId = createdPlayer.PlayerId;

                        // Dodaj u cache
                        existingPlayersDict[key] = dbPlayerId;

                        _logger.LogInformation("Created player: {Name} (ID: {PlayerId})",
                            $"{player.PlayerFirstName} {player.PlayerLastName}", dbPlayerId);
                    }

                    // Mapiranje: API player ID -> DB player ID
                    playerMapping[$"{apiPlayer.Id}"] = dbPlayerId;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to sync player: {PlayerName}", apiPlayer.Name);
                }
            }
        }

        return playerMapping;
    }
}