using Parkla.Core.Entities;
using Parkla.Core.Enums;

namespace Parkla.Web.Models;
public class ReceivedSpaceStatusDto
{
    public int Id { get; set; }
    public int SpaceId { get; set; }
    public ParkSpace Space { get; set; }
    public int RealSpaceId { get; set; }
    public RealParkSpace RealSpace { get; set; }
    public SpaceStatus Status { get; set; }
    public DateTime DateTime { get; set; }
}