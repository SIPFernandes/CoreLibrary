using CoreLibrary.Shared.Filters.ControllerFilterModels.FilterModels;

namespace CoreLibrary.Shared.Filters.ControllerFilterModels
{
    public class UpdateWhereControllerFilter
    {
        public CombinedFilter? Filters { get; set; } = null;
        public required PropertyUpdateModel[] UpdateProperties { get; set; }
    }

    public class PropertyUpdateModel
    {
        public required string PropertyName { get; set; }
        public required string? Value { get; set; }
    }
}
