using System.Text.Json.Serialization;

namespace SuperLeague.Application.ExternalAPI.DTOs
{
    public class SquadApiResponse
    {
        [JsonPropertyName("team")]
        public TeamBasicApiDto Team { get; set; } = null!;

        [JsonPropertyName("players")]
        public List<PlayerApiDto> Players { get; set; } = new();
    }

    public class TeamBasicApiDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty!;
        [JsonPropertyName("logo")]
        public string Logo { get; set; } = null!;
    }

    public class PlayerApiDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("age")]
        public int? Age { get; set; }
        [JsonPropertyName("number")]
        public int? Number { get; set; }
        [JsonPropertyName("position")]
        public string Position { get; set; } = string.Empty;
        [JsonPropertyName("photo")]
        public string Photo { get; set; } = null!;

    }

    public class SquadListApiResponse
    {
        [JsonPropertyName("response")]
        public List<SquadApiResponse> Response { get; set; } = new();
    }
}
