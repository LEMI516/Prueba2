# Arquitectura de Alto Nivel — AML

## Objetivo

Habilitar integraciones multi-cliente entre canales de atención (Genesys/IA) y APIs externas de clientes BPO, con un modelo escalable basado en configuración.

---

## Vista de componentes (alto nivel)

```mermaid
flowchart LR
    %% Canales de entrada
    subgraph CH["Canales"]
        IA["IA / NLU"]
        GEN["Genesys Cloud"]
        EXT["Sistemas externos vía SDK"]
    end

    %% Capa de entrada
    subgraph GW["AML.Gateway"]
        API["Controllers + Middlewares + Filters"]
    end

    %% Capa de negocio
    subgraph APP["Aplicación y Orquestación"]
        APL["AML.Application<br/>DTOs + Commands/Queries"]
        ORQ["AML.Orchestrator<br/>OrchestratorService"]
        RUL["AML.RulesEngine<br/>BusinessRuleEngine"]
        REG["AdapterRegistry + DynamicAdapterResolver"]
    end

    %% Capa de integración
    subgraph ADP["Adaptadores"]
        DYN["AML.Adapters.Dynamic<br/>DynamicAdapter"]
        BAS["AML.Adapters.Base<br/>AuthHandlers + FieldMapper + ConnectionTester"]
    end

    %% Capa técnica
    subgraph INF["Infraestructura"]
        PERS["AML.Infrastructure<br/>EF Core + Repositories + Security + Resilience"]
        DB[("SQL Server")]
        LOG["Telemetry / Logging"]
    end

    %% Consumo externo
    subgraph OUT["Sistemas Cliente"]
        BPO["APIs Cliente BPO"]
    end

    %% Flujos
    IA --> API
    GEN --> API
    EXT --> API

    API --> APL --> ORQ
    ORQ --> RUL
    ORQ --> REG
    REG --> DYN --> BAS --> BPO
    ORQ --> PERS --> DB
    API --> LOG
    ORQ --> LOG
```

---

## Responsabilidad por capa

- **Gateway:** punto único de entrada HTTP; aplica validaciones transversales (middlewares/filtros).
- **Application/Orchestrator:** resuelve intención + cliente, ejecuta reglas y selecciona adaptador.
- **Adapters:** construyen y ejecutan la llamada HTTP externa con auth, headers y mapping.
- **Infrastructure:** persistencia, repositorios, seguridad técnica, resiliencia y telemetría.

---

## Flujo funcional resumido

1. Llega solicitud con `clientId`, `intent` y parámetros.
2. Orquestador busca configuración activa del servicio.
3. Motor de reglas valida precondiciones.
4. `DynamicAdapter` aplica configuración (auth/headers/mapeo) y llama API del cliente.
5. Se normaliza la respuesta al contrato AML.
6. Se persiste trazabilidad (logs/correlación) y se responde al consumidor.

---

## Principios de diseño

- **Separación de responsabilidades por capa.**
- **Configuración sobre customización por cliente.**
- **Observabilidad y trazabilidad desde el diseño.**
- **Evolución segura:** prototipos en `AML.Prototype`, consolidación productiva en `AML.Solution`.
