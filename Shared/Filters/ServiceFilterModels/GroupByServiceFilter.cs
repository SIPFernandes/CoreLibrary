using CoreLibrary.Shared.Filters.ControllerFilterModels;
using System.Linq.Expressions;

namespace CoreLibrary.Shared.Filters.ServiceFilterModels
{
    public class GroupByServiceFilter<TEntity>
    {
        public GroupByServiceFilter(GroupByControllerFilter model) 
        {
            GroupBy = new GroupByServiceModel<TEntity>()
            {
                Expression = ExpressionHelper<TEntity>.GroupByProperty(model.GroupBy.GroupByColumnName)
            };

            if (model.GroupBy.OrderBy != null)
            {
                GroupBy.Select = ExpressionHelper<TEntity>.GetGroupBySelectExpression(
                    model.GroupBy.OrderBy.PropertyName, model.GroupBy.OrderBy.Descending);
            }

            if (model.Filters != null)
            {
                Filter = ExpressionHelper<TEntity>.CombineExpressions(model.Filters);
            }

            if (model.SelectProperty != null)
            {
                Selector = ExpressionHelper<TEntity>.GetSelectExpression([model.SelectProperty]);
            }

            Skip = model.Skip;
            Take = model.Take;
        }

        public GroupByServiceModel<TEntity> GroupBy;
        public Expression<Func<TEntity, bool>>? Filter = null;
        public Expression<Func<TEntity, IDictionary<string, object>>>? Selector = null;
        public int Skip = 0, Take = 10;
        public CancellationTokenSource? Token = null;
    }
}
