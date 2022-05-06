namespace Parkla.Core.Entities;

public class District : IEntity {
    public int Id { get; set; }
    public int CityId { get; set; }
    public virtual City City { get; set; }
    public string Name { get; set; }
    public virtual ICollection<User> Users { get; set; } 
}