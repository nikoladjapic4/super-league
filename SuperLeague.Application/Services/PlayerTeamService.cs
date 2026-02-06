using Microsoft.Extensions.Logging;
using SuperLeague.Domain.Entities;
using SuperLeague.Domain.Interfaces.Repositories;
using SuperLeague.Application.Services.Interfaces;

namespace SuperLeague.Application.Services
{
    public class PlayerTeamService : IPlayerTeamService
    {
        private readonly IPlayerTeamRepository _playerTeamRepository;
        private readonly ILogger<PlayerTeamService> _logger;

        public PlayerTeamService(IPlayerTeamRepository teamRepository, ILogger<PlayerTeamService> logger)
        {
            _playerTeamRepository = teamRepository;
            _logger = logger;
        }

        public async Task LinkPlayerToTeamAsync(int playerId, int teamId, int season, int userId)
        {
            var exists = await _playerTeamRepository.ExistsAsync(playerId, teamId);

            if (exists)
            {
                _logger.LogWarning("Player {PlayerId} is already linked to team {TeamId}", playerId, teamId);
                return;
            }


            var playerTeam = new PlayerTeam
            {
                PlayerId = playerId,
                TeamId = teamId,
                StartDate = DateTime.UtcNow,
                EndDate = null
            };

            await _playerTeamRepository.AddAsync(playerTeam);
            _logger.LogInformation("Linked player {PlayerId} to team {TeamId}", playerId, teamId);
        }
        public async Task<bool> ExistsAsync(int playerId, int teamId)
        {
            return await _playerTeamRepository.ExistsAsync(playerId, teamId);
        }


        public async Task TransferPlayerAsync(int playerId, int newTeamId, int userId)
        {
            var currentLink = await _playerTeamRepository.GetActiveByPlayerIdAsync(playerId);

            if (currentLink != null)
            {
                currentLink.EndDate = DateTime.UtcNow;

                await _playerTeamRepository.UpdateAsync(currentLink);
                _logger.LogInformation("Ended player {PlayerId} link to team {TeamId}", playerId, currentLink.TeamId);

                var newPlayerTeam = new PlayerTeam
                {
                    PlayerId = playerId,
                    TeamId = newTeamId,
                    StartDate = DateTime.UtcNow,
                    EndDate = null
                };

                await _playerTeamRepository.AddAsync(newPlayerTeam);
                _logger.LogInformation("Linked player {PlayerId} to team {TeamId}", playerId, newTeamId);
            }
        }
    }
}
