using AML.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AML.Infrastructure.Persistence.Configurations;

public sealed class ServiceLogConfiguration : IEntityTypeConfiguration<ServiceLog>
{
    public void Configure(EntityTypeBuilder<ServiceLog> builder)
    {
        builder.ToTable("ServiceLogs");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.CorrelationId).HasMaxLength(64).IsRequired();
        builder.Property(x => x.DurationMs).IsRequired();
        builder.Property(x => x.Success).IsRequired();
        builder.Property(x => x.RequestPayload).HasMaxLength(4000);
        builder.Property(x => x.ResponsePayload).HasMaxLength(4000);
        builder.Property(x => x.ErrorMessage).HasMaxLength(1000);

        builder.HasIndex(x => x.ClientServiceId);
        builder.HasIndex(x => x.CorrelationId);
        builder.HasIndex(x => x.RequestedAtUtc);
    }
}
