using System.Linq.Expressions;
using Parkla.Core.Entities;
using Parkla.Core.Helpers;
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
            int nextRecord, 
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>? orderBy = null,
            bool asc = true,
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

        Task<T?> GetAsync<Tkey>(
            Tkey id, 
            CancellationToken cancellationToken = default
        ) where Tkey: struct;

        Task<T?> GetAsync<Tkey>(
            Tkey id, 
            Expression<Func<T, object>>[] includeProps,
            CancellationToken cancellationToken = default
        ) where Tkey: struct;

        Task<T?> GetAsync(
            Expression<Func<T, object>>[] includeProps,
            Expression<Func<T, bool>> filter, 
            CancellationToken cancellationToken = default
        );

        Task<T> AddAsync(
            T entity, 
            CancellationToken cancellationToken = default
        );

        Task<T> AddAsync(
            T entity, 
            Expression<Func<T, object?>>[] includeProps,
            CancellationToken cancellationToken = default
        );

        Task<T> UpdateAsync(
            T entity, 
            CancellationToken cancellationToken = default
        );

        Task<T> UpdateAsync(
            T entity, 
            Expression<Func<T, object?>>[] includeProps,
            CancellationToken cancellationToken = default
        );

        Task<T> UpdateAsync(
            T entity,
            Expression<Func<T, object?>>[] updateProps,
            bool updateOtherProps = false,
            CancellationToken cancellationToken = default
        );

        Task DeleteAsync(
            T entity, 
            CancellationToken cancellationToken = default
        );
    }
}