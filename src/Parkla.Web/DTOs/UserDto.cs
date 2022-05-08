using Parkla.Core.Entities;
using Parkla.Core.Enums;

namespace Parkla.Web.Models;
public class UserDto
{
    public int Id { get; set; }
    public float Wallet { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Phone { get; set; }
    public DateTime Birthdate { get; set; }
    public Gender Gender { get; set; }
    public int CityId { get; set; }
    public virtual City City { get; set; }
    public int DistrictId { get; set; }
    public virtual District District { get; set; }
    public string Address { get; set; }
    public virtual ICollection<Reservation> Reservations { get; set; }
}