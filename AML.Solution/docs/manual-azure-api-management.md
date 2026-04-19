# Manual de uso y mejores practicas de Azure API Management

## 1. Que es Azure API Management

Azure API Management (APIM) es el servicio de Azure para publicar, proteger, transformar, supervisar y gobernar APIs desde un punto central. Permite exponer APIs internas o externas de forma controlada para consumidores internos, partners o desarrolladores externos.

APIM se apoya en cuatro bloques principales:

- **Gateway**: recibe las llamadas, aplica politicas y enruta al backend.
- **Management plane**: administra APIs, productos, versiones, usuarios y configuracion.
- **Developer portal**: portal para descubrir APIs, documentacion y suscripciones.
- **Analytics / observabilidad**: metricas, logs y diagnostico de consumo.

## 2. Casos de uso mas comunes

Azure API Management encaja bien cuando necesitas:

- Publicar varias APIs bajo un dominio y gobierno unificados.
- Proteger servicios backend sin exponerlos directamente a internet.
- Aplicar autenticacion, cuotas, rate limiting, transformaciones y validaciones.
- Versionar APIs sin romper consumidores existentes.
- Ofrecer un portal para desarrolladores y administrar suscripciones.
- Desacoplar el backend del contrato expuesto al cliente.
- Centralizar observabilidad, trazabilidad y politicas de seguridad.

## 3. Conceptos clave que debes dominar

### 3.1 APIs

Representan el contrato expuesto a los consumidores. Se pueden crear desde:

- OpenAPI / Swagger
- WSDL
- GraphQL
- App Service, Function App o Logic App
- APIs definidas manualmente

### 3.2 Operaciones

Cada endpoint o metodo expuesto dentro de una API. Las politicas se pueden aplicar a nivel global, producto, API u operacion.

### 3.3 Productos

Agrupan una o varias APIs y permiten controlar su consumo.

Usalos cuando quieras:

- ofrecer paquetes por area de negocio,
- separar consumidores internos y externos,
- aplicar aprobacion o suscripcion,
- diferenciar limites, cuotas y acceso.

### 3.4 Suscripciones

Son las credenciales funcionales para consumir un producto o una API, normalmente mediante subscription key. Sirven para rastrear consumo y aplicar gobierno.

### 3.5 Versiones y revisiones

- **Versiones**: cambios funcionales o de contrato que pueden convivir.
- **Revisiones**: ajustes no rompientes sobre una misma version antes o despues de publicarla.

### 3.6 Politicas

Las politicas son el corazon de APIM. Permiten ejecutar reglas declarativas en XML para:

- validar tokens,
- limitar trafico,
- transformar request/response,
- agregar headers,
- enrutar por condicion,
- almacenar en cache,
- invocar servicios auxiliares,
- manejar errores.

## 4. Flujo recomendado de implementacion

## Paso 1. Definir la estrategia de gobierno

Antes de crear APIs en APIM, define:

- quien publicara APIs,
- quien aprobara cambios,
- estandar de versionado,
- convenciones de nombres,
- politica de autenticacion por tipo de consumidor,
- requisitos de observabilidad y cumplimiento.

Sin este paso, APIM se vuelve un simple proxy y no una capa de gobierno.

## Paso 2. Organizar productos y dominios

Una estructura recomendable es:

- un grupo por equipo o dominio de negocio,
- productos separados para consumo interno, partners y terceros,
- APIs agrupadas por capacidad funcional, no por microservicio solamente.

Ejemplo:

- `Clientes Internos`
- `Partners`
- `Canales Digitales`
- `Pagos`
- `Identidad`

## Paso 3. Importar o publicar la API

La opcion preferida es importar un contrato OpenAPI bien mantenido. Esto reduce drift entre documentacion e implementacion.

Buenas practicas:

- usa OpenAPI como fuente de verdad,
- agrega ejemplos de request/response,
- documenta codigos de error,
- publica esquemas consistentes y nombres estables.

## Paso 4. Configurar acceso al backend

Define con claridad:

- URL base del backend,
- timeouts,
- certificados si aplica,
- conectividad privada o publica,
- politicas de resiliencia,
- secretos y credenciales.

Evita hardcodear secretos en politicas. Usa **Named Values** y, cuando sea posible, integracion con **Azure Key Vault**.

## Paso 5. Aplicar politicas por capas

Orden sugerido:

1. Politicas globales para estandares comunes.
2. Politicas por producto para gobierno comercial o de consumo.
3. Politicas por API para reglas funcionales.
4. Politicas por operacion solo cuando de verdad cambie el comportamiento.

Esto reduce duplicacion y facilita mantenimiento.

## Paso 6. Publicar y observar

Una vez publicada:

- habilita logs y metricas,
- monitorea latencia y codigos de error,
- revisa consumo por suscripcion,
- inspecciona politicas con mayor costo,
- crea alertas sobre degradacion o abuso.

## 5. Como usar Azure API Management en el dia a dia

## 5.1 Publicar una API REST

Proceso tipico:

1. Crear la instancia de APIM.
2. Crear o importar una API.
3. Definir el `backend service`.
4. Configurar seguridad de entrada.
5. Aplicar politicas.
6. Asociar la API a un producto.
7. Publicar documentacion en el portal de desarrolladores.
8. Probar con la consola integrada o con Postman.

## 5.2 Proteger una API

Patrones frecuentes:

- Subscription key para control basico de acceso y trazabilidad.
- OAuth 2.0 / OpenID Connect para consumidores reales.
- Validacion de JWT en el gateway.
- Mutual TLS para escenarios B2B o altamente regulados.
- Restriccion por IP, red privada o private endpoints si el escenario lo exige.

Para APIs productivas, evita depender solo de subscription keys. Lo recomendable es combinarlas con autenticacion fuerte cuando el riesgo lo amerite.

## 5.3 Transformar peticiones y respuestas

APIM permite:

- reescribir URLs,
- modificar payloads,
- mapear headers,
- eliminar datos sensibles,
- adaptar contratos antiguos mientras el backend evoluciona.

Usa transformaciones ligeras. Si la logica de negocio o el mapeo se vuelve complejo, muvelo al backend o a una capa de integracion especializada.

## 5.4 Aplicar cuotas y limites

Util para:

- proteger backends fragiles,
- controlar abuso,
- ofrecer planes por producto,
- aislar consumidores ruidosos.

Politicas comunes:

- `rate-limit`
- `rate-limit-by-key`
- `quota`
- `quota-by-key`

El criterio de limitacion debe responder al caso real: suscripcion, usuario, tenant, aplicacion o direccion IP.

## 5.5 Caching

APIM puede reducir latencia y costo cuando las respuestas son cacheables.

Conviene usar cache cuando:

- la respuesta no cambia con frecuencia,
- el backend tiene costo alto,
- la operacion es intensiva en lectura,
- el contrato es idempotente.

No uses cache para respuestas personalizadas o sensibles sin una estrategia explicita de variacion por clave.

## 5.6 Portal para desarrolladores

El developer portal es valioso para:

- onboarding de consumidores,
- descubrimiento de APIs,
- publicacion de ejemplos,
- autoservicio de suscripciones,
- documentacion centralizada.

Mantenlo alineado con tu catalogo real. Un portal desactualizado genera mas soporte que valor.

## 6. Mejores practicas de arquitectura

### 6.1 Diseña APIM como capa de gobierno, no como backend alterno

APIM no debe concentrar logica de negocio compleja. Su rol ideal es:

- proteger,
- validar,
- transformar ligeramente,
- observar,
- gobernar.

Si empiezas a implementar procesos complejos, orquestacion extensa o reglas de negocio pesadas en politicas, la mantenibilidad cae rapidamente.

### 6.2 Usa contratos primero

Prioriza un enfoque contract-first:

- OpenAPI versionado en repositorio,
- revisiones por pull request,
- validacion automatica,
- ejemplos y errores documentados.

Esto mejora consistencia entre equipos y reduce cambios sorpresivos.

### 6.3 Separa entornos correctamente

Mantén separados dev, qa y prod, idealmente con configuraciones y despliegues automatizados por entorno.

No compartas:

- secrets,
- backends,
- certificados,
- productos de prueba con produccion.

### 6.4 Estandariza politicas reutilizables

Crea una biblioteca de patrones:

- autenticacion JWT,
- correlacion,
- logging,
- manejo de errores,
- rate limit,
- headers de seguridad,
- mascarado de datos sensibles.

La estandarizacion evita politicas copiadas y pegadas con variaciones peligrosas.

### 6.5 Usa Named Values para configuracion

Los Named Values ayudan a desacoplar la politica de datos variables:

- URLs,
- claves,
- feature flags,
- identificadores de audiencia,
- configuraciones por entorno.

Combinalos con Key Vault para secretos de alto valor.

### 6.6 Define bien la estrategia de versionado

Recomendaciones:

- usa versionado explicito,
- evita cambios rompientes silenciosos,
- documenta fecha de deprecacion,
- mantén convivencia entre versiones cuando el negocio lo requiera.

Patrones comunes:

- version en path: `/v1/clientes`
- version en header
- version en query string

El mas simple de operar y comunicar suele ser **version en path**.

## 7. Mejores practicas de seguridad

### 7.1 No expongas secretos en politicas o repositorios

Evita:

- tokens embebidos en XML,
- credenciales en archivos exportados,
- secretos en variables sin proteccion.

Usa:

- Managed Identity,
- Key Vault,
- Named Values protegidos,
- RBAC minimo necesario.

### 7.2 Valida JWT en el gateway

Cuando uses OAuth 2.0 / OIDC:

- valida issuer,
- valida audience,
- valida expiracion,
- exige scopes o claims cuando aplique.

No delegues toda la validacion al backend si APIM es tu puerta de entrada principal.

### 7.3 Implementa zero trust y minimo privilegio

Buenas practicas:

- acceso admin con RBAC granular,
- redes privadas si el backend es interno,
- aprobacion controlada para cambios de produccion,
- auditoria de despliegues,
- suscripciones separadas por consumidor.

### 7.4 Sanitiza datos sensibles

No registres innecesariamente:

- passwords,
- tokens,
- datos financieros completos,
- informacion personal sensible.

Aplica mascarado o exclusiones en diagnostico y logging.

## 8. Mejores practicas de rendimiento y resiliencia

### 8.1 Minimiza el costo de las politicas

Cada transformacion adicional agrega latencia. Mantén las politicas simples y medibles.

Evita abusar de:

- expresiones complejas,
- transformaciones grandes de payload,
- multiples llamadas auxiliares,
- logica condicional excesiva.

### 8.2 Controla timeouts y dependencias

Configura timeouts razonables entre gateway y backend. Si el backend falla con frecuencia:

- protege con rate limiting,
- considera cache para lecturas,
- revisa reintentos fuera de APIM si la semantica lo permite,
- identifica cuellos de botella con observabilidad.

### 8.3 Piensa en capacidad y tier desde el principio

Selecciona el tier de APIM segun:

- volumen esperado,
- aislamiento de red,
- SLA requerido,
- multi-region,
- costo aceptable,
- soporte a escenarios internos o externos.

No sobredimensiones por costumbre, pero tampoco dejes la capacidad al limite si APIM sera un punto critico de entrada.

## 9. Mejores practicas de observabilidad

Implementa trazabilidad de extremo a extremo:

- correlation id,
- logs por API, operacion, producto y suscripcion,
- metricas de latencia, error y throughput,
- alertas por picos de 4xx y 5xx,
- integracion con Azure Monitor / Application Insights / Log Analytics.

Preguntas que siempre deberias poder responder:

- que consumidor genero la llamada,
- que version de API uso,
- que backend fue invocado,
- cuanto tardo,
- por que fallo,
- si el problema fue del cliente, del gateway o del backend.

## 10. Mejores practicas de ciclo de vida y DevOps

### 10.1 Trata APIM como codigo

Evita cambios manuales directos en produccion como mecanismo principal.

Lo recomendable:

- contratos OpenAPI en git,
- politicas versionadas,
- despliegue automatizado,
- revisiones por pull request,
- promocion entre entornos con pipeline.

### 10.2 Define artefactos claros

Versiona al menos:

- definiciones OpenAPI,
- politicas,
- configuracion de productos,
- named values no sensibles o referencias a secretos,
- scripts o plantillas de despliegue.

### 10.3 Usa revisiones para cambios controlados

Las revisiones son utiles para validar cambios menores antes de hacerlos oficiales. Son especialmente valiosas cuando debes probar:

- ajustes de politica,
- cambios no rompientes,
- correcciones de documentacion,
- refinamientos operativos.

### 10.4 Gestiona deprecaciones

No retires una version sin:

- avisar a consumidores,
- definir ventana de salida,
- medir uso residual,
- ofrecer guia de migracion.

## 11. Antipatrones que conviene evitar

Evita estos errores frecuentes:

1. **Usar APIM como reemplazo del backend**  
   Cuando el gateway contiene demasiada logica, el mantenimiento se vuelve fragil.

2. **Crear politicas duplicadas por todos lados**  
   Aumenta el riesgo de comportamientos inconsistentes.

3. **No versionar APIs desde el inicio**  
   Obliga a romper consumidores cuando el contrato cambia.

4. **Depender solo de subscription keys**  
   Es insuficiente para muchos escenarios corporativos o regulados.

5. **Publicar sin observabilidad**  
   Sin trazabilidad no hay forma rapida de diagnosticar incidentes.

6. **Exponer backends sensibles sin segmentacion de red**  
   Puede anular el valor de seguridad que APIM deberia aportar.

7. **Hacer cambios manuales sin pipeline**  
   Dificulta auditoria, repetibilidad y recuperacion.

## 12. Checklist de publicacion de una API

Antes de publicar, confirma:

- [ ] Existe contrato OpenAPI actualizado.
- [ ] La estrategia de versionado esta definida.
- [ ] Se eligio el producto correcto.
- [ ] La autenticacion y autorizacion estan validadas.
- [ ] Los secretos salen de Key Vault o Named Values protegidos.
- [ ] Hay rate limits y/o quotas segun el caso.
- [ ] Se definieron logs, metricas y alertas.
- [ ] La documentacion tiene ejemplos y errores comunes.
- [ ] El portal de desarrolladores refleja el estado real.
- [ ] El despliegue esta automatizado y versionado.

## 13. Ejemplos de politicas utiles

### 13.1 Correlation id

```xml
<inbound>
    <base />
    <set-header name="x-correlation-id" exists-action="skip">
        <value>@(context.RequestId.ToString())</value>
    </set-header>
</inbound>
```

### 13.2 Rate limiting por suscripcion

```xml
<inbound>
    <base />
    <rate-limit-by-key calls="100" renewal-period="60"
        counter-key="@(context.Subscription?.Key ?? context.Request.IpAddress)" />
</inbound>
```

### 13.3 Validacion de JWT

```xml
<inbound>
    <base />
    <validate-jwt header-name="Authorization" failed-validation-httpcode="401" require-scheme="Bearer">
        <openid-config url="https://login.microsoftonline.com/{tenant-id}/v2.0/.well-known/openid-configuration" />
        <audiences>
            <audience>api://mi-api</audience>
        </audiences>
    </validate-jwt>
</inbound>
```

## 14. Recomendacion practica para empezar bien

Si vas a implementar APIM desde cero, una secuencia sensata es:

1. Definir estandares de contrato y versionado.
2. Seleccionar el tier correcto y el modelo de red.
3. Publicar una API piloto con OpenAPI.
4. Aplicar autenticacion fuerte y rate limiting.
5. Activar observabilidad desde el primer dia.
6. Automatizar despliegues y versionado de politicas.
7. Escalar el catalogo solo despues de validar gobierno y soporte operativo.

## 15. Resumen ejecutivo

Azure API Management entrega mucho valor cuando se usa como una capa de gobierno y seguridad para APIs, no solo como un proxy. Las decisiones mas importantes suelen estar en:

- la estrategia de productos y consumidores,
- el modelo de seguridad,
- el versionado,
- la observabilidad,
- la automatizacion del ciclo de vida.

Si estos cinco puntos estan bien resueltos, APIM se convierte en una plataforma estable para exponer APIs de forma segura, medible y escalable.
