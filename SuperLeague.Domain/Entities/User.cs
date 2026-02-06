namespace SuperLeague.Domain.Entities
{
    public class User
    {
        public int UserID { get; set; }
        public required string Username { get; set; }
        public required string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }



    }
}
