using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Core.Persistence;
using Promasy.Domain.Organizations;

namespace Promasy.Persistence.Configurations
{
    public class OrganizationConfiguration : BaseConfiguration<Organization>
    {
        protected override void Config(EntityTypeBuilder<Organization> builder)
        {
            builder.Property(b => b.Name)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired();
            
            builder.Property(b => b.Email)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired(false);
            
            builder.Property(b => b.Edrpou)
                .HasMaxLength(20)
                .IsRequired(false);
            
            builder.Property(b => b.FaxNumber)
                .HasMaxLength(30)
                .IsRequired(false);
            
            builder.Property(b => b.PhoneNumber)
                .HasMaxLength(30)
                .IsRequired(false);
        }
    }
}