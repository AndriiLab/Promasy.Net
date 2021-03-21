using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Common.Persistence;
using Promasy.Domain.Bids;

namespace Promasy.Persistence.Configurations
{
    public class ReasonForSupplierConfiguration : BaseConfiguration<ReasonForSupplier>
    {
        protected override void Config(EntityTypeBuilder<ReasonForSupplier> builder)
        {
            builder.Property(b => b.Description)
                .HasMaxLength(PersistenceConstant.FieldLarge)
                .IsRequired();
        }
    }
}