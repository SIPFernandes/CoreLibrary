using CoreLibrary.Core.Entities;
using CoreLibrary.Core.Interfaces;
using CoreLibrary.Filters;
using CoreLibrary.Filters.ControllerFilterModels;
using CoreLibrary.Filters.ServiceFilterModels;
using CoreLibrary.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreLibrary.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public abstract class GenericController<TDto, TEntity>(IGenericService<IRepository<TEntity>, TDto, TEntity> genericService,
        ILogger<GenericController<TDto, TEntity>> logger) : ControllerBase
        where TEntity : BaseEntity, IAggregateRoot
        where TDto : BaseDto
    {
        private readonly IGenericService<IRepository<TEntity>, TDto, TEntity> _genericService = genericService;
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
                var serviceFilter = new GetItemsServiceFilter<TDto, TEntity>(model);

                var dtos = await _genericService.GetItemsFiltered(serviceFilter);

                return Ok(dtos);
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
                var expression = FilterHelper<TEntity>.CombineExpressions(combinedFilter);

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
