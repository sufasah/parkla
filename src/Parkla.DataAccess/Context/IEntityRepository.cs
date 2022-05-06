using System.Linq.Expressions;
using Parkla.Core.Entities;

namespace Parkla.DataAccess.Context
{
    public interface IEntityRepository<T> 
        where T:class, IEntity, new()
    {
        List<T> GetList(Expression<Func<T,bool>>? filter = null);
        
        T Get(Expression<Func<T,bool>> filter);

        T Add(T entity);

        T Update(T entity);

        void Delete(T entity);
    }
}