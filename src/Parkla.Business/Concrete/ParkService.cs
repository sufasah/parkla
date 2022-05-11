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

    public override async Task<Park> AddAsync(Park entity, CancellationToken cancellationToken = default)
    {
        var result = await _validator.ValidateAsync(entity, o => o.IncludeRuleSets("add"), cancellationToken).ConfigureAwait(false);
        if (!result.IsValid)
            throw new ParklaException(result.ToString(), HttpStatusCode.BadRequest);
            
        return await _parkRepo.AddAsync(entity, cancellationToken).ConfigureAwait(false);
    }
}