
namespace Parkla.Core.Models;
public class InstantParkAreaIdReservedSpace
{
    public int AreaId { get; set; }
    public int ReservedSpaceCount { get; set; }

    public InstantParkAreaIdReservedSpace(int areaId, int reservedSpaceCount)
    {
        AreaId = areaId;
        ReservedSpaceCount = reservedSpaceCount;
    }
}