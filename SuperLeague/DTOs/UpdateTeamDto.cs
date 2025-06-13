namespace SuperLeague.DTOs
{
    public class UpdateTeamDto
    {
        public required string TeamName { get; set; }
        public required DateTime DateOfFoundation { get; set; }
        public required string Stadium {  get; set; }
        public required string City {  get; set; }
        public required byte[] VersionRow { get; set; }
    }
}
