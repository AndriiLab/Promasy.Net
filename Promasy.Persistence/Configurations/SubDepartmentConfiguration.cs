using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Core.Persistence;
using Promasy.Domain.Organizations;

namespace Promasy.Persistence.Configurations
{
    public class SubDepartmentConfiguration : BaseConfiguration<SubDepartment>
    {
        protected override void Config(EntityTypeBuilder<SubDepartment> builder)
        {
            builder.Property(b => b.Name)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired();

            builder.HasMany(b => b.Employees)
                .WithOne(e => e.SubDepartment)
                .HasForeignKey(e => e.SubDepartmentId);
        }
    }
}