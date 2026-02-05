// Services/Sync/PlayerTeamSyncService.cs
using SuperLeague.Domain.Models;
using SuperLeague.ExternalAPI.DTOs;
using SuperLeague.Interfaces.Repository;
using SuperLeague.Interfaces.Sync;

public class PlayerTeamSyncService : IPlayerTeamSyncService
{
    private readonly IPlayerTeamRepository _playerTeamRepository;
    private readonly ILogger<PlayerTeamSyncService> _logger;

    public PlayerTeamSyncService(
        IPlayerTeamRepository playerTeamRepository,
        ILogger<PlayerTeamSyncService> logger)
    {
        _playerTeamRepository = playerTeamRepository;
        _logger = logger;
    }

    public async Task SyncPlayerTeamLinksAsync(
        List<SquadApiResponse> squads,
        Dictionary<int, int> teamMapping,
        Dictionary<string, int> playerMapping,
        int season,
        int userId)
    {
        // ✅ UČITAJ SVE AKTIVNE VEZE JEDNOM
        var existingLinks = await _playerTeamRepository.GetAllActiveLinksAsync();
        var existingLinksSet = existingLinks
            .Select(pt => $"{pt.PlayerId}|{pt.TeamId}")
            .ToHashSet();

        var linksToCreate = new List<PlayerTeam>();

        foreach (var squad in squads)
        {
            if (!teamMapping.TryGetValue(squad.Team.Id, out var dbTeamId))
            {
                _logger.LogWarning("Team mapping not found for API TeamId {ApiTeamId}",
                    squad.Team.Id);
                continue;
            }

            foreach (var apiPlayer in squad.Players)
            {
                if (!playerMapping.TryGetValue($"{apiPlayer.Id}", out var dbPlayerId))
                {
                    _logger.LogWarning("Player mapping not found for API PlayerId {ApiPlayerId}",
                        apiPlayer.Id);
                    continue;
                }

                var linkKey = $"{dbPlayerId}|{dbTeamId}";

                if (!existingLinksSet.Contains(linkKey))
                {
                    linksToCreate.Add(new PlayerTeam
                    {
                        PlayerId = dbPlayerId,
                        TeamId = dbTeamId,
                        StartDate = DateTime.UtcNow,
                        EndDate = null
                    });

                    // Dodaj u cache da izbegnemo duplikate u istom batch-u
                    existingLinksSet.Add(linkKey);
                }
            }
        }

        if (linksToCreate.Any())
        {
            await _playerTeamRepository.BulkAddAsync(linksToCreate);
            _logger.LogInformation("Created {Count} new player-team links", linksToCreate.Count);
        }
    }
}