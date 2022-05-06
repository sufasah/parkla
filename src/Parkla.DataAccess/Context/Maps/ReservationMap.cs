using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Parkla.Core.Entities;

public class ReservationMap : IEntityTypeConfiguration<Reservation> {
    public void Configure(EntityTypeBuilder<Reservation> b)
    {
        b.ToTable(@"reservaions",@"public");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
            .UseIdentityAlwaysColumn();
        b.Property(x => x.StartTime)
            .IsRequired();
        b.Property(x => x.EndTime)
            .IsRequired();
            
        b.HasOne(x => x.User).WithMany(x => x.Reservations).HasForeignKey(x => x.UserId);
        b.HasOne(x => x.Space).WithMany(x => x.Reservations).HasForeignKey(x => x.SpaceId);
        b.HasOne(x => x.Pricing).WithMany(x => x.Reservations).HasForeignKey(x => x.PricingId);

        b.HasCheckConstraint("CK_STARTTIME_LESS_THAN_ENDTIME","[StartTime] < [EndTime]");
    }
}