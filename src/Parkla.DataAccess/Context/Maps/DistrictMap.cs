using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Parkla.Core.Entities;

public class DistrictMap : IEntityTypeConfiguration<District> {
    public void Configure(EntityTypeBuilder<District> b)
    {
        b.ToTable(@"districts",@"public");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
            .UseIdentityAlwaysColumn();
        b.Property(x => x.Name)
            .HasMaxLength(30)
            .IsRequired();

        b.HasOne(x => x.City)
            .WithMany(x => x.Districts)
            .HasForeignKey(x => x.CityId)
            .IsRequired();
    }
}