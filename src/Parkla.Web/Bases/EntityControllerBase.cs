using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parkla.Business.Abstract;
using Parkla.Core.Entities;
using Parkla.Core.Options;
using Parkla.Web.Models;

namespace Parkla.Web.Controllers;
[ApiController]
[Route(WebOptions.API_PREFIX+"/[controller]")]
[Authorize]
public class EntityControllerBase<TEntity, TEntityDto> : ControllerBase
    where TEntity: class, IEntity, new()
    where TEntityDto: class, new()
{
    private readonly IEntityService<TEntity> _service;
    private readonly IMapper _mapper;

    public EntityControllerBase(
        IEntityService<TEntity> service,
        IMapper mapper
    ) {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet("all")]
    public virtual async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken) {
        var result = await _service.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpGet("")]
    public virtual async Task<IActionResult> GetPageAsync(
        [FromQuery] PageDto pageDto,
        CancellationToken cancellationToken
    ) {
        var result = await _service.GetPageAsync(pageDto.PageNumber, pageDto.PageSize, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpPost("")]
    public virtual async Task<IActionResult> AddAsync(
        [FromBody] TEntityDto entityDto,
        CancellationToken cancellationToken
    ) {
        var entity = _mapper.Map<TEntity>(entityDto);
        var result = await _service.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpPut("")]
    public virtual async Task<IActionResult> UpdateAsync(
        [FromBody] TEntityDto entityDto,
        CancellationToken cancellationToken
    ) {
        var entity = _mapper.Map<TEntity>(entityDto);
        var result = await _service.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpDelete("")]
    public virtual async Task<IActionResult> DeleteAsync(
        [FromBody] TEntityDto entityDto,
        CancellationToken cancellationToken
    ) {
        var entity = _mapper.Map<TEntity>(entityDto);
        await _service.DeleteAsync(entity, cancellationToken).ConfigureAwait(false);
        return Ok();
    }
    [HttpPost("novalidate")]
    public virtual async Task<IActionResult> NoValidateAddAsync(
        [FromBody] TEntityDto entityDto,
        CancellationToken cancellationToken
    ) {
        var entity = _mapper.Map<TEntity>(entityDto);
        var result = await _service.NoValidateAddAsync(entity, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpPut("novalidate")]
    public virtual async Task<IActionResult> NoValidateUpdateAsync(
        [FromBody] TEntityDto entityDto,
        CancellationToken cancellationToken
    ) {
        var entity = _mapper.Map<TEntity>(entityDto);
        var result = await _service.NoValidateUpdateAsync(entity, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpDelete("novalidate")]
    public virtual async Task<IActionResult> NoValidateDeleteAsync(
        [FromBody] TEntityDto entityDto,
        CancellationToken cancellationToken
    ) {
        var entity = _mapper.Map<TEntity>(entityDto);
        await _service.NoValidateDeleteAsync(entity, cancellationToken).ConfigureAwait(false);
        return Ok();
    }
}
