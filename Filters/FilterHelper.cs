﻿using CoreLibrary.Filters.ControllerFilterModels;
using System.Linq.Expressions;

namespace CoreLibrary.Filters
{
    public static class FilterHelper<T>
    {
        public static Expression<Func<T, bool>> BuildExpression(FilterModel filter)
        {
            var parameter = Expression.Parameter(typeof(T), "e");
            var property = Expression.Property(parameter, filter.PropertyName);
            var value = Expression.Constant(Convert.ChangeType(filter.Value, property.Type));

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
            Expression combinedExpression = BuildExpression(filters.Filters[0]).Body;
            var parameter = Expression.Parameter(typeof(T), "e");

            foreach (var filter in filters.Filters.Skip(1))
            {
                var filterExpression = BuildExpression(filter).Body;

                combinedExpression = filters.CombineOperator switch
                {
                    FilterCombineOperatorEnum.And => Expression.AndAlso(combinedExpression, filterExpression),
                    FilterCombineOperatorEnum.Or => Expression.OrElse(combinedExpression, filterExpression),
                    _ => throw new ArgumentException("Invalid logical operator. Use 'And' or 'Or'.", filters.CombineOperator.ToString()),
                };
            }

            return Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
        }

        public static Expression<Func<T, W>> OrderByProperty<W>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T), "e");
            var property = Expression.Property(parameter, propertyName);
            var expression = Expression.Convert(property, typeof(W));

            return Expression.Lambda<Func<T, W>>(expression, parameter);
        }

        public static Expression<Func<T, W>> GetSelectExpression<W>(IEnumerable<string> properties)
        {
            var parameter = Expression.Parameter(typeof(T), "e");
            var bindings = new List<MemberBinding>();

            foreach (var propertyName in properties)
            {
                var property = Expression.Property(parameter, propertyName);

                var propertyInfo = typeof(T).GetProperty(propertyName) ??
                    throw new ArgumentException($"Property name {propertyName} does not exist in object {typeof(T).FullName}");

                var binding = Expression.Bind(propertyInfo, property); bindings.Add(binding);
            }

            var body = Expression.MemberInit(Expression.New(typeof(T)), bindings);
            var lambda = Expression.Lambda<Func<T, W>>(body, parameter);

            return lambda;
        }
    }
}
