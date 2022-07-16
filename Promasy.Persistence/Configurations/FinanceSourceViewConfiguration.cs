using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Domain.Persistence.Views;

namespace Promasy.Persistence.Configurations;

public class FinanceSourceViewConfiguration : IEntityTypeConfiguration<FinanceSourceView>
{
    public void Configure(EntityTypeBuilder<FinanceSourceView> builder)
    {
        builder.ToView("VW_FinanceSources");
        builder.HasKey(b => b.Id);
    }
}