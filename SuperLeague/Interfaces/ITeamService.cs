
using SuperLeague.DTOs.Team;

namespace SuperLeague.Interfaces
{
    public interface ITeamService
    {
        Task<IEnumerable<TeamDto>> GetAllTeamsAsync();
        Task<TeamDto> GetTeamByIdAsync(int teamId);
        Task<TeamDto> CreateTeamAsync(CreateTeamDto dto, int createdBy);
        Task<TeamDto> UpdateTeamAsync(int teamId, UpdateTeamDto dto, int updatedBy);
        Task<bool> DeleteTeamAsync(int teamId, int deletedBy);
        Task<bool> RestoreTeamAsync(int teamId);
        Task LockTeamAsync(int teamId, int lockedBy);
        Task UnlockTeamAsync(int teamId, int userId);
    }
}
