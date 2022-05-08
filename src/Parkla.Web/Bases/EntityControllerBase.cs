using Microsoft.AspNetCore.Mvc;
using Parkla.Business.Abstract;
using Parkla.Core.Entities;

namespace Parkla.Web.Controllers;

[ApiController]
public class EntityControllerBase<TEntity> : ApiControllerBase
    where TEntity: class, IEntity, new() 
{
    private readonly IEntityService<TEntity> _service;

    public EntityControllerBase(
        IEntityService<TEntity> service
    ) {
        _service = service;
    }
    public virtual async Task<List<TEntity>> GetPageAsync(
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken
    ) {
        return await _service.GetPageAsync(pageNumber, pageSize, cancellationToken);
    }

    public virtual async Task<TEntity> AddAsync(
        TEntity entity,
        CancellationToken cancellationToken
    ) {
        return await _service.AddAsync(entity, cancellationToken);
    }

    public virtual async Task<TEntity> UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken
    ) {
        return await _service.UpdateAsync(entity, cancellationToken);
    }

    public virtual async Task DeleteAsync(
        TEntity entity,
        CancellationToken cancellationToken
    ) {
        await _service.DeleteAsync(entity, cancellationToken);
    }

    public virtual async Task<TEntity> NoValidateAddAsync(
        TEntity entity,
        CancellationToken cancellationToken
    ) {
        return await _service.NoValidateAddAsync(entity, cancellationToken);
    }

    public virtual async Task<TEntity> NoValidateUpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken
    ) {
        return await _service.NoValidateUpdateAsync(entity, cancellationToken);
    }

    public virtual async Task NoValidateDeleteAsync(
        TEntity entity,
        CancellationToken cancellationToken
    ) {
        await _service.NoValidateDeleteAsync(entity, cancellationToken);
    }
}
