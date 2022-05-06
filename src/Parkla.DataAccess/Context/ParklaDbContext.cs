using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Parkla.Core.Entities;

namespace Parkla.DataAccess.Contexts;
public class ParklaDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public ParklaDbContext(
        DbContextOptions<ParklaDbContext> options, 
        IConfiguration configuration
    ) : base(options)
    {
        _configuration = configuration;
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CityMap());
        modelBuilder.ApplyConfiguration(new DistrictMap());
        modelBuilder.ApplyConfiguration(new ParkMap());
        modelBuilder.ApplyConfiguration(new ParkAreaMap());
        modelBuilder.ApplyConfiguration(new ParkSpaceMap());
        modelBuilder.ApplyConfiguration(new PricingMap());
        modelBuilder.ApplyConfiguration(new RealParkSpaceMap());
        modelBuilder.ApplyConfiguration(new ReservationMap());
        modelBuilder.ApplyConfiguration(new ReceivedSpaceStatusMap());
        modelBuilder.ApplyConfiguration(new UserMap());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("parkla-admin"), b => {
            b.EnableRetryOnFailure(30);
            b.SetPostgresVersion(13,6);
        });
    }

    public DbSet<City> Cities { get; set; } 
    public DbSet<District> Districts { get; set; } 
    public DbSet<Park> Parks { get; set; } 
    public DbSet<ParkArea> ParkAreas { get; set; } 
    public DbSet<ParkSpace> ParkSpaces { get; set; } 
    public DbSet<Pricing> Pricings { get; set; } 
    public DbSet<RealParkSpace> RealParkSpaces { get; set; } 
    public DbSet<Reservation> Reservations { get; set; } 
    public DbSet<ReceivedSpaceStatus> ReceivedSpaceStatuses { get; set; } 
    public DbSet<User> Users { get; set; } 
}