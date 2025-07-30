using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CoreLibrary.Shared.Filters.ControllerFilterModels.FilterModels
{
    public class FilterModel
    {
        public required string PropertyName { get; set; }
        [DefaultValue(nameof(OperatorEnum.Equal))]
        [EnumDataType(typeof(OperatorEnum))]
        public required string Operator { get; set; }
        public required string? Value { get; set; }
    }

    public enum OperatorEnum
    {
        Equal, NotEqual, GreaterThan, GreaterThanOrEqual, LessThan, LessThanOrEqual, Contains, IsNull, IsNotNull
    }
}
