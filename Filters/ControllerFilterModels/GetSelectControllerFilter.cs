using CoreLibrary.Filters.ControllerFilterModels.FilterModels;

namespace CoreLibrary.Filters.ControllerFilterModels
{
    public class GetSelectControllerFilter
    {
        public SelectModel? Selector { get; set; } = null;
        public string[]? Includes { get; set; } = null;
    }
}
