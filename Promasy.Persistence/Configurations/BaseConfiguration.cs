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
            Config(builder);
        }

        protected abstract void Config(EntityTypeBuilder<TEntity> builder);
    }
}