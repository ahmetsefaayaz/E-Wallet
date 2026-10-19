using Identity.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Api.Data.Configurations;

public class UserConfiguration:IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserName).HasMaxLength(128).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(128).IsRequired();
        builder.Property(x => x.PasswordHash).IsRequired();
        builder.HasIndex(x => x.Email).IsUnique();
        builder.OwnsOne(u => u.Address, a =>
        {
            a.Property(p => p.AddressLine).HasColumnName("AddressLine").HasMaxLength(100);
            a.Property(p => p.City).HasColumnName("City").HasMaxLength(100);
            a.Property(p => p.State).HasColumnName("State").HasMaxLength(100);
            a.Property(p => p.ZipCode).HasColumnName("ZipCode").HasMaxLength(100);
            a.Property(p => p.Country).HasColumnName("Country").HasMaxLength(100);
        });
    }
}