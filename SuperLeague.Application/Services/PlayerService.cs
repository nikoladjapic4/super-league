using Microsoft.Extensions.Logging;
using SuperLeague.Application.DTOs.Player;
using SuperLeague.Application.Exceptions;
using SuperLeague.Domain.Interfaces.Repositories;
using SuperLeague.Application.Services.Interfaces;
using SuperLeague.Domain.Entities;

namespace SuperLeague.Application.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger<PlayerService> _logger;

        public PlayerService(
            IPlayerRepository playerRepository,
            ILogger<PlayerService> logger)
        {
            _playerRepository = playerRepository;
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