namespace CoreLibrary.Shared.Models
{
    public abstract class BaseForm
    {
        public required virtual Guid CreatorId { get; set; }
    }
}
