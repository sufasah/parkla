using Parkla.Core.Entities;

namespace Parkla.Web.Models;
public class ParkDto
{
    public Guid? Id { get; set; }
    public int? UserId { get; set; }
    public string? Name { get; set; }
    public string? Location { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string[]? Extras { get; set; }
    public uint xmin { get; set; }
    public DateTime? StatusUpdateTime { get; set; }
    public int? EmptySpace { get; set; }
    public int? ReservedSpace { get; set; }
    public int? OccupiedSpace { get; set; }
    public float? MinPrice { get; set; }
    public float? AveragePrice { get; set; }
    public float? MaxPrice { get; set; }
}