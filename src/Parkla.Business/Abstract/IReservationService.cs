using Parkla.Core.Entities;
using Parkla.Core.Helpers;

namespace Parkla.Business.Abstract;
public interface IReservationService : IEntityService<Reservation>
{
    Task<List<Reservation>> GetUserReservationsAsync(
        int userId,
        CancellationToken cancellationToken
    );
}