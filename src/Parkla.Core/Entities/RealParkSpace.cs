using Parkla.Core.Enums;

namespace Parkla.Core.Entities;

public class RealParkSpace : IEntity {
    public RealParkSpace()
    {
        ReceivedSpaceStatusses = new HashSet<ReceivedSpaceStatus>();
    }

    public int? Id { get; set; }
    public Guid? ParkId { get; set; }
    public virtual Park? Park { get; set; }
    public int? SpaceId { get; set; }
    public virtual ParkSpace? Space { get; set; }
    public string? Name { get; set; }
    public virtual uint xmin { get; set; }
    public DateTime? StatusUpdateTime { get; set; }
    public virtual SpaceStatus? Status { get; set; }
    public virtual ICollection<ReceivedSpaceStatus> ReceivedSpaceStatusses { get; set; }
}