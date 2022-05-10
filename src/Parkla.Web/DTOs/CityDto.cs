using Parkla.Core.Entities;

namespace Parkla.Web.Models;
public class CityDto
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public virtual ICollection<District>? Districts { get; set; } 
    public virtual ICollection<User>? Users { get; set; } 
}