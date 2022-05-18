using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parkla.Core.Enums;
using Parkla.Core.Entities;

namespace Parkla.DataAccess.Context.Maps;

public class RealParkSpaceMap : IEntityTypeConfiguration<RealParkSpace> {
    public void Configure(EntityTypeBuilder<RealParkSpace> b)
    {
        b.ToTable(@"real_park_spaces",@"public");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
            .IsRequired()
            .HasColumnName("id")
            .UseIdentityAlwaysColumn();
        b.Property(x => x.ParkId)
            .HasColumnName("park_id")
            .IsRequired();
        b.Property(x => x.SpaceId)
            .HasColumnName("space_id")
            .IsRequired(false);
        b.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(30)
            .IsRequired();
        b.Property(x => x.RowVersion)
            .IsRowVersion();
        b.Property(x => x.StatusUpdateTime)
            .HasColumnName("status_update_time");
        b.Property(x => x.Status)
            .HasColumnName("status")
            .HasDefaultValue(SpaceStatus.UNKNOWN)
            .IsRequired();
        
            
        b.HasOne(x => x.Park).WithMany(x => x.RealSpaces).HasForeignKey(x => x.ParkId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Space).WithOne(x => x.RealSpace).HasForeignKey<RealParkSpace>(x => x.SpaceId).OnDelete(DeleteBehavior.SetNull);

        b.HasCheckConstraint("CK_UPDATE_TIME_LESS_THAN_NOW_UTC","status_update_time < (now() at time zone 'utc')");
    }
}