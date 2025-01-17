using System.Linq.Expressions;

namespace CoreLibrary.Filters.ServiceFilterModels
{
    public class OrderByServiceModel<TEntity, TDto>
    {
        public required Expression<Func<TEntity, TDto>> OrderBy;
        public bool Descending = false;
    }
}
