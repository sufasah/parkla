using Parkla.Core.Entities;
using Parkla.Core.Models;

namespace Parkla.DataAccess.Abstract;

public interface IParkRepo : IEntityRepository<Park> {
    Task<List<InstantParkIdReservedSpace>> GetAllParksReservedSpaceCountAsync(
        CancellationToken cancellationToken = default
    );

    Task<List<InstantParkReservedSpace>> GetAllParksAsync(
        CancellationToken cancellationToken = default
    );
}