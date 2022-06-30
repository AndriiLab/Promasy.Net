using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Core.Persistence;
using Promasy.Domain.Orders;

namespace Promasy.Persistence.Configurations
{
    public class UnitConfiguration : BaseConfiguration<Unit>
    {
        protected override void Config(EntityTypeBuilder<Unit> builder)
        {
            builder.Property(p => p.Name)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired();
        }
    }
}