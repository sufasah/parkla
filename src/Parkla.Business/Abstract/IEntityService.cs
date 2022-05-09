using System.Linq.Expressions;
using Parkla.Core.Entities;
using Parkla.Web.Helpers;

namespace Parkla.Business.Abstract;
public interface IEntityService<TEntity> 
    where TEntity: class, IEntity, new()
{
        Task<List<TEntity>> GetAllAsync(
            CancellationToken cancellationToken = default
        );

        Task<PagedList<TEntity>> GetPageAsync(
            int pageNumber, 
            int PageSize,
            CancellationToken cancellationToken = default
        );

        Task<TEntity?> GetAsync(
            Expression<Func<TEntity,bool>> filter,
            CancellationToken cancellationToken = default
        );

        Task<TEntity> AddAsync(
            TEntity entity,
            CancellationToken cancellationToken = default
        );

        Task<TEntity> UpdateAsync(
            TEntity entity,
            CancellationToken cancellationToken = default
        );

        Task DeleteAsync(
            TEntity entity,
            CancellationToken cancellationToken = default
        );

        Task<TEntity> NoValidateAddAsync(
            TEntity entity,
            CancellationToken cancellationToken = default
        );

        Task NoValidateDeleteAsync(
            TEntity entity,
            CancellationToken cancellationToken = default
        );
        Task<TEntity> NoValidateUpdateAsync(
            TEntity entity,
            CancellationToken cancellationToken = default
        );

}