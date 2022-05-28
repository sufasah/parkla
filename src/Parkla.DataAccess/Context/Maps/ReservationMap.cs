using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parkla.Core.Entities;

namespace Parkla.DataAccess.Context.Maps;

public class ReservationMap : IEntityTypeConfiguration<Reservation> {
    public void Configure(EntityTypeBuilder<Reservation> b)
    {
        b.ToTable(@"reservations",@"public");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired()
            .UseIdentityAlwaysColumn();
        b.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();
        b.Property(x => x.SpaceId)
            .HasColumnName("space_id")
            .IsRequired();
        
        b.Property(x => x.StartTime)
            .HasColumnName("start_time")
            .IsRequired();
        b.Property(x => x.EndTime)
            .HasColumnName("end_time")
            .IsRequired();
            
        b.HasOne(x => x.User).WithMany(x => x.Reservations).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Space).WithMany(x => x.Reservations).HasForeignKey(x => x.SpaceId).OnDelete(DeleteBehavior.Cascade);

        b.HasCheckConstraint("CK_STARTTIME_LESS_THAN_ENDTIME","start_time < end_time");
    }
}