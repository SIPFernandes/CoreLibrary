﻿using CoreLibrary.Core.Entities;
using CoreLibrary.Core.Interfaces;
using CoreLibrary.Shared.Filters.ServiceFilterModels;
using CoreLibrary.Shared.Filters;
using CoreLibrary.Shared.Filters.ControllerFilterModels;
using CoreLibrary.Shared.Filters.ControllerFilterModels.FilterModels;
using CoreLibrary.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using CoreLibrary.Core.Exceptions;

namespace CoreLibrary.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public abstract class GenericController<TDto, TEntity>(IGenericService<IRepository<TEntity>, TDto, TEntity> genericService,
        ILogger<GenericController<TDto, TEntity>> logger) : ControllerBase
        where TEntity : BaseEntity, IAggregateRoot
        where TDto : BaseDto
    {
        protected readonly IGenericService<IRepository<TEntity>, TDto, TEntity> _genericService = genericService;
        private readonly ILogger<GenericController<TDto, TEntity>> _logger = logger;

        [HttpGet]
        public virtual async Task<IActionResult> GetAll()
        {
            try
            {
                var dtos = await _genericService.GetAll();

                return Ok(dtos);
            }
            catch (Exception ex) 
            {
                var innerException = ex.InnerException?.Message;

                _logger.LogError(innerException ?? ex.Message);

                return StatusCode(500, new { ex.Message, innerException });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> GetItemsFiltered([FromBody] GetItemsControllerFilter model)
        {
            try
            {
                var serviceFilter = new GetItemsServiceFilter<TEntity>(model);

                var dtos = await _genericService.GetItemsFiltered(serviceFilter);

                return Ok(dtos);
            }
            catch (Exception ex) when (ex is NotSupportedException ||
            ex is ArgumentException || ex is InvalidOperationException)
            {
                var innerException = ex.InnerException?.Message;

                _logger.LogError(innerException ?? ex.Message);

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException?.Message;

                _logger.LogError(innerException ?? ex.Message);

                return StatusCode(500, new { ex.Message, innerException });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> GroupByFiltered([FromBody] GroupByControllerFilter model)
        {
            try
            {
                var serviceFilter = new GroupByServiceFilter<TEntity>(model);

                var dtos = await _genericService.GroupByFiltered(serviceFilter);

                return Ok(dtos);
            }
            catch (Exception ex) when (ex is NotSupportedException ||
            ex is ArgumentException || ex is InvalidOperationException)
            {
                var innerException = ex.InnerException?.Message;

                _logger.LogError(innerException ?? ex.Message);

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException?.Message;

                _logger.LogError(innerException ?? ex.Message);

                return StatusCode(500, new { ex.Message, innerException });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> CountFiltered([FromBody] CombinedFilter? filter)
        {
            try
            {
                int count;

                if (filter == null)
                {
                    count = await _genericService.CountFiltered();
                }
                else
                {
                    var filterExpression = ExpressionHelper<TEntity>.CombineExpressions(filter);

                    count = await _genericService.CountFiltered(filterExpression);
                }

                return Ok(count);
            }
            catch (Exception ex) when (ex is NotSupportedException ||
            ex is ArgumentException || ex is InvalidOperationException)
            {
                var innerException = ex.InnerException?.Message;

                _logger.LogError(innerException ?? ex.Message);

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException?.Message;

                _logger.LogError(innerException ?? ex.Message);

                return StatusCode(500, new { ex.Message, innerException });
            }
        }

        //GET ENTITY BY ID
        /// <summary>
        /// Gets a specific entity by id.
        /// </summary>
        /// <response code="200">Success.</response>
        /// <response code="400">Unable to get the entity due to validation error.</response>
        /// <response code="404">Entity not found!</response>
        [HttpGet("{id}")]
        public virtual async Task<ActionResult> Get(Guid id)
        {
            try
            {
                var dto = await _genericService.Get(id);

                return Ok(dto);
            }
            catch (EntityDoesNotExistException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException?.Message;

                _logger.LogError(innerException ?? ex.Message);

                return StatusCode(500, new { ex.Message, innerException });
            }
        }

        //GET ENTITY BY ID
        /// <summary>
        /// Gets a specific entity by id.
        /// </summary>
        /// <response code="200">Success.</response>
        /// <response code="400">Unable to get the entity due to validation error.</response>
        /// <response code="404">Entity not found!</response>
        [HttpPost("{id}")]
        public virtual async Task<ActionResult> GetSelectFilter(Guid id, [FromBody] GetSelectControllerFilter model)
        {
            try
            {
                var serviceFilter = new GetSelectServiceFilter<TEntity>(model);

                var dto = await _genericService.GetSelectFilter(id, serviceFilter);

                return Ok(dto);
            }
            catch(EntityDoesNotExistException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                var innerException = ex.InnerException?.Message;

                _logger.LogError(innerException ?? ex.Message);

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException?.Message;

                _logger.LogError(innerException ?? ex.Message);

                return StatusCode(500, new { ex.Message, innerException });
            }
        }

        //CREATE ENTITY
        /// <summary>
        /// Creates a entity.
        /// </summary>
        /// <response code="201">Entity created successfully.</response>
        /// <response code="400">Unable to create the entity due to validation error.</response>
        [HttpPost]
        public virtual async Task<ActionResult> Insert([FromBody] TDto dto)
        {
            if (dto == null || !ModelState.IsValid)
            {
                var errorMessage = ModelState.Values.SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage);

                return BadRequest(errorMessage);
            }

            try
            {
                var result = await _genericService.Insert(dto);

                _logger.LogInformation($"{typeof(TEntity).FullName} with id {result.Id} created");

                return Created("Entity created successfully", result);
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException?.Message;

                _logger.LogError(innerException ?? ex.Message);

                return StatusCode(500, new { ex.Message, innerException });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> BulkInsert([FromBody] IEnumerable<TDto> dtos)
        {
            if (dtos == null || !ModelState.IsValid)
            {
                var errorMessage = ModelState.Values.SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage);

                return BadRequest(errorMessage);
            }

            try
            {
                var result = await _genericService.BulkInsert(dtos);

                _logger.LogInformation($"List of {typeof(TEntity).FullName} objects added in bulk");

                return Created("Entities created successfully", result);
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException?.Message;

                _logger.LogError(innerException ?? ex.Message);

                return StatusCode(500, new { ex.Message, innerException });
            }
        }

        //UPDATE ENTITY BY ID
        /// <summary>
        /// Updates a specific entity by id.
        /// </summary>
        /// <response code="201">Entity updated successfully.</response>
        /// <response code="400">Unable to update the entity due to validation error.</response>
        [HttpPut("{id}")]
        public virtual async Task<ActionResult> Update(Guid id, [FromBody] TDto dto)
        {
            if (id == Guid.Empty || dto == null || !ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage);

                return BadRequest(error);
            }

            try
            {
                if (id != dto.Id)
                {
                    return BadRequest();
                }

                var result = await _genericService.Update(id, dto);

                if (result is null)
                {
                    return BadRequest("Validation error");
                }
                else
                {
                    _logger.LogInformation($"{typeof(TEntity).FullName} with id {id} updated");

                    return Ok(result);
                }
            }
            catch (EntityDoesNotExistException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException?.Message;

                _logger.LogError(innerException ?? ex.Message);

                return StatusCode(500, new { ex.Message, innerException });
            }
        }

        [HttpPut]
        public virtual async Task<ActionResult> BulkAddOrUpdate([FromBody] IEnumerable<TDto> dtos)
        {
            if (dtos == null || !ModelState.IsValid)
            {
                var errorMessage = ModelState.Values.SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage);

                return BadRequest(errorMessage);
            }

            try
            {
                var result = await _genericService.BulkAddOrUpdate(dtos);

                if (result is null)
                {
                    return BadRequest("Validation error");
                }
                else
                {
                    _logger.LogInformation($"List of {typeof(TEntity).FullName} objects added/updated in bulk");

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException?.Message;

                _logger.LogError(innerException ?? ex.Message);

                return StatusCode(500, new { ex.Message, innerException });
            }
        }

        [HttpPut]
        public virtual async Task<ActionResult> UpdateWhere([FromBody] UpdateWhereControllerFilter updateModel)
        {
            if (updateModel == null || !ModelState.IsValid)
            {
                var errorMessage = ModelState.Values.SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage);

                return BadRequest(errorMessage);
            }

            try
            {
                Expression<Func<TEntity, bool>>? expression = null;

                if (updateModel.Filters != null)
                {
                    expression = ExpressionHelper<TEntity>.CombineExpressions(updateModel.Filters);
                }

                var setPropertyExpression = ExpressionHelper<TEntity>.BuildSetPropertyExpression(updateModel.UpdateProperties);

                await _genericService.UpdatePropertiesInMultipleItems(expression, setPropertyExpression);

                _logger.LogInformation($"{typeof(TEntity).FullName} updated according to the expressions: {expression}, {setPropertyExpression}");

                return Ok();
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException?.Message;

                _logger.LogError(innerException ?? ex.Message);

                return StatusCode(500, new { ex.Message, innerException });
            }
        } 

        //DELETE ENITTY BY ID
        /// <summary>
        /// Deletes a specific entity by id.
        /// </summary>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Entity not found!</response>
        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> DeleteById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid Guid" );
            }

            try
            {
                await _genericService.DeleteById(id);

                _logger.LogInformation($"{typeof(TEntity).FullName} deleted according to given id: {id}");

                return Ok();
            }
            catch (EntityDoesNotExistException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException?.Message;

                _logger.LogError(innerException ?? ex.Message);

                return StatusCode(500, new { ex.Message, innerException });
            }
        }

        [HttpDelete]
        public virtual async Task<IActionResult> BulkDeleteByIds([FromBody] List<Guid> ids)
        {
            if (ids == null|| ids.Count <= 0)
            {
                return BadRequest("Invalid ids");
            }

            try
            {
                await _genericService.BulkDeleteById(ids);

                _logger.LogInformation($"{typeof(TEntity).FullName} deleted according to the list of ids: {string.Join(", ", ids)}");

                return Ok();
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException?.Message;

                _logger.LogError(innerException ?? ex.Message);

                return StatusCode(500, new { ex.Message, innerException });
            }
        }

        [HttpDelete]
        public virtual async Task<IActionResult> DeleteWhere([FromBody] CombinedFilter combinedFilter)
        {
            if (combinedFilter == null)
            {
                return BadRequest("Invalid Filter");
            }

            try
            {
                var expression = ExpressionHelper<TEntity>.CombineExpressions(combinedFilter);

                await _genericService.DeleteWhere(expression);

                _logger.LogInformation($"{typeof(TEntity).FullName} deleted according to the expression: {expression}");

                return Ok();
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException?.Message;

                _logger.LogError(innerException ?? ex.Message);

                return StatusCode(500, new { ex.Message, innerException });
            }
        }
    }
}
