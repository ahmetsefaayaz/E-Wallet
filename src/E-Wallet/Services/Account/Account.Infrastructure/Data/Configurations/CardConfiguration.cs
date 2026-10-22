using Account.Domain.Models;
using Account.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.Data.Configurations;

public class CardConfiguration: IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(a => a.Id).HasConversion(
            aId => aId.Value,
            dbId => CardId.Of(dbId));
        
        builder.Property(c => c.AccountId).HasConversion(
            id => id.Value,
            dbId => AccountId.Of(dbId)).IsRequired();
        
        builder.Property(c => c.CardHolderName).HasConversion(
            name => name.Value,
            dbName => CardHolderName.Of(dbName)).IsRequired();
        
        builder.Property(c => c.CardExpiryDate).HasConversion(
            name => name.Value,
            dbName => CardExpiryDate.Of(dbName)).IsRequired();
        
        builder.Property(c => c.CardNumber).HasConversion(
            name => name.Value,
            dbName => CardNumber.Of(dbName)).IsRequired();
        
        builder.Property(c => c.CardSecurityNumber).HasConversion(
            name => name.Value,
            dbName => CardSecurityNumber.Of(dbName)).IsRequired();

    }
}