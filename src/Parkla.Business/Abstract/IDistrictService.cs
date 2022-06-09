using Parkla.Core.Entities;

namespace Parkla.Business.Abstract;
public interface IDistrictService : IEntityService<District>
{
    Task<List<District>> SearchAsync(
        int cityId,
        string search,
        CancellationToken cancellationToken = default
    );
}