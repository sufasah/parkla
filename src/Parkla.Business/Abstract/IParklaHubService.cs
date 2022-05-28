using Parkla.Core.Entities;

namespace Parkla.Business.Abstract;
public interface IParklaHubService
{
    Task ParkChangesAsync(
        Park park, 
        bool isDelete
    );

    Task ReservationChangesAsync(
        Reservation reservation, 
        bool isDelete
    );

    Task ParkSpaceChangesAsync(
        ParkSpace space,
        bool isDelete
    );

    Task ParkAreaChangesAsync(
        ParkArea space,
        bool isDelete
    );
}