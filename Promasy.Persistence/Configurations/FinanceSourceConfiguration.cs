using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Common.Persistence;
using Promasy.Domain.Finances;

namespace Promasy.Persistence.Configurations
{
    public class FinanceSourceConfiguration : BaseConfiguration<FinanceSource>
    {
        protected override void Config(EntityTypeBuilder<FinanceSource> builder)
        {
            builder.Property(b => b.Name)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired();
            builder.Property(b => b.Number)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired();
            builder.Property(b => b.Kpkvk)
                .HasMaxLength(10)
                .IsRequired();
        }
    }
}