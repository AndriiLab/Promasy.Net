using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Domain.Persistence.Views;

namespace Promasy.Persistence.Configurations;

public class FinanceDepartmentsViewConfiguration : IEntityTypeConfiguration<FinanceSubDepartmentsView>
{
    public void Configure(EntityTypeBuilder<FinanceSubDepartmentsView> builder)
    {
        builder.ToView("VW_FinanceSubDepartments");
        builder.HasKey(b => b.Id);
    }
}