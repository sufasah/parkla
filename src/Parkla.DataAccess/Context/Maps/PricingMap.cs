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
            .UseIdentityAlwaysColumn();
        b.Property(x => x.Unit)
            .IsRequired();
        b.Property(x => x.Amount)
            .IsRequired();
        b.Property(x => x.Price)
            .HasPrecision(30,2)
            .IsRequired();
            
        b.HasOne(x => x.Area).WithMany(x => x.Pricings).HasForeignKey(x => x.AreaId);
    }
}