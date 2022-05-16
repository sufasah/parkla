using Parkla.Core.Entities;

namespace Parkla.Business.Abstract;
public interface IParkSpaceService : IEntityService<ParkSpace>
{
    public Task<List<ParkSpace>> GetAllAsync(
        int? areaId,
        bool includeReservations,
        CancellationToken cancellationToken = default
    );
}