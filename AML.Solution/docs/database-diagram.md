# AML Database Diagram (Base)

```mermaid
erDiagram
    Clients ||--o{ ClientServices : has
    ClientServices ||--|| ServiceEndpoints : endpoint
    ClientServices ||--|| ServiceAuths : auth
    ClientServices ||--o{ ServiceHeaders : headers
    ClientServices ||--o{ ServiceFieldMappings : mappings
    ClientServices ||--o{ ServiceLogs : logs

    Clients {
      uniqueidentifier Id PK
      nvarchar Code UK
      nvarchar Name
      bit IsActive
      datetime2 CreatedAtUtc
      datetime2 UpdatedAtUtc
    }

    ClientServices {
      uniqueidentifier Id PK
      uniqueidentifier ClientId FK
      nvarchar Name
      nvarchar IntentKey
      int ServiceType
      bit IsActive
      int Priority
      datetime2 CreatedAtUtc
      datetime2 UpdatedAtUtc
    }

    ServiceEndpoints {
      uniqueidentifier Id PK
      uniqueidentifier ClientServiceId FK_UQ
      nvarchar Url
      int HttpMethod
      int TimeoutSeconds
    }

    ServiceAuths {
      uniqueidentifier Id PK
      uniqueidentifier ClientServiceId FK_UQ
      int AuthType
      nvarchar ApiKeyHeaderName
      nvarchar EncryptedApiKey
      nvarchar EncryptedUsername
      nvarchar EncryptedPassword
      nvarchar EncryptedClientId
      nvarchar EncryptedClientSecret
      nvarchar TokenUrl
      nvarchar Scope
    }

    ServiceHeaders {
      uniqueidentifier Id PK
      uniqueidentifier ClientServiceId FK
      nvarchar HeaderKey
      nvarchar HeaderValue
      bit IsSensitive
    }

    ServiceFieldMappings {
      uniqueidentifier Id PK
      uniqueidentifier ClientServiceId FK
      nvarchar SourceField
      nvarchar TargetField
      int Direction
      bit IsRequired
      nvarchar DefaultValue
    }

    ServiceLogs {
      uniqueidentifier Id PK
      uniqueidentifier ClientServiceId FK
      nvarchar CorrelationId
      datetime2 RequestedAtUtc
      int StatusCode
      bit Success
      bigint DurationMs
      nvarchar RequestPayload
      nvarchar ResponsePayload
      nvarchar ErrorMessage
    }
```

## Reglas principales del modelo

- `Clients.Code` es único.
- `ClientServices` tiene unicidad por `(ClientId, IntentKey)`.
- `ServiceEndpoints` y `ServiceAuths` son relaciones 1:1 con `ClientServices` (únicos por `ClientServiceId`).
- `ServiceHeaders` evita duplicados por `(ClientServiceId, HeaderKey)`.
- `ServiceFieldMappings` evita duplicados por `(ClientServiceId, SourceField, TargetField)`.
- Todos los hijos de `ClientServices` usan borrado en cascada.
