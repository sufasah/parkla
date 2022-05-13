using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Parkla.Core.Entities;
using Parkla.Core.Helpers;
using Parkla.DataAccess.Abstract;
namespace Parkla.DataAccess.Bases
{
    public class EntityRepoBase<TEntity,TContext> : IEntityRepository<TEntity> 
        where TEntity :class, IEntity, new()
        where TContext:DbContext, new()
    {
        public async Task<TEntity> AddAsync(
            TEntity entity, 
            CancellationToken cancellationToken = default
        ) {
            using var context = new TContext();
            var result = context.Entry(entity);
            result.State = EntityState.Added;
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return entity;
        }

        public async Task DeleteAsync(
            TEntity entity, 
            CancellationToken cancellationToken = default
        ) {
            using var context = new TContext();
            var result = context.Entry(entity);
            result.State = EntityState.Deleted;
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<TEntity?> GetAsync(
            Expression<Func<TEntity, bool>> filter, 
            CancellationToken cancellationToken = default
        ) {
            using var context = new TContext();
            TEntity? result = await context.Set<TEntity>()
                .AsNoTracking()
                .SingleOrDefaultAsync(filter, cancellationToken)
                .ConfigureAwait(false);
            return result;
        }

        public async Task<List<TEntity>> GetListAsync(
            Expression<Func<TEntity, bool>>? filter = null, 
            CancellationToken cancellationToken = default
        ) {
            using var context = new TContext();
            IQueryable<TEntity> resultSet;
            if (filter == null)
                resultSet = context.Set<TEntity>().AsNoTracking();
            else
                resultSet = context.Set<TEntity>().AsNoTracking().Where(filter);
            
            var result = await resultSet.ToListAsync(cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<List<TEntity>> GetListAsync(
            Expression<Func<TEntity, object>>[] includeProps,
            Expression<Func<TEntity, bool>>? filter = null, 
            CancellationToken cancellationToken = default
        ) {
            using var context = new TContext();
            var resultSet = context.Set<TEntity>().AsNoTracking();

            if (filter != null)
                resultSet = resultSet.Where(filter);
            
            foreach (var prop in includeProps)
            {
                resultSet = resultSet.Include(prop);
            }

            var result = await resultSet.ToListAsync(cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<PagedList<TEntity>> GetListAsync(
            int pageNumber, 
            int pageSize, 
            Expression<Func<TEntity, bool>>? filter = null, 
            CancellationToken cancellationToken = default
        ) {
            using var context = new TContext();
            IQueryable<TEntity> resultSet;
            if(filter == null)
                resultSet = context.Set<TEntity>().AsNoTracking();
            else
                resultSet = context.Set<TEntity>().AsNoTracking().Where(filter);

            var result = await ToPagedListAsync(resultSet, pageNumber, pageSize, cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<TEntity> UpdateAsync(
            TEntity entity, 
            CancellationToken cancellationToken = default
        ) {
            using var context = new TContext();
            var result = context.Entry(entity);
            result.State = EntityState.Modified;
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return entity;
        }

        public async Task<TEntity> UpdateAsync(
            TEntity entity,
            Expression<Func<TEntity, object?>>[] updateProps,
            bool updateOtherProps = true,
            CancellationToken cancellationToken = default
        ) {
            using var context = new TContext();
            var result = context.Entry(entity);
            result.State = updateOtherProps ? EntityState.Modified : EntityState.Unchanged;
            foreach (var prop in updateProps)
            {
                result.Property(prop).IsModified = !updateOtherProps;
            }
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return entity;
        }

        private static async Task<PagedList<TEntity>> ToPagedListAsync(
            IQueryable<TEntity> source, 
            int pageNumber, 
            int pageSize, 
            CancellationToken cancellationToken = default
        ) {
            var count = source.Count();
            var items = await source.Skip((pageNumber-1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            
            return new PagedList<TEntity>(items, pageNumber, pageSize, count);
        }

        public async Task<TEntity?> GetAsync(
            Expression<Func<TEntity, object>>[] includeProps, 
            Expression<Func<TEntity, bool>> filter, 
            CancellationToken cancellationToken = default
        ) {
            using var context = new TContext();
            var query = context.Set<TEntity>()
                .AsNoTracking();
            
            foreach (var prop in includeProps)
            {
                query = query.Include(prop);
            }
            
            var result = await query.SingleOrDefaultAsync(filter, cancellationToken)
                .ConfigureAwait(false);
            return result;
        }

        public async Task<TEntity> AddAsync(
            TEntity entity, 
            Expression<Func<TEntity, object?>>[] includeProps, 
            CancellationToken cancellationToken = default
        ) {
            using var context = new TContext();
            var result = context.Entry(entity);
            result.State = EntityState.Added;
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            foreach (var prop in includeProps)
            {
                await result.Reference(prop).LoadAsync(cancellationToken);
            }
            return entity;
        }

        public async Task<TEntity> UpdateAsync(
            TEntity entity, 
            Expression<Func<TEntity, object?>>[] includeProps, 
            CancellationToken cancellationToken = default
        ) {
            using var context = new TContext();
            var result = context.Entry(entity);
            result.State = EntityState.Modified;
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            foreach (var prop in includeProps)
            {
                await result.Reference(prop).LoadAsync(cancellationToken);
            }
            return entity;
        }
    }
}