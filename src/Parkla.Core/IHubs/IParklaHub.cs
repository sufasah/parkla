using Microsoft.AspNetCore.SignalR;

namespace Parkla.Core.IHubs;

public interface IParklaHub<T> : IHubContext<T>
    where T: Hub
{
}