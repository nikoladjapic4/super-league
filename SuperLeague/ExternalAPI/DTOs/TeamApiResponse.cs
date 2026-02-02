using System.Text.Json.Serialization;

namespace SuperLeague.ExternalAPI.DTOs
{
    public class TeamApiResponse
    {
        [JsonPropertyName("team")]
        public TeamApiDto Team { get; set; } = null!;
        [JsonPropertyName("venue")]
        public VenueApiDto Venue { get; set; } = null!;
    }
    public class TeamApiDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
        [JsonPropertyName("code")]
        public string? Code { get; set; }
        [JsonPropertyName("country")]
        public string Country { get; set; } = null!;
        [JsonPropertyName("founded")]
        public int? Founded { get; set; }
        [JsonPropertyName("national")]
        public bool National { get; set; }
        [JsonPropertyName("logo")]
        public string Logo { get; set; } = null!;
    }

    public class VenueApiDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
        [JsonPropertyName("address")]
        public string Address { get; set; } = null!;
        [JsonPropertyName("city")]
        public string City { get; set; } = null!;
        [JsonPropertyName("capacity")]
        public int? Capacity { get; set; }
        [JsonPropertyName("surface")]
        public string Surface { get; set; } = null!;
        [JsonPropertyName("image")]
        public string Image { get; set; } = null!;
    }
    public class TeamListApiResponse
    {
        [JsonPropertyName("response")]
        public List<TeamApiResponse> Response { get; set; } = new();
    }


}
