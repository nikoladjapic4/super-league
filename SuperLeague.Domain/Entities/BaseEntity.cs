namespace SuperLeague.Domain.Entities
{
    public abstract class BaseEntity
    {
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }

        public DateTime? LockedAt { get; set; }
        public int? LockedBy { get; set; }

        public DateTime? DeletedAt { get; set; }
        public int? DeletedBy { get; set; }

        public byte[]? VersionRow { get; set; }
    }
}
