using Parkla.Core.Enums;

namespace Parkla.Core.Entities;

public class ParkSpace : IEntity {
    public int? Id { get; set; }
    public int? AreaId { get; set; }
    public virtual ParkArea? Area { get; set; }
    public int? RealSpaceId { get; set; }
    public virtual RealParkSpace? RealSpace { get; set; }
    public string? Name { get; set; }
    public DateTime? StatusUpdateTime { get; set; }
    public virtual SpaceStatus? Status { get; set; }
    public int[][]? SpacePath { get; set; }
    public virtual ICollection<Reservation>? Reservations { get; set; }
    public virtual ICollection<ReceivedSpaceStatus>? ReceivedSpaceStatuses { get; set; }
}