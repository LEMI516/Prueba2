# AML.Solution

Base inicial de la plataforma AML con arquitectura modular:

- `AML.Core`: dominio, contratos y modelos.
- `AML.Application`: comandos/queries/DTOs base.
- `AML.Infrastructure`: persistencia EF Core, configuraciones y repositorios.
- `AML.Orchestrator`: orquestación y resolución de adapters.
- `AML.Adapters.Base` y `AML.Adapters.Dynamic`: ejecución dinámica de integraciones REST.
- `AML.Gateway`: API ASP.NET Core (admin + integration + webhooks).
- `AML.SDK`: cliente reutilizable para invocar el Gateway desde otros sistemas.
- `AML.Genesys`: base para integración con Genesys Cloud.
- `AML.RulesEngine`: motor de reglas de negocio inicial.

## Requisitos

- .NET SDK 8+
- SQL Server (local o remoto)

## Estructura de base de datos

Ver diagrama y detalle de relaciones:

- `docs/database-diagram.md`

## Cadena de conexión

Configurada en:

- `src/AML.Gateway/appsettings.json`
- `src/AML.Gateway/appsettings.Development.json`

Clave usada: `ConnectionStrings:AmlDatabase`

## Migrations EF Core

La primera migración generada:

- `InitialCreate`
- Ubicación: `src/AML.Infrastructure/Persistence/Migrations`

Comandos útiles (desde la raíz del repo):

```bash
/workspace/.tools/dotnet-ef migrations add <NombreMigracion> \
  --project /workspace/AML.Solution/src/AML.Infrastructure/AML.Infrastructure.csproj \
  --startup-project /workspace/AML.Solution/src/AML.Gateway/AML.Gateway.csproj \
  --context AmlDbContext \
  --output-dir Persistence/Migrations
```

```bash
/workspace/.tools/dotnet-ef database update \
  --project /workspace/AML.Solution/src/AML.Infrastructure/AML.Infrastructure.csproj \
  --startup-project /workspace/AML.Solution/src/AML.Gateway/AML.Gateway.csproj \
  --context AmlDbContext
```

## Ejecutar API Gateway

```bash
/workspace/.dotnet/dotnet run --project /workspace/AML.Solution/src/AML.Gateway/AML.Gateway.csproj
```

Endpoints base:

- `POST /api/integration/process-intent`
- `POST /api/integration/genesys/webhook`
- `GET /api/admin/clients`

