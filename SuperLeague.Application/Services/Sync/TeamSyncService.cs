using Microsoft.Extensions.Logging;
using SuperLeague.Application.DTOs.Team;
using SuperLeague.Application.Services.Interfaces;
using SuperLeague.Application.ExternalAPI.DTOs;
using SuperLeague.Application.ExternalAPI.Mappers;
using SuperLeague.Interfaces.Sync;

namespace SuperLeague.Application.Services.Sync
{
    public class TeamSyncService : ITeamSyncService
    {
        private readonly ITeamService _teamService;
        private readonly ILogger<TeamSyncService> _logger;

        public TeamSyncService(ITeamService teamService, ILogger<TeamSyncService> logger)
        {
            _teamService = teamService;
            _logger = logger;
        }

        public async Task<Dictionary<int, int>> SyncTeamsAsync(
            List<TeamApiResponse> apiTeams,
            int userId)
        {
            // ✅ UČITAJ SVE TIMOVE JEDNOM
            var existingTeams = await _teamService.GetAllTeamsAsync();
            var existingTeamsDict = existingTeams
                .ToDictionary(t => $"{t.TeamName}|{t.City}", t => t.TeamId);

            var teamMapping = new Dictionary<int, int>();

            foreach (var apiTeam in apiTeams)
            {
                try
                {
                    var team = TeamMapper.MapToDomain(apiTeam, userId);
                    var key = $"{team.TeamName}|{team.City}";

                    int dbTeamId;

                    if (existingTeamsDict.TryGetValue(key, out var existingTeamId))
                    {
                        dbTeamId = existingTeamId;
                        _logger.LogInformation("Team already exists: {TeamName} (ID: {TeamId})",
                            team.TeamName, dbTeamId);
                    }
                    else
                    {
                        var createDto = new CreateTeamDto
                        {
                            TeamName = team.TeamName,
                            City = team.City,
                            Stadium = team.Stadium,
                            DateOfFoundation = team.DateOfFoundation
                        };

                        var createdTeam = await _teamService.CreateTeamAsync(createDto, userId);
                        dbTeamId = createdTeam.TeamId;

                        // Dodaj u cache
                        existingTeamsDict[key] = dbTeamId;

                        _logger.LogInformation("Created team: {TeamName} (ID: {TeamId})",
                            team.TeamName, dbTeamId);
                    }

                    teamMapping[apiTeam.Team.Id] = dbTeamId;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to sync team: {TeamName}", apiTeam.Team.Name);
                }
            }

            return teamMapping;
        }
    }
}
