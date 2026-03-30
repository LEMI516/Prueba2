# Presentación Ejecutiva — Proyecto AML

## Diapositiva 1 — Portada

**Proyecto:** AML Platform (Gateway + Orquestación + Dynamic Adapter + SDK)  
**Objetivo:** habilitar integración multi-cliente sin desarrollo por cliente  
**Estado:** Base técnica inicial implementada y compilando

---

## Diapositiva 2 — Contexto y Problema

### Situación actual en integraciones BPO
- Cada nuevo cliente suele requerir código específico.
- El time-to-market crece por alta dependencia de desarrollo.
- Costos de mantenimiento aumentan con múltiples variantes.

### Necesidad del negocio
- Integrar servicios externos por configuración.
- Estandarizar contratos entre IA ↔ AML ↔ sistemas del cliente.
- Escalar onboarding de clientes con menor esfuerzo técnico.

---

## Diapositiva 3 — Visión de la Solución AML

### Principio clave
**Configuration over customization**:  
Nuevos clientes se habilitan desde panel admin + base de datos, con mínimo/no código.

### Flujo de alto nivel
1. IA envía intención (`intent`) y parámetros.
2. AML resuelve cliente + servicio activo.
3. Dynamic Adapter aplica:
   - autenticación,
   - headers,
   - mapeo de campos.
4. AML invoca API externa y retorna respuesta normalizada a IA.

---

## Diapositiva 4 — Arquitectura Base Implementada

Se creó la solución `AML.Solution` con estos módulos:

- `AML.Core` → dominio, contratos, excepciones.
- `AML.Application` → comandos/queries/DTOs base.
- `AML.Infrastructure` → EF Core + DbContext + repositorios.
- `AML.Orchestrator` → lógica de resolución y orquestación.
- `AML.Adapters.Base` → base técnica de integración HTTP.
- `AML.Adapters.Dynamic` → adapter dinámico configurable.
- `AML.Gateway` → API principal (Admin + Integration + Genesys webhook).
- `AML.SDK` → cliente reutilizable para consumir el Gateway.
- `AML.Genesys` → base de integración de eventos.
- `AML.RulesEngine` → reglas de negocio iniciales.

---

## Diapositiva 5 — Gateway y API

### Endpoints iniciales habilitados
- `POST /api/integration/process-intent`
- `POST /api/integration/genesys/webhook`
- `GET /api/admin/clients`

### Capacidades técnicas base
- Middlewares: excepciones, logging, auth placeholder, tenant resolution.
- Filtros: validación de entrada y autorización admin básica.
- DI configurado para orquestación, reglas, adapters y persistencia.

---

## Diapositiva 6 — Modelo de Datos (EF Core)

### Entidades principales
- `Clients`
- `ClientServices`
- `ServiceEndpoints`
- `ServiceAuths`
- `ServiceHeaders`
- `ServiceFieldMappings`
- `ServiceLogs`

### Reglas estructurales clave
- `Clients.Code` único.
- `ClientServices (ClientId, IntentKey)` único.
- Relación 1:1 de `ClientServices` con `ServiceEndpoints` y `ServiceAuths`.
- Índices para trazabilidad: `CorrelationId`, `RequestedAtUtc`.

> Diagrama ER disponible en: `docs/database-diagram.md`

---

## Diapositiva 7 — Migraciones y Persistencia

### Estado actual
- `AmlDbContext` implementado.
- Configuraciones Fluent API por entidad.
- Migración inicial creada: `InitialCreate`.
- Snapshot de modelo generado.

### Beneficio inmediato
- Base de datos versionada y reproducible para dev/test/qa.

---

## Diapositiva 8 — SDK (AML.SDK) para consumo del Gateway

### Componentes implementados
- `IAmlGatewayClient`
- `AmlGatewayClient` (HTTP client typed)
- `AmlGatewayClientOptions`
- DTOs de request/response
- Registro DI del SDK

### Beneficio
- Otros servicios/equipos pueden integrar AML de manera estandarizada y rápida.

---

## Diapositiva 9 — Dynamic Adapter (pieza estratégica)

### Qué habilita
- Ejecutar integraciones con APIs externas por configuración.
- Soportar autenticación (ApiKey/Basic/OAuth2 base técnica).
- Aplicar headers y mappings dinámicos.

### Valor para negocio
- Menor dependencia de desarrollos específicos por cliente.
- Mejor escalabilidad operativa para onboarding.

---

## Diapositiva 10 — Estado de avance (resumen ejecutivo)

### Ya implementado
- Esqueleto completo de arquitectura.
- Dominio + contratos + repositorios.
- Persistencia EF Core y migración inicial.
- Gateway con endpoints base.
- SDK funcional inicial.
- Documentación técnica base.

### Calidad técnica
- Solución compilando en verde (`dotnet build` OK).

---

## Diapositiva 11 — Riesgos y brechas actuales

1. **Seguridad**
   - Auth/JWT en estado base (placeholder en middleware).
2. **Admin funcional completo**
   - Falta cerrar CRUDs completos con validaciones avanzadas.
3. **Observabilidad**
   - Falta integración completa con métricas/tracing centralizado.
4. **Pruebas**
   - Pendiente suite de unit tests e integration tests end-to-end.

---

## Diapositiva 12 — Próximos pasos propuestos

### Sprint técnico siguiente (prioridad)
1. Completar CRUD Admin (clientes, servicios, headers, mappings).
2. Endurecer seguridad (JWT, políticas y manejo de secretos).
3. Completar Dynamic Adapter productivo (mapeos avanzados + resiliencia).
4. Implementar pruebas:
   - unitarias por capa,
   - integración con DB,
   - smoke tests de API.
5. Dashboard Angular Admin (wizard de 4 pasos).

---

## Diapositiva 13 — Indicadores de éxito sugeridos

- **Onboarding de cliente:** reducción de esfuerzo técnico.
- **Tiempo de integración:** desde días/semanas a configuración guiada.
- **Reusabilidad:** % de integraciones sin código adicional.
- **Confiabilidad:** tasa de éxito de llamadas y latencia p95.
- **Operación:** tiempo de diagnóstico por `CorrelationId`.

---

## Diapositiva 14 — Cierre

La base del proyecto AML ya está construida para evolucionar a un modelo escalable de integraciones multi-cliente.  
La siguiente fase debe enfocarse en **completar capacidades productivas** (seguridad, CRUD full, pruebas, observabilidad) para habilitar pilotos con clientes reales.

