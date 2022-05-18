using Parkla.Core.Entities;

namespace Parkla.Business.Abstract;
public interface IParklaHubService
{
    public Task ParkChanges(
        Park park, 
        bool isDelete, 
        CancellationToken cancellationToken = default
    );

    public Task ParkSpaceChanges(
        ParkSpace space,
        bool isDelete,
        CancellationToken cancellationToken = default
    );
}