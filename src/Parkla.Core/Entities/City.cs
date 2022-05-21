namespace Parkla.Core.Entities;

public class City : IEntity {
    public City()
    {
        Districts = new HashSet<District>();
        Users = new HashSet<User>();
    }

    public int? Id { get; set; }
    public string? Name { get; set; }
    public virtual ICollection<District> Districts { get; set; } 
    public virtual ICollection<User> Users { get; set; } 
}