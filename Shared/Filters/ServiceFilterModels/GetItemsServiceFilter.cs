using CoreLibrary.Shared.Filters.ControllerFilterModels;
using System.Linq.Expressions;

namespace CoreLibrary.Shared.Filters.ServiceFilterModels
{
    public class GetItemsServiceFilter<TEntity>
    {
        public GetItemsServiceFilter() { }
        public GetItemsServiceFilter(GetItemsControllerFilter model)
        {
            if (model.Selector != null)
            {
                Selector = ExpressionHelper<TEntity>.GetSelectExpression(model.Selector.Properties);
            }

            if (model.Filter != null)
            {
                Filter = ExpressionHelper<TEntity>.BuildExpression(model.Filter);
            }

            if (model.CombinedFilters != null)
            {
                Filter = ExpressionHelper<TEntity>.CombineExpressions(model.CombinedFilters);
            }

            if (model.OrderdBy != null)
            {
                OrderedBy = new OrderByServiceModel<TEntity>()
                {
                    OrderBy = ExpressionHelper<TEntity>.OrderByProperty(model.OrderdBy.PropertyName),
                    Descending = model.OrderdBy.Descending
                };
            }

            Skip = model.Skip;
            Take = model.Take;
            Includes = model.Includes;
        }

        public Expression<Func<TEntity, IDictionary<string, object>>>? Selector = null;
        public Expression<Func<TEntity, bool>>? Filter = null;
        public OrderByServiceModel<TEntity>? OrderedBy = null;
        public int Skip = 0, Take = 10;
        public string[]? Includes = null;
        public CancellationTokenSource? Token = null;
    }
}
