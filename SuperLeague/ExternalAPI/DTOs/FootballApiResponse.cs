using System.Text.Json;
using System.Text.Json.Serialization;

namespace SuperLeague.ExternalAPI.DTOs
{
    public class FootballApiResponse<T>
    {
        [JsonPropertyName("response")]
        public List<T> Response { get; set; } = new();

        [JsonPropertyName("errors")]
        public JsonElement? Errors { get; set; } 
    }
}
