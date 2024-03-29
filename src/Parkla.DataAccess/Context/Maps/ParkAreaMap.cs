using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parkla.Core.Entities;
namespace Parkla.DataAccess.Context.Maps;

public class ParkAreaMap : IEntityTypeConfiguration<ParkArea> {
    public void Configure(EntityTypeBuilder<ParkArea> b)
    {
        b.ToTable(@"park_areas",@"public");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired()
            .UseIdentityAlwaysColumn();
        b.Property(x => x.ParkId)
            .HasColumnName("park_id")
            .IsRequired();
        b.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(50)
            .IsRequired();
        b.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(200);
        b.Property(x => x.TemplateImage)
            .HasColumnName("template_image")
            .HasMaxLength(500);
        b.Property(x => x.ReservationsEnabled)
            .HasColumnName("reservations_enabled")
            .IsRequired();
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
            .HasPrecision(30,2)
            .HasDefaultValue(null);
        b.Property(x => x.AveragePrice)
            .HasColumnName("average_price")
            .HasPrecision(30,2)
            .HasDefaultValue(null);
        b.Property(x => x.MaxPrice)
            .HasColumnName("max_price")
            .HasPrecision(30,2)
            .HasDefaultValue(null);

        b.UseXminAsConcurrencyToken();

        b.HasOne(x => x.Park).WithMany(x => x.Areas).HasForeignKey(x => x.ParkId).OnDelete(DeleteBehavior.Cascade);

        b.HasCheckConstraint("CK_UPDATE_TIME_LESS_THAN_OR_EQUAL_NOW","status_update_time <= now()");
        b.HasCheckConstraint("CK_PRICES_VALID","min_price <= average_price and average_price <= max_price");
    }
}