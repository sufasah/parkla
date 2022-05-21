using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parkla.Core.Enums;
using Parkla.Core.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Parkla.DataAccess.Context.Maps;

public class ParkSpaceMap : IEntityTypeConfiguration<ParkSpace> {
    public void Configure(EntityTypeBuilder<ParkSpace> b)
    {
        b.ToTable(@"park_spaces",@"public");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired()
            .UseIdentityAlwaysColumn();
        b.Property(x => x.AreaId)
            .HasColumnName("area_id")
            .IsRequired();
        b.Property(x => x.RealSpaceId)
            .HasColumnName("real_space_id")
            .IsRequired(false);
        b.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(30)
            .IsRequired();
        b.Property(x => x.StatusUpdateTime)
            .HasColumnName("status_update_time");
        b.Property(x => x.Status)
            .HasColumnName("status")
            .HasDefaultValue(SpaceStatus.UNKNOWN)
            .IsRequired();
        b.Property(x => x.TemplatePath)
            .HasColumnName("space_path")
            .HasConversion(
                x => $"{x![0][0]}:{x[0][1]},{x[1][0]}:{x[1][1]},{x[2][0]}:{x[2][1]},{x[3][0]}:{x[3][1]}",
                x => x.Split(',', StringSplitOptions.None)
                    .Select(y => y.Split(':', StringSplitOptions.None)
                        .Select(z => int.Parse(z)).ToArray()).ToArray(),
                new ValueComparer<int[][]>(
                    (x,y) => (x == null && y == null) || (x != null && y != null && x.SequenceEqual(y)),
                    (x) => x.GetHashCode()))
            .IsRequired();

        b.UseXminAsConcurrencyToken();

        b.HasOne(x => x.Area).WithMany(x => x.Spaces).HasForeignKey(x => x.AreaId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.RealSpace).WithOne(x => x.Space).HasForeignKey<RealParkSpace>(x => x.SpaceId).OnDelete(DeleteBehavior.SetNull);

        b.HasCheckConstraint("CK_UPDATE_TIME_LESS_THAN_OR_EQUAL_NOW","status_update_time <= now()");
    }
}