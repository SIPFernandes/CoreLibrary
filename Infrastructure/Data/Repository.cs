using AutoMapper;
using AutoMapper.QueryableExtensions;
using CoreLibrary.Core.Entities;
using CoreLibrary.Core.Exceptions;
using CoreLibrary.Core.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace CoreLibrary.Infrastructure.Data
{
    public abstract class Repository<T, Context>(IDbContextFactory<Context> dbContextFact,
        IMapper mapper, ILogger<Repository<T, Context>> logger) : IRepository<T> 
        where T : BaseEntity, IAggregateRoot 
        where Context : DbContext
    {
        protected readonly IDbContextFactory<Context> _dbContextFact = dbContextFact;
        protected readonly IMapper _mapper = mapper;
        protected readonly ILogger<Repository<T, Context>> _logger = logger;

        public virtual async Task DeleteMultiple(IEnumerable<T> list,
            CancellationTokenSource? token = null)
        {
            using var context = await _dbContextFact.CreateDbContextAsync();

            var entities = context.Set<T>();

            entities.RemoveRange(list);

            await SaveChangesAsync(context, token);
        }

        public async Task DeleteMultipleLeafType(Expression<Func<T, bool>> expression,
            CancellationTokenSource? token = null)
        {
            using var context = await _dbContextFact.CreateDbContextAsync();

            var entities = context.Set<T>();

            await entities.Where(expression).ExecuteDeleteAsync();
        }

        public async Task DeleteMultipleLeafType<W>(Expression<Func<W, bool>> expression,
            CancellationTokenSource? token = null)
            where W : BaseEntity
        {
            using var context = await _dbContextFact.CreateDbContextAsync();

            var entities = context.Set<W>();

            await entities.Where(expression).ExecuteDeleteAsync();
        }

        public virtual async Task Delete(T? entity, CancellationTokenSource? token = null)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using var context = await _dbContextFact.CreateDbContextAsync();

            var entities = context.Set<T>();

            entities.Remove(entity);

            await SaveChangesAsync(context, token);
        }

        public virtual async Task<T> DeleteById(Guid id, CancellationTokenSource? token = null)
        {
            var entity = await Get(id);

            await Delete(entity, token);

            return entity!;
        }

        public async Task<T> Get(Guid id, IEnumerable<string>? includes = null)
        {
            using var context = await _dbContextFact.CreateDbContextAsync();

            var entities = context.Set<T>().AsQueryable();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    entities = entities.Include(include);
                }
            }

            var entity = await entities.SingleOrDefaultAsync(x => x.Id == id);

            if (entity != null)
            {
                return entity;
            }
            else
            {
                throw new EntityDoesNotExistException();
            }
        }

        public async Task<W> Get<W>(Guid id, IEnumerable<string>? includes = null)
        {
            using var context = await _dbContextFact.CreateDbContextAsync();

            var entities = context.Set<T>().AsQueryable();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    entities = entities.Include(include);
                }
            }

            var entity = await entities.Where(x => x.Id == id)
                .ProjectTo<W>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

            if (entity != null)
            {
                return entity;
            }
            else
            {
                throw new EntityDoesNotExistException();
            }
        }

        public async Task<Z> GetChild<W, Z>(Guid id, IEnumerable<string>? includes = null)
            where W : BaseEntity
            where Z : class
        {
            using var context = await _dbContextFact.CreateDbContextAsync();

            var entities = context.Set<W>().AsQueryable();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    entities = entities.Include(include);
                }
            }

            var entity = await entities.Where(x => x.Id == id)
                .ProjectTo<Z>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

            if (entity != null)
            {
                return entity;
            }
            else
            {
                throw new EntityDoesNotExistException();
            }
        }

        public async Task<W> Get<W>(Guid id, Expression<Func<T, W>> selector, IEnumerable<string>? includes = null)
        {
            using var context = await _dbContextFact.CreateDbContextAsync();

            var entities = context.Set<T>().AsQueryable();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    entities = entities.Include(include);
                }
            }

            var entity = await entities.Where(x => x.Id == id)
                .Select(selector)
                .SingleOrDefaultAsync();

            if (entity != null)
            {
                return entity;
            }
            else
            {
                throw new EntityDoesNotExistException();
            }
        }

        public async Task<T> Get(Guid id)
        {
            using var context = await _dbContextFact.CreateDbContextAsync();

            var entities = context.Set<T>();

            var entity = await entities.FindAsync(id);

            if (entity != null)
            {
                return entity;
            }
            else
            {
                throw new EntityDoesNotExistException();
            }
        }

        public async Task<W> Get<W>(Guid id, Expression<Func<T, W>> selector)
        {
            using var context = await _dbContextFact.CreateDbContextAsync();

            var entities = context.Set<T>();

            var entity = await entities.Where(x => x.Id == id)
                .Select(selector)
                .SingleOrDefaultAsync();

            if (entity != null)
            {
                return entity;
            }
            else
            {
                throw new EntityDoesNotExistException();
            }
        }

        public async Task<T> GetFirst()
        {
            using var context = await _dbContextFact.CreateDbContextAsync();

            var entities = context.Set<T>();

            return entities.First();
        }

        public async Task<T> GetFirst(IEnumerable<string>? includes)
        {
            using var context = await _dbContextFact.CreateDbContextAsync();

            var entities = context.Set<T>()
                .AsQueryable();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    entities = entities.Include(include);
                }
            }

            return entities.First();
        }

        public async Task<W> GetFirst<W>(Expression<Func<T, W>> selector, IEnumerable<string>? includes = null)
        {
            using var context = await _dbContextFact.CreateDbContextAsync();

            var entities = context.Set<T>()
                .AsQueryable();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    entities = entities.Include(include);
                }
            }

            return await entities.Select(selector).FirstAsync();
        }

        public async Task<List<T>> GetAll(CancellationTokenSource? token = null)
        {
            using var context = await _dbContextFact.CreateDbContextAsync();

            var entities = context.Set<T>();

            return await ToListAsync(entities, token);
        }

        public async Task<List<W>> GetNItemsWhere<W>(Expression<Func<W, W>> selector,
            Expression<Func<W, bool>>? filter = null, int skip = 0, int take = 0,
            IEnumerable<string>? includes = null, CancellationTokenSource? token = null) where W : BaseEntity
        {
            using var context = await _dbContextFact.CreateDbContextAsync();

            var entities = context.Set<W>().AsQueryable();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    entities = entities.Include(include);
                }
            }

            var query = filter == null ? entities : entities.Where(filter);

            return await GetItemsList(query, selector, skip, take, token);
        }

        public async Task<List<object>> GetNItemsWhere<W>(Expression<Func<W, object>> selector,
            Expression<Func<W, bool>>? filter = null, int skip = 0, int take = 0,
            IEnumerable<string>? includes = null, CancellationTokenSource? token = null) 
            where W : BaseEntity
        {
            using var context = await _dbContextFact.CreateDbContextAsync();

            var entities = context.Set<W>().AsQueryable();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    entities = entities.Include(include);
                }
            }

            var query = filter == null ? entities.Select(selector) 
                : entities.Where(filter).Select(selector);

            return await GetItemsList(query, skip, take, token);
        }

        public async Task<List<W>> GetNItemsWhere<W>(Expression<Func<T, W>> selector,
            Expression<Func<T, bool>>? filter = null, int skip = 0, int take = 10,
            IEnumerable<string>? includes = null, CancellationTokenSource? token = null)
        {
            using var context = await _dbContextFact.CreateDbContextAsync();

            var entities = context.Set<T>().AsQueryable();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    entities = entities.Include(include);
                }
            }

            var query = filter == null ? entities : entities.Where(filter);

            return await GetItemsList(query, selector, skip, take, token);
        }

        public async Task<List<T>> GetNItemsWhereOrdered<Z>(Expression<Func<T, Z>> orderedBy,
            bool descending, Expression<Func<T, bool>>? filter = null,
            int skip = 0, int take = 10, IEnumerable<string>? includes = null,
            CancellationTokenSource? token = null)
        {
            using var context = await _dbContextFact.CreateDbContextAsync();

            var entities = context.Set<T>().AsQueryable();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    entities = entities.Include(include);
                }
            }

            var query = filter == null ? entities : entities.Where(filter);

            return await GetItemsListOrdered(query, orderedBy, descending, null, skip, take, token);
        }

        public async Task<List<W>> GetNItemsWhereOrdered<W, Z>(Expression<Func<T, W>> selector,
            Expression<Func<T, Z>> orderedBy, bool descending, Expression<Func<T, bool>>? filter = null,
            int skip = 0, int take = 10,
            IEnumerable<string>? includes = null, CancellationTokenSource? token = null)
        {
            using var context = await _dbContextFact.CreateDbContextAsync();

            var entities = context.Set<T>().AsQueryable();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    entities = entities.Include(include);
                }
            }

            var query = filter == null ? entities : entities.Where(filter);

            return await GetItemsListOrdered(query, selector, orderedBy, descending, skip, take, token);
        }

        public async Task<List<T>> GetNItemsWhere(Expression<Func<T, T>>? selector = null,
            Expression<Func<T, bool>>? filter = null, int skip = 0, int take = 10,
            IEnumerable<string>? includes = null, CancellationTokenSource? token = null)
        {
            using var context = await _dbContextFact.CreateDbContextAsync();

            var entities = context.Set<T>().AsQueryable();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    entities = entities.Include(include);
                }
            }

            var query = filter == null ? entities : entities.Where(filter);

            return await GetItemsList(query, selector, skip, take, token);
        }

        public virtual async Task<T> Insert(T entity, CancellationTokenSource? token = null)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using var context = await _dbContextFact.CreateDbContextAsync();

            var entities = context.Set<T>();

            entities.Add(entity);

            await SaveChangesAsync(context, token);

            return entity;
        }

        public virtual async Task<IEnumerable<T>> BulkInsert(IEnumerable<T> items, CancellationTokenSource? token = null)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            using var context = await _dbContextFact.CreateDbContextAsync();

            var entities = context.Set<T>();

            entities.AddRange(items);

            await SaveChangesAsync(context, token);

            return items;
        }

        public virtual async Task<T> Update(Guid id, T entity, CancellationTokenSource? token = null)
        {
            ArgumentNullException.ThrowIfNull(entity);

            using var context = await _dbContextFact.CreateDbContextAsync();

            return await Update(id, entity, token, context, true);
        }

        protected virtual async Task<T> Update(Guid id, T entity,
            CancellationTokenSource? token = null, DbContext? context = null, bool saveChanges = true)
        {
            ArgumentNullException.ThrowIfNull(entity);

            context ??= await _dbContextFact.CreateDbContextAsync();

            var entities = context.Set<T>();

            entity.ModifiedAt = DateTime.UtcNow;

            var entry = entities.Update(entity);

            entry.Property(x => x.Id).IsModified = false;
            entry.Property(x => x.CreatedAt).IsModified = false;
            entry.Property(x => x.IsDeleted).IsModified = false;

            if (saveChanges)
            {
                await SaveChangesAsync(context, token);

                await context.DisposeAsync();
            }

            return entity;
        }

        public virtual async Task<IEnumerable<W>> UpdateMultipleChilds<W>(IEnumerable<W> entitiesToUpdate,
            Expression<Func<W, bool>> expression, CancellationTokenSource? token = null, 
            Action<DbSet<W>, W, W>? updateAction = null, bool deleteMissing = true)
            where W : BaseEntity
        {
            using var context = await _dbContextFact.CreateDbContextAsync();

            return await UpdateMultipleChilds(entitiesToUpdate, expression, token, updateAction, context, true, deleteMissing);
        }

        protected virtual async Task<IEnumerable<W>> UpdateMultipleChilds<W>(IEnumerable<W> entitiesToUpdate,
            Expression<Func<W, bool>> expression, CancellationTokenSource? token = null,
            Action<DbSet<W>, W, W>? updateAction = null, DbContext? context = null, bool saveChanges = true,
            bool deleteMissing = true) where W : BaseEntity
        {
            ArgumentNullException.ThrowIfNull(entitiesToUpdate);

            context ??= await _dbContextFact.CreateDbContextAsync();
            
            var table = context.Set<W>();

            var entities = await table.Where(expression)
                .ProjectTo<W>(_mapper.ConfigurationProvider)
                .ToDictionaryAsync(x => x.Id, x => x);

            foreach (var entityToUpdate in entitiesToUpdate)
            {
                if (entityToUpdate.Id == Guid.Empty)
                {
                    table.Add(entityToUpdate);
                }
                else if (entities.TryGetValue(entityToUpdate.Id, out var entity))
                {
                    entityToUpdate.ModifiedAt = DateTime.UtcNow;

                    if (updateAction != null)
                    {
                        updateAction(table, entityToUpdate, entity);
                    }
                    else
                    {
                        var entry = table.Update(entityToUpdate);

                        entry.Property(x => x.Id).IsModified = false;
                        entry.Property(x => x.CreatedAt).IsModified = false;
                        entry.Property(x => x.IsDeleted).IsModified = false;
                    }
                }
            }

            if (deleteMissing)
            {
                var entitiesToUpdateDict = entitiesToUpdate.ToDictionary(x => x.Id, x => x);

                foreach (var entity in entities)
                {
                    if (!entitiesToUpdateDict.ContainsKey(entity.Key))
                    {
                        table.Remove(entity.Value);
                    }
                }
            }

            if (saveChanges)
            {
                await SaveChangesAsync(context, token);

                await context.DisposeAsync();
            }

            return entitiesToUpdate;
        }

        public async Task UpdateMultipleLeafType(Expression<Func<T, bool>> expression,
            Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setPropertyExpression,
            CancellationTokenSource? token = null)
        {
            using var context = await _dbContextFact.CreateDbContextAsync();

            var entities = context.Set<T>();

            await entities.Where(expression).ExecuteUpdateAsync(setPropertyExpression);
        }

        protected async Task<List<T>> GetNItemsWhere(IIncludableQueryable<T, object> include,
            Expression<Func<T, T>>? selector = null, Expression<Func<T, bool>>? filter = null,
            int skip = 0, int take = 10, CancellationTokenSource? token = null)
        {
            var query = filter == null ? include : include.Where(filter);

            return await GetItemsList(query, selector, skip, take, token);
        }

        protected async Task<List<W>> GetNItemsWhere<W>(IIncludableQueryable<T, object> include,
            Expression<Func<T, W>> selector, Expression<Func<T, bool>>? filter = null,
            int skip = 0, int take = 10, CancellationTokenSource? token = null)
        {
            var query = filter == null ? include : include.Where(filter);

            return await GetItemsList(query, selector, skip, take, token);
        }

        protected async Task<List<Z>> GetItemsList<Z>(IQueryable<Z> query,
            int skip = 0, int take = 0,
            CancellationTokenSource? token = null) 
            where Z : class
        {
            var itemsQuery = query.Skip(skip);

            if (take > 0)
            {
                itemsQuery = itemsQuery.Take(take);
            }

            return await ToListAsync(itemsQuery, token);
        }

        protected async Task<List<W>> GetItemsList<W>(IQueryable<W> query,
            Expression<Func<W, W>> selector, int skip = 0, int take = 0,
            CancellationTokenSource? token = null) where W : BaseEntity
        {
            var itemsQuery = query.OrderByDescending(x => x.ModifiedAt)
                .Skip(skip);

            if (take > 0)
            {
                itemsQuery = itemsQuery.Take(take);
            }

            if (selector != null)
            {
                itemsQuery = itemsQuery.Select(selector);
            }

            return await ToListAsync(itemsQuery, token);
        }

        protected async Task<List<W>> GetItemsList<W>(IQueryable<T> query,
            Expression<Func<T, W>> selector, int skip = 0, int take = 10,
            CancellationTokenSource? token = null)
        {
            var itemsQuery = query.OrderByDescending(x => x.ModifiedAt)
                .Skip(skip).Take(take).Select(selector);

            return await ToListAsync(itemsQuery, token);
        }

        protected async Task<List<T>> GetItemsListOrdered<Z>(IQueryable<T> query,
            Expression<Func<T, Z>> orderedBy, bool descending,
            Expression<Func<T, T>>? selector = null,
            int skip = 0, int take = 10, CancellationTokenSource? token = null)
        {
            var itemsQuery = descending ? query.OrderByDescending(orderedBy).Skip(skip) :
                query.OrderBy(orderedBy).Skip(skip);

            if (take > 0)
            {
                itemsQuery = itemsQuery.Take(take);
            }

            if (selector != null)
            {
                itemsQuery = itemsQuery.Select(selector);
            }

            return await ToListAsync(itemsQuery, token);
        }

        protected async Task<List<W>> GetItemsListOrdered<W, Z>(IQueryable<T> query,
            Expression<Func<T, W>> selector, Expression<Func<T, Z>> orderedBy, bool descending,
            int skip = 0, int take = 10, CancellationTokenSource? token = null)
        {
            var itemsQuery = descending ? query.OrderByDescending(orderedBy).Skip(skip).Take(take).Select(selector) :
                query.OrderBy(orderedBy).Skip(skip).Take(take).Select(selector);

            return await ToListAsync(itemsQuery, token);
        }

        protected async Task<List<T>> GetItemsList(IQueryable<T> query,
            Expression<Func<T, T>>? selector = null, int skip = 0, int take = 10,
            CancellationTokenSource? token = null)
        {
            var itemsQuery = query.OrderByDescending(x => x.CreatedAt)
                .Skip(skip);

            if (take > 0)
            {
                itemsQuery = itemsQuery.Take(take);
            }

            itemsQuery = selector == null ? itemsQuery :
                itemsQuery.Select(selector);

            return await ToListAsync(itemsQuery, token);
        }

        protected async Task<List<W>> ToListAsync<W>(IQueryable<W> query,
            CancellationTokenSource? token = null)
        {
            if (token != null)
            {
                try
                {
                    return await query.ToListAsync(token.Token);
                }
                catch (SqlException ex) when (token.IsCancellationRequested)
                {
                    _logger.LogError(ex.InnerException?.Message ?? ex.Message);

                    throw;
                }
            }
            else
            {
                return await query.ToListAsync();
            }
        }

        protected async Task SaveChangesAsync(DbContext context,
            CancellationTokenSource? token = null)
        {
            if (token != null)
            {
                try
                {
                    await context.SaveChangesAsync(token.Token);
                }
                catch (SqlException ex) when (token.IsCancellationRequested)
                {
                    _logger.LogError(ex.InnerException?.Message ?? ex.Message);

                    throw;
                }
            }
            else
            {
                await context.SaveChangesAsync();
            }
        }
    }
}
