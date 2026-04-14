# Historias de Usuario — AML.Solution (Proyecto Principal)

Este documento define un backlog inicial de historias para llevar `AML.Solution` a estado productivo.

## Convenciones

- **Prioridad:** Alta / Media / Baja
- **Estado sugerido:** Pendiente / En progreso / Hecho
- **Formato:** historia + criterios de aceptación

---

## Épica 1: Administración de Clientes y Servicios

### US-AML-001 — Crear cliente BPO
**Como** administrador  
**Quiero** registrar un cliente con código único y nombre  
**Para** habilitar servicios por tenant en la plataforma.

- Prioridad: **Alta**
- Estado sugerido: Pendiente

**Criterios de aceptación**
- Se puede crear cliente desde API Admin.
- `Code` no permite duplicados.
- Se guarda `CreatedAtUtc` y estado activo por defecto.
- Devuelve `201 Created` con el recurso creado.

### US-AML-002 — Configurar servicio por intención
**Como** administrador  
**Quiero** asociar un servicio a un cliente con `intentKey`  
**Para** enrutar correctamente solicitudes desde IA.

- Prioridad: **Alta**
- Estado sugerido: Pendiente

**Criterios de aceptación**
- Se registra servicio con `clientId`, `intentKey`, prioridad y tipo.
- La combinación `(clientId, intentKey)` debe ser única.
- Se puede activar/desactivar servicio sin eliminarlo.

### US-AML-003 — Gestionar endpoint, auth, headers y mapeos
**Como** administrador  
**Quiero** configurar endpoint y credenciales del servicio  
**Para** ejecutar integraciones sin escribir código nuevo.

- Prioridad: **Alta**
- Estado sugerido: Pendiente

**Criterios de aceptación**
- Se puede configurar URL, método HTTP y timeout.
- Se soporta auth `ApiKey`, `Basic` y `OAuth2`.
- Se permiten headers personalizados y mappings request/response.
- Se validan campos obligatorios por tipo de auth.

---

## Épica 2: Orquestación y Ejecución de Integraciones

### US-AML-004 — Procesar intención de negocio
**Como** motor de IA  
**Quiero** enviar `clientId + intent + params`  
**Para** recibir una respuesta estandarizada desde AML.

- Prioridad: **Alta**
- Estado sugerido: Pendiente

**Criterios de aceptación**
- Endpoint de integración acepta contrato estándar.
- Orquestador busca servicio activo por cliente e intención.
- Si no existe servicio, responde error funcional controlado.

### US-AML-005 — Ejecutar integración dinámica
**Como** orquestador  
**Quiero** usar `DynamicAdapter` con configuración de BD  
**Para** llamar APIs externas sin adaptar código por cliente.

- Prioridad: **Alta**
- Estado sugerido: Pendiente

**Criterios de aceptación**
- DynamicAdapter arma request con auth, headers y payload mapeado.
- Respeta timeout configurado por servicio.
- Devuelve respuesta en formato estándar AML.

### US-AML-006 — Manejar fallback ante fallas externas
**Como** plataforma AML  
**Quiero** gestionar fallos de API externa  
**Para** evitar interrupciones no controladas al canal de atención.

- Prioridad: **Media**
- Estado sugerido: Pendiente

**Criterios de aceptación**
- Errores de red/timeout se traducen a respuesta controlada.
- Se registra causa técnica para diagnóstico.
- El consumidor recibe código y mensaje consistentes.

---

## Épica 3: Seguridad y Gobierno

### US-AML-007 — Autenticación y autorización robusta
**Como** responsable de seguridad  
**Quiero** proteger endpoints admin e integración  
**Para** asegurar acceso solo a actores autorizados.

- Prioridad: **Alta**
- Estado sugerido: Pendiente

**Criterios de aceptación**
- Se valida JWT en middleware.
- Se aplican políticas/roles para endpoints admin.
- Solicitudes sin token válido son rechazadas con `401/403`.

### US-AML-008 — Protección de secretos
**Como** responsable de seguridad  
**Quiero** almacenar credenciales encriptadas  
**Para** evitar exposición de información sensible.

- Prioridad: **Alta**
- Estado sugerido: Pendiente

**Criterios de aceptación**
- Secretos se guardan cifrados en persistencia.
- DTOs admin no exponen valores sensibles.
- Rotación de secretos no requiere cambiar contratos públicos.

---

## Épica 4: Observabilidad y Operación

### US-AML-009 — Trazabilidad por correlación
**Como** equipo de soporte  
**Quiero** rastrear cada integración por `CorrelationId`  
**Para** diagnosticar incidentes rápidamente.

- Prioridad: **Alta**
- Estado sugerido: Pendiente

**Criterios de aceptación**
- Cada ejecución genera log con tiempos, estado y payload resumido.
- Se puede consultar historial por `CorrelationId`.
- Log incluye servicio, cliente, duración y resultado.

### US-AML-010 — Métricas y salud operativa
**Como** equipo de operación  
**Quiero** tener indicadores de disponibilidad y latencia  
**Para** anticipar degradaciones del servicio.

- Prioridad: **Media**
- Estado sugerido: Pendiente

**Criterios de aceptación**
- Endpoint health operativo.
- Métricas de éxito/error y latencia p95 disponibles.
- Alertas básicas para fallos recurrentes.

---

## Épica 5: Calidad y Entrega

### US-AML-011 — Cobertura de pruebas por capa
**Como** equipo de desarrollo  
**Quiero** pruebas unitarias y de integración  
**Para** reducir regresiones y estabilizar releases.

- Prioridad: **Alta**
- Estado sugerido: Pendiente

**Criterios de aceptación**
- Pruebas unitarias para reglas, mappings y orquestador.
- Pruebas de integración para repositorios y API.
- Pipeline falla si pruebas críticas no pasan.

### US-AML-012 — Flujo de migración Prototype -> Solution
**Como** líder técnico  
**Quiero** un criterio formal de promoción  
**Para** pasar integraciones validadas del sandbox al core productivo.

- Prioridad: **Media**
- Estado sugerido: Pendiente

**Criterios de aceptación**
- Existe checklist de promoción (seguridad, resiliencia, observabilidad).
- Contrato funcional validado en prototipo.
- Integración productiva documentada y versionada.

---

## Recomendación de priorización inicial (MVP productivo)

1. US-AML-001, 002, 003  
2. US-AML-004, 005  
3. US-AML-007, 008  
4. US-AML-009, 011  
5. US-AML-006, 010, 012
