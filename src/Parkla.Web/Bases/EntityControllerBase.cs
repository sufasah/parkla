using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Parkla.Business.Abstract;
using Parkla.Core.Entities;
using Parkla.Web.Models;

namespace Parkla.Web.Controllers;

public class EntityControllerBase<TEntity, TEntityDto> : ApiControllerBase
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
    public virtual async Task<List<TEntityDto>> GetAllAsync(CancellationToken cancellationToken) {
        var result = await _service.GetAllAsync(cancellationToken);
        return _mapper.Map<List<TEntityDto>>(result);
    }

    [HttpGet("")]
    public virtual async Task<List<TEntityDto>> GetPageAsync(
        [FromQuery] PageDto pageDto,
        CancellationToken cancellationToken
    ) {
        var result = await _service.GetPageAsync(pageDto.PageNumber, pageDto.PageSize, cancellationToken);
        return _mapper.Map<List<TEntityDto>>(result);
    }

    public virtual async Task<TEntityDto> AddAsync(
        [FromBody] TEntityDto entityDto,
        CancellationToken cancellationToken
    ) {
        var entity = _mapper.Map<TEntity>(entityDto);
        var result = await _service.AddAsync(entity, cancellationToken);
        return _mapper.Map<TEntityDto>(result);
    }

    public virtual async Task<TEntityDto> UpdateAsync(
        [FromBody] TEntityDto entityDto,
        CancellationToken cancellationToken
    ) {
        var entity = _mapper.Map<TEntity>(entityDto);
        var result = await _service.UpdateAsync(entity, cancellationToken);
        return _mapper.Map<TEntityDto>(result);
    }

    public virtual async Task DeleteAsync(
        [FromBody] TEntityDto entityDto,
        CancellationToken cancellationToken
    ) {
        var entity = _mapper.Map<TEntity>(entityDto);
        await _service.DeleteAsync(entity, cancellationToken);
    }

    public virtual async Task<TEntityDto> NoValidateAddAsync(
        [FromBody] TEntityDto entityDto,
        CancellationToken cancellationToken
    ) {
        var entity = _mapper.Map<TEntity>(entityDto);
        var result = await _service.NoValidateAddAsync(entity, cancellationToken);
        return _mapper.Map<TEntityDto>(result);
    }

    public virtual async Task<TEntityDto> NoValidateUpdateAsync(
        [FromBody] TEntityDto entityDto,
        CancellationToken cancellationToken
    ) {
        var entity = _mapper.Map<TEntity>(entityDto);
        var result = await _service.NoValidateUpdateAsync(entity, cancellationToken);
        return _mapper.Map<TEntityDto>(result);
    }

    public virtual async Task NoValidateDeleteAsync(
        [FromBody] TEntityDto entityDto,
        CancellationToken cancellationToken
    ) {
        var entity = _mapper.Map<TEntity>(entityDto);
        await _service.NoValidateDeleteAsync(entity, cancellationToken);
    }
}
