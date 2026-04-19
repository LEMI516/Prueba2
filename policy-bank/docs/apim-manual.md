# Manual practico de Azure API Management (APIM)

## Que es APIM y para que sirve

Azure API Management permite publicar, proteger, transformar, observar y gobernar APIs con un gateway central.

## Componentes clave

- Gateway
- Management plane
- Developer portal
- APIs y operations
- Products y subscriptions
- Policies

## Flujo recomendado

1. Crear instancia APIM.
2. Importar API (OpenAPI/WSDL).
3. Configurar versionado (version sets/revisions).
4. Asociar APIs a products.
5. Aplicar seguridad (JWT/OAuth2, mTLS, rate limit, quota).
6. Configurar observabilidad.
7. Publicar documentacion.
8. Desplegar via IaC y CI/CD.

## Mejores practicas resumidas

- API-first con OpenAPI.
- Versionado explicito para cambios breaking.
- Secretos en Key Vault/Named Values.
- Validacion de claims (audience, issuer, scopes/roles).
- Trazabilidad con `x-correlation-id`.
- Politicas reutilizables por pack.

## Cifrado y datos sensibles

- TLS 1.2+ obligatorio.
- mTLS para integraciones B2B sensibles.
- Si el payload llega cifrado (JWE/PGP), APIM opera pass-through.
- Desencriptacion idealmente en backend con Key Vault o HSM.

