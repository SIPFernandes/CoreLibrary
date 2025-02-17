using AutoMapper;
using CoreLibrary.Core.Entities;
using CoreLibrary.Core.Interfaces;
using CoreLibrary.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreLibrary.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class SingleRowController<TForm, TDto, TEntity>(IGenericService<IRepository<TEntity>, TDto, TEntity> service,
        IMapper mapper, ILogger<SingleRowController<TForm, TDto, TEntity>> logger) : ControllerBase
        where TEntity : BaseEntity, IAggregateRoot
        where TDto : BaseDto
        where TForm : BaseForm
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var dtos = await service.GetAll();

            var dto = dtos.FirstOrDefault();

            var result = mapper.Map<TForm>(dto);

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(TForm form)
        {
            try
            {
                var dtos = await service.GetAll();

                var first = dtos.FirstOrDefault();

                var dto = mapper.Map<TDto>(form);

                if (first != null)
                {
                    dto.Id = first.Id;

                    dto = await service.Update(first.Id, dto);
                }
                else
                {
                    dto = await service.Insert(dto);
                }                

                var result = mapper.Map<TForm>(dto);

                return Ok(result);
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException?.Message;

                logger.LogError(innerException ?? ex.Message);

                return StatusCode(500, new { ex.Message, innerException });
            }
        }
    }
}
