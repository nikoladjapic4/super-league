using System.ComponentModel.DataAnnotations;

namespace SuperLeague.DTOs
{
    public class CreateTeamDto
    {
        [Required(ErrorMessage = "You didn't populate team name.")]
        [StringLength(20, ErrorMessage = "Team cannot exceed 20 characters.")]
        public string TeamName { get; set; } = string.Empty;

        [Required(ErrorMessage = "You didn't populate team date of foundation.")]
        public DateTime DateOfFoundation { get; set; } 
        [Required(ErrorMessage = "You didn't enter team stadium.")]
        [StringLength(30, ErrorMessage = "Stadium cannot exceed 30 characters.")]
        public string Stadium { get; set; } = string.Empty;

        [Required(ErrorMessage = "You didn't enter team city.")]
        [StringLength(20, ErrorMessage = "Team cannot exceed 20 characters.")]
        public string City { get; set; } = string.Empty;
    }
}
