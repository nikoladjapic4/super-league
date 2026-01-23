namespace SuperLeague.DTOs.Team
{
    public class CreateTeamDto
    {
        public string TeamName { get; set; } = string.Empty;
        public DateTime DateOfFoundation { get; set; }
        public string Stadium { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }
}