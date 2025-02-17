namespace CoreLibrary.Core.Entities
{
    public abstract class BaseEntity(Guid creatorId)
    {
        public virtual Guid Id { get; protected set; }
        public bool IsDeleted { get; set; } = false;
        public virtual Guid CreatorId { get; protected set; } = creatorId;
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
