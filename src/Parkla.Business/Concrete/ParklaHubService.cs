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
        if(!isDelete) {
            var newPark = await IncludeUserAsync(park).ConfigureAwait(false);
            if(newPark == null) return;
            park = newPark;
        }

        await _hub.Clients
            .Group(HubConstants.EventParkChangesGroup)
            .SendAsync(HubConstants.EventParkChanges, park, isDelete)
            .ConfigureAwait(false);
    }

    public async Task ParkSpaceChangesAsync(ParkSpace space, bool isDelete)
    {
        await _hub.Clients
            .Group(HubConstants.EventParkSpaceChangesGroup(space.AreaId!.Value))
            .SendAsync(HubConstants.EventParkSpaceChanges, space, isDelete)
            .ConfigureAwait(false);
    }

    public async Task ReservationChangesAsync(Reservation reservation, bool isDelete)
    {
        await _hub.Clients
            .Group(HubConstants.EventReservationChangesGroup(reservation.Space!.AreaId!.Value))
            .SendAsync(HubConstants.EventReservationChanges, reservation, isDelete)
            .ConfigureAwait(false);
    }

    public async Task ParkAreaChangesAsync(ParkArea area, bool isDelete)
    {
        await _hub.Clients
            .Group(HubConstants.EventParkAreaChangesGroup(area.ParkId!.Value))
            .SendAsync(HubConstants.EventParkAreaChanges, area)
            .ConfigureAwait(false);
    }
}