using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promasy.Common.Persistence;

namespace Promasy.Persistence.Configurations
{
    public abstract class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : class, IEntity
    {
        public void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasQueryFilter(p => !p.Deleted);
            Config(builder);
        }

        protected abstract void Config(EntityTypeBuilder<TEntity> builder);
    }
}