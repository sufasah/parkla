using Parkla.Core.Enums;

namespace Parkla.Core.Entities;

public class ReceivedSpaceStatus : IEntity {
   public int? Id { get; set; }
    public int? SpaceId { get; set; }
    public virtual ParkSpace? Space { get; set; }
    public int? RealSpaceId { get; set; }
    public virtual RealParkSpace? RealSpace { get; set; }
    public SpaceStatus? Status { get; set; }
    public DateTime? DateTime { get; set; }
}