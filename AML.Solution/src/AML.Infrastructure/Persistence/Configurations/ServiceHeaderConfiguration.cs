using AML.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AML.Infrastructure.Persistence.Configurations;

public sealed class ServiceHeaderConfiguration : IEntityTypeConfiguration<ServiceHeader>
{
    public void Configure(EntityTypeBuilder<ServiceHeader> builder)
    {
        builder.ToTable("ServiceHeaders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.HeaderKey)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.HeaderValue)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasIndex(x => new { x.ClientServiceId, x.HeaderKey })
            .IsUnique();
    }
}
