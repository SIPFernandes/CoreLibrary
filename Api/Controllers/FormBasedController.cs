using AutoMapper;
using CoreLibrary.Core.Entities;
using CoreLibrary.Core.Interfaces;
using CoreLibrary.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreLibrary.Api.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public abstract class FormBasedController<TForm, TDto, TEntity>(
        IGenericService<IRepository<TEntity>, TDto, TEntity> genericService,
        IMapper mapper, ILogger<FormBasedController<TForm, TDto, TEntity>> logger) :
        GenericController<TDto, TEntity>(genericService, logger)
        where TForm : BaseForm
        where TEntity : BaseEntity, IAggregateRoot
        where TDto : BaseDto
    {
        protected readonly IMapper _mapper = mapper;

        [NonAction]
        public override Task<ActionResult> Insert([FromBody] TDto form)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public virtual async Task<ActionResult> Insert([FromBody] TForm form)
        {
            var dto = _mapper.Map<TDto>(form);

            return await base.Insert(dto);
        }

        [NonAction]
        public override Task<ActionResult> Update(Guid id, [FromBody] TDto form)
        {
            throw new NotImplementedException();
        }

        [HttpPut("{id}")]
        public virtual async Task<ActionResult> Update(Guid id, [FromBody] TForm form)
        {
            var dto = _mapper.Map<TDto>(form);

            dto.Id = id;

            return await base.Update(id, dto);
        }
    }
}
