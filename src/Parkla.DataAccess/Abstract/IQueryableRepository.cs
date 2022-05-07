using Parkla.Core.Entities;

namespace Parkla.DataAccess.Abstract
{
    public interface IQueryableRepository<T> 
        where T:class, IEntity, new()
    {
         IQueryable<T> Table {get;}
    }
}