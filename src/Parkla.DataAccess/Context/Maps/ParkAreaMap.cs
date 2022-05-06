using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Parkla.Core.Entities;

public class ParkAreaMap : IEntityTypeConfiguration<ParkArea> {
    public void Configure(EntityTypeBuilder<ParkArea> b)
    {
        b.ToTable(@"park_areas",@"public");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
            .UseIdentityAlwaysColumn();
        b.Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();
        b.Property(x => x.Description)
            .HasMaxLength(200);
        b.Property(x => x.TemplateImage)
            .HasMaxLength(500);
        b.Property(x => x.ReservationsEnabled)
            .IsRequired();
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

        b.HasOne(x => x.Park).WithMany(x => x.Areas).HasForeignKey(x => x.ParkId);

        b.HasCheckConstraint("CK_UPDATE_TIME_LESS_THAN_NOW_UTC","[StatusUpdateTime] < (now() at time zone 'utc')");
        b.HasCheckConstraint("CK_PRICES_VALID","[MinPrice] <= [AvaragePrice] and [AvaragePrice] <= [MaxPrice]");
    }
}