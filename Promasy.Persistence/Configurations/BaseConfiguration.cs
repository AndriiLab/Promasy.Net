using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Core.Persistence;

namespace Promasy.Persistence.Configurations
{
    public abstract class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : class, IBaseEntity
    {
        public void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.CreatedDate)
                .HasDefaultValueSql("now()");

            if (typeof(ISoftDeletable).IsAssignableFrom(typeof(TEntity)))
            {
                builder.HasQueryFilter(p => !EF.Property<bool>(p, nameof(ISoftDeletable.Deleted)));
            }
            
            Config(builder);
        }

        protected abstract void Config(EntityTypeBuilder<TEntity> builder);
    }
}