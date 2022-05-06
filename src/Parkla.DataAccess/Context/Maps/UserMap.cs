using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Parkla.Core.Entities;

public class UserMap : IEntityTypeConfiguration<User> {
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable(@"users",@"public");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
            .UseIdentityAlwaysColumn();
        b.Property(x => x.Wallet)
            .HasPrecision(30,2)
            .IsRequired();
        b.Property(x => x.Username)
            .HasMaxLength(30)
            .IsRequired();
        b.Property(x => x.Password)
            .HasMaxLength(50)
            .IsRequired();
        b.Property(x => x.Email)
            .HasMaxLength(320)
            .IsRequired();
        b.Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();
        b.Property(x => x.Surname)
            .HasMaxLength(50)
            .IsRequired();
        b.Property(x => x.Phone)
            .HasMaxLength(20)
            .IsRequired();
        b.Property(x => x.Birthdate);
        b.Property(x => x.Gender);
        b.Property(x => x.Address)
            .HasMaxLength(200);
        
            
        b.HasOne(x => x.City).WithMany(x => x.Users).HasForeignKey(x => x.CityId);
        b.HasOne(x => x.District).WithMany(x => x.Users).HasForeignKey(x => x.DistrictId);
    }
}