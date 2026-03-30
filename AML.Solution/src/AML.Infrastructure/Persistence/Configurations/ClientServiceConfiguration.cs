using AML.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AML.Infrastructure.Persistence.Configurations;

public sealed class ClientServiceConfiguration : IEntityTypeConfiguration<ClientService>
{
    public void Configure(EntityTypeBuilder<ClientService> builder)
    {
        builder.ToTable("ClientServices");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(180)
            .IsRequired();

        builder.Property(x => x.IntentKey)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(x => x.ServiceType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Priority)
            .HasDefaultValue(1);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(x => new { x.ClientId, x.IntentKey })
            .IsUnique();

        builder.HasOne(x => x.Client)
            .WithMany(x => x.Services)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Endpoint)
            .WithOne(x => x.ClientService)
            .HasForeignKey<ServiceEndpoint>(x => x.ClientServiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Auth)
            .WithOne(x => x.ClientService)
            .HasForeignKey<ServiceAuth>(x => x.ClientServiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Headers)
            .WithOne(x => x.ClientService)
            .HasForeignKey(x => x.ClientServiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.FieldMappings)
            .WithOne(x => x.ClientService)
            .HasForeignKey(x => x.ClientServiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Logs)
            .WithOne(x => x.ClientService)
            .HasForeignKey(x => x.ClientServiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
