using Parkla.Core.Enums;

namespace Parkla.Core.Entities;

public class Pricing : IEntity {
    public int? Id { get; set; }
    public int? AreaId { get; set; }
    public virtual ParkArea? Area { get; set; }
    public string? Type { get; set; }
    public TimeUnit? Unit { get; set; }
    public int? Amount { get; set; }
    public float? Price { get; set; }
    public virtual ICollection<Reservation>? Reservations { get; set; }
}