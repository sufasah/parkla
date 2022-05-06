using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Parkla.Core.Entities;

public class CityMap : IEntityTypeConfiguration<City> {
    public void Configure(EntityTypeBuilder<City> b)
    {
        b.ToTable(@"cities",@"public");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
            .UseIdentityAlwaysColumn();
        b.Property(x => x.Name)
            .HasMaxLength(20)
            .IsRequired();
    }
}