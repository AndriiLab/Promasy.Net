using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Common.Persistence;
using Promasy.Domain.Institutes;

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

            builder.HasOne(b => b.Creator)
                .WithMany()
                .HasForeignKey(b => b.CreatorId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(b => b.Modifier)
                .WithMany()
                .HasForeignKey(b => b.ModifierId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}