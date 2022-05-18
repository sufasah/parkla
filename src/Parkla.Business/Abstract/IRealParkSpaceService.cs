using Parkla.Core.Entities;
using Parkla.Core.Helpers;

namespace Parkla.Business.Abstract;
public interface IRealParkSpaceService : IEntityService<RealParkSpace>
{
    public Task<PagedList<RealParkSpace>> GetPageAsync(
        Guid parkId,
        int nextRecord, 
        int pageSize,
        string? search, 
        string? orderBy, 
        bool ascending, 
        CancellationToken cancellationToken = default
    );
}