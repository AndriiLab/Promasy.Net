using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Core.Persistence;
using Promasy.Domain.Organizations;

namespace Promasy.Persistence.Configurations
{
    public class AddressConfiguration : BaseConfiguration<Address>
    {
        protected override void Config(EntityTypeBuilder<Address> builder)
        {
            builder.Property(b => b.Country)
                .HasMaxLength(PersistenceConstant.FieldMini)
                .IsRequired();

            builder.Property(b => b.PostalCode)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(b => b.Region)
                .HasMaxLength(PersistenceConstant.FieldMini)
                .IsRequired();

            builder.Property(b => b.City)
                .HasMaxLength(PersistenceConstant.FieldMini)
                .IsRequired();

            builder.Property(b => b.Street)
                .HasMaxLength(PersistenceConstant.FieldMini)
                .IsRequired();

            builder.Property(b => b.BuildingNumber)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(b => b.InternalNumber)
                .HasMaxLength(10)
                .IsRequired(false);
        }
    }
}