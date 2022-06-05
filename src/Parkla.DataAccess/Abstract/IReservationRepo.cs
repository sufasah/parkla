using Parkla.Core.Entities;

namespace Parkla.DataAccess.Abstract;

public interface IReservationRepo : IEntityRepository<Reservation> {
    Task<List<Reservation>> UserReservationListAsync(
        int userId, 
        CancellationToken cancellationToken
    );

    Task<Reservation?> DeleteReservationAsync(
        Reservation reservation,
        CancellationToken cancellationToken
    );
}