using System.Linq.Expressions;
using Microsoft.AspNetCore.SignalR;
using Parkla.Business.Abstract;
using Parkla.Core.Constants;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;

namespace Parkla.Business.Concrete;

public class ParklaHubService<T> : IParklaHubService
    where T: Hub
{
    private readonly IHubContext<T> _hub;
    private readonly IParkRepo _parkRepo;

    public ParklaHubService(
        IHubContext<T> hub,
        IParkRepo parkRepo
    ) {
        _hub = hub;
        _parkRepo = parkRepo;
    }
    
    private async Task<Park?> IncludeUserAsync(Park park) {
        if(park.User == null) {
            return await _parkRepo.GetAsync(
                park.Id!.Value,
                new Expression<Func<Park,object>>[] {x => x.User!}
            ).ConfigureAwait(false);
        }
        return park;
    }

    public async Task ParkChangesAsync(Park park, bool isDelete) {
        var newPark = await IncludeUserAsync(park).ConfigureAwait(false);
        if(newPark == null) return;

        await _hub.Clients
            .Group(HubConstants.EventParkChangesGroup)
            .SendAsync(HubConstants.EventParkChanges, newPark, isDelete)
            .ConfigureAwait(false);
    }

    public async Task ParkSpaceChangesAsync(ParkSpace space, bool isDelete)
    {
        await _hub.Clients
            .Group(HubConstants.EventParkSpaceChangesGroup)
            .SendAsync(HubConstants.EventParkSpaceChanges, space, isDelete)
            .ConfigureAwait(false);
    }
}