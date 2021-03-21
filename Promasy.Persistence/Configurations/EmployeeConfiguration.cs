using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Common.Persistence;
using Promasy.Domain.Users;

namespace Promasy.Persistence.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");

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
            
            builder.Property(b => b.PhoneNumber)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired(false);
            
            builder.Property(b => b.PhoneReserve)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired(false);

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