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
            .UseIdentityAlwaysColumn();
        b.Property(x => x.Status)
            .HasDefaultValue(SpaceStatus.OCCUPIED)
            .IsRequired();
        b.Property(x => x.DateTime)
            .HasDefaultValue(new DateTime(0L, DateTimeKind.Utc))
            .IsRequired();
            
        b.HasOne(x => x.RealSpace).WithMany(x => x.ReceivedSpaceStatuses).HasForeignKey(x => x.RealSpaceId);
        b.HasOne(x => x.Space).WithMany(x => x.ReceivedSpaceStatuses).HasForeignKey(x => x.SpaceId);

        b.HasCheckConstraint("CK_DATETIME_LESS_THAN_NOW_UTC","[DateTime] < (now() at time zone 'utc')");
    }
}