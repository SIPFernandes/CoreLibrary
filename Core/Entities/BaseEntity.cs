namespace CoreLibrary.Core.Entities
{
    public abstract class BaseEntity(Guid userId)
    {
        public virtual Guid Id { get; protected set; }
        public virtual Guid CreatorId { get; protected set; } = userId;
        public virtual Guid UpdatedById { get; protected set; } = userId;
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public DateTime? DeletedAt { get; set; } = null;
    }
}
