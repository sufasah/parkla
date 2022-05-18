using System.Linq.Expressions;
using Parkla.Core.Entities;
using Parkla.Core.Helpers;

namespace Parkla.Business.Abstract;
public interface IEntityService<TEntity> 
    where TEntity: class, IEntity, new()
{
        Task<List<TEntity>> GetAllAsync(
            CancellationToken cancellationToken = default
        );

        Task<PagedList<TEntity>> GetPageAsync(
            int nextRecord, 
            int pageSize, 
            CancellationToken cancellationToken = default
        );

        public Task<PagedList<TEntity>> GetPageAsync(
            int nextRecord, 
            int pageSize,  
            string? search, 
            string? orderBy, 
            bool ascending, 
            CancellationToken cancellationToken = default
        );

        Task<TEntity?> GetAsync(
            Expression<Func<TEntity,bool>> filter,
            CancellationToken cancellationToken = default
        );

        Task<TEntity?> GetAsync<TKey>(
            TKey id,
            CancellationToken cancellationToken = default
        ) where TKey: struct;

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