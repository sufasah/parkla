using Parkla.Core.Entities;

namespace Parkla.Web.Models;
public class DistrictDto
{
    public int? Id { get; set; }
    public int? CityId { get; set; }
    public virtual City? City { get; set; }
    public string? Name { get; set; }
    public virtual ICollection<User>? Users { get; set; } 
}