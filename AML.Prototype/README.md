# AML.Prototype

Sandbox independiente para construir y validar integraciones sin bloquear `AML.Solution`.

## Objetivo

- Permitir que el equipo de integraciones trabaje en paralelo.
- Probar llamadas HTTP dinámicas por configuración (`key + baseUrl + path + method + headers`).
- Iterar rápido con mocks locales antes de mover cambios al orquestador principal.

## Estructura

- `src/AML.Prototype.Contracts`: contratos compartidos (requests/responses).
- `src/AML.Prototype.Engine`: store en memoria + ejecutor dinámico HTTP.
- `src/AML.Prototype.Api`: endpoints CRUD de integraciones + ejecución + mocks.

## Ejecutar localmente

Desde la raíz del repo:

```bash
/workspace/.dotnet/dotnet run --project /workspace/AML.Prototype/src/AML.Prototype.Api/AML.Prototype.Api.csproj
```

Swagger:

- `http://localhost:5000/swagger` o `https://localhost:5001/swagger` (según profile/puerto)

Health:

- `GET /health`

## Flujo rápido de uso

1. **Sembrar integraciones demo locales**
   - `POST /api/prototype/integrations/seed-local`
2. **Ejecutar integración mock billing**
   - `POST /api/prototype/executions/run`
   - payload:

```json
{
  "integrationKey": "mock-billing",
  "pathParameters": {
    "customerId": "abc123"
  }
}
```

3. **Ejecutar integración mock payment**
   - `POST /api/prototype/executions/run`
   - payload:

```json
{
  "integrationKey": "mock-payment",
  "payload": {
    "customerId": "abc123",
    "amount": 1200,
    "currency": "USD"
  }
}
```

## Endpoints principales

- `GET /api/prototype/integrations`
- `GET /api/prototype/integrations/{key}`
- `POST /api/prototype/integrations/{key}`
- `PUT /api/prototype/integrations/{key}`
- `DELETE /api/prototype/integrations/{key}`
- `POST /api/prototype/integrations/seed-local`
- `POST /api/prototype/executions/run`
- `GET /api/prototype/mock/billing/{customerId}`
- `POST /api/prototype/mock/payment/authorize`

## Estrategia de convivencia con AML.Solution

- Todo prototipado de integraciones nuevas se realiza primero en `AML.Prototype`.
- Cuando una integración sea estable:
  - se formaliza contrato en `AML.Core`/`AML.Application`,
  - se migra la lógica a `AML.Adapters.Dynamic` o adapter específico,
  - se añade persistencia y observabilidad en `AML.Infrastructure`.

De esta manera, `AML.Solution` mantiene estabilidad mientras el equipo avanza en paralelo.
