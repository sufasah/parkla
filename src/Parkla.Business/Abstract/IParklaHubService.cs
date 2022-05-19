using Parkla.Core.Entities;

namespace Parkla.Business.Abstract;
public interface IParklaHubService
{
    public Task ParkChangesAsync(
        Park park, 
        bool isDelete
    );

    public Task ParkSpaceChangesAsync(
        ParkSpace space,
        bool isDelete
    );
}