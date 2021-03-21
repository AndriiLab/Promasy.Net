using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Common.Persistence;
using Promasy.Domain.Vocabulary;

namespace Promasy.Persistence.Configurations
{
    public class CpvConfiguration : IEntityTypeConfiguration<Cpv>
    {
        public void Configure(EntityTypeBuilder<Cpv> builder)
        {
            builder.Property(b => b.CpvCode)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired();
            
            builder.Property(b => b.CpvEng)
                .HasMaxLength(PersistenceConstant.FieldLarge)
                .IsRequired();
                
            builder.Property(b => b.CpvUkr)
                .HasMaxLength(PersistenceConstant.FieldLarge)
                .IsRequired();

            builder.HasKey(b => b.CpvCode);
        }
    }
}