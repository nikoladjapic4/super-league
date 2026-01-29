using SuperLeague.DTOs.Player;
using SuperLeague.Exceptions;
using SuperLeague.Interfaces;
using SuperLeague.Models;
using SuperLeague.Repositories;

namespace SuperLeague.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IExternalApiService _externalApiService;
        private readonly ILogger<PlayerService> _logger;

        public PlayerService(
            IPlayerRepository playerRepository,
            IExternalApiService externalApiService,
            ILogger<PlayerService> logger)
        {
            _playerRepository = playerRepository;
            _externalApiService = externalApiService;
            _logger = logger;
        }

        public async Task<IEnumerable<PlayerDto>> GetAllPlayersAsync()
        {
            var players = await _playerRepository.GetAllActiveAsync();
            return players.Select(MapToDto);
        }

        public async Task<PlayerDto> GetPlayerByIdAsync(int playerId)
        {
            var player = await _playerRepository.GetByIdAsync(playerId);

            if (player == null || !player.IsActive)
            {
                throw new NotFoundException($"Igrač sa ID-om {playerId} nije pronađen");
            }

            return MapToDto(player);
        }

        public async Task<PlayerDto> CreatePlayerAsync(CreatePlayerDto dto, int createdBy)
        {
            var exists = await _playerRepository.ExistsAsync(dto.FirstName, dto.LastName, dto.BirthDate);
            if (exists)
            {
                throw new BusinessRuleException(
                    $"Igrač '{dto.FirstName} {dto.LastName}' rođen {dto.BirthDate:dd.MM.yyyy} već postoji");
            }

            var player = new Player
            {
                PlayerFirstName = dto.FirstName,
                PlayerLastName = dto.LastName,
                JerseyNumber = dto.JerseyNumber,
                Nationality = dto.Nationality,
                Position = dto.Position,
                BirthDate = dto.BirthDate,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy,
                IsActive = true
            };

            var newPlayerId = await _playerRepository.AddAsync(player);
            player.PlayerId = newPlayerId;

            _logger.LogInformation("Igrač kreiran: {PlayerId} - {FirstName} {LastName}",
                newPlayerId, player.PlayerFirstName, player.PlayerLastName);

            return MapToDto(player);
        }

        public async Task<PlayerDto> UpdatePlayerAsync(int playerId, UpdatePlayerDto dto, int updatedBy)
        {
            var player = await _playerRepository.GetByIdAsync(playerId);
            if (player == null || !player.IsActive)
            {
                throw new NotFoundException($"Igrač sa ID-om {playerId} nije pronađen");
            }

            if (player.LockedAt.HasValue && player.LockedBy.HasValue && player.LockedBy != updatedBy)
            {
                throw new PlayerLockedException(
                    $"Igrač je trenutno zaključan od strane drugog korisnika (ID: {player.LockedBy})");
            }

            var exists = await _playerRepository.ExistsAsync(dto.FirstName, dto.LastName, dto.BirthDate, playerId);
            if (exists)
            {
                throw new BusinessRuleException("Drugi igrač sa istim podacima već postoji");
            }

            player.PlayerFirstName = dto.FirstName;
            player.PlayerLastName = dto.LastName;
            player.JerseyNumber = dto.JerseyNumber;
            player.Nationality = dto.Nationality;
            player.Position = dto.Position;
            player.BirthDate = dto.BirthDate;
            player.VersionPlayer = dto.VersionPlayer;

            var success = await _playerRepository.UpdateAsync(player);
            if (!success)
            {
                throw new ConcurrencyException(
                    "Igrač je izmenjen od strane drugog korisnika. Molimo osvežite stranicu.");
            }

            _logger.LogInformation("Igrač ažuriran: {PlayerId}", playerId);

            var updatedPlayer = await _playerRepository.GetByIdAsync(playerId);
            return MapToDto(updatedPlayer!);
        }

        public async Task<bool> DeletePlayerAsync(int playerId, int deletedBy)
        {
            var player = await _playerRepository.GetByIdAsync(playerId);
            if (player == null)
            {
                throw new NotFoundException($"Igrač sa ID-om {playerId} nije pronađen");
            }

            if (!player.IsActive)
            {
                throw new BusinessRuleException("Igrač je već obrisan");
            }

            if (player.LockedAt.HasValue && player.LockedBy.HasValue && player.LockedBy != deletedBy)
            {
                throw new PlayerLockedException("Igrač je zaključan od strane drugog korisnika");
            }

            player.IsActive = false;
            player.DeletedAt = DateTime.UtcNow;
            player.DeletedBy = deletedBy;

            var success = await _playerRepository.UpdateAsync(player);

            if (success)
            {
                _logger.LogInformation("Igrač soft obrisan: {PlayerId}", playerId);
            }

            return success;
        }

        public async Task<bool> RestorePlayerAsync(int playerId)
        {
            var player = await _playerRepository.GetByIdAsync(playerId);
            if (player == null)
            {
                throw new NotFoundException($"Igrač sa ID-om {playerId} nije pronađen");
            }

            if (player.IsActive)
            {
                throw new BusinessRuleException("Igrač je već aktivan");
            }

            player.IsActive = true;
            player.DeletedAt = null;
            player.DeletedBy = null;

            var success = await _playerRepository.UpdateAsync(player);

            if (success)
            {
                _logger.LogInformation("Igrač vraćen: {PlayerId}", playerId);
            }

            return success;
        }
        public async Task LockPlayerAsync(int playerId, int lockedBy)
        {
            var player = await _playerRepository.GetByIdAsync(playerId);
            if (player == null || !player.IsActive)
            {
                throw new NotFoundException($"Tim sa ID-om {playerId} nije pronađen");
            }
            if (player.LockedAt.HasValue &&
                    player.LockedAt.Value < DateTime.UtcNow.AddMinutes(-15))
            {
                player.LockedAt = null;
                player.LockedBy = null;

            }

            if (player.LockedBy.HasValue)
            {
                throw new TeamLockedException(
                    $"Tim je već zaključan od strane korisnika (ID: {player.LockedBy})");
            }

            player.LockedAt = DateTime.UtcNow;
            player.LockedBy = lockedBy;

            await _playerRepository.UpdateAsync(player);

            _logger.LogInformation("Tim zaključan: {TeamId} od strane {UserId}", playerId, lockedBy);
        }
        public async Task UnlockPlayerAsync(int playerId, int userId)
        {
            var player = await _playerRepository.GetByIdAsync(playerId);
            if (player == null || !player.IsActive)
            {
                throw new NotFoundException($"Tim sa ID-om {playerId} nije pronađen");
            }

            if (!player.LockedAt.HasValue)
            {
                throw new BusinessRuleException("Igrac nije zaključan");
            }

            player.LockedAt = null;
            player.LockedBy = null;

            await _playerRepository.UpdateAsync(player);

            _logger.LogInformation("Igarc je otključan: {PlayerId}", playerId);
        }
        public async Task SyncPlayersAsync(int leagueId, int season)
        {
            _logger.LogInformation("Započinjem sinhronizaciju igrača za ligu {LeagueId}, sezona {Season}", leagueId, season);

            // Fetch actual DB column limits
            var columnLimits = await _playerRepository.GetColumnLengthsAsync();
            var teamNameLimit = columnLimits.GetValueOrDefault("TeamName", 50);
            var cityLimit = columnLimits.GetValueOrDefault("City", 50);
            var stadiumLimit = columnLimits.GetValueOrDefault("Stadium", 50);

            _logger.LogInformation("Column limits: TeamName={TeamNameLimit}, City={CityLimit}, Stadium={StadiumLimit}",
                teamNameLimit, cityLimit, stadiumLimit);

            var apiTeams = await _externalApiService.GetTeamsByLeagueAsync(leagueId, season);

            foreach (var apiTeam in apiTeams)
            {
                var teamName = Truncate(apiTeam.Team.Name, teamNameLimit);
                var city = Truncate(apiTeam.Venue.City, cityLimit);

                // Provera da li tim već postoji u bazi (koristimo postojeći ExistsAsync)
                var exists = await _teamRepository.ExistsAsync(teamName, city);

                if (!exists)
                {
                    var newTeam = new Team
                    {
                        TeamName = teamName,
                        City = city,
                        Stadium = Truncate(apiTeam.Venue.Name, stadiumLimit),
                        DateOfFoundation = apiTeam.Team.Founded.HasValue
                            ? new DateTime(apiTeam.Team.Founded.Value, 1, 1)
                            : new DateTime(1900, 1, 1),
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = 1, // System/Admin
                        IsActive = true
                    };

                    await _teamRepository.AddAsync(newTeam);
                    _logger.LogInformation("Dodat novi tim: {TeamName}", teamName);
                }
                else
                {
                    _logger.LogInformation("Tim već postoji: {TeamName}", teamName);
                    // Ovde bi mogao dodati i update logiku ako želiš da osvežavaš podatke
                }
            }

            _logger.LogInformation("Sinhronizacija završena.");
        }

        private static PlayerDto MapToDto(Player player)
        {
            return new PlayerDto
            {
                PlayerId = player.PlayerId,
                FirstName = player.PlayerFirstName,
                LastName = player.PlayerLastName,
                JerseyNumber = player.JerseyNumber,
                Nationality = player.Nationality,
                Position = player.Position,
                BirthDate = player.BirthDate,
                IsActive = player.IsActive,
                CreatedAt = player.CreatedAt
            };
        }
    }
}