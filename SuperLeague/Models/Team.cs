using System.Data.SqlTypes;

namespace SuperLeague.Models
{
    public class Team : BaseEntity
    {
        public int TeamId { get; set; }
        public required string TeamName { get; set; }
        public required DateTime DateOfFoundation { get; set; }
        public required string Stadium { get; set; }
        public required string City { get; set; }
        
    }
}
