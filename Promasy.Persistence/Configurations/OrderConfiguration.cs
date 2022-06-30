using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Core.Persistence;
using Promasy.Domain.Orders;

namespace Promasy.Persistence.Configurations
{
    public class OrderConfiguration : BaseConfiguration<Order>
    {
        protected override void Config(EntityTypeBuilder<Order> builder)
        {
            builder.Property(b => b.Description)
                .HasMaxLength(PersistenceConstant.FieldLarge)
                .IsRequired();

            builder.Property(b => b.CatNum)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired(false);

            builder.HasOne(b => b.Cpv)
                .WithMany()
                .HasForeignKey(b => b.CpvId);
        }
    }
}