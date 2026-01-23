namespace SuperLeague.Models
{
    public class PlayerStats
    {
        public int PlayerId { get; set; }
        public string PlayerFirstName { get; set; } = string.Empty;
        public string PlayerLastName { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public string SeasonName { get; set; } = string.Empty;
        public int TotalGoals { get; set; }
        public int TotalAssists { get; set; }
        public int TotalYellows { get; set; }
        public int TotalReds { get; set; }
        public int TotalMinutes { get; set; }
        public int MatchesPlayed { get; set; }

    }
}