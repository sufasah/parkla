using Parkla.Core.Entities;

namespace Parkla.DataAccess.Context
{
    public interface IQueryableRepository<T> 
        where T:class, IEntity, new()
    {
         IQueryable<T> Table {get;}
    }
}