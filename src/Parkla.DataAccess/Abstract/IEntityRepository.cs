using System.Linq.Expressions;
using Parkla.Core.Entities;
using Parkla.Web.Helpers;

namespace Parkla.DataAccess.Abstract
{
    public interface IEntityRepository<T> 
        where T:class, IEntity, new()
    {
        Task<List<T>> GetListAsync(
            Expression<Func<T,bool>>? filter = null, 
            CancellationToken cancellationToken = default
        );
        Task<PagedList<T>> GetListAsync(
            int pageNumber, 
            int pageSize, 
            Expression<Func<T,bool>>? filter = null, 
            CancellationToken cancellationToken = default
        );
        Task<List<T>> GetListAsync(
            Expression<Func<T, object>>[] includeProps,
            Expression<Func<T, bool>>? filter = null, 
            CancellationToken cancellationToken = default
        );        
        Task<T?> GetAsync(
            Expression<Func<T,bool>> filter, 
            CancellationToken cancellationToken = default
        );

        Task<T> AddAsync(
            T entity, 
            CancellationToken cancellationToken = default
        );

        Task<T> UpdateAsync(
            T entity, 
            CancellationToken cancellationToken = default
        );

        Task<T> UpdateAsync(
            T entity,
            Expression<Func<T, object?>>[] properties,
            bool excludes = true,
            CancellationToken cancellationToken = default
        );

        Task DeleteAsync(
            T entity, 
            CancellationToken cancellationToken = default
        );
    }
}