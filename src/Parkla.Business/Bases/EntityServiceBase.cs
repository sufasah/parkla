using System;
using System.Linq.Expressions;
using System.Net;
using FluentValidation;
using Parkla.Business.Abstract;
using Parkla.Core.Entities;
using Parkla.Core.Exceptions;
using Parkla.Core.Helpers;
using Parkla.DataAccess.Abstract;

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

        public virtual async Task<TEntity> NoValidateAddAsync(
            TEntity entity,
            CancellationToken cancellationToken = default
        ) {
            return await _entityRepository.AddAsync(entity, cancellationToken);
        }

        public virtual async Task NoValidateDeleteAsync(
            TEntity entity,
            CancellationToken cancellationToken = default
        ) {
            await _entityRepository.DeleteAsync(entity, cancellationToken);
        }

        public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var entities = await _entityRepository.GetListAsync(cancellationToken: cancellationToken);
            return entities;
        }

        public virtual async Task<PagedList<TEntity>> GetPageAsync(
            int pageNumber, 
            int pageSize,
            CancellationToken cancellationToken = default
        ) {
            var entities = _entityRepository.GetListAsync(pageNumber, pageSize, cancellationToken: cancellationToken);
            return await entities;
        }

        public virtual async Task<TEntity> NoValidateUpdateAsync(
            TEntity entity, 
            CancellationToken cancellationToken = default
        ) {
            return await _entityRepository.UpdateAsync(entity, cancellationToken);
        }
        
        private void ValidateAndThrow(TEntity entity) {
            var result = _validator.Validate(entity);
            if (!result.IsValid)
                throw new ParklaException(result.ToString(), HttpStatusCode.BadRequest);
        }
        
        private void ValidateAndThrow(ICollection<TEntity> entities) {
            foreach (var entity in entities)
            {
                var result = _validator.Validate(entity);
                if (!result.IsValid)
                    throw new ParklaException(result.ToString(), HttpStatusCode.BadRequest);
            }
        }

        public virtual async Task<TEntity> AddAsync(
            TEntity entity, 
            CancellationToken cancellationToken = default
        ) {
            ValidateAndThrow(entity);
            return await NoValidateAddAsync(entity, cancellationToken);
        }

        public virtual async Task DeleteAsync(
            TEntity entity,
            CancellationToken cancellationToken = default
        ) {
            await NoValidateDeleteAsync(entity, cancellationToken);
        }
        public virtual async Task<TEntity> UpdateAsync(
            TEntity entity,
            CancellationToken cancellationToken = default
        ) {
            ValidateAndThrow(entity);
            return await NoValidateUpdateAsync(entity, cancellationToken);
        }

        public virtual async Task<TEntity?> GetAsync(
            Expression<Func<TEntity,bool>> filter,
            CancellationToken cancellationToken = default
        ) {
            return await _entityRepository.GetAsync(filter, cancellationToken);
        }

        public virtual async Task<TEntity?> GetAsync(
            int id,
            CancellationToken cancellationToken = default
        ) {
            return await _entityRepository.GetAsync(
                id,
                cancellationToken
            );
        }
    }
}