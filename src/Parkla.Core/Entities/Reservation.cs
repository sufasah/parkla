namespace Parkla.Core.Entities;

public class Reservation : IEntity {
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int SpaceId { get; set; }
    public ParkSpace Space { get; set; }
    public float PricingId { get; set; }
    public Pricing Pricing { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime{ get; set; }
}