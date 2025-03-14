using CoreLibrary.Core.Entities;
using CoreLibrary.Shared.Filters.ServiceFilterModels;
using CoreLibrary.Shared.Models;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace CoreLibrary.Core.Interfaces
{
    public interface IGenericService<TRepository, TDto, TEntity>
        where TRepository : IRepository<TEntity>
        where TDto : BaseDto 
        where TEntity : BaseEntity, IAggregateRoot
    {
        Task<IEnumerable<TDto>> GetAll();
        Task<IEnumerable<object>> GetItemsFiltered(GetItemsServiceFilter<TEntity> model);
        Task<int> CountFiltered(Expression<Func<TEntity, bool>>? filter = null);
        Task<TDto?> GetFirstOrDefault();
        Task<object> GetFirstSelectFilter(GetSelectServiceFilter<TEntity> model);
        Task<TDto> Get(Guid id);
        Task<TDto> Get(Guid id, Expression<Func<TEntity, TDto>> selector);
        Task<object> GetSelectFilter(Guid id, GetSelectServiceFilter<TEntity> model);
        Task<TDto> Insert(TDto dto);
        Task<IEnumerable<TEntity>> BulkInsert(IEnumerable<TDto> list);
        Task<bool> Validate(TDto dto, object? validationObj = null);
        Task<TDto?> Update(Guid id, TDto dto);
        Task<IEnumerable<TEntity>?> BulkAddOrUpdate(IEnumerable<TDto> dtos);
        Task UpdatePropertiesInMultipleItems(Expression<Func<TEntity, bool>> expression,
            Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> setPropertyExpression);
        Task Delete(TDto dto);
        Task DeleteById(Guid id);
        Task BulkDeleteById(IEnumerable<Guid> id);
        Task DeleteWhere(Expression<Func<TEntity, bool>> expression);
    }
}
