using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Core.Persistence;
using Promasy.Domain.Orders;

namespace Promasy.Persistence.Configurations;

public class OrderGroupConfiguration : BaseConfiguration<OrderGroup>
{
    protected override void Config(EntityTypeBuilder<OrderGroup> builder)
    {
        builder.Property(b => b.FileKey)
            .HasMaxLength(PersistenceConstant.FieldMini)
            .IsRequired();
        
        builder
            .HasMany(p => p.Orders)
            .WithMany( o=> o.Groups)
            .UsingEntity(j => j.ToTable("OrderOrderGroups"));

        builder.HasIndex(b => b.FileKey)
            .IsUnique();
    }
}