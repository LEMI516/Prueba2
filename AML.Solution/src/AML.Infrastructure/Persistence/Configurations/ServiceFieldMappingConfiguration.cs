using AML.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AML.Infrastructure.Persistence.Configurations;

public sealed class ServiceFieldMappingConfiguration : IEntityTypeConfiguration<ServiceFieldMapping>
{
    public void Configure(EntityTypeBuilder<ServiceFieldMapping> builder)
    {
        builder.ToTable("ServiceFieldMappings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SourceField)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.TargetField)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.DefaultValue)
            .HasMaxLength(1000);

        builder.HasIndex(x => new { x.ClientServiceId, x.SourceField, x.TargetField })
            .IsUnique();
    }
}
