using Parkla.Core.Entities;
using Parkla.Core.Helpers;
using Parkla.Core.Models;

namespace Parkla.Business.Abstract;
public interface IParkAreaService : IEntityService<ParkArea>
{
    Task<ParkArea> UpdateAsync(
        ParkArea parkArea,
        int userId,
        bool templateMode,
        CancellationToken cancellationToken = default
    );

    public Task DeleteAsync(
        ParkArea parkArea,
        int userId,
        CancellationToken cancellationToken = default
    );

    Task<PagedList<InstantParkAreaReservedSpace>> GetPageAsync(
        Guid parkId, 
        int nextRecord, 
        int pageSize, 
        string? search, 
        string? orderBy, 
        bool ascending, 
        CancellationToken cancellationToken = default
    );

    Task<List<InstantParkAreaIdReservedSpace>> GetParkAreasReserverdSpaceCountAsync(
        int[] ids,
        CancellationToken cancellationToken
    );

    Task<List<Pricing>> GetAreaPricingsAsync(
        int areaId, 
        CancellationToken cancellationToken
    );
}