using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parkla.Core.Entities;

namespace Parkla.DataAccess.Context.Maps;

public class ParkMap : IEntityTypeConfiguration<Park> {
    public void Configure(EntityTypeBuilder<Park> b)
    {
        b.ToTable(@"parks",@"public");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired()
            .ValueGeneratedOnAdd();
        b.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();
        b.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(50)
            .IsRequired();
        b.Property(x => x.Location)
            .HasColumnName("location")
            .HasMaxLength(200)
            .IsRequired();
        b.Property(x => x.Latitude)
            .HasColumnName("latitude")
            .HasPrecision(2,15)
            .IsRequired();
        b.Property(x => x.Longitude)
            .HasColumnName("longitude")
            .HasPrecision(2,15)
            .IsRequired();
        b.Property(x => x.Extras)
            .HasColumnName("extras")
            .IsRequired()
            .HasMaxLength(10);
        b.Property(x => x.StatusUpdateTime)
            .HasColumnName("status_update_time");
        b.Property(x => x.EmptySpace)
            .HasColumnName("empty_space")
            .IsRequired();
        b.Property(x => x.OccupiedSpace)
            .HasColumnName("occupied_space")
            .IsRequired();
        b.Property(x => x.MinPrice)
            .HasColumnName("min_price")
            .HasDefaultValue(null)
            .HasPrecision(30,2);
        b.Property(x => x.AvaragePrice)
            .HasColumnName("avarage_price")
            .HasDefaultValue(null)
            .HasPrecision(30,2);
        b.Property(x => x.MaxPrice)
            .HasColumnName("max_price")
            .HasDefaultValue(null)
            .HasPrecision(30,2);

        b.UseXminAsConcurrencyToken();

        b.HasOne(x => x.User).WithMany(x => x.Parks).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        
        b.HasCheckConstraint("CK_UPDATE_TIME_LESS_THAN_OR_EQUAL_NOW","status_update_time <= now()");
        b.HasCheckConstraint("CK_LATITUDE_AND_LONGITUDE_ARE_VALID","latitude >= -90 and latitude <= 90 and longitude >= -180 and longitude <= 180");
        b.HasCheckConstraint("CK_PRICES_VALID","min_price <= avarage_price and avarage_price <= max_price");
    }
}