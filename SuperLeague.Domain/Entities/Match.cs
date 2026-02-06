namespace SuperLeague.Domain.Entities
{
    public class Match
    {
        public int MatchID { get; set; }
        public required DateTime MatchDate { get; set; }
        public required int GameweekRound { get; set; }
        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }

    }
}
