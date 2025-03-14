using CoreLibrary.Core.Interfaces;

namespace CoreLibrary.Shared.Models
{
    public abstract class BaseForm : IBinderInterface
    {
        public virtual Guid CreatorId { get; set; }
    }
}
