using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Core.Persistence;
using Promasy.Domain.Employees;

namespace Promasy.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Token)
            .HasMaxLength(PersistenceConstant.FieldMedium)
            .IsRequired();
        
        builder.Property(b => b.CreatedByIp)
            .HasMaxLength(PersistenceConstant.FieldMini)
            .IsRequired();
        
        builder.Property(b => b.RevokedByIp)
            .HasMaxLength(PersistenceConstant.FieldMini)
            .IsRequired(false);
    }
}