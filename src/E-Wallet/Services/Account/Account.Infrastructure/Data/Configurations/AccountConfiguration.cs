using Account.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.Data.Configurations;

public class AccountConfiguration: IEntityTypeConfiguration<Domain.Models.Account>
{
    public void Configure(EntityTypeBuilder<Domain.Models.Account> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasConversion(
            aId => aId.Value,
            dbId => AccountId.Of(dbId));
        builder.Property(a => a.UserId).HasConversion(
            uId => uId.Value,
            dbId => UserId.Of(dbId)).IsRequired();
        builder.Property(a => a.AccountBalance).IsRequired();
        builder.Property(a => a.Version)
            .IsRowVersion()
            .HasColumnName("xmin"); 
    }
}