using SuperLeague.Application.ExternalAPI.DTOs;
using SuperLeague.Application.ExternalAPI.Interfaces;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text.Json;

namespace SuperLeague.Infrastructure.ExternalAPI.Services
{
    public class FootballApiService : IFootballApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FootballApiService> _logger;
        private const string BaseUrl = "https://v3.football.api-sports.io";

        public FootballApiService(
            HttpClient httpClient,
            ILogger<FootballApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        // Implementacija metoda iz IFootballApiService interfejsa
        public async Task<List<TeamApiResponse>> GetTeamsByLeagueAsync(int leagueId, int season)
        {
            try
            {
                var url = $"{BaseUrl}/teams?league={leagueId}&season={season}";
                _logger.LogInformation("Fetching team for league {LeagueId}, season {Season}", leagueId, season);

                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API error: {StatusCode}, Content: {Content}", response.StatusCode, content);
                    return new List<TeamApiResponse>();
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var apiResponse = JsonSerializer.Deserialize<FootballApiResponse<TeamApiResponse>>(content, options);

                if (apiResponse is null)
                {
                    _logger.LogWarning("Teams API returned empty or unexpected payload. Content: {Content}", content);
                    return new List<TeamApiResponse>();
                }

                if (apiResponse.Errors.HasValue && apiResponse.Errors.Value.ValueKind != JsonValueKind.Null)
                {
                    _logger.LogWarning("API returned errors: {Errors}", apiResponse.Errors.Value.ToString());
                }

                _logger.LogInformation("Successfully fetched {Count} teams", apiResponse.Response?.Count ?? 0);
                return apiResponse.Response ?? new List<TeamApiResponse>();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while fetching teams for league {LeagueId}, season {Season}", leagueId, season);
                throw;
            }

        }

        public async Task<SquadApiResponse?> GetSquadByTeamAsync(int teamId)
        {
            try
            {
                // corrected endpoint: "/players/squads?team={teamId}"
                var url = $"{BaseUrl}/players/squads?team={teamId}";
                _logger.LogInformation("Fetching squad for team {TeamId}", teamId);

                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API error: {StatusCode}, Content: {Content}", response.StatusCode, content);
                    return null;
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var apiResponse = JsonSerializer.Deserialize<FootballApiResponse<SquadApiResponse>>(content, options);

                if (apiResponse is null)
                {
                    _logger.LogWarning("Squad API returned empty or unexpected payload for team {TeamId}. Content: {Content}", teamId, content);
                    return null;
                }

                if (apiResponse.Errors.HasValue && apiResponse.Errors.Value.ValueKind != JsonValueKind.Null)
                {
                    _logger.LogWarning("API returned errors for team {TeamId}: {Errors}", teamId, apiResponse.Errors.Value.ToString());
                }

                return apiResponse.Response.FirstOrDefault();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while fetching players for team {TeamId}.", teamId);
                throw;
            }

        }

        public async Task<List<SquadApiResponse>> GetAllSquadsByLeagueAsync(int leagueId, int season)
        {
            var teams = await GetTeamsByLeagueAsync(leagueId, season);
            var squads = new List<SquadApiResponse>();

            foreach (var teamResponse in teams)
            {
                try
                {
                    
                    var squad = await GetSquadByTeamAsync(teamResponse.Team.Id);
                    if (squad != null)
                    {
                        squads.Add(squad);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to fetch squad for team {TeamId}", teamResponse.Team.Id);
                }
            }

            return squads;
        }
    }
}

