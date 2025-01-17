using CoreLibrary.Filters.ControllerFilterModels;
using System.Linq.Expressions;

namespace CoreLibrary.Filters.ServiceFilterModels
{
    public class GetItemsServiceFilter<TEntity>
    {
        public GetItemsServiceFilter(GetItemsControllerFilter model)
        {
            if (model.Selector != null)
            {
                Selector = FilterHelper<TEntity>.GetSelectExpression(model.Selector.Properties);
            }

            if (model.Filter != null)
            {
                Filter = FilterHelper<TEntity>.BuildExpression(model.Filter);
            }

            if (model.CombinedFilters != null)
            {
                Filter = FilterHelper<TEntity>.CombineExpressions(model.CombinedFilters);
            }

            if (model.OrderdBy != null)
            {
                OrderedBy = new OrderByServiceModel<TEntity>()
                {
                    OrderBy = FilterHelper<TEntity>.OrderByProperty(model.OrderdBy.PropertyName),
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
