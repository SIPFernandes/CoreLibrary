using System.ComponentModel;

namespace CoreLibrary.Filters.ControllerFilterModels
{
    public class GetItemsControllerFilter
    {
        public SelectModel? Selector { get; set; } = null;
        public FilterModel? Filter { get; set; } = null;
        public CombinedFilter? CombinedFilters { get; set; } = null;
        public OrderByModel? OrderdBy { get; set; } = null;
        public int Skip { get; set; } = 0;
        [DefaultValue(10)]
        public int Take { get; set; } = 10;
        public string[]? Includes { get; set; } = null;
    }
}
