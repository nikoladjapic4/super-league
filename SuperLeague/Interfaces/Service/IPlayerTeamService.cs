namespace SuperLeague.Interfaces.Service
{
    public interface IPlayerTeamService
    {
        Task LinkPlayerToTeamAsync(int playerId, int teamId, int season, int userId);
        Task<bool> ExistsAsync(int playerId, int teamId);
        Task TransferPlayerAsync(int playerId, int newTeamId, int userId);

    }
}
