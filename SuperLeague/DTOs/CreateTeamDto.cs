using System.ComponentModel.DataAnnotations;

namespace SuperLeague.DTOs
{
    public class CreateTeamDto
    {
        [Required]
        public string TeamName { get; set; }

        [Required]
        public DateTime DateOfFoundation { get; set; }
        [Required]
        public string Stadium { get; set; }

        [Required]
        public string City { get; set; }
    }
}
