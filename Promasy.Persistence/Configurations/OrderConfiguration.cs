using Microsoft.EntityFrameworkCore;
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

            builder.Property(b => b.Kekv)
                .HasMaxLength(PersistenceConstant.FieldMini)
                .IsRequired(false);

            builder.Property(b => b.Total)
                .HasComputedColumnSql(@"CASE WHEN ""Deleted"" = FALSE AND ""OnePrice"" * ""Amount"" > 0 THEN ""OnePrice"" * ""Amount"" ELSE 0 END", stored: true);

            builder.HasOne(b => b.Cpv)
                .WithMany()
                .HasForeignKey(b => b.CpvId);

            builder.HasIndex(c => c.Description)
                .HasMethod("GIN")
                .IsTsVectorExpressionIndex("simple");
        }
    }
}