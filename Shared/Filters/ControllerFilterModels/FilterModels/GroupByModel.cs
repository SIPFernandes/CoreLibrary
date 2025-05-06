namespace CoreLibrary.Shared.Filters.ControllerFilterModels.FilterModels
{
    public class GroupByModel
    {
        public required string GroupByColumnName { get; set; }
        public OrderByModel? OrderBy { get; set; }
    }
}
