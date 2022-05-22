using Parkla.Core.Entities;
using Parkla.Core.Enums;

namespace Parkla.Web.Models;
public class ReceivedSpaceStatusDto
{
    public int? Id { get; set; }
    public int? SpaceId { get; set; }
    public virtual ParkSpace? Space { get; set; }
    public string? SpaceName { get; set; }
    public int? RealSpaceId { get; set; }
    public virtual RealParkSpace? RealSpace { get; set; }
    public string? RealSpaceName { get; set; }
    public SpaceStatus? OldSpaceStatus { get; set; }
    public SpaceStatus? NewSpaceStatus { get; set; }
    public SpaceStatus? OldRealSpaceStatus { get; set; }
    public SpaceStatus? NewRealSpaceStatus { get; set; }
    public DateTime? ReceivedTime { get; set; }
    public DateTime? StatusDataTime { get; set; }
}