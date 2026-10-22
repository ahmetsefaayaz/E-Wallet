using Account.Domain.Models;
using Account.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.Data.Configurations;

public class TransactionConfiguration: IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.Id);
        
        builder.Property(a => a.Id).HasConversion(
            aId => aId.Value,
            dbId => TransactionId.Of(dbId));
        
        builder.Property(t => t.CardId).HasConversion(
            cId => cId.Value,
            dbId => CardId.Of(dbId)).IsRequired();
        
        builder.Property(t => t.AccountId).HasConversion(
            aId => aId.Value,
            dbId => AccountId.Of(dbId)).IsRequired();
        
        builder.Property(t => t.TransactionType).IsRequired();
        
        builder.Property(t => t.Status).IsRequired();

        builder.Property(t => t.Amount).IsRequired();


    }
}