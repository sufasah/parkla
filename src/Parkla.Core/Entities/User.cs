using Parkla.Core.Enums;

namespace Parkla.Core.Entities;

public class User : IEntity {
    public User()
    {
        Parks = new HashSet<Park>();
        Reservations = new HashSet<Reservation>();
    }

    public int? Id { get; set; }
    public float? Wallet { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime? Birthdate { get; set; }
    public Gender? Gender { get; set; }
    public string? VerificationCode { get; set; }
    public string? RefreshTokenSignature { get; set; }
    public uint xmin { get; set; }
    public int? CityId { get; set; }
    public virtual City? City { get; set; }
    public int? DistrictId { get; set; }
    public virtual District? District { get; set; }
    public virtual ICollection<Reservation> Reservations { get; set; }
    public virtual ICollection<Park> Parks { get; set; }
}