using AML.Prototype.Engine.Abstractions;
using AML.Prototype.Engine.Services;
using AML.Prototype.Engine.Stores;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IIntegrationDefinitionStore, InMemoryIntegrationDefinitionStore>();
builder.Services.AddHttpClient<IIntegrationExecutor, IntegrationExecutor>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "aml-prototype-api" }));

app.Run();
