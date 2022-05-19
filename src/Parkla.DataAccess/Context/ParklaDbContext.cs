using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Parkla.Core.Entities;
using Parkla.DataAccess.Context.Maps;

namespace Parkla.DataAccess.Contexts;
#pragma warning disable CS8618
public class ParklaDbContext : DbContext
{
    public static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(b => b.AddConsole());

        
    private static readonly IConfiguration _configuration = new ConfigurationBuilder()
        .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "Parkla.Web"))
        .AddJsonFile("appsettings.json")
        .Build();

    public ParklaDbContext() {}
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
        
        optionsBuilder
            //.UseLoggerFactory(loggerFactory)
            .UseLazyLoadingProxies(false);
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