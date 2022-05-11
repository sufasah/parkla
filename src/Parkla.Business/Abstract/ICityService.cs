using Parkla.Core.Entities;

namespace Parkla.Business.Abstract;
public interface ICityService : IEntityService<City>
{
    Task<List<City>> SearchAsync(
        string search,
        CancellationToken cancellationToken = default
    );
}