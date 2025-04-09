namespace CoreLibrary.Shared.Filters.ControllerFilterModels.FilterModels
{
    public class FilterModel
    {
        public required string PropertyName { get; set; }
        public required string Operator { get; set; }
        public required string? Value { get; set; }
    }
}
