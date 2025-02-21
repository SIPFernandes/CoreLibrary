using CoreLibrary.Shared.Filters.ControllerFilterModels.FilterModels;

namespace CoreLibrary.Shared.Filters.ControllerFilterModels
{
    public class GetSelectControllerFilter
    {
        public SelectModel? Selector { get; set; } = null;
        public string[]? Includes { get; set; } = null;
    }
}
