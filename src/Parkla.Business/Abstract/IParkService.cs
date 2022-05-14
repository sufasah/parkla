using Parkla.Core.Entities;

namespace Parkla.Business.Abstract;
public interface IParkService : IEntityService<Park>
{

    Task<Park> UpdateAsync(
        Park park,
        int userId,
        CancellationToken cancellationToken = default
    );

    public Task DeleteAsync(
        Park park, 
        int userId, 
        CancellationToken cancellationToken = default
    );
}