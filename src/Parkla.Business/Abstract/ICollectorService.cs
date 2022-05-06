using Parkla.Core.DTOs;

namespace Parkla.Business.Abstract;
public interface ICollectorService
{
    public void CollectParkSpaceStatus(ParkSpaceStatusDto dto);
    public void CollectParkSpaceStatus(IEnumerable<ParkSpaceStatusDto> dto);
}