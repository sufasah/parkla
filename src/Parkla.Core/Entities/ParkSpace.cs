using Parkla.Core.Enums;

namespace Parkla.Core.Entities;

public class ParkSpace : IEntity {
    public ParkSpace()
    {
        ReceivedSpaceStatusses = new HashSet<ReceivedSpaceStatus>();
        Reservations = new HashSet<Reservation>();
    }

    public int? Id { get; set; }
    public int? AreaId { get; set; }
    public virtual ParkArea? Area { get; set; }
    public int? RealSpaceId { get; set; }
    public virtual RealParkSpace? RealSpace { get; set; }
    public string? Name { get; set; }
    public virtual uint xmin { get; set; }
    public DateTime? StatusUpdateTime { get; set; }
    public virtual SpaceStatus? Status { get; set; }
    public int[][]? TemplatePath { get; set; }
    public virtual ICollection<Reservation>? Reservations { get; set; }
    public virtual ICollection<ReceivedSpaceStatus> ReceivedSpaceStatusses { get; set; }
}