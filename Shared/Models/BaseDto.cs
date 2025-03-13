using CoreLibrary.Core.Interfaces;
using System.ComponentModel;

namespace CoreLibrary.Shared.Models
{
    public abstract class BaseDto(Guid userId) : IBinderInterface
    {
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid Id { get; set; }
        public virtual Guid CreatorId { get; set; } = userId;
        public virtual Guid UpdatedById { get; set; } = userId;
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        [DefaultValue(null)]
        public DateTime? DeletedAt { get; set; } = null;
    }
}
