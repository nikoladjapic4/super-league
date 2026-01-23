namespace SuperLeague.Models
{
    public class Player
    {
        public int PlayerId { get; set; }
        public string PlayerFirstName { get; set; } = string.Empty;
        public string PlayerLastName { get; set; } = string.Empty;
        public int? JerseyNumber { get; set; }
        public string Nationality { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }

        // Soft Delete
        public bool IsActive { get; set; } = true;
        public DateTime? DeletedAt { get; set; }
        public int? DeletedBy { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }

        // Locking
        public DateTime? LockedAt { get; set; }
        public int? LockedBy { get; set; }

        // Concurrency
        public byte[] VersionPlayer { get; set; } = Array.Empty<byte>();
    }
}