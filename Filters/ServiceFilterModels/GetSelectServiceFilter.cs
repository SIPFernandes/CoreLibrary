using CoreLibrary.Filters.ControllerFilterModels;
using System.Linq.Expressions;

namespace CoreLibrary.Filters.ServiceFilterModels
{
    public class GetSelectServiceFilter<TEntity>
    {
        public GetSelectServiceFilter(GetSelectControllerFilter model) 
        {
            if (model.Selector != null)
            {
                Selector = FilterHelper<TEntity>.GetSelectExpression(model.Selector.Properties);
            }

            Includes = model.Includes;
        }

        public Expression<Func<TEntity, IDictionary<string, object>>>? Selector = null;
        public string[]? Includes = null;
    }
}
