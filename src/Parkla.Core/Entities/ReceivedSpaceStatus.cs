using Parkla.Core.Enums;

namespace Parkla.Core.Entities;

public class ReceivedSpaceStatus : IEntity {
    public int Id { get; set; }
    public int? SpaceId { get; set; }
    public ParkSpace? Space { get; set; }
    public int? RealSpaceId { get; set; }
    public RealParkSpace? RealSpace { get; set; }
    public SpaceStatus Status { get; set; }
    public DateTime DateTime { get; set; }
}