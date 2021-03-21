using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Common.Persistence;
using Promasy.Domain.Producers;

namespace Promasy.Persistence.Configurations
{
    public class ProducerConfiguration : BaseConfiguration<Producer>
    {
        protected override void Config(EntityTypeBuilder<Producer> builder)
        {
            builder.Property(b => b.Name)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired();
        }
    }
}