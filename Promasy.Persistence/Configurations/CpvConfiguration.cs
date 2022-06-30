using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Core.Persistence;
using Promasy.Domain.Vocabulary;

namespace Promasy.Persistence.Configurations
{
    public class CpvConfiguration : IEntityTypeConfiguration<Cpv>
    {
        public void Configure(EntityTypeBuilder<Cpv> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Code)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired();
            
            builder.Property(b => b.DescriptionEnglish)
                .HasMaxLength(PersistenceConstant.FieldLarge)
                .IsRequired();
                
            builder.Property(b => b.DescriptionUkrainian)
                .HasMaxLength(PersistenceConstant.FieldLarge)
                .IsRequired();
        }
    }
}