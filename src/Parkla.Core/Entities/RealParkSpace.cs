using Parkla.Core.Enums;

namespace Parkla.Core.Entities;

public class RealParkSpace : IEntity {
    public int Id { get; set; }
    public int ParkId { get; set; }
    public Park Park { get; set; }
    public int SpaceId { get; set; }
    public ParkSpace Space { get; set; }
    public string Name { get; set; }
    public DateTime StatusUpdateTime { get; set; }
    public SpaceStatus Status { get; set; }
    public virtual ICollection<ReceivedSpaceStatus> ReceivedSpaceStatuses { get; set; }
}