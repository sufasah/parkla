using Parkla.Core.Entities;
using Parkla.DataAccess.Context;

namespace Parkla.DataAccess.Abstract;

public interface IParkSpaceRepo : IEntityRepository<ParkSpace> {
    
    Task<List<ParkSpace>> GetListAsync(
        int? areaId,
        bool includeReservations,
        CancellationToken cancellationToken = default
    );
}