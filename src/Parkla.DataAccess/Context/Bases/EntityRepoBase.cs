using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Parkla.Core.Entities;

namespace Parkla.DataAccess.Context
{
    public class EntityRepoBase<TEntity,TContext> : IEntityRepository<TEntity> 
        where TEntity :class, IEntity, new()
        where TContext:DbContext,new()
    {
        public TEntity Add(TEntity entity)
        {
            using var context = new TContext();
            var result = context.Entry(entity);
            result.State = EntityState.Added;
            context.SaveChanges();
            return entity;
        }

        public void Delete(TEntity entity)
        {
            using var context = new TContext();
            var result = context.Entry(entity);
            result.State = EntityState.Deleted;
            context.SaveChanges();
        }

        public TEntity Get(Expression<Func<TEntity, bool>> filter)
        {
            using var context = new TContext();
            TEntity result = context.Set<TEntity>().SingleOrDefault(filter);
            return result;
        }

        public List<TEntity> GetList(Expression<Func<TEntity, bool>> filter = null)
        {
            using var context = new TContext();
            if (filter == null)
                return context.Set<TEntity>().ToList();
            List<TEntity> result = context.Set<TEntity>().Where(filter).ToList();
            return result;
        }

        public TEntity Update(TEntity entity)
        {
            using var context = new TContext();
            var result = context.Entry(entity);
            result.State = EntityState.Modified;
            context.SaveChanges();
            return entity;
        }
    }
}