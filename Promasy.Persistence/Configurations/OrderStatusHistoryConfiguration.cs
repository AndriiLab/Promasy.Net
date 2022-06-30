using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Domain.Orders;

namespace Promasy.Persistence.Configurations
{
    public class OrderStatusHistoryConfiguration : BaseConfiguration<OrderStatusHistory>
    {
        protected override void Config(EntityTypeBuilder<OrderStatusHistory> builder)
        {
        }
    }
}