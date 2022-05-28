using System.Linq.Expressions;
using Parkla.Core.Entities;
using Parkla.Core.Helpers;
using Parkla.Core.Models;

namespace Parkla.DataAccess.Abstract;

public interface IParkAreaRepo : IEntityRepository<ParkArea>
{
    new Task<ParkArea> AddAsync(
        ParkArea area, 
        CancellationToken cancellationToken = default
    );

    new Task<Tuple<ParkArea, Park?>> UpdateAsync(
        ParkArea area,
        CancellationToken cancellationToken = default
    );
    
    Task<Tuple<ParkArea, Park?>> UpdateTemplateAsync(
        ParkArea area,
        CancellationToken cancellationToken = default
    );

    new Task<Tuple<ParkArea?,Park?>> DeleteAsync(
        ParkArea area, 
        CancellationToken cancellationToken = default
    );

    Task<PagedList<InstantParkAreaReservedSpace>> GetParkAreaPage(
        int nextRecord, 
        int pageSize, 
        Expression<Func<ParkArea, bool>>? filter = null,
        Expression<Func<ParkArea, object>>? orderBy = null,
        bool asc = true,
        CancellationToken cancellationToken = default
    );

    Task<InstantParkAreaReservedSpace?> GetParkAreaAsync(
        Expression<Func<ParkArea, bool>>? filter = null,
        CancellationToken cancellationToken = default
    );

    Task<List<InstantParkAreaIdReservedSpace>> GetParkAreasReserverdSpaceCountAsync(
        int[] ids, 
        CancellationToken cancellationToken
    );
}