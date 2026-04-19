# Manual de Azure API Management — Uso y Mejores Prácticas

## Tabla de Contenido

1. [Introducción](#1-introducción)
2. [¿Qué es Azure API Management?](#2-qué-es-azure-api-management)
3. [Arquitectura y Componentes](#3-arquitectura-y-componentes)
4. [Niveles de Servicio (Tiers)](#4-niveles-de-servicio-tiers)
5. [Creación de una Instancia de APIM](#5-creación-de-una-instancia-de-apim)
6. [Importación y Publicación de APIs](#6-importación-y-publicación-de-apis)
7. [Productos y Suscripciones](#7-productos-y-suscripciones)
8. [Políticas (Policies)](#8-políticas-policies)
   - 8.1 [Estructura de una Política](#81-estructura-de-una-política)
   - 8.2 [Políticas de Restricción de Acceso](#82-políticas-de-restricción-de-acceso)
   - 8.3 [Rate Limiting y Throttling](#83-rate-limiting-y-throttling)
   - 8.4 [Transformación de Solicitudes y Respuestas](#84-transformación-de-solicitudes-y-respuestas)
   - 8.5 [Caché de Respuestas](#85-caché-de-respuestas)
   - 8.6 [Validación de JWT / OAuth 2.0](#86-validación-de-jwt--oauth-20)
   - 8.7 [Políticas para LLMs / Azure OpenAI](#87-políticas-para-llms--azure-openai)
9. [Seguridad — Mejores Prácticas](#9-seguridad--mejores-prácticas)
10. [Versionado y Revisiones de APIs](#10-versionado-y-revisiones-de-apis)
11. [Portal del Desarrollador](#11-portal-del-desarrollador)
12. [Monitoreo y Observabilidad](#12-monitoreo-y-observabilidad)
13. [Redes y Aislamiento de Red](#13-redes-y-aislamiento-de-red)
14. [Alta Disponibilidad y Recuperación ante Desastres](#14-alta-disponibilidad-y-recuperación-ante-desastres)
15. [CI/CD y APIOps](#15-cicd-y-apiops)
16. [Optimización de Costos](#16-optimización-de-costos)
17. [Patrones de Arquitectura Comunes](#17-patrones-de-arquitectura-comunes)
18. [Checklist de Mejores Prácticas](#18-checklist-de-mejores-prácticas)
19. [Referencias](#19-referencias)

---

## 1. Introducción

Este manual está diseñado como una guía completa para el uso y las mejores prácticas de **Azure API Management (APIM)**. Cubre desde los conceptos fundamentales hasta patrones avanzados de arquitectura empresarial, políticas de seguridad, gobernanza de APIs y optimización de costos.

**Audiencia:** Arquitectos de soluciones, desarrolladores backend, ingenieros DevOps y equipos de plataforma que gestionan APIs en Azure.

---

## 2. ¿Qué es Azure API Management?

Azure API Management es una plataforma como servicio (PaaS) que actúa como **puerta de enlace (gateway) y plataforma de gestión** para APIs en todos los entornos, incluyendo híbridos y multi-nube.

### Capacidades Principales

| Capacidad | Descripción |
|---|---|
| **Gateway de API** | Proxy inverso que enruta solicitudes de clientes a servicios backend. Aplica políticas de seguridad, transformación y control de tráfico. |
| **Plano de Gestión** | Interfaz para configurar APIs, productos, usuarios, políticas y analíticas (Azure Portal, CLI, ARM, Bicep, Terraform). |
| **Portal del Desarrollador** | Portal auto-hospedado y personalizable donde los consumidores descubren, prueban y se suscriben a APIs. |
| **Analíticas** | Métricas de uso, latencia, errores y tráfico integradas con Azure Monitor y Application Insights. |

### Casos de Uso Típicos

- Exponer microservicios como APIs unificadas.
- Modernizar APIs legadas (SOAP → REST).
- Implementar fachadas de API (API Facade) para simplificar contratos.
- Gobernar el acceso de consumidores externos e internos.
- Actuar como gateway para servicios de IA/LLM (Azure OpenAI).

---

## 3. Arquitectura y Componentes

```
┌─────────────────────────────────────────────────────────────────────┐
│                        CLIENTES / CONSUMIDORES                      │
│          (Apps móviles, SPAs, servicios B2B, agentes IA)            │
└──────────────────────────────┬──────────────────────────────────────┘
                               │  HTTPS
                               ▼
┌─────────────────────────────────────────────────────────────────────┐
│                     AZURE API MANAGEMENT                            │
│                                                                     │
│  ┌────────────┐  ┌────────────────────┐  ┌───────────────────────┐  │
│  │  Gateway    │  │  Plano de Gestión  │  │  Portal del           │  │
│  │  (Data      │  │  (Control Plane)   │  │  Desarrollador        │  │
│  │   Plane)    │  │                    │  │                       │  │
│  └──────┬─────┘  └────────────────────┘  └───────────────────────┘  │
│         │                                                           │
│  ┌──────┴──────────────────────────────────────────────────────┐    │
│  │  POLÍTICAS (Inbound → Backend → Outbound → On-Error)       │    │
│  └─────────────────────────────────────────────────────────────┘    │
└──────────────────────────────┬──────────────────────────────────────┘
                               │
                               ▼
┌─────────────────────────────────────────────────────────────────────┐
│                     SERVICIOS BACKEND                                │
│  (Azure Functions, App Service, AKS, VMs, APIs externas, OpenAI)   │
└─────────────────────────────────────────────────────────────────────┘
```

### Componentes Clave

- **Gateway (Data Plane):** Recibe todas las solicitudes de API, aplica políticas y las reenvía al backend. Es el componente crítico de rendimiento.
- **Control Plane:** API de Azure Resource Manager (ARM) para gestionar la configuración. Nunca usar la API de gestión directa (deprecada).
- **Self-Hosted Gateway:** Contenedor Docker que extiende el gateway a entornos on-premises o multi-nube.

---

## 4. Niveles de Servicio (Tiers)

| Tier | Uso recomendado | Zona de disponibilidad | VNet | SLA |
|---|---|---|---|---|
| **Consumption** | Cargas serverless, bajo tráfico | No | No | 99.95% |
| **Developer** | Solo desarrollo y pruebas | No | Sí | Sin SLA |
| **Basic** | Cargas de trabajo de desarrollo/pruebas básicas | No | No | 99.95% |
| **Basic v2** | Cargas de trabajo de producción ligeras | No | Inyección VNet | 99.95% |
| **Standard** | Producción de volumen medio | No | No | 99.95% |
| **Standard v2** | Producción recomendada | No | Inyección VNet | 99.95% |
| **Premium** | Producción empresarial, multi-región | Sí | Sí (completo) | 99.95% / 99.99% |
| **Premium v2** | Producción empresarial de nueva generación | Sí | Sí (completo) | 99.95% / 99.99% |

### Recomendación

> **Para producción empresarial**, use **Standard v2**, **Premium** o **Premium v2**. El tier Developer no tiene SLA y nunca debe usarse para producción.

---

## 5. Creación de una Instancia de APIM

### Usando Azure CLI

```bash
# Crear grupo de recursos
az group create \
  --name rg-apim-produccion \
  --location eastus2

# Crear instancia de APIM (Standard v2)
az apim create \
  --name mi-apim-empresa \
  --resource-group rg-apim-produccion \
  --publisher-name "Mi Empresa S.A." \
  --publisher-email admin@miempresa.com \
  --sku-name StandardV2 \
  --sku-capacity 1 \
  --location eastus2
```

### Usando Bicep (IaC)

```bicep
resource apim 'Microsoft.ApiManagement/service@2023-09-01-preview' = {
  name: 'mi-apim-empresa'
  location: 'eastus2'
  sku: {
    name: 'StandardV2'
    capacity: 1
  }
  properties: {
    publisherEmail: 'admin@miempresa.com'
    publisherName: 'Mi Empresa S.A.'
  }
}
```

### Tiempos de Aprovisionamiento

| Tier | Tiempo aproximado |
|---|---|
| Consumption | 1–2 minutos |
| Standard v2 / Basic v2 | 5–10 minutos |
| Premium / Premium v2 | 30–60 minutos |
| Classic (Developer, Basic, Standard) | 30–45 minutos |

---

## 6. Importación y Publicación de APIs

### Formatos Soportados para Importación

- **OpenAPI / Swagger** (JSON o YAML) — el más recomendado.
- **WSDL** (para servicios SOAP).
- **WADL**.
- **Azure Functions**, **App Service**, **Logic Apps** — importación directa desde Azure.
- **GraphQL** — passthrough o sintético.

### Ejemplo: Importar una API desde OpenAPI

```bash
az apim api import \
  --resource-group rg-apim-produccion \
  --service-name mi-apim-empresa \
  --api-id mi-api-pedidos \
  --path pedidos \
  --specification-format OpenApi \
  --specification-url "https://mi-backend.azurewebsites.net/swagger/v1/swagger.json" \
  --display-name "API de Pedidos" \
  --protocols https
```

### Operaciones Wildcard

Para APIs con gran cantidad de endpoints (> 100 operaciones), se puede utilizar un enfoque wildcard:

```xml
<!-- En lugar de registrar cada operación individualmente -->
<api name="mi-api" path="api">
  <!-- Operación wildcard que captura todas las rutas -->
  <!-- Método: ANY, URL Template: /{*path} -->
</api>
```

Luego, aplique políticas de `rewrite-uri` para enrutar dinámicamente al backend correspondiente.

> **Nota:** Azure APIM tiene un límite de **100 operaciones por API**. Use segmentación de APIs y operaciones wildcard para manejar APIs con más endpoints.

---

## 7. Productos y Suscripciones

### Conceptos

- **Producto:** Agrupación lógica de una o más APIs. Los consumidores se suscriben a productos, no a APIs individuales.
- **Suscripción:** Clave de acceso que un consumidor obtiene al suscribirse a un producto. APIM valida estas claves automáticamente.

### Diseño Recomendado de Productos

| Producto | APIs incluidas | Aprobación | Rate Limit |
|---|---|---|---|
| **Free** | APIs públicas de solo lectura | Automática | 10 req/min |
| **Standard** | APIs principales CRUD | Manual | 100 req/min |
| **Premium** | Todas las APIs + tiempo real | Manual | 1000 req/min |
| **Internal** | APIs internas (sin portal) | Solo admin | Sin límite |

### Crear un Producto

```bash
az apim product create \
  --resource-group rg-apim-produccion \
  --service-name mi-apim-empresa \
  --product-id producto-standard \
  --display-name "Plan Standard" \
  --description "Acceso a APIs principales con límite de 100 req/min" \
  --subscription-required true \
  --approval-required true \
  --state published
```

---

## 8. Políticas (Policies)

Las políticas son el corazón de APIM. Son declaraciones XML que se ejecutan en el pipeline de procesamiento de cada solicitud.

### 8.1 Estructura de una Política

```xml
<policies>
    <inbound>
        <!-- Se ejecutan ANTES de enviar la solicitud al backend -->
        <base />
        <!-- Políticas de autenticación, rate limiting, transformación -->
    </inbound>
    <backend>
        <!-- Configuración del reenvío al servicio backend -->
        <base />
    </backend>
    <outbound>
        <!-- Se ejecutan DESPUÉS de recibir la respuesta del backend -->
        <base />
        <!-- Transformación de respuestas, headers adicionales -->
    </outbound>
    <on-error>
        <!-- Se ejecuta cuando ocurre un error en cualquier sección -->
        <base />
        <!-- Logging, respuestas de error personalizadas -->
    </on-error>
</policies>
```

> **`<base />`** hereda las políticas del ámbito padre (global → producto → API → operación). Siempre inclúyala a menos que tenga una razón específica para no hacerlo.

### 8.2 Políticas de Restricción de Acceso

#### Filtrar por IP

```xml
<inbound>
    <base />
    <ip-filter action="allow">
        <address-range from="10.0.0.0" to="10.0.0.255" />
        <address>203.0.113.50</address>
    </ip-filter>
</inbound>
```

#### Requerir Suscripción con Header Personalizado

```xml
<inbound>
    <base />
    <check-header name="X-API-Key" failed-check-httpcode="401"
                  failed-check-error-message="Clave de API requerida" />
</inbound>
```

#### Restricción por CORS

```xml
<inbound>
    <base />
    <cors allow-credentials="true">
        <allowed-origins>
            <origin>https://mi-app.com</origin>
            <origin>https://portal.miempresa.com</origin>
        </allowed-origins>
        <allowed-methods preflight-result-max-age="300">
            <method>GET</method>
            <method>POST</method>
            <method>PUT</method>
            <method>DELETE</method>
            <method>OPTIONS</method>
        </allowed-methods>
        <allowed-headers>
            <header>Content-Type</header>
            <header>Authorization</header>
            <header>X-API-Key</header>
        </allowed-headers>
    </cors>
</inbound>
```

### 8.3 Rate Limiting y Throttling

#### Rate Limit por Suscripción

```xml
<inbound>
    <base />
    <!-- Máximo 100 llamadas por cada 60 segundos por suscripción -->
    <rate-limit calls="100" renewal-period="60"
        remaining-calls-header-name="X-RateLimit-Remaining"
        remaining-calls-variable-name="remainingCalls" />
</inbound>
```

#### Rate Limit por Clave Personalizada (IP)

```xml
<inbound>
    <base />
    <!-- 50 llamadas por minuto por dirección IP -->
    <rate-limit-by-key
        calls="50"
        renewal-period="60"
        counter-key="@(context.Request.IpAddress)"
        remaining-calls-header-name="X-RateLimit-Remaining" />
</inbound>
```

#### Rate Limit por Claim de JWT (Usuario)

```xml
<inbound>
    <base />
    <rate-limit-by-key
        calls="30"
        renewal-period="60"
        counter-key="@(context.Request.Headers.GetValueOrDefault("Authorization","").AsJwt()?.Claims["email"].FirstOrDefault())" />
</inbound>
```

#### Combinar Rate Limit + Cuota Diaria

```xml
<inbound>
    <base />
    <!-- Ráfaga: máximo 100 llamadas por minuto -->
    <rate-limit calls="100" renewal-period="60" />
    <!-- Cuota diaria: máximo 10,000 llamadas por día -->
    <quota calls="10000" renewal-period="86400" />
</inbound>
```

#### Rate Limit Diferenciado por Tier de Producto

```xml
<inbound>
    <base />
    <choose>
        <when condition="@(context.Product.Name == "Premium")">
            <rate-limit-by-key calls="1000" renewal-period="60"
                counter-key="@(context.Subscription.Id)" />
        </when>
        <when condition="@(context.Product.Name == "Standard")">
            <rate-limit-by-key calls="100" renewal-period="60"
                counter-key="@(context.Subscription.Id)" />
        </when>
        <otherwise>
            <rate-limit-by-key calls="10" renewal-period="60"
                counter-key="@(context.Subscription.Id)" />
        </otherwise>
    </choose>
</inbound>
```

### 8.4 Transformación de Solicitudes y Respuestas

#### Agregar/Modificar Headers

```xml
<inbound>
    <base />
    <!-- Agregar header de correlación -->
    <set-header name="X-Correlation-Id" exists-action="skip">
        <value>@(Guid.NewGuid().ToString())</value>
    </set-header>
    <!-- Pasar identidad del consumidor al backend -->
    <set-header name="X-Consumer-Id" exists-action="override">
        <value>@(context.Subscription.Id)</value>
    </set-header>
</inbound>

<outbound>
    <base />
    <!-- Remover headers internos del backend -->
    <set-header name="X-Powered-By" exists-action="delete" />
    <set-header name="Server" exists-action="delete" />
</outbound>
```

#### Reescribir URL del Backend

```xml
<inbound>
    <base />
    <rewrite-uri template="/api/v2/{path}" />
</inbound>
```

#### Transformar Body (JSON → JSON)

```xml
<outbound>
    <base />
    <set-body>
    @{
        var response = context.Response.Body.As<JObject>();
        return new JObject(
            new JProperty("datos", response["data"]),
            new JProperty("total", response["count"]),
            new JProperty("pagina", context.Request.OriginalUrl.Query.GetValueOrDefault("page", "1"))
        ).ToString();
    }
    </set-body>
</outbound>
```

### 8.5 Caché de Respuestas

#### Caché Simple

```xml
<inbound>
    <base />
    <!-- Cachear respuestas por 300 segundos (5 minutos) -->
    <cache-lookup vary-by-developer="false"
                  vary-by-developer-groups="false"
                  downstream-caching-type="none" />
</inbound>
<outbound>
    <base />
    <cache-store duration="300" />
</outbound>
```

#### Caché con Redis Externo

```xml
<inbound>
    <base />
    <cache-lookup-value key="@("response-" + context.Request.Url.Path)"
                        variable-name="cachedResponse" />
    <choose>
        <when condition="@(context.Variables.ContainsKey("cachedResponse"))">
            <return-response>
                <set-status code="200" reason="OK" />
                <set-header name="Content-Type" exists-action="override">
                    <value>application/json</value>
                </set-header>
                <set-body>@((string)context.Variables["cachedResponse"])</set-body>
            </return-response>
        </when>
    </choose>
</inbound>
<outbound>
    <base />
    <cache-store-value key="@("response-" + context.Request.Url.Path)"
                       value="@(context.Response.Body.As<string>(preserveContent: true))"
                       duration="300" />
</outbound>
```

> **Mejor práctica:** Implementar caché en el nivel de APIM puede descargar hasta un **60% del tráfico** de los servicios backend.

### 8.6 Validación de JWT / OAuth 2.0

#### Validar Token de Azure AD (Entra ID)

```xml
<inbound>
    <base />
    <validate-jwt header-name="Authorization"
                  failed-validation-httpcode="401"
                  failed-validation-error-message="Token inválido o expirado"
                  require-expiration-time="true"
                  require-signed-tokens="true">
        <openid-config url="https://login.microsoftonline.com/{tenant-id}/v2.0/.well-known/openid-configuration" />
        <required-claims>
            <claim name="aud">
                <value>{client-id-de-la-api}</value>
            </claim>
            <claim name="roles" match="any">
                <value>API.Read</value>
                <value>API.Write</value>
            </claim>
        </required-claims>
    </validate-jwt>
</inbound>
```

#### Validar JWT con Clave Simétrica

```xml
<inbound>
    <base />
    <validate-jwt header-name="Authorization"
                  failed-validation-httpcode="401">
        <issuer-signing-keys>
            <key>{{jwt-signing-key}}</key>
        </issuer-signing-keys>
        <issuers>
            <issuer>https://mi-idp.com</issuer>
        </issuers>
        <audiences>
            <audience>https://mi-api.miempresa.com</audience>
        </audiences>
    </validate-jwt>
</inbound>
```

> **`{{jwt-signing-key}}`** es un **Named Value** que debe almacenarse en **Azure Key Vault**, nunca en texto plano en la política.

### 8.7 Políticas para LLMs / Azure OpenAI

Con el auge de los modelos de lenguaje, APIM incluye políticas específicas:

#### Limitar Tokens de Azure OpenAI

```xml
<inbound>
    <base />
    <azure-openai-token-limit
        counter-key="@(context.Subscription.Id)"
        tokens-per-minute="10000"
        estimate-prompt-tokens="true"
        remaining-tokens-header-name="X-Remaining-Tokens" />
</inbound>
```

#### Limitar Tokens de LLM Genérico

```xml
<inbound>
    <base />
    <llm-token-limit
        counter-key="@(context.Request.IpAddress)"
        tokens-per-minute="5000"
        estimate-prompt-tokens="true" />
</inbound>
```

#### Verificación de Seguridad de Contenido

```xml
<inbound>
    <base />
    <azure-openai-content-safety
        backend-id="content-safety-backend" />
</inbound>
```

---

## 9. Seguridad — Mejores Prácticas

### 9.1 Checklist de Seguridad Empresarial

| Control de Seguridad | Implementación Recomendada |
|---|---|
| **Gestión de Secretos** | Integrar con Azure Key Vault para certificados, claves y Named Values. Nunca almacenar secretos en las políticas. |
| **Identidad** | OAuth 2.0 / JWT obligatorio para todas las APIs externas. Usar Managed Identity para la comunicación APIM ↔ Backend. |
| **Aislamiento de Red** | Desplegar APIM en modo VNet Interno (tier Premium). Usar NSGs + Azure Firewall + WAF (Application Gateway). |
| **TLS** | Exigir TLS 1.2+ en todas las conexiones. Usar certificados de cliente (mTLS) para backends críticos. |
| **API de Gestión Directa** | **Deshabilitar** la API de gestión directa (deprecada). Usar Azure Resource Manager como plano de control. |
| **Subscription Keys** | Nunca enviar claves en la URL (query string). Usar headers (`Ocp-Apim-Subscription-Key`). |
| **CORS** | Configurar orígenes permitidos explícitamente. Nunca usar `*` en producción. |
| **Validación de Entrada** | Usar políticas `validate-content` para validar el body contra el schema de la API. |
| **Headers de Seguridad** | Remover headers internos (`Server`, `X-Powered-By`) en el outbound. Agregar `X-Content-Type-Options: nosniff`. |

### 9.2 Autenticación del Backend con Managed Identity

```xml
<inbound>
    <base />
    <authentication-managed-identity resource="https://mi-backend.azurewebsites.net" />
</inbound>
```

### 9.3 Certificados de Cliente (mTLS)

```xml
<inbound>
    <base />
    <choose>
        <when condition="@(context.Request.Certificate == null ||
            context.Request.Certificate.Thumbprint != "EXPECTED_THUMBPRINT")">
            <return-response>
                <set-status code="403" reason="Certificado inválido" />
            </return-response>
        </when>
    </choose>
</inbound>
```

---

## 10. Versionado y Revisiones de APIs

### Estrategia de Versionado

APIM soporta tres esquemas de versionado:

| Esquema | Ejemplo | Uso recomendado |
|---|---|---|
| **Path** | `/v1/pedidos`, `/v2/pedidos` | El más común y recomendado. Claro y explícito. |
| **Header** | `Api-Version: 2024-01-01` | Cuando no se quiere modificar la URL. |
| **Query String** | `/pedidos?api-version=v1` | Menos recomendado, pero soportado. |

### Versionado vs. Revisiones

| Concepto | Propósito | Visibilidad |
|---|---|---|
| **Versión** | Cambios **breaking** (nueva interfaz, contrato diferente). | Cada versión es una API distinta para el consumidor. |
| **Revisión** | Cambios **no-breaking** (bug fixes, nueva política). | El consumidor no lo nota; se puede hacer revisión actual sin afectar la producción. |

### Flujo Recomendado

1. **Desarrollo:** Crear una nueva **revisión** para probar cambios.
2. **Pruebas:** Probar la revisión con un subset de tráfico (si aplica).
3. **Promoción:** Hacer la revisión como "actual" cuando sea estable.
4. **Cambio breaking:** Crear una nueva **versión** y mantener la anterior con una fecha de deprecación.

---

## 11. Portal del Desarrollador

El portal del desarrollador es un sitio web completamente personalizable donde los consumidores de APIs pueden:

- Descubrir y explorar APIs disponibles.
- Leer documentación interactiva (tipo Swagger UI).
- Registrarse, obtener claves de suscripción y probar APIs.
- Ver analíticas de uso de sus suscripciones.

### Mejores Prácticas del Portal

- **Personalizar el branding:** Logo, colores corporativos, dominio personalizado.
- **Documentar exhaustivamente:** Cada API debe tener descripción, ejemplos de uso y códigos de error.
- **Publicar Change Logs:** Informar a los consumidores sobre cambios en las APIs.
- **Habilitar autenticación con Azure AD:** Para controlar quién accede al portal.
- **No exponer el portal al público** si las APIs son solo internas.

---

## 12. Monitoreo y Observabilidad

### Integración con Application Insights

```bash
az apim update \
  --name mi-apim-empresa \
  --resource-group rg-apim-produccion \
  --set properties.customProperties."Microsoft.WindowsAzure.ApiManagement.Gateway.Reporting.Backend.ApplicationInsights.InstrumentationKey"="<INSTRUMENTATION_KEY>"
```

### Política de Diagnóstico Personalizada

```xml
<inbound>
    <base />
    <set-variable name="requestTime" value="@(DateTime.UtcNow)" />
</inbound>
<outbound>
    <base />
    <set-variable name="responseTime" value="@(DateTime.UtcNow)" />
    <trace source="api-diagnostics" severity="information">
        <message>
            @{
                var requestTime = (DateTime)context.Variables["requestTime"];
                var responseTime = (DateTime)context.Variables["responseTime"];
                var duration = (responseTime - requestTime).TotalMilliseconds;
                return $"API: {context.Api.Name}, Operation: {context.Operation.Name}, " +
                       $"Duration: {duration}ms, Status: {context.Response.StatusCode}, " +
                       $"Subscription: {context.Subscription.Id}";
            }
        </message>
    </trace>
</outbound>
```

### Métricas Clave a Monitorear

| Métrica | Descripción | Umbral de Alerta Sugerido |
|---|---|---|
| **Latencia del Gateway** | Tiempo total desde que APIM recibe la solicitud hasta que envía la respuesta. | > 2 segundos |
| **Latencia del Backend** | Tiempo que tarda el backend en responder. | > 1.5 segundos |
| **Tasa de Errores 4xx** | Porcentaje de respuestas 4xx. | > 10% |
| **Tasa de Errores 5xx** | Porcentaje de respuestas 5xx. | > 1% |
| **Solicitudes Totales** | Volumen de tráfico por minuto. | Dependiente del negocio |
| **Solicitudes Bloqueadas (429)** | Rate limiting activado. | > 5% del total |
| **Uso de Capacidad** | Porcentaje de capacidad del gateway. | > 70% |

---

## 13. Redes y Aislamiento de Red

### Modos de Conectividad de Red

| Modo | Descripción | Tiers Soportados |
|---|---|---|
| **Sin VNet** | APIM expuesto en Internet público. Backend accesible por Internet. | Todos |
| **VNet Externo** | Gateway accesible desde Internet. Backend accesible vía VNet. | Premium, Premium v2 |
| **VNet Interno** | Gateway accesible solo dentro de la VNet. Se requiere Application Gateway o Front Door para exposición externa. | Premium, Premium v2 |
| **VNet Injection (v2)** | Inyección de VNet para tiers v2, con conectividad privada al backend. | Standard v2, Basic v2 |

### Arquitectura Recomendada (Producción Empresarial)

```
Internet
    │
    ▼
┌──────────────────┐
│  Azure Front Door │  ← WAF + CDN + Global Load Balancing
│  o App Gateway    │
└────────┬─────────┘
         │ (Private Link / VNet)
         ▼
┌──────────────────┐
│  APIM (VNet       │  ← Modo Interno
│  Interno)         │
└────────┬─────────┘
         │ (Private Endpoints / VNet)
         ▼
┌──────────────────┐
│  Backends (AKS,  │
│  App Service,    │
│  Functions)      │
└──────────────────┘
```

### Configuración de NSG para APIM

Puertos que deben estar abiertos:

| Dirección | Puerto | Protocolo | Propósito |
|---|---|---|---|
| Inbound | 443 | TCP | Tráfico de API (HTTPS) |
| Inbound | 3443 | TCP | Plano de gestión (requerido por Azure) |
| Outbound | 443 | TCP | Dependencias de Azure (Storage, SQL, Key Vault) |
| Outbound | 1433 | TCP | Azure SQL (configuración interna) |
| Outbound | 5671, 5672, 443 | TCP | Event Hub (logging) |

---

## 14. Alta Disponibilidad y Recuperación ante Desastres

### Zonas de Disponibilidad

Disponible en los tiers **Premium** y **Premium v2**:

```bash
# Habilitar redundancia de zona al crear la instancia
az apim create \
  --name mi-apim-ha \
  --resource-group rg-apim-produccion \
  --publisher-name "Mi Empresa S.A." \
  --publisher-email admin@miempresa.com \
  --sku-name Premium \
  --sku-capacity 2 \
  --location eastus2 \
  --zones 1 2
```

> **Mejor práctica:** Desplegar al menos **2 unidades** con redundancia de zona para garantizar resiliencia ante la caída de un datacenter dentro de la región.

### Despliegue Multi-Región

```bash
# Agregar una ubicación secundaria
az apim update \
  --name mi-apim-ha \
  --resource-group rg-apim-produccion \
  --set additionalLocations[0].location=westus2 \
  --set additionalLocations[0].sku.name=Premium \
  --set additionalLocations[0].sku.capacity=1 \
  --set additionalLocations[0].zones="[1,2]"
```

### Estrategia de DR

| Componente | Estrategia |
|---|---|
| **Configuración** | Backup/restore automatizado vía ARM/Bicep en repositorio git (APIOps). |
| **Suscripciones de usuario** | Replicar manualmente o con scripts de automatización. |
| **Gateway** | Multi-región activo-activo con Azure Traffic Manager o Front Door. |
| **Certificados y secretos** | Almacenar en Key Vault con replicación geográfica. |

---

## 15. CI/CD y APIOps

### ¿Qué es APIOps?

APIOps aplica los principios de DevOps/GitOps a la gestión de APIs. Toda la configuración de APIM (APIs, productos, políticas, Named Values) se almacena como código y se despliega mediante pipelines.

### Pipeline Recomendado

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│  Repositorio │ →  │  Build &    │ →  │  Deploy a   │ →  │  Deploy a   │
│  Git (IaC)   │    │  Validate   │    │  APIM Dev   │    │  APIM Prod  │
└─────────────┘    └─────────────┘    └─────────────┘    └─────────────┘
       │                 │                  │                   │
   OpenAPI specs   Lint de specs       Pruebas auto        Aprobación
   Políticas XML   Validar XML         Test funcional      manual + gate
   Bicep/ARM       Bicep build         Test de carga       Blue/Green
```

### Ejemplo: GitHub Actions

```yaml
name: Deploy APIM APIs

on:
  push:
    branches: [main]
    paths:
      - 'apim/**'

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Login to Azure
        uses: azure/login@v2
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}

      - name: Validate OpenAPI Spec
        run: |
          npx @redocly/cli lint apim/specs/*.yaml

      - name: Deploy APIM Configuration
        uses: azure/arm-deploy@v2
        with:
          resourceGroupName: rg-apim-produccion
          template: apim/main.bicep
          parameters: apim/parameters.prod.json
```

### Herramientas de APIOps

- **Azure API Management DevOps Resource Kit** — toolkit oficial de Microsoft.
- **Extractor:** Exporta configuración existente de APIM a plantillas ARM.
- **Creator:** Genera plantillas ARM a partir de archivos de configuración.
- **Redocly / Spectral:** Linting de especificaciones OpenAPI.

---

## 16. Optimización de Costos

### Estrategias de Reducción de Costos

| Estrategia | Ahorro Estimado | Complejidad |
|---|---|---|
| **Caché de respuestas** | 30–60% menos llamadas al backend | Baja |
| **Tier adecuado** | Variable | Baja |
| **Políticas ligeras** | Menor consumo de unidades | Media |
| **Consolidar instancias** | Evitar instancias duplicadas | Media |
| **Apagar Dev/QA fuera de horario** | 50% en entornos no-prod | Baja |
| **Consumption tier para APIs de bajo tráfico** | Pago por uso | Baja |

### Modelo de Costos por Tier

| Tier | Modelo de Precio |
|---|---|
| **Consumption** | Pago por llamada (primeras 1M gratis/mes). |
| **Developer** | Precio fijo mensual por unidad (sin SLA). |
| **Basic / Standard** | Precio fijo mensual por unidad. |
| **Basic v2 / Standard v2** | Precio fijo mensual por unidad. |
| **Premium / Premium v2** | Precio fijo mensual por unidad (incluye VNet, zonas, multi-región). |

> **Usar la [Calculadora de Precios de Azure](https://azure.microsoft.com/pricing/calculator/)** para estimar costos con la cuenta de su organización.

---

## 17. Patrones de Arquitectura Comunes

### 17.1 API Gateway Pattern

APIM actúa como punto de entrada único para todos los microservicios:

```
Clientes → APIM → Microservicio A
                 → Microservicio B
                 → Microservicio C
```

### 17.2 Backend for Frontend (BFF)

Diferentes productos/APIs de APIM para diferentes clientes:

```
App Móvil   → APIM (Producto Móvil)   → API Móvil Backend
App Web     → APIM (Producto Web)     → API Web Backend
Partners B2B → APIM (Producto B2B)    → API B2B Backend
```

### 17.3 API Facade (Strangler Fig)

Para modernización progresiva de sistemas legacy:

```xml
<inbound>
    <base />
    <choose>
        <!-- Rutas ya migradas van al nuevo servicio -->
        <when condition="@(context.Request.Url.Path.StartsWith("/api/v2/pedidos"))">
            <set-backend-service base-url="https://nuevo-servicio.azurewebsites.net" />
        </when>
        <!-- Rutas legacy van al sistema viejo -->
        <otherwise>
            <set-backend-service base-url="https://legacy-system.onprem.empresa.com" />
        </otherwise>
    </choose>
</inbound>
```

### 17.4 Agregador de APIs

Combinar respuestas de múltiples backends en una sola respuesta:

```xml
<inbound>
    <base />
    <send-request mode="new" response-variable-name="perfil" timeout="10">
        <set-url>https://api-usuarios.internal/perfil/@(context.Request.MatchedParameters["userId"])</set-url>
        <set-method>GET</set-method>
    </send-request>
    <send-request mode="new" response-variable-name="pedidos" timeout="10">
        <set-url>https://api-pedidos.internal/usuario/@(context.Request.MatchedParameters["userId"])/pedidos</set-url>
        <set-method>GET</set-method>
    </send-request>
    <return-response>
        <set-status code="200" reason="OK" />
        <set-header name="Content-Type" exists-action="override">
            <value>application/json</value>
        </set-header>
        <set-body>
        @{
            var perfil = ((IResponse)context.Variables["perfil"]).Body.As<JObject>();
            var pedidos = ((IResponse)context.Variables["pedidos"]).Body.As<JArray>();
            return new JObject(
                new JProperty("perfil", perfil),
                new JProperty("pedidos", pedidos)
            ).ToString();
        }
        </set-body>
    </return-response>
</inbound>
```

### 17.5 AI Gateway (Azure OpenAI)

APIM como gateway para servicios de IA con control de tokens y costos:

```xml
<inbound>
    <base />
    <validate-jwt header-name="Authorization" ... />
    <azure-openai-token-limit
        counter-key="@(context.Subscription.Id)"
        tokens-per-minute="20000"
        estimate-prompt-tokens="true" />
    <set-backend-service backend-id="openai-backend" />
</inbound>
```

---

## 18. Checklist de Mejores Prácticas

### Diseño y Gobernanza

- [ ] Adoptar enfoque **Design-First** con OpenAPI Specification antes de codificar.
- [ ] Definir una estrategia clara de **versionado** (preferir path-based: `/v1/`, `/v2/`).
- [ ] Usar **revisiones** para cambios no-breaking antes de promoverlos a producción.
- [ ] Registrar todas las APIs en el **Portal del Desarrollador** con documentación completa.
- [ ] Implementar un **catálogo centralizado** (considerar Azure API Center).
- [ ] Establecer estándares de nomenclatura y convenciones para APIs, productos y operaciones.

### Seguridad

- [ ] Validar JWT/OAuth 2.0 en **todas las APIs externas** con `validate-jwt`.
- [ ] Usar **Managed Identity** para autenticación entre APIM y backends.
- [ ] Almacenar secretos en **Azure Key Vault**, nunca en texto plano.
- [ ] Desplegar en **modo VNet Interno** con Application Gateway/Front Door delante.
- [ ] **Deshabilitar** la API de gestión directa (legacy).
- [ ] Exigir **TLS 1.2+** en todas las conexiones.
- [ ] Implementar **CORS** con orígenes explícitos (nunca `*`).
- [ ] Usar `validate-content` para validar schemas de entrada.
- [ ] Remover headers internos del backend en el outbound.

### Rendimiento

- [ ] Implementar **caché de respuestas** para endpoints de lectura frecuente.
- [ ] Mantener **políticas ligeras** (evitar lógica condicional compleja).
- [ ] Usar **operaciones wildcard** para APIs con muchos endpoints.
- [ ] Delegar **lógica de enrutamiento compleja** al backend, no a las políticas.
- [ ] Monitorear la **capacidad del gateway** y escalar proactivamente.

### Confiabilidad

- [ ] Habilitar **redundancia de zona** (Premium/Premium v2) con al menos 2 unidades.
- [ ] Considerar despliegue **multi-región** para latencia global y DR.
- [ ] Implementar **retry policies** para backends inestables.
- [ ] Configurar **circuit breaker** con la política `retry` y `set-backend-service`.
- [ ] Realizar **backup/restore** periódico de la configuración.

### Operaciones

- [ ] Implementar **APIOps** (CI/CD para configuración de APIM).
- [ ] Integrar con **Application Insights** para trazas distribuidas.
- [ ] Configurar **alertas** en métricas clave (latencia, errores 5xx, capacidad).
- [ ] Usar **Azure Monitor** y **Log Analytics** para auditoría y análisis.
- [ ] Automatizar **pruebas de API** (funcionales, carga, seguridad) en el pipeline.

### Costos

- [ ] Seleccionar el **tier adecuado** según necesidades reales.
- [ ] Usar **Consumption tier** para APIs de bajo tráfico o serverless.
- [ ] Implementar **caché** para reducir llamadas al backend.
- [ ] Revisar y **limpiar APIs y operaciones** no utilizadas regularmente.
- [ ] Apagar o escalar a cero entornos **Dev/QA** fuera de horario laboral.

---

## 19. Referencias

| Recurso | URL |
|---|---|
| Documentación oficial de Azure API Management | https://learn.microsoft.com/azure/api-management/ |
| Referencia completa de políticas | https://learn.microsoft.com/azure/api-management/api-management-policies |
| Azure Well-Architected Framework — APIM | https://learn.microsoft.com/azure/well-architected/service-guides/azure-api-management |
| Límites y cuotas de APIM | https://learn.microsoft.com/azure/azure-resource-manager/management/azure-subscription-service-limits#api-management-limits |
| Calculadora de precios de Azure | https://azure.microsoft.com/pricing/calculator/ |
| APIOps con Azure API Management | https://learn.microsoft.com/azure/api-management/devops-api-development-templates |
| Tutorial: Transformar y proteger APIs | https://learn.microsoft.com/azure/api-management/transform-api |
| Self-Hosted Gateway | https://learn.microsoft.com/azure/api-management/self-hosted-gateway-overview |

---

> **Última actualización:** Abril 2026  
> **Versión del documento:** 1.0
