using Parkla.Core.DTOs;
using Parkla.Core.Entities;

namespace Parkla.Business.Abstract;
public interface IUserService : IEntityService<User>
{
    Task<User?> LoadMoneyAsync(
        int id, 
        float amount, 
        CancellationToken cancellationToken
    );

    Task<DashboardDto> GetDashboardAsync(
        int id,
        CancellationToken cancellationToken
    );
}