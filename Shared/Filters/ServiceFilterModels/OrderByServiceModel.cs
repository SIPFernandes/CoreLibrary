using System.Linq.Expressions;

namespace CoreLibrary.Shared.Filters.ServiceFilterModels
{
    public class OrderByServiceModel<TEntity>
    {
        public required Expression<Func<TEntity, object>> OrderBy;
        public bool Descending = false;
    }
}
