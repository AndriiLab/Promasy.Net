using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Promasy.Persistence.Configurations
{
    public class DeviceFlowCodesConfiguration : IEntityTypeConfiguration<DeviceFlowCodes>
    {
        public void Configure(EntityTypeBuilder<DeviceFlowCodes> builder)
        {
            builder.Property<int>("Id");
            builder.HasKey("Id");
        }
    }
}