
namespace Parkla.Core.Models;
public class InstantParkIdReservedSpace
{
    public Guid ParkId { get; set; }
    public int ReservedSpaceCount { get; set; }

    public InstantParkIdReservedSpace(Guid parkId, int reservedSpaceCount)
    {
        ParkId = parkId;
        ReservedSpaceCount = reservedSpaceCount;
    }
}