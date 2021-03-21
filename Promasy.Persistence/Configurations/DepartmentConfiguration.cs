using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Common.Persistence;
using Promasy.Domain.Institutes;

namespace Promasy.Persistence.Configurations
{
    public class DepartmentConfiguration : BaseConfiguration<Department>
    {
        protected override void Config(EntityTypeBuilder<Department> builder)
        {
            builder.Property(b => b.Name)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired();
        }
    }
}