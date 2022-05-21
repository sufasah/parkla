using System.Linq.Expressions;
using System.Net;
using FluentValidation;
using Parkla.Business.Abstract;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.Core.Exceptions;
using Parkla.Core.Models;
using Parkla.DataAccess.Abstract;

namespace Parkla.Business.Concrete;
public class ParkService : EntityServiceBase<Park>, IParkService
{
    private readonly IParkRepo _parkRepo;
    private readonly IValidator<Park> _validator;
    private readonly IParklaHubService _parklaHubService;

    public ParkService(
        IParkRepo parkRepo, 
        IValidator<Park> validator,
        IParklaHubService parklaHubService
    ) : base(parkRepo, validator)
    {
        _parkRepo = parkRepo;
        _validator = validator;
        _parklaHubService = parklaHubService;
    }

    public new async Task<List<InstantParkReservedSpace>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _parkRepo.GetAllParksAsync(cancellationToken).ConfigureAwait(false);
    }

    public override async Task<Park> AddAsync(Park entity, CancellationToken cancellationToken = default)
    {
        var result = await _validator.ValidateAsync(entity, o => o.IncludeRuleSets("add"), cancellationToken).ConfigureAwait(false);
        if (!result.IsValid)
            throw new ParklaException(result.ToString(), HttpStatusCode.BadRequest);
        
        entity.StatusUpdateTime = null;
        entity.EmptySpace = 0;
        entity.OccupiedSpace = 0;
        entity.MinPrice = null;
        entity.AvaragePrice = null;
        entity.MaxPrice = null;
        

        var newPark = await _parkRepo.AddAsync(entity,cancellationToken).ConfigureAwait(false);

        _ = _parklaHubService.ParkChangesAsync(newPark, false);

        return newPark;
    }

    public async Task DeleteAsync(Park park, int userId, CancellationToken cancellationToken = default)
    {
        await ThrowIfUserNotMatch(park.Id, userId, cancellationToken).ConfigureAwait(false);

        await base.DeleteAsync(park, cancellationToken).ConfigureAwait(false);   

        _ = _parklaHubService.ParkChangesAsync(park, true);
    }

    public async Task<Park> UpdateAsync(Park park, int userId, CancellationToken cancellationToken = default)
    {
        var result = await _validator.ValidateAsync(park, o => o.IncludeRuleSets("add","id"), cancellationToken).ConfigureAwait(false);
        if (!result.IsValid)
            throw new ParklaException(result.ToString(), HttpStatusCode.BadRequest);
        
        var props = new Expression<Func<Park,object?>>[]{
            x => x.Name,
            x => x.Location,
            x => x.Latitude,
            x => x.Longitude,
            x => x.Extras
        };

        await ThrowIfUserNotMatch(park.Id, userId, cancellationToken).ConfigureAwait(false);

        var newPark = await _parkRepo.UpdateAsync(park, props, false, cancellationToken).ConfigureAwait(false);

        _ = _parklaHubService.ParkChangesAsync(newPark, false);
        
        return newPark;
    }

    private async Task ThrowIfUserNotMatch(Guid?  parkId, int userId, CancellationToken cancellationToken) {
        var notAllowed = new ParklaException("User requested is not permitted to update or delete other user's park", HttpStatusCode.BadRequest);

        if(parkId == null) throw notAllowed;

        var park = await _parkRepo.GetAsync(x => x.Id == parkId, cancellationToken).ConfigureAwait(false);
        
        if(park!.UserId != userId) throw notAllowed;
    }

    public async Task<List<InstantParkIdReservedSpace>> GetAllParksReservedSpaceCount(CancellationToken cancellationToken = default)
    {
        return await _parkRepo.GetAllParksReservedSpaceCountAsync(cancellationToken).ConfigureAwait(false);
    }
}