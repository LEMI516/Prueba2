using AML.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AML.Infrastructure.Persistence;

public sealed class AmlDbContext : DbContext
{
    public AmlDbContext(DbContextOptions<AmlDbContext> options)
        : base(options)
    {
    }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<ClientService> ClientServices => Set<ClientService>();
    public DbSet<ServiceEndpoint> ServiceEndpoints => Set<ServiceEndpoint>();
    public DbSet<ServiceAuth> ServiceAuths => Set<ServiceAuth>();
    public DbSet<ServiceHeader> ServiceHeaders => Set<ServiceHeader>();
    public DbSet<ServiceFieldMapping> ServiceFieldMappings => Set<ServiceFieldMapping>();
    public DbSet<ServiceLog> ServiceLogs => Set<ServiceLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AmlDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
