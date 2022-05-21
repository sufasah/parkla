
using Parkla.Core.Entities;

namespace Parkla.Core.Models;
public class InstantParkAreaReservedSpace
{
    public ParkArea ParkArea { get; set; }
    public int ReservedSpaceCount { get; set; }

    public InstantParkAreaReservedSpace(ParkArea parkArea, int reservedSpaceCount)
    {
        ParkArea = parkArea;
        ReservedSpaceCount = reservedSpaceCount;
    }
}