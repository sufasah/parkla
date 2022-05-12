using System;
using System.Linq.Expressions;
using System.Net;
using FluentValidation;
using Parkla.Business.Abstract;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.Core.Exceptions;
using Parkla.DataAccess.Abstract;

namespace Parkla.Business.Concrete;
public class ParkService : EntityServiceBase<Park>, IParkService
{
    private readonly IParkRepo _parkRepo;
    private readonly IValidator<Park> _validator;

    public ParkService(
        IParkRepo parkRepo, 
        IValidator<Park> validator
    ) : base(parkRepo, validator)
    {
        _parkRepo = parkRepo;
        _validator = validator;
    }

    public override async Task<List<Park>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var includeProps = new Expression<Func<Park,object>>[]{
            x => x.User!
        };

        return await _parkRepo.GetListAsync(includeProps, null, cancellationToken);
    }

    public override async Task<Park> AddAsync(Park entity, CancellationToken cancellationToken = default)
    {
        var result = await _validator.ValidateAsync(entity, o => o.IncludeRuleSets("add"), cancellationToken).ConfigureAwait(false);
        if (!result.IsValid)
            throw new ParklaException(result.ToString(), HttpStatusCode.BadRequest);
        
        entity.StatusUpdateTime = new DateTime(0, DateTimeKind.Utc);
        entity.EmptySpace = 0;
        entity.ReservedSpace = 0;
        entity.OccupiedSpace = 0;
        entity.MinPrice = -1;
        entity.AvaragePrice = -1;
        entity.MaxPrice = -1;
        
        return await _parkRepo.AddAsync(entity, cancellationToken).ConfigureAwait(false);
    }

    public override async Task<Park> UpdateAsync(Park entity, CancellationToken cancellationToken = default)
    {
        var result = await _validator.ValidateAsync(entity, o => o.IncludeRuleSets("add","id"), cancellationToken).ConfigureAwait(false);
        if (!result.IsValid)
            throw new ParklaException(result.ToString(), HttpStatusCode.BadRequest);
        
        var props = new Expression<Func<Park,object?>>[]{
            x => x.StatusUpdateTime,
            x => x.EmptySpace,
            x => x.ReservedSpace,
            x => x.OccupiedSpace,
            x => x.MinPrice,
            x => x.AvaragePrice,
            x => x.MaxPrice,
        };
        
        return await _parkRepo.UpdateAsync(entity, props, true, cancellationToken).ConfigureAwait(false);
    }
}