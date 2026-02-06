using SuperLeague.Application.DTOs.Player;

namespace SuperLeague.Application.Services.Interfaces
{
    public interface IPlayerService
    {
        Task<IEnumerable<PlayerDto>> GetAllPlayersAsync();
        Task<PlayerDto> GetPlayerByIdAsync(int playerId);
        Task<PlayerDto> CreatePlayerAsync(CreatePlayerDto dto, int createdBy);
        Task<PlayerDto> UpdatePlayerAsync(int playerId, UpdatePlayerDto dto, int updatedBy);
        Task<bool> DeletePlayerAsync(int playerId, int deletedBy);
        Task<bool> RestorePlayerAsync(int playerId);
    }
}