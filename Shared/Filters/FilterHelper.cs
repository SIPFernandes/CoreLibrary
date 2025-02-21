using CoreLibrary.Shared.Filters.ControllerFilterModels.FilterModels;
using System.Linq.Expressions;

namespace CoreLibrary.Shared.Filters
{
    public static class FilterHelper<T>
    {
        public static Expression<Func<T, bool>> BuildExpression(FilterModel filter, ParameterExpression? parameter = null)
        {
            if (parameter is null)
            {
                parameter = Expression.Parameter(typeof(T), "e");
            }

            var property = Expression.Property(parameter, filter.PropertyName);

            var propertyType = property.Type;

            Expression value;

            if (propertyType == typeof(Guid))
            {
                value = Expression.Constant(Guid.Parse(filter.Value));
            }
            else
            {
                value = Expression.Constant(Convert.ChangeType(filter.Value, propertyType));
            }

            BinaryExpression comparison = filter.Operator switch
            {
                nameof(Expression.Equal) => Expression.Equal(property, value),
                nameof(Expression.NotEqual) => Expression.NotEqual(property, value),
                nameof(Expression.GreaterThan) => Expression.GreaterThan(property, value),
                nameof(Expression.GreaterThanOrEqual) => Expression.GreaterThan(property, value),
                nameof(Expression.LessThan) => Expression.GreaterThan(property, value),
                nameof(Expression.LessThanOrEqual) => Expression.GreaterThan(property, value),

                _ => throw new NotSupportedException($"Operator {filter.Operator} is not supported. " +
                $"Supported operators are: {nameof(Expression.Equal)}, {nameof(Expression.NotEqual)}, {nameof(Expression.GreaterThan)}," +
                $" {nameof(Expression.GreaterThanOrEqual)}, {nameof(Expression.LessThan)}, {nameof(Expression.LessThanOrEqual)}"),
            };
            return Expression.Lambda<Func<T, bool>>(comparison, parameter);
        }

        public static Expression<Func<T, bool>> CombineExpressions(CombinedFilter filters)
        {
            var parameter = Expression.Parameter(typeof(T), "e");

            Expression combinedExpression = BuildExpression(filters.Filters[0], parameter).Body;

            foreach (var filter in filters.Filters.Skip(1))
            {
                var filterExpression = BuildExpression(filter, parameter).Body;

                combinedExpression = filters.CombineOperator switch
                {
                    FilterCombineOperatorEnum.And => Expression.AndAlso(combinedExpression, filterExpression),
                    FilterCombineOperatorEnum.Or => Expression.OrElse(combinedExpression, filterExpression),
                    _ => throw new ArgumentException("Invalid logical operator. Use 'And' or 'Or'.", filters.CombineOperator.ToString()),
                };
            }

            return Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
        }

        public static Expression<Func<T, object>> OrderByProperty(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T), "e");
            var property = Expression.Property(parameter, propertyName);
            var expression = Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<T, object>>(expression, parameter);
        }

        public static Expression<Func<T, IDictionary<string, object>>> GetSelectExpression(IEnumerable<string> properties)
        {
            var parameter = Expression.Parameter(typeof(T), "e");

            var bindings = properties.Select(propertyName =>
            {
                var property = Expression.Property(parameter, propertyName);
                var converted = Expression.Convert(property, typeof(object));
                return new KeyValuePair<string, Expression>(propertyName, converted);
            }).ToList();

            var keyValuePairs = bindings.Select(b => Expression.ElementInit(
                typeof(Dictionary<string, object>).GetMethod("Add", [typeof(string), typeof(object)])!,
                Expression.Constant(b.Key),
                b.Value
            )).ToArray();

            var newExpression = Expression.ListInit(Expression.New(typeof(Dictionary<string, object>)), keyValuePairs);

            var lambda = Expression.Lambda<Func<T, IDictionary<string, object>>>(newExpression, parameter);
            return lambda;
        }
    }
}
