
using Parkla.Core.Entities;

namespace Parkla.Core.Models;
public class InstantParkReservedSpace
{
    public Park Park { get; set; }
    public int ReservedSpaceCount { get; set; }

    public InstantParkReservedSpace(Park park, int reservedSpaceCount)
    {
        Park = park;
        ReservedSpaceCount = reservedSpaceCount;
    }
}