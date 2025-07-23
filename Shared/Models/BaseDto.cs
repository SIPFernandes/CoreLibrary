using CoreLibrary.Core.Interfaces;
using System.ComponentModel;

namespace CoreLibrary.Shared.Models
{
    public abstract class BaseDto(Guid userId) : IBinderInterface
    {
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public virtual Guid Id { get; set; }
        [DefaultValue(false)]
        public bool IsDeleted { get; set; } = false;
        public virtual Guid CreatorId { get; set; } = userId;
        public virtual Guid UpdatedById { get; set; } = userId;
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
