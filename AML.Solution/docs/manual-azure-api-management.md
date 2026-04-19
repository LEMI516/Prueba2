# Manual de uso y mejores practicas de Azure API Management

## 1. Objetivo

Este manual resume como usar Azure API Management (APIM) de forma ordenada, segura y mantenible. La idea es que sirva como guia base para:

- publicar APIs hacia consumidores internos o externos,
- proteger backends,
- aplicar politicas transversales,
- versionar cambios sin romper clientes,
- automatizar despliegues y
- operar la plataforma con observabilidad y gobierno.

## 2. Que es Azure API Management

Azure API Management es una plataforma de gestion de APIs que actua como fachada entre los consumidores y tus servicios backend.

Con APIM puedes:

- exponer APIs REST, SOAP o backends HTTP,
- aplicar autenticacion y autorizacion,
- limitar trafico con cuotas y rate limits,
- transformar requests y responses,
- ocultar detalles internos del backend,
- centralizar versionado, productos y suscripciones,
- publicar documentacion en un portal para desarrolladores y
- monitorear uso, errores y latencia.

## 3. Cuando conviene usarlo

APIM encaja muy bien cuando necesitas una o varias de estas capacidades:

- publicar varias APIs con una puerta de entrada comun,
- estandarizar seguridad y politicas en muchos servicios,
- exponer APIs a partners, terceros o equipos internos,
- desacoplar consumidores del backend real,
- controlar consumo por producto, cliente o aplicacion,
- tener observabilidad funcional y tecnica a nivel API.

No siempre hace falta si solo existe un servicio simple, sin necesidades de gobierno, sin consumidores externos y sin politicas transversales. En esos casos un reverse proxy sencillo puede bastar.

## 4. Conceptos clave que debes dominar

### 4.1 API

Es la definicion publicada en APIM. Normalmente se importa desde OpenAPI, aunque tambien puede crearse manualmente.

### 4.2 Operation

Cada endpoint o metodo especifico dentro de una API.

### 4.3 Backend

Es el servicio real al que APIM reenvia la llamada. Puede ser App Service, AKS, Functions, Logic Apps, servicios on-prem o cualquier endpoint HTTP accesible.

### 4.4 Product

Agrupa una o varias APIs para ofrecerlas a consumidores. Permite asociar reglas de acceso, suscripciones y visibilidad.

### 4.5 Subscription

Clave de consumo asociada a un producto o una API. Sirve para identificar al consumidor y aplicar politicas por cliente.

### 4.6 Policy

Bloques declarativos en XML que se ejecutan en APIM para transformar trafico, validar tokens, cachear, cambiar headers, enrutar, registrar eventos o manejar errores.

### 4.7 Named Values

Variables reutilizables para configuracion. Se usan para URLs, flags, identificadores y referencias a secretos.

### 4.8 Version y Revision

- **Version**: cambio funcional o contractual que puede afectar clientes.
- **Revision**: cambio no disruptivo sobre una misma version.

### 4.9 Developer Portal

Portal para documentar APIs, publicar ejemplos, administrar suscripciones y facilitar el onboarding de consumidores.

## 5. Arranque rapido: publicar una API en APIM

Este flujo sirve como guia operativa basica para una primera publicacion en el portal de Azure:

1. entra a tu instancia de **API Management** en Azure Portal,
2. abre **APIs** y selecciona **Add API**,
3. importa tu contrato desde OpenAPI, Function App, App Service o URL,
4. define:
   - nombre visible,
   - nombre interno,
   - URL suffix,
   - version o version set si aplica,
   - backend service URL,
5. revisa las operations creadas automaticamente,
6. agrega o ajusta policies de entrada y salida,
7. crea o asigna un **Product**,
8. habilita suscripciones si quieres controlar consumidores por clave,
9. publica documentacion y ejemplos en el **Developer Portal**,
10. prueba llamadas desde el portal o desde Postman/curl,
11. activa diagnostico y observabilidad antes de abrir acceso masivo.

**Consejo:** si el contrato viene de OpenAPI, el primer objetivo no debe ser "hacerlo funcionar a mano", sino dejarlo repetible y versionado desde git.

## 6. Flujo recomendado de uso

### 6.1 Disenar primero el contrato

Antes de publicar en APIM:

- define el contrato en OpenAPI,
- acuerda naming, rutas, codigos HTTP y errores,
- separa claramente recursos y operaciones,
- evita acoplar la API publica con el modelo interno del backend.

**Buena practica:** diseña la API como producto, no como espejo tecnico del microservicio.

### 6.2 Importar la API

Puedes importar desde:

- OpenAPI/Swagger,
- WSDL,
- Function App,
- App Service,
- Logic App,
- API existente por URL.

**Recomendacion:** usa OpenAPI versionado en git como fuente de verdad. No dependas de cambios manuales en el portal para definir el contrato.

### 6.3 Configurar el acceso al backend

Al publicar una API:

- define la URL base del backend,
- protege el backend para que solo APIM pueda consumirlo cuando sea posible,
- usa Managed Identity, certificados o OAuth2 segun el escenario,
- evita credenciales hardcodeadas.

### 6.4 Agrupar APIs en productos

Crea productos segun el modelo de negocio o el tipo de consumidor:

- interno,
- partner,
- publico,
- sandbox,
- premium.

Cada producto puede tener:

- visibilidad,
- requisitos de suscripcion,
- terminos de uso,
- limites de consumo diferentes.

### 6.5 Aplicar politicas transversales

Centraliza en APIM lo repetitivo:

- validacion de JWT,
- headers comunes,
- correlacion,
- rate limit,
- cache,
- transformaciones simples,
- masking de datos sensibles,
- manejo uniforme de errores.

### 6.6 Publicar y observar

Despues de publicar:

- habilita diagnostico y metricas,
- integra Application Insights o tu plataforma observabilidad,
- mide errores, latencia, throughput y uso por consumidor,
- revisa limites, cuotas y picos de trafico.

## 7. Mejores practicas de arquitectura

### 7.1 Separa APIM por ambiente

Ten entornos distintos para dev, qa/staging y produccion.

Evita:

- compartir secretos entre ambientes,
- probar manualmente en produccion,
- usar el mismo gateway para todos los ciclos sin control.

### 7.2 Usa APIM como capa de gobierno, no como backend complejo

APIM es excelente para mediacion ligera y gobierno. No conviertas las policies en un motor de negocio complejo.

**Regla practica:**

- logica de negocio -> backend,
- logica transversal de API -> APIM.

### 7.3 Estandariza naming

Define convenciones para:

- APIs,
- products,
- version sets,
- named values,
- loggers,
- tags,
- operaciones.

Ejemplo:

- API: `customers-v1`
- Product: `partners-gold`
- Named value: `backend-customers-base-url`
- Version set: `customers`

### 7.4 Diseña pensando en dominios

Evita meter muchas APIs sin estructura. Organiza por dominio funcional:

- customers,
- orders,
- billing,
- notifications.

Eso facilita ownership, versionado y seguridad.

## 8. Mejores practicas de seguridad

### 8.1 No expongas el backend directamente

Siempre que sea posible:

- restringe acceso por red,
- usa private endpoints, VNets o allowlists,
- permite que el backend confie en APIM y no en cualquier origen.

### 8.2 Usa Microsoft Entra ID / OAuth2 / JWT

Para APIs empresariales, prioriza autenticacion basada en tokens.

Aplica validacion en APIM con politicas de JWT para:

- issuer,
- audience,
- expiracion,
- claims requeridos.

### 8.3 Guarda secretos fuera del codigo

Nunca dejes secretos en:

- repositorios,
- policies fijas,
- named values sin proteccion adecuada.

Usa:

- Azure Key Vault,
- referencias seguras,
- Managed Identity.

### 8.4 Aplica minimo privilegio

Cada consumidor debe tener solo acceso a lo necesario:

- por producto,
- por API,
- por scope,
- por claims o grupo.

### 8.5 Protege contra abuso

Configura:

- `rate-limit`,
- `quota`,
- validacion de payload cuando aplique,
- tamano maximo de mensajes,
- filtros basicos de headers requeridos.

## 9. Mejores practicas de versionado y ciclo de vida

### 9.1 Usa versionado explicito

Evita cambios breaking en una API existente sin version.

Opciones comunes:

- version en path: `/v1/customers`,
- version en header,
- version en query string.

**Recomendacion general:** usa version en path cuando la simplicidad operativa sea prioritaria.

### 9.2 Usa revisions para cambios no disruptivos

Las revisiones sirven para:

- ajustar politicas,
- corregir documentacion,
- hacer cambios internos sin alterar el contrato.

### 9.3 Manten politicas de deprecacion

Cuando publiques una nueva version:

- define fecha de sunset,
- avisa a consumidores con anticipacion,
- documenta diferencias,
- monitorea quien sigue consumiendo la version vieja.

## 10. Mejores practicas de policies

### 10.1 Manten las policies pequenas y legibles

Divide responsabilidades:

- autenticacion,
- transformacion,
- observabilidad,
- resiliencia.

Evita policies gigantes con demasiada logica condicional.

### 10.2 Centraliza reusable fragments cuando sea posible

Si varias APIs comparten la misma logica:

- usa fragments o estructura comun,
- documenta quien es owner de cada politica,
- versiona cambios cuidadosamente.

### 10.3 Registra correlacion

Asegura que cada request tenga un correlation id o trace id.

Si no viene desde el cliente:

- generarlo en APIM,
- reenviarlo al backend,
- incluirlo en logs y respuestas si tiene sentido.

### 10.4 Evita transformaciones complejas

Transformaciones ligeras estan bien:

- agregar/quitar headers,
- reescribir URL,
- mapear campos simples,
- ocultar informacion sensible.

Si la transformacion implica reglas de negocio, branching complejo o enriquecimiento pesado, muvela al backend o a una capa especializada.

### 10.5 Ejemplo simple de politica de entrada

```xml
<policies>
  <inbound>
    <base />
    <validate-jwt header-name="Authorization"
                  failed-validation-httpcode="401"
                  failed-validation-error-message="Unauthorized">
      <openid-config url="https://login.microsoftonline.com/<tenant-id>/v2.0/.well-known/openid-configuration" />
      <required-claims>
        <claim name="aud">
          <value>api://mi-api</value>
        </claim>
      </required-claims>
    </validate-jwt>
    <rate-limit-by-key calls="100" renewal-period="60"
      counter-key="@(context.Subscription?.Key ?? context.Request.IpAddress)" />
    <set-header name="x-correlation-id" exists-action="override">
      <value>@(context.RequestId.ToString())</value>
    </set-header>
  </inbound>
  <backend>
    <base />
  </backend>
  <outbound>
    <base />
  </outbound>
  <on-error>
    <base />
  </on-error>
</policies>
```

## 11. Resiliencia y rendimiento

### 11.1 Usa cache cuando el caso lo permita

La cache es util para:

- catalogos,
- configuraciones,
- respuestas publicas o semiestaticas,
- tokens o metadata no sensible.

No la uses indiscriminadamente para datos altamente dinamicos o sensibles.

### 11.2 Controla timeouts

No permitas que APIM espere indefinidamente al backend.

Define timeouts razonables segun:

- SLA esperado,
- tipo de operacion,
- experiencia del cliente,
- capacidad de reintento.

### 11.3 No abuses de retries ciegos

Si decides implementar retry, hazlo solo para errores transitorios y operaciones idempotentes. Un mal retry puede duplicar efectos o empeorar la latencia.

### 11.4 Mide antes de optimizar

Observa:

- p50, p95 y p99,
- errores 4xx y 5xx,
- backend time,
- saturacion por producto o suscripcion.

## 12. Observabilidad y operacion

### 12.1 Habilita diagnosticos desde el inicio

Integra APIM con:

- Application Insights,
- Azure Monitor,
- Log Analytics,
- Event Hub o SIEM si tu organizacion lo requiere.

### 12.2 Define un set minimo de senales

Como minimo monitorea:

- total requests,
- tasa de error,
- latencia por API,
- latencia por operacion,
- consumo por suscripcion,
- errores de autenticacion,
- throttling,
- disponibilidad del backend.

### 12.3 No registres datos sensibles innecesarios

Evita guardar en logs:

- passwords,
- tokens completos,
- PII sin necesidad,
- payloads enteros en procesos sensibles.

Si debes registrar contenido, aplica masking o truncado.

### 12.4 Crea alertas accionables

No configures alertas de ruido. Enfocate en:

- aumento de 5xx,
- degradacion de latencia,
- backend caido,
- errores de autenticacion recurrentes,
- agotamiento de cuota o capacidad.

## 13. Productos, suscripciones y onboarding

### 13.1 Modela productos segun consumo real

Un buen producto responde a reglas claras:

- quien puede acceder,
- que APIs incluye,
- que limites tiene,
- que SLA o soporte lo acompana.

### 13.2 No mezcles todo en un solo producto

Separar productos ayuda a:

- controlar acceso,
- medir adopcion,
- monetizar si aplica,
- reducir errores operativos.

### 13.3 Cuida el Developer Portal

El portal debe incluir:

- descripcion clara,
- ejemplos de request/response,
- codigos de error,
- autenticacion explicada,
- pasos para obtener suscripcion,
- changelog o notas de version.

## 14. Infraestructura como codigo y CI/CD

### 14.1 No dependas del portal como unica fuente de verdad

El portal es util para operar, pero no debe ser la fuente principal de configuracion.

Usa IaC para versionar:

- instancia APIM,
- APIs,
- products,
- named values,
- loggers,
- diagnosticos,
- politicas.

### 14.2 Herramientas recomendadas

Puedes automatizar con:

- Bicep,
- ARM,
- Terraform,
- GitHub Actions,
- Azure DevOps.

### 14.3 Promociona cambios por pipeline

Flujo recomendado:

1. cambio en OpenAPI o policies en git,
2. validacion automatica,
3. despliegue a dev,
4. pruebas,
5. promocion a staging,
6. despliegue controlado a produccion.

### 14.4 Evita drift de configuracion

Si haces cambios manuales en portal y no los llevas a codigo, tarde o temprano apareceran inconsistencias entre ambientes.

## 15. Gobierno y operacion en equipos grandes

### 15.1 Define owners claros

Cada API debe tener:

- owner tecnico,
- owner funcional,
- politica de versionado,
- contacto operativo.

### 15.2 Usa checklists de publicacion

Antes de publicar una API valida:

- contrato OpenAPI revisado,
- seguridad definida,
- productos asignados,
- politicas aplicadas,
- observabilidad habilitada,
- limites configurados,
- documentacion publicada.

### 15.3 Revisa periodicamente APIs huerfanas

Detecta APIs:

- sin trafico,
- sin owner,
- con versiones obsoletas,
- con subscriptions viejas,
- con errores recurrentes.

## 16. Anti-patrones comunes

Evita estos errores frecuentes:

1. **Usar APIM como reemplazo del backend**
   - Si toda la logica termina en policies complejas, la solucion sera fragil.

2. **Cambiar contratos sin versionar**
   - Rompe consumidores y dificulta soporte.

3. **Publicar sin limites**
   - Sin rate limits ni cuotas, un cliente puede degradar todo el servicio.

4. **Guardar secretos en texto plano**
   - Riesgo directo de seguridad y cumplimiento.

5. **Administrar todo manualmente**
   - Hace dificil auditar, repetir y promover configuracion.

6. **Loggear informacion sensible**
   - Puede violar requisitos legales o internos.

7. **No desacoplar API publica del backend**
   - Cualquier cambio interno termina afectando consumidores.

## 17. Checklist rapido para una API nueva en APIM

Antes de salir a produccion confirma:

- [ ] Existe contrato OpenAPI claro y versionado.
- [ ] La URL publica sigue la convencion definida.
- [ ] El backend no queda expuesto libremente.
- [ ] La autenticacion/autorizacion esta definida.
- [ ] Los secretos salen de Key Vault o equivalente.
- [ ] Hay rate limit y/o quota.
- [ ] Existe correlation id end-to-end.
- [ ] Hay logs, metricas y alertas utiles.
- [ ] La documentacion del portal esta completa.
- [ ] La publicacion via pipeline fue probada en ambientes previos.

## 18. Recomendacion de enfoque de implementacion

Si vas a arrancar desde cero, una estrategia simple y sana es:

1. definir dominios de APIs,
2. crear OpenAPI por cada dominio,
3. publicar en APIM con versionado desde el inicio,
4. proteger acceso con Entra ID/JWT,
5. crear productos por tipo de consumidor,
6. agregar rate limiting, trazabilidad y diagnostico,
7. automatizar todo con IaC y pipeline,
8. operar con metricas, alertas y deprecacion controlada.

## 19. Resumen ejecutivo

Azure API Management aporta mucho valor cuando se usa como capa de gobierno, seguridad, visibilidad y exposicion controlada de APIs. Las mejores implementaciones comparten estos principios:

- contratos primero,
- seguridad por defecto,
- versionado explicito,
- politicas simples,
- automatizacion por codigo,
- observabilidad desde el dia uno,
- productos y suscripciones bien modelados.

Si esos principios se respetan, APIM deja de ser solo un gateway y se convierte en una plataforma ordenada para escalar integraciones y consumo de APIs.
