using CoreLibrary.Shared.Filters.ControllerFilterModels;
using CoreLibrary.Shared.Filters.ControllerFilterModels.FilterModels;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace CoreLibrary.Shared.Filters
{
    public static class ExpressionHelper<T>
    {
        public static Expression<Func<T, bool>> BuildExpression(FilterModel filter, ParameterExpression? parameter = null)
        {
            parameter ??= Expression.Parameter(typeof(T), "e");

            var property = Expression.Property(parameter, filter.PropertyName);

            var propertyType = property.Type;

            Expression value = GetValueExpressionBody(filter.Value, propertyType);

            BinaryExpression comparison = filter.Operator switch
            {
                nameof(Expression.Equal) => Expression.Equal(property, value),
                nameof(Expression.NotEqual) => Expression.NotEqual(property, value),
                nameof(Expression.GreaterThan) => Expression.GreaterThan(property, value),
                nameof(Expression.GreaterThanOrEqual) => Expression.GreaterThanOrEqual(property, value),
                nameof(Expression.LessThan) => Expression.LessThan(property, value),
                nameof(Expression.LessThanOrEqual) => Expression.LessThanOrEqual(property, value),

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

        public static Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> BuildSetPropertyExpression(PropertyUpdateModel[] propertyValues)
        {
            var parameter = Expression.Parameter(typeof(SetPropertyCalls<T>), "e");
            Expression body = parameter;

            foreach (var item in propertyValues)
            {
                var propertyName = item.PropertyName;
                var value = item.Value;

                var setPropertyMethodDefinition = typeof(SetPropertyCalls<T>).GetMethods()
                    .FirstOrDefault(x => x.Name == "SetProperty" && x.GetParameters().Length == 2 &&
                    x.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>)) ??
                    throw new InvalidOperationException("SetProperty method not found.");

                var propertyType = (typeof(T).GetProperty(propertyName)?.PropertyType) ??
                    throw new InvalidOperationException($"Property '{propertyName}' not found on type '{typeof(T).Name}'.");

                var setPropertyMethod = setPropertyMethodDefinition.MakeGenericMethod(propertyType);

                var propertyParameter = Expression.Parameter(typeof(T), "x");
                var propertyExpression = Expression.Lambda(
                    Expression.Property(propertyParameter, propertyName),
                    propertyParameter
                );

                var valueParameter = Expression.Parameter(typeof(T), "x");

                var valueExpressionBody = GetValueExpressionBody(value, propertyType);

                var valueExpression = Expression.Lambda(valueExpressionBody, valueParameter);

                body = Expression.Call(body, setPropertyMethod, propertyExpression, valueExpression);
            }

            return Expression.Lambda<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>(body, parameter);
        }

        private static Expression GetValueExpressionBody(string value, Type propertyType)
        {
            Expression valueExpression;

            if (propertyType == typeof(Guid) || propertyType == typeof(Guid?))
            {
                if (Guid.TryParse(value, out Guid parsedValue))
                {
                    valueExpression = Expression.Constant(parsedValue, propertyType);
                }
                else
                {
                    valueExpression = Expression.Constant(null, propertyType);
                }
            }
            else
            {
                try
                {
                    var convertType = Convert.ChangeType(value, propertyType);

                    valueExpression = Expression.Constant(convertType);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to convert value '{value}' to type '{propertyType.Name}'.", ex);
                }
            }

            return valueExpression;
        }
    }
}
