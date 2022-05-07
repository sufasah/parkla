using System.Linq.Expressions;
using FluentValidation;
using Parkla.Business.Abstract;
using Parkla.Core.Entities;
using Parkla.DataAccess.Context;
using Parkla.Web.Helpers;

namespace Parkla.Business.Bases
{
    public abstract class EntityServiceBase<TEntity> : IEntityService<TEntity>
        where TEntity :class, IEntity, new()
    {
        private readonly IEntityRepository<TEntity> _entityRepository;
        private readonly IValidator<TEntity> _validator;

        protected EntityServiceBase(
            IEntityRepository<TEntity> entityRepository,
            IValidator<TEntity> validator
        )
        {
            _entityRepository = entityRepository;
            _validator = validator;
        }

        public virtual TEntity Add(TEntity entity)
        {
            return _entityRepository.Add(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            _entityRepository.Delete(entity);
        }

        public virtual List<TEntity> GetAll()
        {
            var entities = _entityRepository.GetList();
            return entities;
        }

        public virtual PagedList<TEntity> GetPage(int pageNumber, int pageSize, Expression<Func<TEntity, bool>>? filter = null)
        {
            var entities = _entityRepository.GetList(pageNumber, pageSize, filter);
            return entities;
        }

        public virtual TEntity Update(TEntity entity)
        {
            return _entityRepository.Update(entity);
        }
        
        private void ValidateAndThrow(TEntity entity) {
            _validator.ValidateAndThrow(entity);
        }
        private void ValidateAndThrow(ICollection<TEntity> entities) {
            foreach (var entity in entities)
            {
                ValidateAndThrow(entity);
            }
        }

        public virtual TEntity ValidateAdd(TEntity entity)
        {
            ValidateAndThrow(entity);
            return Add(entity);
        }

        public virtual void ValidateDelete(TEntity entity)
        {
            ValidateAndThrow(entity);
            Delete(entity);
        }
        public virtual TEntity ValidateUpdate(TEntity entity)
        {
            ValidateAndThrow(entity);
            return Update(entity);
        }
    }
}