// DTOs/Team/TeamDto.cs - What you RETURN
namespace SuperLeague.DTOs.Team
{
    public class TeamDto
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public DateTime DateOfFoundation { get; set; }
        public string Stadium { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

