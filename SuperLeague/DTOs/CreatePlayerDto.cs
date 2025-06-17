using System.ComponentModel.DataAnnotations;

namespace SuperLeague.DTOs
{
    public class CreatePlayerDto
    {
        [Required(ErrorMessage = "You didn't populate player first name.")]
        [StringLength(20, ErrorMessage = "Player first name cannot exceed 20 characters.")]
        public string PlayerFirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "You didn't populate player last name.")]
        [StringLength(20, ErrorMessage = "Player last name cannot exceed 20 characters.")]
        public string PlayerLastName { get; set; } = string.Empty;
        [Range(1, 99, ErrorMessage = "Jersey number must be between 1 and 99.")]
        public int JerseyNumber { get; set; }

        [Required(ErrorMessage = "You didn't enter nationality.")]
        [StringLength(50, ErrorMessage = "Nationality cannot exceed 50 characters.")]
        public string Nationality { get; set; } = string.Empty;

        [Required(ErrorMessage = "You didn't enter position.")]
        [StringLength(20, ErrorMessage = "Position cannot exceed 20 characters.")]
        public string Position { get; set; } = string.Empty;

        [Required(ErrorMessage = "You didn't enter birth date.")]
        public DateTime BirthDate { get; set; }

        public int TeamId { get; set; }
    }
}
