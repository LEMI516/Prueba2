# Escenarios APIM - Catalogo operativo

Esta guia resume los escenarios mas comunes para implementar APIs en Azure API Management con menor riesgo de error.

## Campos de referencia

- **Encryption in transit**: TLS/mTLS.
- **Payload encryption**: cifrado de aplicacion (JWE/PGP/cifrado propio).
- **Auth model**: API Key, JWT, OAuth2 token opaco, mTLS.
- **Methods**: GET/POST/PUT/PATCH/DELETE segun uso.
- **Recommended packs**: politicas reutilizables en `../policies/packs`.

## Escenarios

### ESC-001 Public GET con cache
- Uso: catalogos o contenido no sensible.
- Requiere: cache controlado + throttling.
- Riesgo comun: cachear datos personalizados.

### ESC-002 Public POST simple
- Uso: formularios/registro.
- Requiere: auth fuerte y limites de trafico.
- Riesgo comun: aceptar escritura sin anti-abuso.

### ESC-003 Internal microservices
- Uso: consumo app-to-app interno.
- Requiere: validacion estricta de claims/audience.
- Riesgo comun: validar token sin revisar `aud` y scopes.

### ESC-004 Partner standard
- Uso: B2B con seguridad intermedia.
- Requiere: JWT + subscription + cuota.
- Riesgo comun: una sola suscripcion para varios partners.

### ESC-005 Partner strict (regulated)
- Uso: integraciones sensibles.
- Requiere: mTLS + JWT + allowlist.
- Riesgo comun: falta de consistencia de certificados entre ambientes.

### ESC-006 Encrypted payload pass-through
- Uso: payload llega cifrado a nivel aplicacion.
- Requiere: APIM no desencripta; backend desencripta con Key Vault/HSM.
- Riesgo comun: intentar desencriptar en APIM con llaves embebidas.

### ESC-007 OAuth2 opaque token
- Uso: IdP devuelve token no JWT.
- Requiere: introspeccion remota.
- Riesgo comun: usar `validate-jwt` para token opaco.

### ESC-008 User delegated auth (web/mobile)
- Uso: usuario final.
- Requiere: Authorization Code + PKCE en cliente; validacion de scopes.
- Riesgo comun: no separar scopes de lectura/escritura.

### ESC-009 Incoming webhook
- Uso: recepcion de eventos externos.
- Requiere: validacion de firma y antireplay.
- Riesgo comun: no validar timestamp/nonce.

### ESC-010 Legacy backend auth bridge
- Uso: APIM moderno frente a backend legado.
- Requiere: transformar auth de entrada hacia backend.
- Riesgo comun: credenciales en texto plano en politica.

### ESC-011 GET high-volume cache
- Uso: lectura intensiva.
- Requiere: politicas de cache y variacion correctas.
- Riesgo comun: leakage entre usuarios por cache mal configurado.

### ESC-012 Critical idempotent write
- Uso: pagos/ordenes/operaciones criticas.
- Requiere: idempotencia y auditoria.
- Riesgo comun: duplicados por reintentos de POST.

### ESC-013 File upload
- Uso: carga de documentos.
- Requiere: limite de tamano, content-type y timeout.
- Riesgo comun: saturacion por archivos grandes.

### ESC-014 HTTP no encryption (blocked)
- Uso: no permitido en produccion.
- Requiere: bloqueo o redireccion a HTTPS.
- Riesgo comun: endpoints inseguros activos por error.
