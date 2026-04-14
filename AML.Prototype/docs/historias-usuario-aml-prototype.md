# Historias de Usuario — AML.Prototype (Sandbox de Integraciones)

Este backlog está orientado a validar integraciones de forma rápida y aislada, sin afectar el proyecto principal.

## Convenciones

- **Prioridad:** Alta / Media / Baja
- **Estado sugerido:** Pendiente / En progreso / Hecho
- Historias enfocadas en velocidad de aprendizaje y validación temprana.

---

## Épica P1: Configuración rápida de integraciones

### US-PROT-001 — Crear definición de integración
**Como** ingeniero de integración  
**Quiero** crear una integración por `key` con URL, path y método  
**Para** probar conectividad sin depender del core productivo.

- Prioridad: **Alta**
- Estado sugerido: Pendiente

**Criterios de aceptación**
- API permite crear definición con `POST /api/prototype/integrations/{key}`.
- La definición incluye base URL, path, método y timeout.
- Si la key existe, responde conflicto.

### US-PROT-002 — Actualizar y eliminar definición
**Como** ingeniero de integración  
**Quiero** editar y borrar definiciones rápidamente  
**Para** iterar sobre cambios de API externa sin fricción.

- Prioridad: **Alta**
- Estado sugerido: Pendiente

**Criterios de aceptación**
- Se puede upsert con `PUT` y borrar con `DELETE`.
- `GET` lista todas las definiciones activas.
- Cambios quedan reflejados de inmediato en ejecuciones siguientes.

---

## Épica P2: Ejecución dinámica y validación funcional

### US-PROT-003 — Ejecutar integración por `integrationKey`
**Como** ingeniero de integración  
**Quiero** ejecutar una integración con path/query/header/payload  
**Para** validar contrato real contra APIs externas o mocks.

- Prioridad: **Alta**
- Estado sugerido: Pendiente

**Criterios de aceptación**
- Endpoint `POST /api/prototype/executions/run` ejecuta llamada HTTP.
- Soporta reemplazo de path params y query params.
- Retorna estado HTTP, duración y body de respuesta.

### US-PROT-004 — Validar timeout y errores controlados
**Como** ingeniero de integración  
**Quiero** observar fallas de timeout y conectividad  
**Para** ajustar configuración antes de mover cambios al core.

- Prioridad: **Alta**
- Estado sugerido: Pendiente

**Criterios de aceptación**
- Si excede timeout, responde error claro.
- Si API externa falla, devuelve mensaje legible y no rompe la API del prototipo.
- Se registra duración de ejecución incluso en error.

---

## Épica P3: Pruebas con mocks y colaboración de equipo

### US-PROT-005 — Sembrar integraciones demo locales
**Como** equipo de integración  
**Quiero** cargar integraciones mock predefinidas  
**Para** iniciar pruebas en minutos.

- Prioridad: **Media**
- Estado sugerido: Pendiente

**Criterios de aceptación**
- Endpoint `seed-local` crea definiciones demo (billing/payment).
- Flujo completo se puede probar sin dependencias externas.
- README documenta pasos exactos.

### US-PROT-006 — Compartir casos de prueba reproducibles
**Como** miembro del equipo  
**Quiero** requests de ejemplo versionados  
**Para** repetir validaciones y facilitar handoff entre desarrolladores.

- Prioridad: **Media**
- Estado sugerido: Pendiente

**Criterios de aceptación**
- Existe archivo `.http` con ejemplos funcionales.
- Casos cubren GET y POST mock.
- Cualquier desarrollador puede ejecutar pruebas desde entorno limpio.

---

## Épica P4: Transición hacia AML.Solution

### US-PROT-007 — Checklist de promoción a proyecto principal
**Como** líder técnico  
**Quiero** un criterio formal para promover integraciones validadas  
**Para** evitar retrabajo al migrar de prototipo a producción.

- Prioridad: **Alta**
- Estado sugerido: Pendiente

**Criterios de aceptación**
- Existe checklist con:
  - contrato validado,
  - estrategia de auth,
  - mappings mínimos,
  - manejo de errores,
  - consideraciones de seguridad.
- La historia de promoción referencia la integración y evidencias.

### US-PROT-008 — Paquete de evidencia técnica por integración
**Como** ingeniero de integración  
**Quiero** entregar evidencia de request/response y comportamiento  
**Para** acelerar implementación en `AML.Solution`.

- Prioridad: **Media**
- Estado sugerido: Pendiente

**Criterios de aceptación**
- Se documentan entradas/salidas esperadas.
- Se reportan errores observados y mitigaciones.
- Se adjuntan payloads de prueba representativos.

---

## Priorización recomendada para ciclo corto

1. US-PROT-001, 002, 003, 004  
2. US-PROT-005, 006  
3. US-PROT-007, 008

Con esta secuencia, el prototipo genera valor temprano y prepara migración ordenada al proyecto principal.
