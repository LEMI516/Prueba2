using AML.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AML.Infrastructure.Persistence.Configurations;

public sealed class ServiceAuthConfiguration : IEntityTypeConfiguration<ServiceAuth>
{
    public void Configure(EntityTypeBuilder<ServiceAuth> builder)
    {
        builder.ToTable("ServiceAuths");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AuthType)
            .IsRequired();

        builder.Property(x => x.ApiKeyHeaderName)
            .HasMaxLength(100);

        builder.Property(x => x.EncryptedApiKey)
            .HasMaxLength(2000);

        builder.Property(x => x.EncryptedUsername)
            .HasMaxLength(2000);

        builder.Property(x => x.EncryptedPassword)
            .HasMaxLength(2000);

        builder.Property(x => x.EncryptedClientId)
            .HasMaxLength(2000);

        builder.Property(x => x.EncryptedClientSecret)
            .HasMaxLength(2000);

        builder.Property(x => x.TokenUrl)
            .HasMaxLength(1000);

        builder.Property(x => x.Scope)
            .HasMaxLength(500);

        builder.HasIndex(x => x.ClientServiceId)
            .IsUnique();
    }
}
