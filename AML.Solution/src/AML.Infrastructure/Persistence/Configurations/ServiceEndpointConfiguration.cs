using AML.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AML.Infrastructure.Persistence.Configurations;

public sealed class ServiceEndpointConfiguration : IEntityTypeConfiguration<ServiceEndpoint>
{
    public void Configure(EntityTypeBuilder<ServiceEndpoint> builder)
    {
        builder.ToTable("ServiceEndpoints");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.ClientServiceId).IsRequired();

        builder.Property(x => x.Url)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.HttpMethod)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.TimeoutSeconds)
            .IsRequired();

        builder.HasIndex(x => x.ClientServiceId).IsUnique();
    }
}
