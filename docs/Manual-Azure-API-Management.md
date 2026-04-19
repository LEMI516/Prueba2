# Manual de Uso y Mejores Prácticas de Azure API Management (APIM)

> Guía práctica en español para arquitectos, desarrolladores y operadores que diseñan, publican y operan APIs sobre **Azure API Management**.

---

## Tabla de contenido

1. [Introducción](#1-introducción)
2. [Conceptos clave](#2-conceptos-clave)
3. [Arquitectura y componentes](#3-arquitectura-y-componentes)
4. [Tiers (SKUs) y cuándo elegir cada uno](#4-tiers-skus-y-cuándo-elegir-cada-uno)
5. [Aprovisionamiento de una instancia](#5-aprovisionamiento-de-una-instancia)
6. [Publicación de una API paso a paso](#6-publicación-de-una-api-paso-a-paso)
7. [Políticas (Policies)](#7-políticas-policies)
8. [Seguridad](#8-seguridad)
9. [Versionado y revisiones](#9-versionado-y-revisiones)
10. [Productos, suscripciones y Developer Portal](#10-productos-suscripciones-y-developer-portal)
11. [Redes: VNet, Private Endpoints y zonas de disponibilidad](#11-redes-vnet-private-endpoints-y-zonas-de-disponibilidad)
12. [Observabilidad y monitoreo](#12-observabilidad-y-monitoreo)
13. [CI/CD e Infrastructure as Code](#13-cicd-e-infrastructure-as-code)
14. [Rendimiento, caché y limitación de tasa](#14-rendimiento-caché-y-limitación-de-tasa)
15. [Costos y FinOps](#15-costos-y-finops)
16. [Mejores prácticas consolidadas](#16-mejores-prácticas-consolidadas)
17. [Antipatrones comunes](#17-antipatrones-comunes)
18. [Checklist de Go-Live](#18-checklist-de-go-live)
19. [Recursos oficiales](#19-recursos-oficiales)

---

## 1. Introducción

**Azure API Management (APIM)** es la plataforma PaaS de Microsoft para publicar, asegurar, transformar, observar y monetizar APIs. Funciona como un *API Gateway* gestionado que se interpone entre los consumidores (apps móviles, web, B2B, partners) y los *backends* (App Service, Functions, AKS, Service Fabric, máquinas virtuales, servicios on‑premise, SaaS, etc.).

Casos de uso típicos:

- Exponer microservicios internos de forma controlada hacia internet.
- Componer una **fachada** uniforme sobre múltiples backends heterogéneos (REST, SOAP, GraphQL, WebSocket, gRPC).
- Implementar **gobierno de APIs**: autenticación, rate limiting, cuotas, transformación, validación y versionado.
- Habilitar un **portal de desarrolladores** para descubrimiento y onboarding.
- Monetización mediante productos y suscripciones.

---

## 2. Conceptos clave

| Concepto | Descripción |
|---|---|
| **API** | Definición lógica de un conjunto de operaciones expuesto por APIM. Puede importarse desde OpenAPI, WSDL, App Service, Function App, Logic App, etc. |
| **Operation** | Cada endpoint individual (verbo HTTP + ruta) dentro de una API. |
| **Backend** | Servicio real que atiende la solicitud (URL, App Service, Function, Service Fabric, etc.). |
| **Product** | Agrupación de una o varias APIs publicadas conjuntamente con políticas y planes de suscripción. |
| **Subscription** | Credencial (clave) que permite a un consumidor invocar APIs de un producto. |
| **Policy** | Lógica declarativa en XML que se ejecuta en pipelines `inbound`, `backend`, `outbound` y `on-error`. |
| **Named Value** | Variable/secreto reutilizable referenciable como `{{nombre}}` en políticas (puede integrarse con Key Vault). |
| **Revision** | Cambio no disruptivo de una API; permite editar sin afectar la versión productiva. |
| **Version** | Cambio disruptivo expuesto por path, query string o header. |
| **Developer Portal** | Sitio web auto‑servicio para que los consumidores descubran y prueben APIs. |
| **Self-hosted Gateway** | Gateway containerizado que se ejecuta en cualquier lugar (on‑prem, otra nube, Edge). |

---

## 3. Arquitectura y componentes

```
+------------------+        +-----------------------+        +---------------------+
|  Consumidores    |  --->  |  Azure API Management |  --->  |  Backends           |
|  (Web, Mobile,   |        |  (Gateway + Portal +  |        |  (App Service, AKS, |
|  Partners, B2B)  |        |   Mgmt Plane)         |        |   Functions, On‑prem)|
+------------------+        +-----------------------+        +---------------------+
                                     |
                                     v
                           +---------------------+
                           |  Observabilidad     |
                           | (App Insights, Log  |
                           |  Analytics, Event   |
                           |  Hub)               |
                           +---------------------+
```

Componentes:

- **Gateway**: ejecuta políticas y enruta tráfico. Disponible *managed* (multi‑tenant o dedicado) o *self‑hosted* (Docker/Kubernetes).
- **Plano de administración**: APIs/Portal Azure/CLI/ARM/Bicep/Terraform.
- **Developer Portal**: portal personalizable (Jamstack), publicable también de forma autohospedada.

---

## 4. Tiers (SKUs) y cuándo elegir cada uno

| SKU | SLA | Aislamiento | VNet | Múltiples regiones | Casos de uso |
|---|---|---|---|---|---|
| **Consumption** | 99.95% | Multi‑tenant serverless | No (sí Private Endpoint) | No | Cargas esporádicas, pago por uso, prototipos. |
| **Developer** | Sin SLA | Dedicado | Sí | No | Pruebas, desarrollo, **nunca producción**. |
| **Basic / Basic v2** | 99.95% | Dedicado | v2 sí | No | Producciones pequeñas. |
| **Standard / Standard v2** | 99.95% | Dedicado | Sí | No (Standard v2 sí en preview) | Producción media. |
| **Premium** | 99.99%* | Dedicado | Sí (modo Internal/External) | Sí (multi‑región, AZ) | Producción crítica, multi‑región, integración VNet privada. |
| **Premium v2 / Workspaces** | 99.99% | Dedicado | Sí | Sí | Grandes plataformas, multi‑equipo con aislamiento. |

> *El SLA de 99.99% en Premium aplica desplegado en **Availability Zones** o **multi‑región**.

**Recomendación**: Para producción seria, **Premium** (multi‑región y AZ) o **Standard v2**. Evita **Developer** en producción: no tiene SLA.

---

## 5. Aprovisionamiento de una instancia

### 5.1 Con Azure CLI

```bash
# Variables
RG="rg-apim-prod"
LOC="eastus2"
APIM="apim-miempresa-prod"
PUBNAME="Mi Empresa"
PUBMAIL="apis@miempresa.com"

az group create -n $RG -l $LOC

az apim create \
  --name $APIM \
  --resource-group $RG \
  --location $LOC \
  --publisher-name "$PUBNAME" \
  --publisher-email "$PUBMAIL" \
  --sku-name Premium \
  --sku-capacity 1 \
  --enable-managed-identity true
```

### 5.2 Con Bicep (recomendado para IaC)

```bicep
param location string = resourceGroup().location
param apimName string
param publisherEmail string
param publisherName string

resource apim 'Microsoft.ApiManagement/service@2023-05-01-preview' = {
  name: apimName
  location: location
  sku: {
    name: 'Premium'
    capacity: 1
  }
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    publisherEmail: publisherEmail
    publisherName: publisherName
    virtualNetworkType: 'Internal'
  }
  zones: [ '1', '2', '3' ]
}
```

> El aprovisionamiento puede tardar **30–45 minutos** (especialmente Premium con VNet). Planifícalo en pipelines.

---

## 6. Publicación de una API paso a paso

1. **Definir el contrato** preferiblemente con **OpenAPI 3.x** (o WSDL para SOAP, schema para GraphQL).
2. **Importar la API** desde el portal o CLI:
   ```bash
   az apim api import \
     --resource-group $RG --service-name $APIM \
     --api-id pedidos-v1 --path pedidos \
     --specification-format OpenApi \
     --specification-url https://miempresa.com/specs/pedidos.yaml \
     --service-url https://backend.miempresa.com
   ```
3. **Asignar a un Product** (p. ej. `Internal`, `Partners`, `Public`).
4. **Configurar políticas** (autenticación, CORS, rate limiting, caché, transformación).
5. **Probar** desde el editor de pruebas integrado o desde el Developer Portal.
6. **Versionar** desde el primer despliegue (`v1`).
7. **Publicar** documentación en el Developer Portal.

---

## 7. Políticas (Policies)

Las políticas son el corazón de APIM. Se escriben en XML y se ejecutan en cuatro fases:

```xml
<policies>
  <inbound>
    <!-- antes de llamar al backend -->
  </inbound>
  <backend>
    <!-- llamada al backend (forward-request por defecto) -->
  </backend>
  <outbound>
    <!-- después de la respuesta del backend -->
  </outbound>
  <on-error>
    <!-- si algo falla -->
  </on-error>
</policies>
```

### 7.1 Ejemplo: validar JWT, limitar tasa y agregar header

```xml
<policies>
  <inbound>
    <base />
    <validate-jwt header-name="Authorization" failed-validation-httpcode="401"
                  failed-validation-error-message="Token inválido">
      <openid-config url="https://login.microsoftonline.com/{{tenantId}}/v2.0/.well-known/openid-configuration" />
      <required-claims>
        <claim name="aud">
          <value>{{audience}}</value>
        </claim>
      </required-claims>
    </validate-jwt>

    <rate-limit-by-key calls="100" renewal-period="60"
                      counter-key="@(context.Subscription?.Id ?? context.Request.IpAddress)" />

    <set-header name="X-Correlation-Id" exists-action="skip">
      <value>@(Guid.NewGuid().ToString())</value>
    </set-header>
  </inbound>

  <backend>
    <forward-request timeout="30" />
  </backend>

  <outbound>
    <base />
    <set-header name="X-Powered-By" exists-action="delete" />
  </outbound>

  <on-error>
    <base />
    <set-header name="X-Error-Trace" exists-action="override">
      <value>@(context.RequestId)</value>
    </set-header>
  </on-error>
</policies>
```

### 7.2 Políticas más usadas

| Categoría | Políticas |
|---|---|
| **Autenticación** | `validate-jwt`, `validate-azure-ad-token`, `authentication-managed-identity`, `authentication-basic`, `authentication-certificate` |
| **Tasa y cuotas** | `rate-limit`, `rate-limit-by-key`, `quota`, `quota-by-key` |
| **Transformación** | `set-body`, `set-header`, `set-query-parameter`, `xml-to-json`, `json-to-xml`, `rewrite-uri` |
| **Caché** | `cache-lookup`, `cache-store`, `cache-lookup-value`, `cache-store-value` |
| **Validación** | `validate-content`, `validate-parameters`, `validate-headers`, `validate-status-code` |
| **Enrutamiento** | `set-backend-service`, `forward-request`, `choose`, `retry`, `circuit-breaker` (preview) |
| **Seguridad** | `ip-filter`, `check-header`, `cors`, `restrict-caller-ips`, `mock-response` |

### 7.3 Buenas prácticas con políticas

- **Define políticas globales** (a nivel de API Management) para CORS, headers comunes, observabilidad y *rate limiting* base.
- **Usa `<base />`** siempre para heredar el comportamiento del nivel superior.
- **Externaliza secretos** con `Named Values` referenciados a **Key Vault**.
- **Versiona los XML** en Git, no los edites únicamente desde el portal.
- Mantén las políticas **idempotentes** y **deterministas**.
- Evita lógica compleja en C#; si necesitas más, llama a una **Function** o usa `send-request`.

---

## 8. Seguridad

### 8.1 Autenticación de consumidores

- **Subscription Key** (`Ocp-Apim-Subscription-Key`): mínimo aceptable, no usar como único factor.
- **OAuth 2.0 / OIDC con Azure AD (Entra ID)**: recomendado para B2B y B2C.
- **mTLS** con certificados de cliente (`validate-client-certificate`).
- **API Keys propias** (en header) sólo para sistemas legados.

### 8.2 Autenticación al backend

- **Managed Identity** (preferido) → `authentication-managed-identity`.
- Certificados → almacenados en APIM o Key Vault.
- Llaves compartidas (último recurso, en Named Values + Key Vault).

### 8.3 Protección de la superficie

- Habilita **WAF** (Application Gateway o Front Door) **delante** de APIM si está expuesto a internet.
- Restringe el **plano de gestión** con RBAC y *Privileged Identity Management*.
- Activa **Defender for APIs** (Microsoft Defender for Cloud) para detección de amenazas.
- Implementa **Zero Trust**: nunca confíes en el origen, valida cada request.
- Habilita **CORS** sólo para los orígenes necesarios.
- Define **políticas de validación de esquema** (`validate-content`) para evitar payloads maliciosos.

### 8.4 Gestión de secretos

- Centraliza secretos en **Azure Key Vault**.
- Crea **Named Values** del tipo *Key Vault* (con identidad administrada) para refresco automático.
- Habilita **rotación** y auditoría desde Key Vault.

---

## 9. Versionado y revisiones

| Mecanismo | Uso |
|---|---|
| **Revisiones** | Cambios **no disruptivos**: ajustes de políticas, correcciones, nuevos campos opcionales. Se publica una revisión sin romper consumidores. |
| **Versiones** | Cambios **disruptivos**: nuevos contratos, eliminación de operaciones. Identificación por `path` (`/v1`, `/v2`), `header` o `query`. |

Buenas prácticas:

- **Versiona desde el día 1** (`v1`), aunque solo exista una versión.
- Prefiere versionado por **path** (más claro y *cache friendly*).
- Documenta el **ciclo de vida** (Beta, GA, Deprecated, Retired) y avisa con anticipación.
- Mantén **compatibilidad hacia atrás** dentro de una misma versión mayor.
- Usa **revisiones** para iterar y promover a `current` cuando esté validado.

---

## 10. Productos, suscripciones y Developer Portal

### 10.1 Productos

- Agrupa APIs por **audiencia** (Internal, Partners, Public, Mobile, IoT) o por **plan** (Free, Gold).
- Asocia políticas específicas por producto (cuotas, autenticación, SLA).
- Permite **aprobación manual** de suscripciones cuando aplique (B2B/Partners).

### 10.2 Suscripciones

- Una suscripción = una clave principal + una secundaria (rotables).
- Usa scopes:
  - **Producto** (recomendado, granular).
  - **API** (cuando se quiere desacoplar de productos).
  - **Todas las APIs** (uso interno, evitar para externos).

### 10.3 Developer Portal

- Personaliza branding, contenido, ejemplos y SDKs.
- Habilita **registro con Azure AD / B2C** o invitación.
- Usa **CI del portal** (`portal-cli` y branch `portal/`) para versionarlo en Git.
- Publica **changelog** y políticas de uso.

---

## 11. Redes: VNet, Private Endpoints y zonas de disponibilidad

### 11.1 Modos de despliegue

- **External**: APIM en VNet pero con IP pública, accesible desde internet.
- **Internal**: APIM en VNet con IP privada únicamente. Combinable con WAF/Application Gateway al frente para exponer selectivamente.
- **Private Endpoint** (Standard v2/Premium v2): para acceso privado al endpoint del gateway.

### 11.2 Recomendaciones de red

- Despliega en **Premium con Availability Zones** para alta disponibilidad regional.
- Usa **multi‑región** con *Traffic Manager / Front Door* para DR.
- Coloca **NSGs** y **rutas UDR** según la matriz de puertos requerida (3443, 6390, 443, etc.).
- Integra **DNS privado** para resolución interna de hostnames personalizados.
- Para llegar a backends on‑prem, usa **ExpressRoute** o **VPN** + APIM Internal.

---

## 12. Observabilidad y monitoreo

### 12.1 Diagnósticos

- Exporta logs y métricas a **Log Analytics**, **Event Hub** y **Storage**.
- Configura **Application Insights** por API (sampling 100% en pruebas, 10–25% en producción).
- Captura request/response **sin headers ni cuerpos sensibles** (PII, tokens).

### 12.2 Métricas clave

- **Capacity** (uso del gateway): mantenerlo < 60–70% para tener margen.
- **Latencia**: gateway vs. backend, percentiles p50/p95/p99.
- **Errores**: tasas 4xx/5xx por API, operación, producto y suscripción.
- **Rate limit hits** y bloqueos.
- **Top consumers** por suscripción.

### 12.3 Alertas recomendadas

- Capacidad del gateway > 70% durante 10 min.
- Errores 5xx > 1% en ventana de 5 min.
- Latencia p95 > umbral por API.
- Fallos de validación de JWT.
- Caducidad próxima de certificados.

### 12.4 Trazabilidad

- Activa **`trace`** en políticas para depuración (con tokens *Ocp-Apim-Trace*).
- Propaga `traceparent` (W3C Trace Context) hacia el backend.
- Correlaciona con App Insights end‑to‑end.

---

## 13. CI/CD e Infrastructure as Code

### 13.1 Estrategias

- **Bicep / ARM / Terraform** para la instancia y recursos compartidos.
- **APIOps** con [`apiops`](https://github.com/Azure/apiops) o [`apim-devops-toolkit`](https://github.com/Azure/azure-api-management-devops-resource-kit) para sincronizar APIs/políticas desde Git a múltiples entornos (Dev → QA → Prod).
- **OpenAPI como fuente de verdad**: cada API se versiona en su repo y se importa en pipeline.

### 13.2 Pipeline típico (Azure DevOps / GitHub Actions)

1. **Lint OpenAPI** (`spectral`, `redocly`).
2. **Validar políticas** (XSD de Microsoft).
3. **Diff** contra el entorno destino.
4. **Deploy** con Bicep o `apiops extractor/publisher`.
5. **Smoke tests** (Newman/Postman, REST Client).
6. **Promote** a siguiente entorno con aprobación.

### 13.3 Estrategia multi‑entorno

- Una **instancia APIM por entorno** (Dev, QA, Pre, Prod). Evita compartir la misma instancia entre prod y no‑prod.
- Usa **Named Values** distintos por entorno.
- Aplica **etiquetas (tags)** para gobierno y costos.

---

## 14. Rendimiento, caché y limitación de tasa

### 14.1 Caching

- Caché interno hasta **Standard**, **Redis externo** en cualquier tier (recomendado en Premium).
- Activa caché para respuestas idempotentes (`GET`) con `Vary` por suscripción/usuario.
- Define `cache-control` claros y TTL acordes al SLA del dato.

```xml
<inbound>
  <base />
  <cache-lookup vary-by-developer="false" vary-by-developer-groups="false">
    <vary-by-header>Accept</vary-by-header>
    <vary-by-query-parameter>id</vary-by-query-parameter>
  </cache-lookup>
</inbound>
<outbound>
  <base />
  <cache-store duration="60" />
</outbound>
```

### 14.2 Rate limiting y cuotas

- Combina **rate-limit** (corto plazo, p. ej. 100 rps) y **quota** (largo plazo, p. ej. 1M/mes).
- Granularidad por **suscripción**, **IP**, **usuario** o **clave personalizada**.
- Comunica los límites en headers (`Retry-After`, `X-RateLimit-*`).

### 14.3 Escalado

- Escala **unidades** del gateway según métrica `Capacity`.
- En Premium, escala por **región** según necesidad geográfica.
- Habilita **Autoscale** (reglas en Azure Monitor) cuidando el tiempo de provisión.

---

## 15. Costos y FinOps

- **Premium** se factura por unidad/hora y por región adicional. Optimiza colocando réplicas sólo donde se necesita.
- **Consumption** se cobra por llamada: ideal para tráfico irregular.
- **Self‑hosted Gateway** se factura aparte por gateway.
- Aplica **tags** (`env`, `owner`, `costcenter`) y revisa con **Cost Management**.
- Apaga instancias **Developer** fuera de horario laboral con automation.
- Reutiliza una sola instancia para múltiples APIs/equipos en lugar de varias pequeñas (cuando el aislamiento lo permita; usa **Workspaces** en Premium v2 para multi‑equipo).

---

## 16. Mejores prácticas consolidadas

### Diseño

- Estandariza con **OpenAPI 3.x** y **estilo REST** (o GraphQL/gRPC cuando aplique).
- Define una **guía de diseño de APIs** (naming, paginación, errores RFC 7807, versionado).
- Aplica **Backend for Frontend (BFF)** cuando convenga, sin sobrecargar APIM con lógica de negocio.

### Seguridad

- **Defensa en profundidad**: WAF + APIM + backend con autenticación propia.
- Nunca expongas backends directamente: el backend debe **rechazar tráfico que no venga de APIM** (mTLS, IP allowlist, VNet o token compartido).
- Rota claves y secretos automáticamente.

### Operación

- **IaC + GitOps**: nada se cambia manualmente en producción.
- **Observabilidad obligatoria**: logs, métricas, traces y alertas para cada API.
- Documenta **runbooks** para incidentes comunes (caída de backend, throttling, expiración de cert).
- Realiza **pruebas de carga** previas a Go‑Live (k6, JMeter, Azure Load Testing).

### Gobierno

- Define **owners** por API y producto.
- Aplica **políticas globales** para estándares mínimos (CORS, headers, logging).
- Mantén un **catálogo de APIs** (puede usarse el Developer Portal o **Azure API Center**).
- Establece un **proceso de deprecación** documentado.

---

## 17. Antipatrones comunes

- **Usar la SKU Developer en producción** (sin SLA).
- **Editar políticas sólo desde el portal** sin versionar en Git.
- **Exponer backends sin restricción**: backends accesibles directamente sin pasar por APIM.
- **Lógica de negocio en políticas** complejas en lugar de en el backend.
- **Una sola suscripción "maestra"** compartida por todos los consumidores.
- **No versionar** desde el inicio.
- **No habilitar Application Insights** o capturar headers/cuerpos con PII.
- **Olvidar la rotación de certificados/secretos**.
- **Despliegues manuales** entre entornos.
- **Cuotas y rate limits inexistentes** o demasiado laxos.

---

## 18. Checklist de Go-Live

- [ ] SKU adecuada (Premium con AZ o multi‑región para crítico).
- [ ] VNet/Private Endpoint configurado según política de red.
- [ ] WAF (App Gateway / Front Door) delante si está expuesto a internet.
- [ ] Identidad administrada habilitada y permisos a Key Vault.
- [ ] Políticas globales: CORS, logging, headers, *rate limit* base.
- [ ] OAuth/OIDC con Azure AD o mecanismo equivalente.
- [ ] Validación de JWT con `audience` e `issuer` correctos.
- [ ] Caching definido para endpoints idempotentes.
- [ ] Rate limit y quotas por producto/suscripción.
- [ ] Monitorización con App Insights y Log Analytics.
- [ ] Alertas configuradas (capacity, errores, latencia, certificados).
- [ ] Backups/IaC en Git (Bicep/Terraform/APIOps).
- [ ] Pruebas de carga ejecutadas y documentadas.
- [ ] Plan de DR (multi‑región) probado.
- [ ] Runbooks operativos publicados.
- [ ] Owners y RACI definidos por API/producto.
- [ ] Developer Portal personalizado y publicado.
- [ ] Política de deprecación y comunicación a consumidores.

---

## 19. Recursos oficiales

- Documentación: <https://learn.microsoft.com/azure/api-management/>
- Referencia de políticas: <https://learn.microsoft.com/azure/api-management/api-management-policies>
- DevOps Resource Kit: <https://github.com/Azure/azure-api-management-devops-resource-kit>
- APIOps: <https://github.com/Azure/apiops>
- Landing Zone Accelerator (APIM): <https://learn.microsoft.com/azure/architecture/example-scenario/integration/app-gateway-internal-api-management-function>
- Azure API Center (catálogo de APIs): <https://learn.microsoft.com/azure/api-center/>
- Defender for APIs: <https://learn.microsoft.com/azure/defender-for-cloud/defender-for-apis-introduction>

---

> **Nota final**: Azure API Management es una pieza clave de la estrategia de integración. Bien gobernado, simplifica el ciclo de vida de las APIs, mejora la seguridad y acelera el time‑to‑market. Mal gobernado, se convierte en un cuello de botella y un riesgo. Sigue las prácticas de este manual y revisa periódicamente la documentación oficial, ya que el servicio evoluciona con frecuencia.
