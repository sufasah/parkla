using Parkla.Core.Entities;

namespace Parkla.Business.Abstract;
public interface IParkAreaService : IEntityService<ParkArea>
{
    Task<ParkArea> UpdateAsync(
        ParkArea parkArea,
        int userId,
        CancellationToken cancellationToken = default
    );

    public Task DeleteAsync(
        ParkArea parkArea, 
        int userId, 
        CancellationToken cancellationToken = default
    );
}