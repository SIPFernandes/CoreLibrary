using CoreLibrary.Core.Entities;
using CoreLibrary.Core.Services;
using CoreLibrary.Shared.Models;
using System.Linq.Expressions;

namespace CoreLibrary.Core.Interfaces
{
    public interface IGenericService<TRepository, TDto, TEntity>
        where TRepository : IRepository<TEntity>
        where TDto : BaseDto 
        where TEntity : BaseEntity, IAggregateRoot
    {
        Task<IEnumerable<TDto>> GetAll();
        Task<IEnumerable<TDto>> GetItemsFiltered(GetItemsFilter<TDto, TEntity> model);
        Task<TDto?> Get(Guid id);
        Task<TDto?> Get(Guid id, Expression<Func<TEntity, TDto>> selector);
        Task<TDto> Insert(TDto dto);
        Task<IEnumerable<TEntity>> BulkInsert(IEnumerable<TDto> list);
        bool Validate(TDto dto, object? validationObj = null);
        Task<TDto?> Update(Guid id, TDto dto);
        Task<IEnumerable<TEntity>?> BulkAddOrUpdate(IEnumerable<TDto> dtos);
        Task Delete(TDto dto);
        Task DeleteById(Guid id);
        Task BulkDeleteById(IEnumerable<Guid> id);
        Task DeleteWhere(Expression<Func<TEntity, bool>> expression);
    }
}
