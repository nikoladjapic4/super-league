namespace SuperLeague.Application.DTOs.Player
{
    public class PlayerDto
    {
        public int PlayerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int? JerseyNumber { get; set; }
        public string Nationality { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}