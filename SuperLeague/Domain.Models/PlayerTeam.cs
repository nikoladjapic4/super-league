namespace SuperLeague.Domain.Models
{
    public class PlayerTeam
    {
        public int PlayerTeamId { get; set; }
        public int PlayerId { get; set; }
        public int TeamId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
