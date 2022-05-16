using Parkla.Core.Entities;

namespace Parkla.DataAccess.Abstract;

public interface IParkAreaRepo : IEntityRepository<ParkArea>
{
    Task<ParkArea> UpdateTemplateAsync(
        ParkArea area,
        CancellationToken cancellationToken = default
    );
}