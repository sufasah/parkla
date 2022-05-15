using System;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
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

        public async Task<PagedList<TEntity>> GetPageAsync(
            int pageNumber, 
            int pageSize, 
            string? search, 
            string? orderBy, 
            bool ascending, 
            CancellationToken cancellationToken = default
        ) {
            if(search != null)
                search = search.Trim();
            if(string.IsNullOrWhiteSpace(search))
                search = null;
            
            if(orderBy != null)
                orderBy = orderBy.Trim();
            if(string.IsNullOrWhiteSpace(orderBy))
                orderBy = null;
            
            var entityType = typeof(TEntity);
            var stringType = typeof(string);
            var stringProps = entityType.GetProperties().Where(x => 
                x.PropertyType == stringType ||
                (x.PropertyType.IsValueType &&
                x.PropertyType.GetMethod("ToString")!.DeclaringType == x.PropertyType)
            ).ToList()!;

            var eParam = Expression.Parameter(entityType, "x");

            Expression eFilter = Expression.Constant(search == null);
            if(search != null) {
                var searchConstant = Expression.Constant(search.ToLower());
                foreach (var prop in stringProps)
                {   
                    var property = Expression.Property(eParam, prop);
                    var notNull = Expression.NotEqual(property, Expression.Constant(null));
                    
                    Expression convert = property;
                    if(prop.PropertyType != stringType)
                        convert = Expression.Call(property, prop.PropertyType.GetMethod("ToString", Type.EmptyTypes)!);
                    
                    var lowerConvert = Expression.Call(convert, stringType.GetMethod("ToLower", Type.EmptyTypes)!);
                    var contains = Expression.Call(lowerConvert, stringType.GetMethod("Contains",new Type[]{stringType})!, searchConstant);

                    var notNullAndContains = Expression.AndAlso(notNull, contains);
                    eFilter = Expression.OrElse(eFilter, notNullAndContains);
                }
            }

            Expression<Func<TEntity,object>>? eOrderBy = null;
            if(orderBy != null) {
                var orderByProp = entityType.GetProperty(
                    orderBy, 
                    BindingFlags.Public | 
                    BindingFlags.Instance | 
                    BindingFlags.IgnoreCase
                );

                if(orderByProp != null) {
                    var property = Expression.Property(eParam, orderByProp);
                    var convert = Expression.Convert(property, typeof(object));
                    eOrderBy = Expression.Lambda<Func<TEntity,object>>(convert, eParam);
                }
            }

            return await _entityRepository.GetListAsync(
                pageNumber, 
                pageSize,
                Expression.Lambda<Func<TEntity, bool>>(eFilter, eParam),
                eOrderBy,
                ascending,
                cancellationToken
            ).ConfigureAwait(false);
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