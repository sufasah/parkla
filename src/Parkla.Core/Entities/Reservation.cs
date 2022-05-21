namespace Parkla.Core.Entities;

public class Reservation : IEntity {
   public int? Id { get; set; }
    public int? UserId { get; set; }
    public virtual User? User { get; set; }
    public int? SpaceId { get; set; }
    public virtual ParkSpace? Space { get; set; }
    public int? PricingId { get; set; }
    public virtual Pricing? Pricing { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime{ get; set; }
}