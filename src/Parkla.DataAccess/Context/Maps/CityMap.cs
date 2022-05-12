using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parkla.Core.Entities;

namespace Parkla.DataAccess.Context.Maps;

public class CityMap : IEntityTypeConfiguration<City> {
    public void Configure(EntityTypeBuilder<City> b)
    {
        b.ToTable(@"cities",@"public");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
            .UseIdentityAlwaysColumn()
            .IsRequired()
            .HasColumnName("id");
        b.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(20)
            .IsRequired();
    }
}