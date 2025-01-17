using System.ComponentModel.DataAnnotations;

namespace CoreLibrary.Filters.ControllerFilterModels
{
    public class CombinedFilter
    {
        [MinLength(1)]
        public required List<FilterModel> Filters { get; set; }
        public FilterCombineOperatorEnum CombineOperator { get; set; } = FilterCombineOperatorEnum.And;
    }

    public enum FilterCombineOperatorEnum
    {
        And,
        Or
    }
}
