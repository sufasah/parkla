using Parkla.Core.DTOs;

namespace Parkla.Business.Abstract;
public interface ICollectorService
{
    public Task CollectParkSpaceStatusAsync(ParkSpaceStatusDto dto);
    public Task CollectParkSpaceStatusAsync(IEnumerable<ParkSpaceStatusDto> dto);
}