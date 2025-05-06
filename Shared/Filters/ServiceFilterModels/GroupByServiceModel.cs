using System.Linq.Expressions;

namespace CoreLibrary.Shared.Filters.ServiceFilterModels
{
    public class GroupByServiceModel<TEntity>
    {
        public required Expression<Func<TEntity, object>> Expression;
        public Expression<Func<IGrouping<object, TEntity>, TEntity>>? Select;
    }
}
