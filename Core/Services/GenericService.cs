using AutoMapper;
using CoreLibrary.Core.Entities;
using CoreLibrary.Core.Interfaces;
using CoreLibrary.Shared.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace CoreLibrary.Core.Services
{
    public class GenericService<TRepository, TDto, TEntity> : IGenericService<TRepository, TDto, TEntity> 
        where TRepository : IRepository<TEntity> 
        where TDto : BaseDto
        where TEntity : BaseEntity, IAggregateRoot
    {
        protected readonly TRepository _repository;
        protected readonly IMapper _mapper;
        protected readonly ILogger<GenericService<TRepository, TDto, TEntity>> _logger;

        public GenericService(TRepository repository, IMapper mapper,
            ILogger<GenericService<TRepository, TDto, TEntity>> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public virtual async Task<IEnumerable<TDto>> GetAll()
        {
            var entities = await _repository.GetAll();

            return _mapper.Map<IEnumerable<TDto>>(entities); 
        }

        public virtual async Task<IEnumerable<TDto>> GetItemsFiltered(GetItemsFilter<TDto, TEntity> model)
        {
            IEnumerable<TDto> result = [];

            if (model != null) {
                if (model.Ordered != null)
                {
                    if (model.Selector != null)
                    {
                        result = await _repository.GetNItemsWhereOrdered(model.Selector, model.Ordered.OrderedBy,
                            model.Ordered.Descending, model.Filter, model.Skip, model.Take, model.Includes, model.Token);
                    }
                    else
                    {
                        var entities = await _repository.GetNItemsWhereOrdered(model.Ordered.OrderedBy,
                            model.Ordered.Descending, model.Filter, model.Skip, model.Take, model.Includes, model.Token);

                        result = _mapper.Map<IEnumerable<TDto>>(entities);
                    }   
                }
                else
                {
                    if (model.Selector != null)
                    {
                        result = await _repository.GetNItemsWhere(model.Selector, model.Filter, model.Skip, model.Take, model.Includes, model.Token);
                    }
                    else
                    {
                        var entities = await _repository.GetNItemsWhere(null, model.Filter,
                            model.Skip, model.Take, model.Includes, model.Token);

                        result = _mapper.Map<IEnumerable<TDto>>(entities);
                    }
                }
            }

            return result;
        }

        public virtual async Task<TDto?> Get(Guid id)
        {
            var entity = await _repository.Get(id);
            return _mapper.Map<TDto>(entity);
        }

        public virtual async Task<TDto?> Get(Guid id, Expression<Func<TEntity, TDto>> selector)
        {
            return await _repository.Get(id, selector);
        }

        public virtual async Task<TDto> Insert(TDto dto)
        {
            try
            {
                if (Validate(dto))
                {
                    var entity = _mapper.Map<TEntity>(dto);

                    entity = await _repository.Insert(entity);

                    dto = _mapper.Map<TDto>(entity);
                }

                return dto;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
            {
                throw sqlEx;
            }
        }

        public virtual async Task<IEnumerable<TEntity>> BulkInsert(IEnumerable<TDto> list)
        {
            try
            {
                var entities = new List<TEntity>();

                foreach(var dto in list)
                {
                    if (Validate(dto))
                    {
                        var entity = _mapper.Map<TEntity>(dto);

                        entities.Add(entity);
                    }
                }

                return await _repository.BulkInsert(entities);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
            {
                throw sqlEx;
            }
        }

        public virtual async Task<TDto?> Update(Guid id, TDto dto)
        {
            try 
            { 
                TDto? result = null;

                if (Validate(dto))
                {
                    var entity = _mapper.Map<TEntity>(dto);

                    entity = await _repository.Update(id, entity);

                    result = _mapper.Map<TDto>(entity);
                }

                return result;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
            {
                throw sqlEx;
            }
        }

        public virtual async Task<IEnumerable<TEntity>?> BulkAddOrUpdate(IEnumerable<TDto> dtos)
        {
            try
            {
                List<TEntity> entities = FromDtosToEntities(dtos);

                return await _repository.UpdateMultipleChilds(entities, x => dtos.Select(y => y.Id).Contains(x.Id), deleteMissing: false);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
            {
                throw sqlEx;
            }
        }

        public virtual async Task Delete(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);

            await _repository.Delete(entity);
        }

        public virtual bool Validate(TDto dto, object? validationObj = null)
        {
            return true;
        }

        public virtual async Task DeleteById(Guid id)
        {
            await _repository.DeleteById(id);
        }

        public virtual async Task BulkDeleteById(IEnumerable<Guid> ids)
        {
            await _repository.DeleteMultipleLeafType(x => ids.Contains(x.Id));
        }

        public virtual async Task DeleteWhere(Expression<Func<TEntity, bool>> expression)
        {
            await _repository.DeleteMultipleLeafType(expression);
        }

        protected List<TEntity> FromDtosToEntities(IEnumerable<TDto> dtos, object? validationObj = null)
        {
            var entities = new List<TEntity>();

            foreach (var dto in dtos)
            {
                if (Validate(dto, validationObj))
                {
                    var entity = _mapper.Map<TEntity>(dto);

                    entities.Add(entity);
                }
            }

            return entities;
        }
    }

    public class GetItemsFilter<TDto, TEntity>
    {
        public Expression<Func<TEntity, TDto>>? Selector = null;
        public Expression<Func<TEntity, bool>>? Filter = null;
        public OrderedFilter? Ordered = null;
        public int Skip = 0, Take = 10;
        public string[]? Includes = null;
        public CancellationTokenSource? Token = null;

        public class OrderedFilter
        {
            public required Expression<Func<TEntity, TDto>> OrderedBy;
            public bool Descending = false;
        }
    }
}
