using CoreLibrary.Shared.Filters.ControllerFilterModels.FilterModels;
using System.ComponentModel;

namespace CoreLibrary.Shared.Filters.ControllerFilterModels
{
    public class GroupByControllerFilter
    {
        public required GroupByModel GroupBy { get; set; }
        public CombinedFilter? Filters { get; set; } = null;
        public int Skip { get; set; } = 0;
        [DefaultValue(10)]
        public int Take { get; set; } = 10;
    }
}
