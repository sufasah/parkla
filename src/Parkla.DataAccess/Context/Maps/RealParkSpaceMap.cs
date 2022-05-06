using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parkla.Core.Enums;

namespace Parkla.Core.Entities;

public class RealParkSpaceMap : IEntityTypeConfiguration<RealParkSpace> {
    public void Configure(EntityTypeBuilder<RealParkSpace> b)
    {
        b.ToTable(@"real_park_spaces",@"public");
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
        
            
        b.HasOne(x => x.Park).WithMany(x => x.RealSpaces).HasForeignKey(x => x.ParkId);

        b.HasCheckConstraint("CK_UPDATE_TIME_LESS_THAN_NOW_UTC","[StatusUpdateTime] < (now() at time zone 'utc')");
    }
}