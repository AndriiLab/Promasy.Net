using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Common.Persistence;
using Promasy.Domain.Bids;

namespace Promasy.Persistence.Configurations
{
    public class BidConfiguration : BaseConfiguration<Bid>
    {
        protected override void Config(EntityTypeBuilder<Bid> builder)
        {
            builder.Property(b => b.Description)
                .HasMaxLength(PersistenceConstant.FieldLarge)
                .IsRequired();

            builder.Property(b => b.CatNum)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired(false);
            
            builder.Property(b => b.Type)
                .HasMaxLength(PersistenceConstant.FieldMedium)
                .IsRequired(false);

            builder.HasOne(b => b.Cpv)
                .WithMany()
                .HasForeignKey(b => b.CpvCode);
        }
    }
}