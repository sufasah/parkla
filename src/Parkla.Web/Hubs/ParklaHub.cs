using Microsoft.AspNetCore.SignalR;
using Parkla.Core.Entities;

namespace Parkla.Web.Hubs;
public class ParklaHub : Hub
{
    private readonly string eventParkChanges = "ParkAddUpdateDelete";
    private readonly string eventParkChangesGroup = "ParkAddUpdateDeleteGroup";
    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("connected", "Parkla hub connection established. This is initialization message", Context.ConnectionAborted);
    }
    
    public async Task RegisterParkChanges() {
        await Groups.AddToGroupAsync(Context.ConnectionId, eventParkChangesGroup, Context.ConnectionAborted);
    }

    public async Task ParkChanges(Park park, bool isDelete) {
        await Clients.Group(eventParkChangesGroup).SendAsync(eventParkChanges, park, isDelete, Context.ConnectionAborted);
    }
    
    public async Task UnRegisterParkChanges() {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, eventParkChangesGroup, Context.ConnectionAborted);
    }
    
}