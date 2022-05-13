using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using Parkla.Business.Abstract;
using Parkla.Core.Entities;
using Parkla.Core.Hubs;
using static Parkla.Business.Abstract.IParklaHubService;

namespace Parkla.Business.Concrete;

public class ParklaHubService : IParklaHubService
{
    private readonly IHubContext<ParklaHub> _hub;

    public ParklaHubService(
        IHubContext<ParklaHub> hub
    ) {
        _hub = hub;
    }

    public async Task ParkChanges(Park park, bool isDelete, CancellationToken cancellationToken = default) {
        await _hub.Clients
            .Group(HubConstants.EventParkChangesGroup)
            .SendAsync(HubConstants.EventParkChanges, park, isDelete, cancellationToken)
            .ConfigureAwait(false);
    }
}