using SuperLeague.Application.ExternalAPI.DTOs;

namespace SuperLeague.Application.ExternalAPI.Interfaces
{
    public interface IFootballApiService
    {
        Task<List<TeamApiResponse>> GetTeamsByLeagueAsync (int leagueId, int season);
        Task<SquadApiResponse> GetSquadByTeamAsync (int teamId);
        Task<List<SquadApiResponse>> GetAllSquadsByLeagueAsync (int leagueId, int season);
    }
}
