using Microsoft.AspNetCore.Mvc.ApiExplorer;
using SuperLeague.DTOs.Player;
using SuperLeague.DTOs.Team;
using SuperLeague.ExternalAPI.Interfaces;
using SuperLeague.ExternalAPI.Mappers;
using SuperLeague.Interfaces;
using System.Linq.Expressions;

namespace SuperLeague.Services.Sync
{
    public class DataSyncService : IDataSyncService
    {
        private readonly IFootballApiService _footballApiService;
        private readonly ITeamService _teamService;
        private readonly IPlayerService _playerService;
        private readonly ILogger<DataSyncService> _logger;

        public DataSyncService(
            IFootballApiService footballApiService,
            ITeamService teamService,
            IPlayerService playerService,
            ILogger<DataSyncService> logger)
        {
            _footballApiService = footballApiService;
            _teamService = teamService;
            _playerService = playerService;
            _logger = logger;
        }

        public async Task SyncLeagueDataAsync(int leagueId, int season, int userId)
        {
            _logger.LogInformation("Starting data sync for league {LeagueId}, season {Season}", leagueId, season);

            try
            {
                // 1. Fetch teams
                var apiTeams = await _footballApiService.GetTeamsByLeagueAsync(leagueId, season);
                _logger.LogInformation("Fetched {Count} teams from API", apiTeams.Count);

                // 2. Sync teams to database
                foreach (var apiTeam in apiTeams)
                {
                    try
                    {
                        var team = TeamMapper.MapToDomain(apiTeam, userId);

                        // Check if team already exists
                        var existingTeams = await _teamService.GetAllTeamsAsync();
                        var exists = existingTeams.Any(t =>
                            t.TeamName == team.TeamName &&
                            t.City == team.City);

                        if (!exists)
                        {
                            var createDto = new CreateTeamDto
                            {
                                TeamName = team.TeamName,
                                City = team.City,
                                Stadium = team.Stadium,
                                DateOfFoundation = team.DateOfFoundation
                            };

                            await _teamService.CreateTeamAsync(createDto, userId);
                            _logger.LogInformation("Created team: {TeamName}", team.TeamName);
                        }
                        else
                        {
                            _logger.LogInformation("Team already exists: {TeamName}", team.TeamName);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to sync team: {TeamName}",
                            apiTeam.Team.Name);
                    }
                }

                var squads = await _footballApiService.GetAllSquadsByLeagueAsync(leagueId, season);
                _logger.LogInformation("Fetched {Count} squads from API", squads.Count);

                // 3. Sync players to database
                foreach (var squad in squads)
                {
                    foreach (var apiPlayer in squad.Players)
                    {
                        try
                        {
                            var player = PlayerMapper.MapToDomain(apiPlayer, userId);

                            // Check if player exists
                            var allPlayers = await _playerService.GetAllPlayersAsync();
                            var exists = allPlayers.Any(x =>
                            x.FirstName == player.PlayerFirstName &&
                            x.LastName == player.PlayerLastName &&
                            x.BirthDate == player.BirthDate);

                            if (!exists)
                            {
                                var createDto = new CreatePlayerDto
                                {
                                    FirstName = player.PlayerFirstName,
                                    LastName = player.PlayerLastName,
                                    JerseyNumber = player.JerseyNumber,
                                    Position = player.Position,
                                    Nationality = player.Nationality,
                                    BirthDate = player.BirthDate,

                                };

                                await _playerService.CreatePlayerAsync(createDto, userId);
                                _logger.LogInformation("Created player: {FirstName} {LastName}", player.PlayerFirstName, player.PlayerLastName);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to sync player: {PlayerName}", apiPlayer.Name);
                        }

                    }
                    
                }



            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Data sync failed");
                throw;
            }
    }
    }
}