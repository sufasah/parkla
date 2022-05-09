using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parkla.Core.Entities;

namespace Parkla.DataAccess.Context.Maps;

public class UserMap : IEntityTypeConfiguration<User> {
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable(@"users",@"public");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
            .HasColumnName("id")
            .UseIdentityAlwaysColumn();
        b.Property(x => x.Wallet)
            .HasColumnName("wallet")
            .HasPrecision(30,2)
            .IsRequired();
        b.Property(x => x.Username)
            .HasColumnName("username")
            .HasMaxLength(30)
            .IsRequired();
        b.Property(x => x.Password)
            .HasColumnName("password")
            .HasMaxLength(80)
            .IsRequired();
        b.Property(x => x.Email)
            .HasColumnName("email")
            .HasMaxLength(320)
            .IsRequired();
        b.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(50)
            .IsRequired();
        b.Property(x => x.Surname)
            .HasColumnName("surname")
            .HasMaxLength(50)
            .IsRequired();
        b.Property(x => x.Phone)
            .HasColumnName("phone")
            .HasMaxLength(20)
            .IsRequired();
        b.Property(x => x.Birthdate)
            .HasColumnName("birthdate");
        b.Property(x => x.Gender)
            .HasColumnName("gender");
        b.Property(x => x.VerificationCode)
            .HasColumnName("verify_code");
        b.Property(x => x.RefreshTokenSignature)
            .HasColumnName("refresh_token_signature")
            .HasMaxLength(400);
        b.Property(x => x.CityId)
            .HasColumnName("city_id");
        b.Property(x => x.DistrictId)
            .HasColumnName("district_id");
        b.Property(x => x.Address)
            .HasColumnName("address")
            .HasMaxLength(200);
        
            
        b.HasOne(x => x.City).WithMany(x => x.Users).HasForeignKey(x => x.CityId);
        b.HasOne(x => x.District).WithMany(x => x.Users).HasForeignKey(x => x.DistrictId);

        b.HasIndex(x => x.Username).IsUnique();
    }
}