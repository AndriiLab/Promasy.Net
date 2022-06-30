using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Core.Persistence;
using Promasy.Domain.Orders;

namespace Promasy.Persistence.Configurations
{
    public class ReasonForSupplierChoiceConfiguration : BaseConfiguration<ReasonForSupplierChoice>
    {
        protected override void Config(EntityTypeBuilder<ReasonForSupplierChoice> builder)
        {
            builder.Property(b => b.Name)
                .HasMaxLength(PersistenceConstant.FieldLarge)
                .IsRequired();
        }
    }
}