using AML.Adapters.Base;
using AML.Adapters.Dynamic;
using AML.Core.Contracts.Adapters;
using AML.Core.Contracts.Services;
using AML.Core.Models.Requests;
using AML.Genesys;
using AML.Gateway.Filters;
using AML.Gateway.Middleware;
using AML.Infrastructure;
using AML.Orchestrator;
using AML.RulesEngine;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAmlInfrastructure(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AmlDatabase")));

builder.Services.AddHttpClient();
builder.Services.AddScoped<HttpClient>();
builder.Services.AddScoped<IBusinessRuleEngine, BusinessRuleEngine>();
builder.Services.AddScoped<IAdapterRegistry, AdapterRegistry>();
builder.Services.AddScoped<DynamicAdapterResolver>();
builder.Services.AddScoped<IServiceAdapter, DynamicAdapter>();
builder.Services.AddScoped<IOrchestrator, OrchestratorService>();
builder.Services.AddScoped<IConnectionTester, ConnectionTester>();
builder.Services.AddScoped<GenesysWebhookHandler>();
builder.Services.AddScoped<GenesysSessionManager>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
    options.Filters.Add<AdminAuthorizationFilter>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<AuthenticationMiddleware>();
app.UseMiddleware<TenantResolutionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "ok", at = DateTime.UtcNow }));
app.MapPost("/api/v1/integration/process", async (
    IOrchestrator orchestrator,
    ServiceRequest request,
    CancellationToken ct) =>
{
    var response = await orchestrator.ProcessAsync(request, ct);
    return Results.Ok(response);
});

app.Run();
