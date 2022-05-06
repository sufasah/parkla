using Microsoft.EntityFrameworkCore;
using Parkla.Core.Entities;

namespace Parkla.DataAccess.Context
{
    public class QueryableRepoBase<TEntity> : IQueryableRepository<TEntity>
        where TEntity : class, IEntity, new()
    {
        private readonly DbContext _context;
        private DbSet<TEntity> _entities;
        public QueryableRepoBase(DbContext context)
        {
            _context = context;
        }
        public IQueryable<TEntity> Table{
            get{
                return Entities;
            }
        }
        protected virtual DbSet<TEntity> Entities{
            get{
                if(_entities==null){
                    _entities = _context.Set<TEntity>();
                }
                return _entities;
            }
            private set {}
        }
    }
}