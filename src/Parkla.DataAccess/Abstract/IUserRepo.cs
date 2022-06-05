using Parkla.Core.DTOs;
using Parkla.Core.Entities;

namespace Parkla.DataAccess.Abstract;

public interface IUserRepo : IEntityRepository<User>
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