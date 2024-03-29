namespace Parkla.Core.Entities;

public class Park : IEntity {
    public Park()
    {
        Areas = new HashSet<ParkArea>();
        RealSpaces = new HashSet<RealParkSpace>();
    }

     public Guid? Id { get; set; }
    public int? UserId { get; set; }
    public virtual User? User { get; set; }
    public string? Name { get; set; }
    public string? Location { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string[]? Extras { get; set; }
    public virtual uint xmin { get; set; }
    public DateTime? StatusUpdateTime { get; set; }
    public int? EmptySpace { get; set; }
    public int? OccupiedSpace { get; set; }
    public float? MinPrice { get; set; }
    public float? AveragePrice { get; set; }
    public float? MaxPrice { get; set; }
    public virtual ICollection<ParkArea> Areas { get; set; }
    public virtual ICollection<RealParkSpace> RealSpaces { get; set; }
}