namespace CoreLibrary.Filters.ControllerFilterModels
{
    public class GetItemsControllerFilter
    {
        public SelectModel? Selector = null;
        public FilterModel? Filter { get; set; } = null;
        public CombinedFilter? CombinedFilters { get; set; } = null;
        public OrderByModel? OrderdBy { get; set; } = null;
        public int Skip = 0, Take = 10;
        public string[]? Includes = null;
    }
}
