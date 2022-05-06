namespace Parkla.Core.Entities;

public class City : IEntity {
    public int Id { get; set; }
    public string Name { get; set; }
    public virtual ICollection<District> Districts { get; set; } 
    public virtual ICollection<User> Users { get; set; } 
}