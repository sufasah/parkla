using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parkla.Core.Entities;

namespace Parkla.DataAccess.Context.Maps;

public class PricingMap : IEntityTypeConfiguration<Pricing> {
    public void Configure(EntityTypeBuilder<Pricing> b)
    {
        b.ToTable(@"pricings",@"public");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired()
            .UseIdentityAlwaysColumn();
        b.Property(x => x.AreaId)
            .HasColumnName("area_id")
            .IsRequired();
        b.Property(x => x.Type)
            .HasMaxLength(30)
            .IsRequired();
        b.Property(x => x.Unit)
            .HasColumnName("unit")
            .IsRequired();
        b.Property(x => x.Amount)
            .HasColumnName("amount")
            .IsRequired();
        b.Property(x => x.Price)
            .HasColumnName("price")
            .HasPrecision(30,2)
            .IsRequired();
            
        b.HasOne(x => x.Area).WithMany(x => x.Pricings).HasForeignKey(x => x.AreaId).OnDelete(DeleteBehavior.Cascade);
    }
}