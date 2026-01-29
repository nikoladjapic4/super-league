using System.Text.Json.Serialization;

namespace SuperLeague.DTOs
{
    public class FootballApiResponse<T>
    {
        [JsonPropertyName("response")]
        public List<T> Response { get; set; } = new();

        [JsonPropertyName("errors")]
        public object Errors { get; set; } // Može biti niz ili objekat, pa koristimo object
    }

    public class TeamResponse
    {
        [JsonPropertyName("team")]
        public TeamDto Team { get; set; } = null!;

        [JsonPropertyName("venue")]
        public VenueDto Venue { get; set; } = null!;
    }

    public class TeamDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("founded")]
        public int? Founded { get; set; }

        [JsonPropertyName("logo")]
        public string Logo { get; set; } = string.Empty;
    }

    public class VenueDto
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("city")]
        public string City { get; set; } = string.Empty;

        [JsonPropertyName("capacity")]
        public int? Capacity { get; set; }
    }
}
