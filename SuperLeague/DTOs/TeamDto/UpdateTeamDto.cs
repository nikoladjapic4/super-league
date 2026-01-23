
namespace SuperLeague.DTOs.Team
{
    public class UpdateTeamDto
    {
        public string TeamName { get; set; } = string.Empty;
        public DateTime DateOfFoundation { get; set; }
        public string Stadium { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public byte[] VersionRow { get; set; } = Array.Empty<byte>(); // For concurrency
    }
}