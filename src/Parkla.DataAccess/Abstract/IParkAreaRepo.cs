using System.Linq.Expressions;
using Parkla.Core.Entities;

namespace Parkla.DataAccess.Abstract;

public interface IParkAreaRepo : IEntityRepository<ParkArea>
{
    new Task<Tuple<ParkArea, Park?>> AddAsync(
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
}