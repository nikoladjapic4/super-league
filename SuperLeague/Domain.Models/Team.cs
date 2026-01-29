namespace SuperLeague.Models
{
    public class Team
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public DateTime DateOfFoundation { get; set; }
        public string Stadium { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;

        // Soft Delete
        public bool IsActive { get; set; } = true;
        public DateTime? DeletedAt { get; set; }
        public int? DeletedBy { get; set; }

        // Concurrency
        public byte[] VersionRow { get; set; } = Array.Empty<byte>();

        // Locking
        public DateTime? LockedAt { get; set; }
        public int? LockedBy { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
    }
}
