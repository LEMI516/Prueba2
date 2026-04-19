# Release checklist APIM (DEV / QA / PROD)

Checklist operativo para despliegues de APIs en APIM, incluyendo escenarios con cifrado de payload.

## DEV

- [ ] OpenAPI validado (sin cambios breaking no controlados).
- [ ] Politicas base aplicadas (auth, throttling, observabilidad).
- [ ] Placeholders reemplazados por valores reales de DEV.
- [ ] Pruebas de respuesta 2xx, 4xx y 5xx.
- [ ] Verificacion de que no se registra payload sensible en logs.

## QA

- [ ] Pruebas funcionales completas (casos felices + negativos).
- [ ] Pruebas de autorizacion por scopes/claims.
- [ ] Pruebas de rate limit y cuota.
- [ ] Pruebas de transformacion (mapping/masking) con datos reales anonimizados.
- [ ] En caso de cifrado de payload:
  - [ ] APIM exige marca de cifrado.
  - [ ] Backend desencripta correctamente.
  - [ ] Requests sin cifrado son rechazadas.
- [ ] Documentacion de Developer Portal actualizada.

## PROD

- [ ] Aprobacion formal de cambio.
- [ ] Pipeline CI/CD ejecutado sin cambios manuales en portal.
- [ ] TLS 1.2+ forzado.
- [ ] mTLS habilitado para canales regulados B2B (si aplica).
- [ ] Alertas activas: 5xx, latencia, throttling, auth failures.
- [ ] Runbook de rollback validado.
- [ ] Plan de rotacion de claves/certificados activo.
- [ ] Comunicacion de version/deprecacion enviada a consumidores (si aplica).
