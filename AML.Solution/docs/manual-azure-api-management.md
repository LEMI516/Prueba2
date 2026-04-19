# Manual de uso y mejores prácticas de Azure API Management

## 1. ¿Qué es Azure API Management?

Azure API Management (APIM) es la plataforma de Azure para publicar, proteger, transformar, observar y gobernar APIs. Actúa como una puerta de enlace entre consumidores y servicios backend, y permite:

- exponer APIs internas y externas con una fachada controlada;
- aplicar autenticación, autorización y rate limiting;
- centralizar políticas de seguridad y transformación;
- versionar y documentar APIs;
- monitorear consumo, errores y rendimiento;
- ofrecer un portal de desarrolladores y administración de suscripciones.

En una arquitectura moderna, APIM suele ubicarse entre:

1. clientes consumidores;
2. backend APIs o microservicios;
3. sistemas de identidad;
4. herramientas de monitoreo y observabilidad.

---

## 2. Componentes principales de APIM

### 2.1 API Gateway

Es el componente que recibe la solicitud del consumidor y ejecuta políticas antes y después de llamar al backend.

### 2.2 APIs

Son las definiciones publicadas dentro de APIM. Pueden crearse a partir de:

- OpenAPI/Swagger;
- APIs REST existentes;
- WebSocket APIs;
- SOAP importado como REST o SOAP passthrough;
- Azure Functions;
- App Service;
- Logic Apps;
- backends manuales.

### 2.3 Products

Agrupan una o varias APIs para exponerlas a consumidores específicos. Son útiles para:

- separar planes internos y externos;
- habilitar acceso por suscripción;
- empaquetar capacidades por línea de negocio.

### 2.4 Subscriptions

Permiten controlar quién consume un producto o una API y con qué clave o credencial.

### 2.5 Policies

Son reglas declarativas que se aplican en distintos puntos del pipeline:

- `inbound`
- `backend`
- `outbound`
- `on-error`

Las políticas son el corazón operativo de APIM.

### 2.6 Developer Portal

Portal para que equipos internos o partners:

- descubran APIs;
- revisen documentación;
- prueben endpoints;
- soliciten suscripciones.

### 2.7 Backends

Representan los servicios reales a los que APIM enruta solicitudes.

---

## 3. Casos de uso típicos

Azure API Management es especialmente útil para:

- publicar microservicios de forma consistente;
- proteger APIs internas expuestas a canales externos;
- aplicar un modelo B2B con partners;
- desacoplar consumidores del backend real;
- agregar transformaciones sin tocar el servicio fuente;
- unificar múltiples backends bajo una misma fachada;
- establecer gobierno de APIs a escala empresarial.

---

## 4. Modelos de despliegue y tiers

Azure API Management ofrece distintos tiers. La elección debe responder a necesidades de:

- disponibilidad;
- capacidad;
- networking;
- costo;
- aislamiento;
- características avanzadas.

### 4.1 Recomendación general

- **Developer**: solo para ambientes de desarrollo o pruebas funcionales. No usar en producción.
- **Basic / Standard / Premium**: para producción, según necesidades de escala, SLA y red.
- **Consumption**: útil para cargas variables y escenarios con costo por uso, evaluando limitaciones del modelo.

### 4.2 Mejor práctica

Elegir el tier por requerimientos no funcionales, no solo por costo inicial. Muchas implementaciones fallan porque se subestima:

- tráfico real;
- latencia por políticas complejas;
- necesidad de VNet;
- requerimientos multi-región;
- observabilidad y alta disponibilidad.

---

## 5. Flujo recomendado de implementación

Un flujo sano de adopción de APIM suele ser:

1. definir el objetivo de negocio de cada API;
2. clasificar consumidores y niveles de acceso;
3. importar o diseñar la especificación OpenAPI;
4. publicar la API en APIM;
5. definir productos y suscripciones;
6. aplicar políticas base de seguridad y control;
7. configurar observabilidad;
8. probar con escenarios reales;
9. automatizar despliegues con IaC y CI/CD;
10. documentar el ciclo de vida de la API.

---

## 6. Cómo crear y publicar una API en APIM

### 6.1 Crear la instancia

Desde Azure Portal:

1. ir a **Create resource**;
2. buscar **API Management**;
3. definir:
   - suscripción;
   - resource group;
   - nombre;
   - región;
   - organización;
   - correo administrador;
   - tier;
4. crear el recurso.

### 6.2 Importar una API

Opciones comunes:

- importar desde OpenAPI;
- importar desde URL de Swagger;
- crear API vacía;
- importar Function App;
- importar App Service.

### 6.3 Configurar el backend

Definir:

- URL base del backend;
- headers necesarios;
- credenciales si aplica;
- timeouts;
- certificados si se usa mTLS.

### 6.4 Publicar a través de un Product

Después de importar la API:

1. asignarla a un producto;
2. configurar si requiere suscripción;
3. definir visibilidad por grupos;
4. validar el onboarding desde el Developer Portal.

---

## 7. Organización recomendada de APIs

### 7.1 Diseñar por dominio, no por equipo técnico

Conviene agrupar APIs por capacidades de negocio:

- clientes;
- pagos;
- órdenes;
- autenticación;
- reportes.

### 7.2 Mantener consistencia en nombres

Definir convenciones para:

- nombre visible;
- path base;
- versión;
- tags;
- operaciones;
- productos;
- grupos.

Ejemplo:

- API: `Customer API`
- path: `/customers`
- versión: `v1`
- operación: `GET /customers/{id}`

### 7.3 Evitar mezclar demasiados contextos

No conviene publicar una sola API “monolítica” que agrupe recursos sin relación clara. Eso complica:

- seguridad;
- versionado;
- ownership;
- documentación;
- analítica.

---

## 8. Policies: uso práctico y mejores prácticas

Las policies permiten personalizar el comportamiento sin modificar el backend.

## 8.1 Dónde aplicar policies

Pueden aplicarse a nivel de:

- global;
- producto;
- API;
- operación.

### Mejor práctica

Aplicar políticas comunes en niveles altos y excepciones en niveles bajos. Evita duplicación y reduce errores operativos.

## 8.2 Políticas más usadas

### Rate limiting

Sirve para controlar abuso o consumo excesivo.

```xml
<inbound>
    <base />
    <rate-limit calls="100" renewal-period="60" />
</inbound>
```

### Quotas

Útiles para límites por periodo más largo.

```xml
<inbound>
    <base />
    <quota calls="10000" renewal-period="2592000" />
</inbound>
```

### Validación de JWT

```xml
<inbound>
    <base />
    <validate-jwt header-name="Authorization"
                  failed-validation-httpcode="401"
                  failed-validation-error-message="Token invalido o ausente">
        <openid-config url="https://login.microsoftonline.com/<tenant-id>/v2.0/.well-known/openid-configuration" />
        <audiences>
            <audience>api://mi-api</audience>
        </audiences>
    </validate-jwt>
</inbound>
```

### Set backend service

```xml
<inbound>
    <base />
    <set-backend-service base-url="https://mi-backend.azurewebsites.net" />
</inbound>
```

### Transformación de headers

```xml
<inbound>
    <base />
    <set-header name="x-correlation-id" exists-action="override">
        <value>@(context.RequestId.ToString())</value>
    </set-header>
</inbound>
```

### Rewrite URL

```xml
<inbound>
    <base />
    <rewrite-uri template="/internal/orders/{id}" />
</inbound>
```

### Mocking

Muy útil para pruebas tempranas y desacople entre equipos.

### Caching

Conveniente en operaciones idempotentes y de lectura frecuente, con cuidado sobre expiración e invalidación.

## 8.3 Mejores prácticas con policies

- mantener políticas pequeñas y legibles;
- evitar lógica excesivamente compleja en expresiones;
- reutilizar fragments o plantillas cuando sea posible;
- documentar políticas críticas;
- probar cambios de policy en ambientes no productivos;
- evitar convertir APIM en un motor de negocio;
- no esconder errores del backend sin trazabilidad;
- establecer `correlation-id` de extremo a extremo.

## 8.4 Qué no conviene hacer con policies

- implementar reglas de negocio complejas;
- encadenar demasiadas transformaciones costosas;
- depender de secretos embebidos en texto plano;
- duplicar la misma policy en decenas de APIs sin reutilización;
- usar APIM para reemplazar un backend mal diseñado.

---

## 9. Seguridad: guía práctica

La seguridad en APIM debe abordarse por capas.

## 9.1 Autenticación y autorización

Opciones frecuentes:

- subscription key;
- OAuth2 / OpenID Connect;
- JWT validation;
- client certificates;
- managed identity para salida hacia Azure services.

### Recomendación

Usar subscription key como mecanismo de control de consumo, pero no como única defensa para APIs sensibles. Para producción, normalmente se debe combinar con:

- Azure AD / Entra ID;
- validación JWT;
- autorización por scopes o claims.

## 9.2 Protección del backend

No expongas el backend directamente si APIM debe ser el único punto de entrada. Asegura:

- restricciones de red;
- allowlist de IPs o private endpoints;
- autenticación entre APIM y backend;
- certificados o managed identity.

## 9.3 Manejo de secretos

Mejor práctica:

- almacenar secretos en Azure Key Vault;
- evitar credenciales fijas en políticas;
- usar named values referenciando secretos;
- rotar credenciales regularmente.

## 9.4 Named Values

Permiten parametrizar configuraciones como:

- endpoints;
- API keys;
- identificadores;
- flags.

Usarlas mejora mantenibilidad y reduce exposición accidental.

## 9.5 CORS

Configura CORS de forma explícita y restrictiva.

Mala práctica:

- permitir `*` en orígenes para APIs sensibles;
- habilitar métodos innecesarios;
- aceptar headers indiscriminadamente.

## 9.6 Validación de entrada

APIM puede complementar controles, pero la validación fuerte debe seguir existiendo en backend. Usa APIM para:

- filtrar tokens inválidos;
- normalizar headers;
- controlar tamaños y frecuencia;
- rechazar patrones claramente indebidos.

---

## 10. Versionado y revisión de APIs

El versionado evita romper consumidores cuando una API evoluciona.

### 10.1 Estrategias comunes

- versión en path: `/v1/customers`
- versión en header;
- versión en query string.

### Recomendación práctica

La versión en path suele ser la más clara para equipos, documentación y troubleshooting.

### 10.2 Mejores prácticas de versionado

- no introducir cambios breaking en la misma versión;
- definir política de deprecación;
- comunicar fechas de retiro;
- mantener coexistencia controlada entre versiones;
- documentar diferencias funcionales;
- medir adopción antes de retirar una versión.

### 10.3 Revisions

Las revisions sirven para cambios no breaking dentro de una misma versión. Úsalas para:

- ajustes de policy;
- mejoras de documentación;
- cambios compatibles de implementación.

No las uses como sustituto del versionado formal.

---

## 11. Productos, grupos y suscripciones

Esta capa define el modelo de consumo.

### 11.1 Recomendación de diseño

Separar productos por:

- tipo de consumidor;
- nivel de servicio;
- sensibilidad de datos;
- entorno;
- monetización si aplica.

Ejemplos:

- `internal-shared`
- `partners-standard`
- `partners-premium`
- `sandbox`

### 11.2 Buenas prácticas

- no mezclar APIs internas críticas con APIs públicas en el mismo producto;
- definir propietarios claros del producto;
- revisar regularmente suscripciones activas;
- revocar accesos obsoletos;
- usar grupos para simplificar permisos.

---

## 12. Observabilidad y monitoreo

Publicar APIs sin observabilidad es una fuente común de incidentes difíciles de diagnosticar.

## 12.1 Qué medir

Como mínimo:

- volumen de llamadas;
- tasa de error;
- latencia promedio y percentiles;
- respuestas 4xx y 5xx;
- throttling;
- consumo por producto o suscripción;
- disponibilidad del gateway;
- performance del backend.

## 12.2 Integraciones recomendadas

- Azure Monitor
- Application Insights
- Log Analytics
- alertas de métricas y logs

## 12.3 Buenas prácticas

- propagar `correlation-id`;
- registrar errores con contexto suficiente;
- separar métricas de negocio y técnicas;
- definir dashboards por API y por producto;
- crear alertas accionables, no ruidosas;
- revisar tendencias, no solo incidentes puntuales.

## 12.4 Logging responsable

Evitar registrar:

- tokens completos;
- contraseñas;
- secretos;
- datos personales innecesarios;
- payloads completos sensibles salvo necesidad controlada.

---

## 13. Rendimiento y escalabilidad

APIM agrega una capa adicional al flujo. Debe diseñarse con criterio de rendimiento.

### 13.1 Recomendaciones

- mantener policies simples;
- evitar transformaciones costosas en payloads grandes;
- usar caching donde tenga sentido;
- revisar timeouts de backend;
- hacer pruebas de carga;
- monitorear latencia agregada por APIM;
- evaluar capacidad por región y tier.

### 13.2 Antipatrones frecuentes

- usar APIM para procesamiento pesado de datos;
- respuesta excesivamente transformada en gateway;
- falta de control de timeout y retry;
- concentrar demasiadas APIs críticas sin aislamiento lógico.

---

## 14. Integración con redes privadas

En entornos empresariales es común requerir conectividad privada.

### Escenarios típicos

- APIM integrado con VNet;
- acceso a backends internos;
- private endpoints;
- exposición controlada a internet;
- resolución DNS privada.

### Buenas prácticas

- diseñar networking desde el inicio;
- validar dependencias DNS y rutas;
- documentar puertos y flujos;
- probar failover y conectividad entre ambientes;
- involucrar seguridad y redes en el diseño.

---

## 15. CI/CD e infraestructura como código

Una operación madura de APIM no debe depender de cambios manuales en portal.

## 15.1 Recomendación

Gestionar APIM con infraestructura como código y despliegues automatizados usando herramientas como:

- Bicep;
- ARM;
- Terraform;
- pipelines de Azure DevOps o GitHub Actions.

## 15.2 Qué conviene automatizar

- creación de instancia;
- APIs;
- products;
- groups;
- named values;
- policies;
- diagnostics;
- logger settings;
- version sets.

## 15.3 Mejores prácticas de automatización

- mantener OpenAPI en repositorio;
- versionar policies como código;
- separar configuración por ambiente;
- evitar cambios manuales fuera de emergencia;
- promover cambios con pipelines;
- incluir validaciones y smoke tests.

---

## 16. Gobierno de APIs

APIM no solo resuelve integración técnica; también habilita gobierno.

### 16.1 Elementos clave de gobierno

- estándares de diseño;
- lineamientos de naming;
- políticas mínimas obligatorias;
- versionado;
- proceso de publicación;
- criterios de deprecación;
- dueños funcionales y técnicos;
- revisiones de seguridad.

### 16.2 Checklist mínimo antes de publicar

- contrato OpenAPI actualizado;
- autenticación definida;
- autorización definida;
- rate limit o quota aplicados;
- monitoreo configurado;
- documentación visible;
- estrategia de versionado clara;
- backend protegido;
- secretos fuera del código;
- pruebas funcionales realizadas.

---

## 17. Buenas prácticas operativas

### 17.1 Separar ambientes

Mantener al menos:

- desarrollo;
- pruebas/qa;
- producción.

No reutilizar la misma instancia para todos los fines.

### 17.2 Definir ownership

Cada API debe tener:

- responsable técnico;
- responsable funcional;
- proceso de soporte;
- canal de incidentes;
- criterio de cambios breaking.

### 17.3 Documentación viva

Mantener actualizados:

- propósito de la API;
- consumidores autorizados;
- autenticación;
- ejemplos de request/response;
- errores comunes;
- límites de uso;
- contactos.

### 17.4 Deprecación controlada

Nunca retirar una API crítica sin:

- comunicación previa;
- periodo de coexistencia;
- métricas de adopción;
- plan de migración;
- validación con consumidores.

---

## 18. Errores comunes en implementaciones de APIM

Los errores más frecuentes son:

1. usar APIM solo como proxy sin gobierno ni seguridad real;
2. depender de configuraciones manuales no versionadas;
3. publicar APIs sin observabilidad;
4. no proteger el backend detrás del gateway;
5. abusar de policies con lógica compleja;
6. no definir ownership por API;
7. no versionar correctamente;
8. exponer productos y suscripciones sin revisión periódica;
9. no probar escenarios de carga;
10. tratar Developer tier como si fuera productivo.

---

## 19. Patrón recomendado para arquitectura empresarial

Un patrón útil en organizaciones medianas y grandes es:

### Capa de experiencia

APIs orientadas al canal o consumidor.

### Capa de proceso

APIs que orquestan reglas de negocio.

### Capa de sistema

APIs que encapsulan sistemas fuente.

APIM puede frontear una o varias de estas capas, según el diseño. La decisión debe considerar:

- seguridad;
- latencia;
- ownership;
- reutilización;
- exposición pública o privada.

---

## 20. Recomendación de implementación inicial

Si una organización va a empezar con Azure API Management, una estrategia prudente es:

1. seleccionar 1 a 3 APIs candidatas;
2. importar contratos OpenAPI;
3. definir productos separados para sandbox e interno;
4. aplicar JWT validation, rate limit y correlation-id;
5. habilitar monitoreo con Azure Monitor/Application Insights;
6. automatizar publicación con IaC;
7. documentar proceso de versionado y deprecación;
8. establecer estándar reusable para siguientes APIs.

Esto permite aprender sin sobrediseñar.

---

## 21. Ejemplo de estándar mínimo por API

Cada API publicada en APIM debería cumplir con algo similar a esto:

- OpenAPI vigente;
- nombre y path siguiendo convención;
- versión explícita;
- policy de autenticación;
- policy de rate limit o quota;
- `correlation-id`;
- backend autenticado o protegido;
- logs y métricas habilitados;
- documentación visible;
- owner asignado;
- despliegue automatizado.

---

## 22. Recomendaciones finales

- usar APIM como capa de gobierno, no solo de exposición;
- tratar policies como código;
- diseñar seguridad de extremo a extremo;
- proteger el backend para que APIM sea el punto de control;
- priorizar observabilidad desde el primer día;
- versionar con disciplina;
- mantener productos y suscripciones bajo control;
- automatizar todo lo posible;
- evitar complejidad innecesaria en el gateway.

---

## 23. Resumen ejecutivo

Azure API Management es una herramienta muy potente cuando se usa con disciplina arquitectónica y operativa. Su mayor valor aparece cuando combina:

- publicación estandarizada;
- seguridad consistente;
- control de consumo;
- observabilidad;
- versionado;
- automatización;
- gobierno.

Si se implementa solo como proxy, se desaprovecha gran parte de su valor. Si se usa como plataforma de control y ciclo de vida de APIs, puede convertirse en un habilitador estratégico para integración, escalabilidad y seguridad.
