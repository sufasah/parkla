using Microsoft.AspNetCore.SignalR;
using Parkla.Business.Abstract;
using Parkla.Core.Constants;
using Parkla.Core.Entities;

namespace Parkla.Business.Concrete;

public class ParklaHubService<T> : IParklaHubService
    where T: Hub
{
    private readonly IHubContext<T> _hub;

    public ParklaHubService(
        IHubContext<T> hub
    ) {
        _hub = hub;
    }

    public async Task ParkChanges(Park park, bool isDelete, CancellationToken cancellationToken = default) {
        await _hub.Clients
            .Group(HubConstants.EventParkChangesGroup)
            .SendAsync(HubConstants.EventParkChanges, park, isDelete, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task ParkSpaceChanges(ParkSpace space, bool isDelete, CancellationToken cancellationToken = default)
    {
        await _hub.Clients
            .Group(HubConstants.EventParkSpaceChangesGroup)
            .SendAsync(HubConstants.EventParkSpaceChanges, space, isDelete, cancellationToken)
            .ConfigureAwait(false);
    }
}