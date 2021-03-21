using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Common.Persistence;
using Promasy.Domain.Institutes;

namespace Promasy.Persistence.Configurations
{
    public class AddressConfiguration : BaseConfiguration<Address>
    {
        protected override void Config(EntityTypeBuilder<Address> builder)
        {
            builder.Property(b => b.BuildingNumber)
                .HasMaxLength(10)
                .IsRequired();
            
            builder.Property(b => b.City)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired();
            
            builder.Property(b => b.CityType)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired();
            
            builder.Property(b => b.CorpusNumber)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired();
            
            builder.Property(b => b.Country)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired();
            
            builder.Property(b => b.PostalCode)
                .HasMaxLength(10)
                .IsRequired();
            
            builder.Property(b => b.Region)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired();
            
            builder.Property(b => b.Street)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired();
            
            builder.Property(b => b.StreetType)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired();
        }
    }
}