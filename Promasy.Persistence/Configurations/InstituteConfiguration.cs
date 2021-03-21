using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Common.Persistence;
using Promasy.Domain.Institutes;

namespace Promasy.Persistence.Configurations
{
    public class InstituteConfiguration : BaseConfiguration<Institute>
    {
        protected override void Config(EntityTypeBuilder<Institute> builder)
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