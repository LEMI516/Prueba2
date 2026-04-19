# Notas de arquitectura para APIM

## Principios de diseno

- API-first con OpenAPI como contrato fuente.
- Separacion de responsabilidades:
  - APIM: seguridad perimetral, control de trafico, transformaciones ligeras.
  - Backend: logica de negocio y transformaciones complejas.
- Infraestructura como codigo para evitar drift de configuracion.

## Cifrado y datos sensibles

- Cifrado en transito: TLS 1.2+ obligatorio.
- Para B2B sensible: mTLS recomendado/obligatorio.
- Para payload cifrado (JWE/PGP/cifrado propio), preferir pass-through en APIM.
- Claves y certificados en Key Vault / Managed HSM.

## Logging y trazabilidad

- Inyectar `x-correlation-id` en toda llamada.
- No registrar PAN completo ni payload sensible en texto plano.
- Centralizar logs en App Insights / Log Analytics con filtros de PII.

## Estrategia de versionado

- Revisions para cambios no breaking.
- Versions para cambios breaking.
- Politica de deprecacion documentada y comunicada.

## Estrategia de pruebas

- Pruebas de seguridad: autenticacion, autorizacion, limites de trafico.
- Pruebas de datos sensibles: mascarado, campos bloqueados.
- Pruebas de resiliencia: timeouts, reintentos, fallbacks.
