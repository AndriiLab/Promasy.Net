using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Domain.Persistence.Views;

namespace Promasy.Persistence.Configurations;

public class FinanceDepartmentsWithSpendConfiguration : IEntityTypeConfiguration<FinanceSubDepartmentsWithSpend>
{
    public void Configure(EntityTypeBuilder<FinanceSubDepartmentsWithSpend> builder)
    {
        builder.ToView(@"""VW_FinanceSubDepartmentsWithSpend""");
        builder.HasKey(b => b.Id);
    }
}