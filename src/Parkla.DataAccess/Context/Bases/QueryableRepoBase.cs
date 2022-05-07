using Microsoft.EntityFrameworkCore;
using Parkla.Core.Entities;

namespace Parkla.DataAccess.Context
{
    public class QueryableRepoBase<TEntity> : IQueryableRepository<TEntity>
        where TEntity : class, IEntity, new()
    {
        private readonly DbContext _context;
        private readonly DbSet<TEntity> _entities;
        public QueryableRepoBase(DbContext context)
        {
            _context = context;
            _entities = _context.Set<TEntity>();

        }
        public IQueryable<TEntity> Table{
            get{
                return Entities;
            }
        }
        protected virtual DbSet<TEntity> Entities{
            get {
                return _entities;
            }
            private set {}
        }
    }
}