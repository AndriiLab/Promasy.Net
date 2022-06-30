using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Core.Persistence;
using Promasy.Domain.Suppliers;

namespace Promasy.Persistence.Configurations
{
    public class SupplierConfiguration : BaseConfiguration<Supplier>
    {
        protected override void Config(EntityTypeBuilder<Supplier> builder)
        {
            builder.Property(b => b.Name)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired();
            
            builder.Property(b => b.Comment)
                .HasMaxLength(PersistenceConstant.FieldLarge)
                .IsRequired(false);
            
            builder.Property(b => b.Phone)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired(false);
        }
    }
}