using AutoMapper;
using CoreLibrary.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace CoreLibrary.Core.Interfaces
{
    public interface IRepository<T> where T : BaseEntity, IAggregateRoot
    {
        Task<List<T>> GetAll(CancellationTokenSource? token = null);
        Task<List<W>> GetNItemsWhere<W>(Expression<Func<W, W>> selector,
            Expression<Func<W, bool>>? filter = null, int skip = 0, int take = 0,
            IEnumerable<string>? includes = null, CancellationTokenSource? token = null) where W : BaseEntity;
        Task<List<object>> GetNItemsWhere<W>(Expression<Func<W, object>> selector,
            Expression<Func<W, bool>>? filter = null, int skip = 0, int take = 0,
            IEnumerable<string>? includes = null, CancellationTokenSource? token = null)
            where W : BaseEntity;
        Task<List<W>> GetNItemsWhere<W>(Expression<Func<T, W>> selector,
            Expression<Func<T, bool>>? filter = null, int skip = 0, int take = 10,
            IEnumerable<string>? includes = null, CancellationTokenSource? token = null);
        Task<List<T>> GetNItemsWhere(Expression<Func<T, T>>? selector = null,
            Expression<Func<T, bool>>? filter = null, int skip = 0, int take = 10,
            IEnumerable<string>? includes = null, CancellationTokenSource? token = null);
        Task<List<T>> GetNItemsWhereOrdered<Z>(Expression<Func<T, Z>> orderedBy,
            bool descending, Expression<Func<T, bool>>? filter = null,
            int skip = 0, int take = 10, IEnumerable<string>? includes = null,
            CancellationTokenSource? token = null);
        Task<List<W>> GetNItemsWhereOrdered<W, Z>(Expression<Func<T, W>> selector,
            Expression<Func<T, Z>> orderedBy, bool descending, Expression<Func<T, bool>>? filter = null,
            int skip = 0, int take = 10, IEnumerable<string>? includes = null,
            CancellationTokenSource? token = null);
        Task<List<W>> GroupByWhereDistinct<W>(Expression<Func<T, object>> groupBy,
            Expression<Func<IGrouping<object, T>, W>> groupBySelect, Expression<Func<T, bool>>? filter = null,
            int skip = 0, int take = 0, CancellationTokenSource? token = null);
        Task<List<T>> GroupByWhereDistinct(Expression<Func<T, object>> groupBy,
            Expression<Func<T, bool>>? filter = null, int skip = 0, int take = 0,
            CancellationTokenSource? token = null);
        Task<int> Count(Expression<Func<T, bool>>? filter = null, CancellationTokenSource? token = null);
        Task<T> Get(Guid id);
        Task<T> Get(Guid id, IEnumerable<string>? includes = null);
        Task<W> Get<W>(Guid id, IEnumerable<string>? includes = null);
        Task<Z> GetAnyType<W, Z>(Guid id, IEnumerable<string>? includes = null)
            where W : BaseEntity where Z : class;
        Task<W> Get<W>(Guid id, Expression<Func<T, W>> selector, IEnumerable<string>? includes = null);
        Task<W> Get<W>(Guid id, Expression<Func<T, W>> selector);
        Task<T?> GetFirstOrDefault();
        Task<T> GetFirst(IEnumerable<string>? includes);
        Task<W> GetFirst<W>(Expression<Func<T, W>> selector, IEnumerable<string>? includes = null);
        Task<T> Insert(T entity, CancellationTokenSource? token = null);
        Task<IEnumerable<T>> BulkInsert(IEnumerable<T> entities, CancellationTokenSource? token = null);
        Task<T> Update(Guid id, T entity, CancellationTokenSource? token = null);
        Task<IEnumerable<W>> UpdateMultipleChilds<W>(IEnumerable<W> entitiesToUpdate,
            Expression<Func<W, bool>> expression, CancellationTokenSource? token = null,
            Action<DbSet<W>, W, W>? updateAction = null, bool deleteMissing = true) 
            where W : BaseEntity;
        Task<int> UpdateMultipleLeafType(Expression<Func<T, bool>>? expression,
            Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setPropertyExpression,
            CancellationTokenSource? token = null);
        Task Delete(T entity, CancellationTokenSource? token = null);
        Task<T> DeleteById(Guid id, CancellationTokenSource? token = null);
        Task DeleteMultiple(IEnumerable<T> list, CancellationTokenSource? token = null);
        Task<int> DeleteMultipleLeafType(Expression<Func<T, bool>> expression,
            CancellationTokenSource? token = null);
        Task<int> DeleteMultipleLeafType<W>(Expression<Func<W, bool>> expression,
            CancellationTokenSource? token = null) where W : BaseEntity;
    }
}
