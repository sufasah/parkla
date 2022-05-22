using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parkla.Core.Enums;
using Parkla.Core.Entities;

namespace Parkla.DataAccess.Context.Maps;

public class ReceivedSpaceStatusMap : IEntityTypeConfiguration<ReceivedSpaceStatus> {
    public void Configure(EntityTypeBuilder<ReceivedSpaceStatus> b)
    {
        b.ToTable(@"received_space_statusses",@"public");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired()
            .UseIdentityAlwaysColumn();
        b.Property(x => x.SpaceId)
            .HasColumnName("space_id")
            .IsRequired(false);
        b.Property(x => x.SpaceName)
            .HasColumnName("space_name")
            .HasMaxLength(30);
        b.Property(x => x.RealSpaceId)
            .HasColumnName("real_space_id")
            .IsRequired(false);
        b.Property(x => x.RealSpaceName)
            .HasColumnName("real_space_name")
            .HasMaxLength(30)
            .IsRequired();
        b.Property(x => x.OldSpaceStatus)
            .HasColumnName("old_space_status");
        b.Property(x => x.NewSpaceStatus)
            .HasColumnName("new_space_status");
        b.Property(x => x.OldRealSpaceStatus)
            .HasColumnName("old_real_space_status")
            .IsRequired();
        b.Property(x => x.NewRealSpaceStatus)
            .HasColumnName("new_real_space_status")
            .IsRequired();
        b.Property(x => x.ReceivedTime)
            .HasColumnName("received_time");
        b.Property(x => x.StatusDataTime)
            .HasColumnName("status_data_time");
            
        b.HasOne(x => x.RealSpace).WithMany(x => x.ReceivedSpaceStatusses).HasForeignKey(x => x.RealSpaceId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.Space).WithMany(x => x.ReceivedSpaceStatusses).HasForeignKey(x => x.SpaceId).OnDelete(DeleteBehavior.SetNull);

        b.HasCheckConstraint("CK_RECEIVED_TIME_LESS_THAN_OR_EQUAL_NOW","received_time <= now()");
        b.HasCheckConstraint("CK_STATUS_DATA_TIME_LESS_THAN_OR_EQUAL_NOW","status_data_time <= now()");
    }
}