using CoreLibrary.Core.Interfaces;
using System.ComponentModel;

namespace CoreLibrary.Shared.Models
{
    public abstract class BaseDto(Guid creatorId) : IBinderInterface
    {
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public virtual Guid Id { get; set; }
        [DefaultValue(false)]
        public bool IsDeleted { get; set; } = false;
        public virtual Guid CreatorId { get; set; } = creatorId;
        public virtual Guid UpdatedById { get; set; } = creatorId;
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
