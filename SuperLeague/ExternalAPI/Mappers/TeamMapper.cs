using SuperLeague.ExternalAPI.DTOs;
using SuperLeague.Models;

namespace SuperLeague.ExternalAPI.Mappers
{
    public static class TeamMapper
    {
        public static Team MapToDomain(TeamApiResponse apiResponse, int createdBy)
        {
            return new Team
            {
                TeamName = TruncateString(apiResponse.Team.Name, 20),
                Stadium = TruncateString(apiResponse.Venue?.Name ?? "Unknown", 30),
                City = TruncateString(apiResponse.Venue?.City ?? "Unknown", 20),
                DateOfFoundation = apiResponse.Team.Founded.HasValue
                    ? new DateTime(apiResponse.Team.Founded.Value, 1, 1)
                    : new DateTime(1945, 1, 1),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy

            };
        }

        private static string TruncateString(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}
