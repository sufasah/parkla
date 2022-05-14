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

    public override async Task<List<Park>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var includeProps = new Expression<Func<Park,object>>[]{
            x => x.User!
        };

        return await _parkRepo.GetListAsync(includeProps, null, cancellationToken).ConfigureAwait(false);
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
        

        var newPark = await _parkRepo.AddAsync(entity,cancellationToken).ConfigureAwait(false);

        HubParkChanges(newPark.Id, false);

        return newPark;
    }

    public async Task DeleteAsync(Park park, int userId, CancellationToken cancellationToken = default)
    {
        await ThrowIfUserNotMatch((int)park.Id!, userId, cancellationToken).ConfigureAwait(false);

        await base.DeleteAsync(park, cancellationToken).ConfigureAwait(false);   

        _parklaHubService.ParkChanges(park, true);
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

        await ThrowIfUserNotMatch((int)park.Id!, userId, cancellationToken).ConfigureAwait(false);

        var newPark = await _parkRepo.UpdateAsync(park, props, false, cancellationToken).ConfigureAwait(false);

        HubParkChanges(newPark.Id, false);
        
        return newPark;
    }

    private async Task ThrowIfUserNotMatch(int parkId, int userId, CancellationToken cancellationToken) {
        var park = await _parkRepo.GetAsync(x => x.Id == parkId, cancellationToken).ConfigureAwait(false);
        
        if(park!.UserId != userId)
            throw new ParklaException("User requested is not permitted to update or delete other user's park", HttpStatusCode.BadRequest);
    }

    private async Task HubParkChanges(int? id, bool isDelete) {
        var newParkWithUser = await _parkRepo.GetAsync(
            new Expression<Func<Park,object>>[]{x => x.User!},
            x => x.Id == id
        ).ConfigureAwait(false);
        
        await _parklaHubService.ParkChanges(newParkWithUser!, isDelete).ConfigureAwait(false);
    }
}