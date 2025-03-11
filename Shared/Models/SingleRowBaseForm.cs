using CoreLibrary.Core.Interfaces;

namespace CoreLibrary.Shared.Models
{
    public abstract class SingleRowBaseForm : IBinderInterface
    {
        public virtual Guid UpdatedById { get; set; }
    }
}
