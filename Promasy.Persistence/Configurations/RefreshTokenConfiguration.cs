using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Core.Persistence;
using Promasy.Domain.Employees;

namespace Promasy.Persistence.Configurations;

public class RefreshTokenConfiguration : BaseConfiguration<RefreshToken>
{
    protected override void Config(EntityTypeBuilder<RefreshToken> builder)
    {
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