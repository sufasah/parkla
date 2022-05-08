using Parkla.Core.Entities;

namespace Parkla.Web.Models;
public class ParkAreaDto
{
    public int Id { get; set; }
    public int ParkId { get; set; }
    public virtual Park Park { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string TemplateImage { get; set; }
    public bool ReservationsEnabled { get; set; }
    public DateTime StatusUpdateTime { get; set; }
    public int EmptySpace { get; set; }
    public int ReservedSpace { get; set; }
    public int OccupiedSpace { get; set; }
    public float MinPrice { get; set; }
    public float AvaragePrice { get; set; }
    public float MaxPrice { get; set; }
    public virtual ICollection<ParkSpace> Spaces { get; set; }
    public virtual ICollection<Pricing> Pricings { get; set; }
}