using System.Linq.Expressions;
using Parkla.Core.Entities;
using Parkla.Web.Helpers;

namespace Parkla.Business.Abstract;
public interface IEntityService<TEntity> 
    where TEntity: class, IEntity, new()
{
        List<TEntity> GetAll();

        PagedList<TEntity> GetPage(int pageNumber, int PageSize, Expression<Func<TEntity, bool>>? filter);

        TEntity Add(TEntity entity);

        TEntity Update(TEntity entity);

        void Delete(TEntity entity);

        TEntity ValidateAdd(TEntity entity);

        void ValidateDelete(TEntity entity);
        TEntity ValidateUpdate(TEntity entity);

}