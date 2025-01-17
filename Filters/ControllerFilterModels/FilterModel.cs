namespace CoreLibrary.Filters.ControllerFilterModels
{
    public class FilterModel
    {
        public required string PropertyName { get; set; }
        public required string Operator { get; set; }
        public required string Value { get; set; }
    }
}
