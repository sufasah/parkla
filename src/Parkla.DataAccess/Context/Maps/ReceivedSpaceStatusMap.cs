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
        b.Property(x => x.RealSpaceId)
            .HasColumnName("real_space_id")
            .IsRequired(false);
        b.Property(x => x.Status)
            .HasColumnName("status")
            .HasDefaultValue(SpaceStatus.UNKNOWN)
            .IsRequired();
        b.Property(x => x.DateTime)
            .HasColumnName("datetime");
            
        b.HasOne(x => x.RealSpace).WithMany(x => x.ReceivedSpaceStatuses).HasForeignKey(x => x.RealSpaceId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.Space).WithMany(x => x.ReceivedSpaceStatuses).HasForeignKey(x => x.SpaceId).OnDelete(DeleteBehavior.SetNull);

        b.HasCheckConstraint("CK_DATETIME_LESS_THAN_NOW_UTC","datetime < (now() at time zone 'utc')");
    }
}