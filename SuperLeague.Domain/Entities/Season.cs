namespace SuperLeague.Domain.Entities
{
    public class Season
    {
        public int SeasonID { get; set; }
        public required string SeasonName { get; set; }
        public required DateTime? StartDate { get; set; }
        public required DateTime? EndDate { get; set; }
    }
}
