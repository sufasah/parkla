using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parkla.Core.Entities;
namespace Parkla.DataAccess.Context.Maps;

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