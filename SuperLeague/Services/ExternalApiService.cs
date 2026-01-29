using SuperLeague.DTOs;

namespace SuperLeague.Services
{
    public interface IExternalApiService
    {
        Task<List<TeamResponse>> GetTeamsByLeagueAsync(int leagueId, int season);
    }

    public class ExternalApiService : IExternalApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ExternalApiService> _logger;

        public ExternalApiService(
            HttpClient httpClient, 
            IConfiguration configuration, 
            ILogger<ExternalApiService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            // Postavljamo BaseAddress i Headere ovde ili u Program.cs preko HttpClientFactory-ja
            var apiKey = _configuration["FootballApi:Key"];
            if (!string.IsNullOrEmpty(apiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("x-apisports-key", apiKey);
            }
        }

        public async Task<List<TeamResponse>> GetTeamsByLeagueAsync(int leagueId, int season)
        {
            try
            {
                var url = $"https://v3.football.api-sports.io/teams?league={leagueId}&season={season}";
                
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("API Response ({StatusCode}): {Content}", response.StatusCode, content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API returned error status code: {StatusCode}", response.StatusCode);
                    return new List<TeamResponse>();
                }

                var apiResponse = System.Text.Json.JsonSerializer.Deserialize<FootballApiResponse<TeamResponse>>(content);

                if (apiResponse?.Errors != null)
                {
                    _logger.LogError("API business error: {Errors}", apiResponse.Errors);
                }

                return apiResponse?.Response ?? new List<TeamResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška prilikom pozivanja Football API-ja");
                throw;
            }
        }
    }
}
