using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parkla.Core.Enums;

namespace Parkla.Core.Entities;

public class ParkSpaceMap : IEntityTypeConfiguration<ParkSpace> {
    public void Configure(EntityTypeBuilder<ParkSpace> b)
    {
        b.ToTable(@"park_spaces",@"public");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
            .UseIdentityAlwaysColumn();
        b.Property(x => x.Name)
            .HasMaxLength(30)
            .IsRequired();
        b.Property(x => x.StatusUpdateTime)
            .HasDefaultValue(new DateTime(0L, DateTimeKind.Utc))
            .IsRequired();
        b.Property(x => x.Status)
            .HasDefaultValue(SpaceStatus.OCCUPIED)
            .IsRequired();
        b.Property(x => x.SpacePath)
            .IsRequired();

        b.HasOne(x => x.Area).WithMany(x => x.Spaces).HasForeignKey(x => x.AreaId);
        b.HasOne(x => x.RealSpace).WithOne(x => x.Space);

        b.HasCheckConstraint("CK_UPDATE_TIME_LESS_THAN_NOW_UTC","[StatusUpdateTime] < (now() at time zone 'utc')");
    }
}