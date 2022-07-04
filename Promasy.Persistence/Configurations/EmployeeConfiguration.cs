using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Core.Persistence;
using Promasy.Domain.Employees;

namespace Promasy.Persistence.Configurations
{
    public class EmployeeConfiguration : BaseConfiguration<Employee>
    {
        protected override void Config(EntityTypeBuilder<Employee> builder)
        {
            builder.Property(b => b.Email)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired();
            
            builder.Property(b => b.FirstName)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired();
            
            builder.Property(b => b.LastName)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired();
            
            builder.Property(b => b.MiddleName)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired(false);
            
            builder.Property(b => b.UserName)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired();
            
            builder.Property(b => b.PrimaryPhone)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired();

            builder.Property(b => b.ReservePhone)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired(false);
            
            builder.Property(b => b.Password)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired();
            
            builder
                .HasMany(p => p.Roles)
                .WithMany(p => p.Employees)
                .UsingEntity(j => j.ToTable("EmployeeRoles"));
        }
    }
}