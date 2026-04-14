# AML Architecture Diagram

```mermaid
flowchart TB
    %% Actores externos
    subgraph EXT["Sistemas Externos"]
        GEN["Genesys Cloud"]
        IA["Motor IA / NLU"]
        BPO["APIs Cliente BPO"]
    end

    %% Gateway
    subgraph GW["AML.Gateway (ASP.NET Core)"]
        CTRLI["IntegrationController"]
        CTRLG["GenesysController"]
        CTRLA["Admin Controllers"]
        MW["Middlewares<br/>- ExceptionHandling<br/>- RequestLogging<br/>- Authentication<br/>- TenantResolution"]
        FILT["Filters<br/>- ValidationFilter<br/>- AdminAuthorizationFilter"]
    end

    %% Capa de aplicación y orquestación
    subgraph APP["Application + Orchestration"]
        APPDTO["AML.Application<br/>Commands / Queries / DTOs"]
        ORCH["OrchestratorService"]
        REG["AdapterRegistry + DynamicAdapterResolver"]
        RULES["BusinessRuleEngine"]
        FALL["FallbackHandler"]
    end

    %% Adaptadores
    subgraph ADP["Adapters"]
        DYN["AML.Adapters.Dynamic<br/>DynamicAdapter"]
        BASE["AML.Adapters.Base<br/>BaseAdapter + AuthHandlers + FieldMapper"]
    end

    %% Infraestructura
    subgraph INFRA["AML.Infrastructure"]
        DBCTX["AmlDbContext (EF Core)"]
        REPO["Repositories<br/>Client / ClientService / ServiceLog"]
        SEC["Security / Encryption (base)"]
        RES["Resilience (base)"]
    end

    %% Persistencia
    subgraph DATA["SQL Server"]
        T1["Clients"]
        T2["ClientServices"]
        T3["ServiceEndpoints"]
        T4["ServiceAuths"]
        T5["ServiceHeaders"]
        T6["ServiceFieldMappings"]
        T7["ServiceLogs"]
    end

    %% SDK
    subgraph SDK["AML.SDK"]
        SDKC["AmlGatewayClient"]
    end

    %% Integración Genesys
    subgraph GENMOD["AML.Genesys"]
        GWH["GenesysWebhookHandler"]
        GSM["GenesysSessionManager"]
        GTS["GenesysTypificationService"]
    end

    %% Flujos principales
    IA -->|"ServiceRequest {clientId,intent,params}"| CTRLI
    GEN -->|"Webhook events"| CTRLG

    CTRLI --> MW --> FILT --> APPDTO --> ORCH
    CTRLA --> MW
    CTRLG --> GWH --> GSM
    GWH --> GTS

    ORCH --> RULES
    ORCH --> REG
    ORCH --> REPO
    ORCH --> FALL

    REG --> DYN
    DYN --> BASE
    BASE -->|"HTTP dinámico + auth + headers + mapping"| BPO

    REPO --> DBCTX --> T1
    DBCTX --> T2
    DBCTX --> T3
    DBCTX --> T4
    DBCTX --> T5
    DBCTX --> T6
    DBCTX --> T7

    SDKC -->|"POST /api/integration/process-intent"| CTRLI

    SEC -. soporte .-> BASE
    RES -. políticas .-> BASE
```

## Flujo resumido

1. IA o SDK invocan `IntegrationController` con intención y parámetros.
2. `OrchestratorService` consulta configuración activa en BD.
3. `DynamicAdapter` aplica autenticación, headers y mappings.
4. Se invoca API del cliente BPO y se normaliza la respuesta.
5. Se registra trazabilidad en `ServiceLogs` y se retorna respuesta estándar.
