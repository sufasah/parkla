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
        public virtual async Task<TEntity> AddAsync(
            TEntity entity, 
            CancellationToken cancellationToken = default
        ) {
            using var context = new TContext();
            var result = context.Entry(entity);
            result.State = EntityState.Added;
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return entity;
        }

        public virtual async Task DeleteAsync(
            TEntity entity, 
            CancellationToken cancellationToken = default
        ) {
            using var context = new TContext();
            var result = context.Entry(entity);
            result.State = EntityState.Deleted;
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<TEntity?> GetAsync(
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
        
        private static Expression<Func<TEntity,bool>> GetIdEqualExpression<Tkey>(
            Tkey id,
            TContext context
        )  where Tkey: struct
        {
            var pk = context.Model.FindEntityType(typeof(TEntity))!
                .FindPrimaryKey()!
                .Properties
                .First(x => x.ClrType == typeof(Tkey) || x.ClrType == typeof(Tkey?));
            
            var param = Expression.Parameter(typeof(TEntity), "x");
            var expression = Expression.Lambda<Func<TEntity,bool>>(
                Expression.Equal(
                    Expression.Convert(Expression.Constant(id), typeof(Tkey?)),
                    Expression.MakeMemberAccess(param, pk.PropertyInfo!)
                ),
                param
            );

            return expression;
        }

        public virtual async Task<TEntity?> GetAsync<Tkey>(
            Tkey id, 
            CancellationToken cancellationToken = default
        )   where Tkey: struct 
        {
            using var context = new TContext();

            var expression = GetIdEqualExpression(id, context);
            TEntity? result = await context.Set<TEntity>()
                .AsNoTracking()
                .SingleOrDefaultAsync(expression, cancellationToken)
                .ConfigureAwait(false);

            return result;
        }

        public virtual async Task<TEntity?> GetAsync<Tkey>(
            Tkey id, 
            Expression<Func<TEntity, object>>[] includeProps, 
            CancellationToken cancellationToken = default
        ) where Tkey : struct
        {
            using var context = new TContext();

            var expression = GetIdEqualExpression(id, context);
            var query = context.Set<TEntity>().AsQueryable();
            
            foreach (var prop in includeProps)
            {
                query = query.Include(prop);
            }

            return await query.AsNoTracking()
                .SingleOrDefaultAsync(expression, cancellationToken)
                .ConfigureAwait(false);
        }

        public virtual async Task<List<TEntity>> GetListAsync(
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

        public virtual async Task<List<TEntity>> GetListAsync(
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

        public virtual async Task<PagedList<TEntity>> GetListAsync(
            int nextRecord, 
            int pageSize, 
            Expression<Func<TEntity, bool>>? filter = null,
            Expression<Func<TEntity, object>>? orderBy = null,
            bool asc = true,
            CancellationToken cancellationToken = default
        ) {
            using var context = new TContext();
            IQueryable<TEntity> resultSet;
            if(filter == null)
                resultSet = context.Set<TEntity>().AsNoTracking();
            else
                resultSet = context.Set<TEntity>().AsNoTracking().Where(filter);

            if(orderBy != null)
                if(asc)
                    resultSet = resultSet.OrderBy(orderBy);
                else
                    resultSet = resultSet.OrderByDescending(orderBy);

            var result = await ToPagedListAsync(resultSet, nextRecord, pageSize, cancellationToken).ConfigureAwait(false);
            return result;
        }

        public virtual async Task<TEntity> UpdateAsync(
            TEntity entity, 
            CancellationToken cancellationToken = default
        ) {
            using var context = new TContext();
            var result = context.Entry(entity);
            result.State = EntityState.Modified;
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return entity;
        }

        public virtual async Task<TEntity> UpdateAsync(
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
            int nextRecord, 
            int pageSize, 
            CancellationToken cancellationToken = default
        ) {
            var count = source.Count();
            var items = await source.Skip(nextRecord)
                .Take(pageSize)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            
            return new PagedList<TEntity>(items, nextRecord, pageSize, count);
        }

        public virtual async Task<TEntity?> GetAsync(
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

        public virtual async Task<TEntity> AddAsync(
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

        public virtual async Task<TEntity> UpdateAsync(
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

        protected async Task RetryOnConcurrencyErrorAsync(
            Func<Task<bool>>? inTry = null, 
            Func<DbUpdateConcurrencyException, Task<bool>>? inError = null, 
            CancellationToken cancellationToken = default
        ) {
            while(!cancellationToken.IsCancellationRequested) {
                try {
                    if(inTry != null)
                    if(!await inTry()) break;
                } catch(DbUpdateConcurrencyException e) {
                    if(inError != null)
                    if(!await inError(e)) break;
                }
            }
        }
    }
}