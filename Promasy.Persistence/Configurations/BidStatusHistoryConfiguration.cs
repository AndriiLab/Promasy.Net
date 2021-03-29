using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Domain.Bids;

namespace Promasy.Persistence.Configurations
{
    public class BidStatusHistoryConfiguration : BaseConfiguration<BidStatusHistory>
    {
        protected override void Config(EntityTypeBuilder<BidStatusHistory> builder)
        {
        }
    }
}