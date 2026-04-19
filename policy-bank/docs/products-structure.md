# Estructura de Products y Subscriptions (Azure API Management)

## Modelo de productos

### public-basic

- Uso: consumidores externos de bajo volumen.
- Seguridad: Subscription Key + JWT opcional segun riesgo.
- Limites: bajos.

### partners-standard

- Uso: integraciones B2B.
- Seguridad: JWT + Subscription Key + mTLS recomendado.
- Limites: medios.

### internal

- Uso: apps y microservicios internos.
- Seguridad: JWT de Entra ID.
- Limites: altos controlados.

### premium

- Uso: alto volumen y SLA diferenciado.
- Seguridad: JWT + Subscription Key + mTLS segun necesidad.
- Limites: altos.

## Grupos recomendados

- developers-internal
- partners
- premium-clients
- admins-apim

## Convencion de suscripciones

Formato:

```text
sub-<org>-<app>-<env>-<product>
```

Ejemplos:

- sub-acme-web-prod-public-basic
- sub-foo-crm-qa-partners-standard
- sub-core-payments-prod-internal

## Buenas practicas de suscripcion

- Una suscripcion por aplicacion y ambiente.
- No compartir keys entre aplicaciones.
- Rotacion de keys periodica.
- Etiquetas: owner, env, costCenter, dataClassification.

## Datos encriptados

- En transito: TLS 1.2+ obligatorio.
- B2B sensible: mTLS recomendado.
- Payload cifrado (JWE/PGP/cifrado propio): APIM opera en pass-through.
- Desencriptacion: backend con claves en Key Vault o Managed HSM.
