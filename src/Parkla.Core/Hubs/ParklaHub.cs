using Microsoft.AspNetCore.SignalR;

namespace Parkla.Core.Hubs;
public class ParklaHub : Hub 
{

    public override async Task OnConnectedAsync()
    {
        await Clients.Caller
            .SendAsync(
                "connected", 
                "Parkla hub connection established. This is initialization message", 
                Context.ConnectionAborted)
            .ConfigureAwait(false);
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