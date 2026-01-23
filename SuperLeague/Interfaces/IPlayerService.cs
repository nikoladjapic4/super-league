using SuperLeague.DTOs.Player;

namespace SuperLeague.Interfaces
{
    public interface IPlayerService
    {
        Task<IEnumerable<PlayerDto>> GetAllPlayersAsync();
        Task<PlayerDto> GetPlayerByIdAsync(int playerId);
        Task<PlayerDto> CreatePlayerAsync(CreatePlayerDto dto, int createdBy);
        Task<PlayerDto> UpdatePlayerAsync(int playerId, UpdatePlayerDto dto, int updatedBy);
        Task<bool> DeletePlayerAsync(int playerId, int deletedBy);
        Task<bool> RestorePlayerAsync(int playerId);
        Task<Dictionary<string, int>> GetPlayerStatisticsByPositionAsync();
    }
}