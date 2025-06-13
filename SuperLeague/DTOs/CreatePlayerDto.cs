using Azure.Core;

namespace SuperLeague.DTOs
{
    public class CreatePlayerDto
    {
        public required string PlayerFirstName { get; set; }
        public required string PlayerLastName { get; set; }
        public int JerseyNumber { get; set; }
        public required string Nationality { get; set; }
        public required string Position { get; set; }
        public DateTime BirthDate { get; set; }
        public int TeamId { get; set; }

    }
}
