using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Core.Persistence;
using Promasy.Domain.Manufacturers;

namespace Promasy.Persistence.Configurations
{
    public class ManufacturerConfiguration : BaseConfiguration<Manufacturer>
    {
        protected override void Config(EntityTypeBuilder<Manufacturer> builder)
        {
            builder.Property(b => b.Name)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired();
        }
    }
}