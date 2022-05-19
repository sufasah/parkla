using Parkla.Core.Entities;

namespace Parkla.DataAccess.Abstract;

public interface IParkAreaRepo : IEntityRepository<ParkArea>
{
    new Task<Tuple<ParkArea, Park>> AddAsync(
        ParkArea area, 
        CancellationToken cancellationToken = default
    );
    Task<ParkArea> UpdateTemplateAsync(
        ParkArea area,
        CancellationToken cancellationToken = default
    );
}