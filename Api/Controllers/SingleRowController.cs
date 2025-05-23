﻿using AutoMapper;
using CoreLibrary.Core.Entities;
using CoreLibrary.Core.Interfaces;
using CoreLibrary.Shared.Filters.ServiceFilterModels;
using CoreLibrary.Shared.Filters.ControllerFilterModels;
using CoreLibrary.Shared.Models;
using DnsClient.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreLibrary.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class SingleRowController<TForm, TDto, TEntity>(IGenericService<IRepository<TEntity>, TDto, TEntity> service,
        IMapper mapper, ILogger<SingleRowController<TForm, TDto, TEntity>> logger) : ControllerBase
        where TEntity : BaseEntity, IAggregateRoot
        where TDto : BaseDto
        where TForm : SingleRowBaseForm
    {
        protected readonly IGenericService<IRepository<TEntity>, TDto, TEntity> _service = service;
        protected readonly IMapper _mapper = mapper;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var dto = await _service.GetFirstOrDefault();

            TForm? result;

            if (dto == null) 
            { 
                result = null;
            }
            else
            {
                result = _mapper.Map<TForm>(dto);
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> GetSelectFilter([FromBody] GetSelectControllerFilter model)
        {
            try
            {
                var serviceFilter = new GetSelectServiceFilter<TEntity>(model);

                var dto = await _service.GetFirstSelectFilter(serviceFilter);

                return Ok(dto);
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException?.Message;

                logger.LogError(innerException ?? ex.Message);

                return StatusCode(500, new { ex.Message, innerException });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(TForm form)
        {
            try
            {
                var first = await _service.GetFirstOrDefault();

                var dto = _mapper.Map<TDto>(form);

                if (first != null)
                {
                    dto.Id = first.Id;

                    dto = await _service.Update(first.Id, dto);
                }
                else
                {
                    dto = await _service.Insert(dto);
                }                

                var result = _mapper.Map<TForm>(dto);

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
