using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Common.Persistence;
using Promasy.Domain.Bids;

namespace Promasy.Persistence.Configurations
{
    public class BidStatusConfiguration : BaseConfiguration<BidStatus>
    {
        protected override void Config(EntityTypeBuilder<BidStatus> builder)
        {
            builder.Property(b => b.Status)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired();
        }
    }
}