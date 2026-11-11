using Account.Domain.Models;
using Account.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.Data.Configurations;

public class IdempotencyRecordConfiguration: IEntityTypeConfiguration<IdempotencyRecord>
{
    public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(
            id => id.Value,
            dbId => IdempotencyId.Of(dbId)).IsRequired();
        builder.Property(x => x.UserId).HasConversion(
            userId => userId.Value,
            dbUserId => UserId.Of(dbUserId)).IsRequired();
        builder.Property(x => x.Operation).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Key).IsRequired().HasMaxLength(200);
        builder.HasIndex(x => new {x.UserId, x.Operation, x.Key}).IsUnique();
    }
}