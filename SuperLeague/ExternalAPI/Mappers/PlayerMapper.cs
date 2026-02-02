using SuperLeague.ExternalAPI.DTOs;
using SuperLeague.Models;

namespace SuperLeague.ExternalAPI.Mappers
{
    public static class PlayerMapper
    {
        public static Player MapToDomain(PlayerApiDto apiPlayer, int createdBy)
        {
            var (firstName, lastName) = SplitName(apiPlayer.Name);

            return new Player
            {
                PlayerFirstName = TruncateString(firstName,20),
                PlayerLastName = TruncateString(lastName, 20),
                JerseyNumber = apiPlayer.Number,
                Position = MapPosition(apiPlayer.Position),
                BirthDate = apiPlayer.Age.HasValue && apiPlayer.Age.Value > 0
                    ? DateTime.UtcNow.AddYears(-apiPlayer.Age.Value)
                    : new DateTime(1990, 1, 1),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            };
        }

        private static (string firstName, string lastName) SplitName(string fullName)
        {
            var parts = fullName.Trim().Split(' ', 2);
            return parts.Length > 1
                ? (parts[0], parts[1])
                : (parts[0], "Unknown");
        }

        private static string MapPosition(string apiPosition)
        {
            return apiPosition switch
            {
                "Goalkeeper" => "GK",
                "Defender" => "DEF",
                "Midfielder" => "MID",
                "Forward" => "FWD",
                _ => "UNK"
            };
        }

        private static string TruncateString(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value.Length <= maxLength
                ? value
                : value.Substring(0, maxLength);
        }
    }
}
