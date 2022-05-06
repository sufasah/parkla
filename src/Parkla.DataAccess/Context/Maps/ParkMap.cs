using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Parkla.Core.Entities;

public class ParkMap : IEntityTypeConfiguration<Park> {
    public void Configure(EntityTypeBuilder<Park> b)
    {
        b.ToTable(@"parks",@"public");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
            .UseIdentityAlwaysColumn();
        b.Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();
        b.Property(x => x.Location)
            .HasMaxLength(200)
            .IsRequired();
        b.Property(x => x.Latitude)
            .HasPrecision(2,15)
            .IsRequired();
        b.Property(x => x.Longitude)
            .HasPrecision(2,15)
            .IsRequired();
        b.Property(x => x.Extras)
            .HasConversion(
                x => string.Join(',', x), 
                x => x.Split(',', StringSplitOptions.None))
            .IsRequired()
            .HasMaxLength(10);
        b.Property(x => x.StatusUpdateTime)
            .HasDefaultValue(new DateTime(0L, DateTimeKind.Utc))
            .IsRequired();
        b.Property(x => x.EmptySpace)
            .IsRequired();
        b.Property(x => x.ReservedSpace)
            .IsRequired();
        b.Property(x => x.OccupiedSpace)
            .IsRequired();
        b.Property(x => x.MinPrice)
            .HasPrecision(30,2)
            .IsRequired();
        b.Property(x => x.AvaragePrice)
            .HasPrecision(30,2)
            .IsRequired();
        b.Property(x => x.MaxPrice)
            .HasPrecision(30,2)
            .IsRequired();
        
        b.HasCheckConstraint("CK_UPDATE_TIME_LESS_THAN_NOW_UTC","[StatusUpdateTime] < (now() at time zone 'utc')");
        b.HasCheckConstraint("CK_LATITUDE_AND_LONGITUDE_ARE_VALID","[Latitude] >= -90 and [Latitude] <=90 and [Longitude] >= -180 and [Longitude] <= 180");
        b.HasCheckConstraint("CK_PRICES_VALID","[MinPrice] <= [AvaragePrice] and [AvaragePrice] <= [MaxPrice]");
    }
}