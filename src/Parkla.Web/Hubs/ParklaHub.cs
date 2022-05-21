using System.Runtime.CompilerServices;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Parkla.Business.Abstract;
using Parkla.Core.Constants;
using Parkla.Core.Entities;
using Parkla.Core.Models;
using Parkla.Web.Models;

namespace Parkla.Web.Hubs;
public class ParklaHub : Hub 
{
    private readonly IParkService _parkService;
    private readonly ILogger<ParklaHub> _logger;
    private readonly IMapper _mapper;

    public ParklaHub(
        IParkService parkService,
        ILogger<ParklaHub> logger,
        IMapper mapper
    ) {
        _parkService = parkService;
        _logger = logger;
        _mapper = mapper;
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

    public async IAsyncEnumerable<ParkIncludesUserDto> AllParksStream(
        [EnumeratorCancellation]
        CancellationToken cancellationToken
    ) {
        var linkedCancellationToken = CancellationTokenSource
            .CreateLinkedTokenSource(cancellationToken, Context.ConnectionAborted).Token;
        
        var allParks = await _parkService.GetAllAsync(linkedCancellationToken);
        
        foreach (var park in allParks)
        {
            if(linkedCancellationToken.IsCancellationRequested) break;
            yield return _mapper.Map<ParkIncludesUserDto>(park);
        }
    }

    public async IAsyncEnumerable<InstantParkIdReservedSpace> AllParksReservedSpaceCount(
        [EnumeratorCancellation]
        CancellationToken cancellationToken
    ) {
        var linkedCancellationToken = CancellationTokenSource
            .CreateLinkedTokenSource(cancellationToken, Context.ConnectionAborted).Token;
        
        while (true)
        {
            var parksWithReservedSpaceCount = await _parkService.GetAllParksReservedSpaceCount(linkedCancellationToken);

            foreach (var item in parksWithReservedSpaceCount)
            {
                if(linkedCancellationToken.IsCancellationRequested) break;
                yield return item;
            }

            await Task.Delay(1000, cancellationToken);
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

    public async Task RegisterParkSpaceChanges() {
        await Groups.AddToGroupAsync(
            Context.ConnectionId, 
            HubConstants.EventParkSpaceChangesGroup, 
            Context.ConnectionAborted)
            .ConfigureAwait(false);
    }
    
    public async Task UnRegisterParkSpaceChanges() {
        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId, 
            HubConstants.EventParkSpaceChangesGroup, 
            Context.ConnectionAborted)
            .ConfigureAwait(false);
    }

    public async Task RegisterParkAreaChanges() {
        await Groups.AddToGroupAsync(
            Context.ConnectionId, 
            HubConstants.EventParkAreaChangesGroup, 
            Context.ConnectionAborted)
            .ConfigureAwait(false);
    }
    
    public async Task UnRegisterParkAreaChanges() {
        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId, 
            HubConstants.EventParkAreaChangesGroup, 
            Context.ConnectionAborted)
            .ConfigureAwait(false);
    }
    
}