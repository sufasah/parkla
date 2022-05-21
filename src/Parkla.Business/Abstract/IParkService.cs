using Parkla.Core.Entities;
using Parkla.Core.Models;

namespace Parkla.Business.Abstract;
public interface IParkService : IEntityService<Park>
{
    new Task<List<InstantParkReservedSpace>> GetAllAsync(
        CancellationToken cancellationToken = default
    );
    Task<Park> UpdateAsync(
        Park park,
        int userId,
        CancellationToken cancellationToken = default
    );

    Task DeleteAsync(
        Park park,
        int userId,
        CancellationToken cancellationToken = default
    );

    Task<List<InstantParkIdReservedSpace>> GetAllParksReservedSpaceCount(
        CancellationToken cancellationToken = default
    );
}