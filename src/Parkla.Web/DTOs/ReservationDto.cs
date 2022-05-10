using Parkla.Core.Entities;

namespace Parkla.Web.Models;
public class ReservationDto
{
    public int? Id { get; set; }
    public int? UserId { get; set; }
    public int? SpaceId { get; set; }
    public float? PricingId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime{ get; set; }
}