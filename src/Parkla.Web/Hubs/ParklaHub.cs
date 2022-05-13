using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.SignalR;
using Parkla.Business.Abstract;
using Parkla.Core.Constants;
using Parkla.Core.Entities;

namespace Parkla.Web.Hubs;
public class ParklaHub : Hub 
{
    private readonly IParkService _parkService;

    public ParklaHub(IParkService parkService)
    {
        _parkService = parkService;
    }
    public override async Task OnConnectedAsync()
    {
        await Clients.Caller
            .SendAsync(
                "connected", 
                "Parkla hub connection established. This is initialization message", 
                Context.ConnectionAborted)
            .ConfigureAwait(false);
    }

    public async IAsyncEnumerable<Park> AllParksStream(
        [EnumeratorCancellation]
        CancellationToken cancellationToken
    ) {
        var linkedCancellationToken = CancellationTokenSource
            .CreateLinkedTokenSource(cancellationToken, Context.ConnectionAborted).Token;
        
        var allParks = await _parkService.GetAllAsync(linkedCancellationToken);
        
        foreach (var park in allParks)
        {
            if(linkedCancellationToken.IsCancellationRequested) break;
            yield return park;
        }
    }

    public async Task RegisterParkChanges() {
        await Groups.AddToGroupAsync(
            Context.ConnectionId, 
            HubConstants.EventParkChangesGroup, 
            Context.ConnectionAborted)
            .ConfigureAwait(false);
    }
    
    public async Task UnRegisterParkChanges() {
        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId, 
            HubConstants.EventParkChangesGroup, 
            Context.ConnectionAborted)
            .ConfigureAwait(false);
    }
    
}