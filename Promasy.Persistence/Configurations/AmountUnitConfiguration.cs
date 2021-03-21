using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Common.Persistence;
using Promasy.Domain.Bids;

namespace Promasy.Persistence.Configurations
{
    public class AmountUnitConfiguration : BaseConfiguration<AmountUnit>
    {
        protected override void Config(EntityTypeBuilder<AmountUnit> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Description)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired();
        }
    }
}