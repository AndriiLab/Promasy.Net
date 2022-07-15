using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Domain.Persistence.Views;

namespace Promasy.Persistence.Configurations;

public class FinanceSourceWithSpendConfiguration : IEntityTypeConfiguration<FinanceSourceWithSpend>
{
    public void Configure(EntityTypeBuilder<FinanceSourceWithSpend> builder)
    {
        builder.ToView(@"""VW_FinanceSourcesWithSpend""");
        builder.HasKey(b => b.Id);
    }
}