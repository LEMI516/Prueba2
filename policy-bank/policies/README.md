# Politicas APIM - Banco reutilizable

Esta carpeta contiene politicas listas para copiar/adaptar en Azure API Management.

## Estructura

```text
policies/
  README.md
  base/
    base-policy.xml
    base-policy-encrypted.xml
  packs/
    PACK-BASE.xml
    PACK-AUTH-JWT.xml
    PACK-AUTH-OAUTH-OPAQUE.xml
    PACK-ENCRYPTION-PAYLOAD.xml
    PACK-THROTTLING.xml
    PACK-CACHE-GET.xml
    PACK-B2B-STRICT.xml
    PACK-OBSERVABILITY.xml
  transformations/
    MAP-ONLY-FIELDS.xml
    MASK-CARD-LAST4.xml
    BLOCK-SENSITIVE-FIELDS.xml
```

## Reglas de uso

1. No subir secretos en texto plano al XML.
2. Usar Named Values y Key Vault para credenciales.
3. Aplicar politicas por scope correcto (Global, Product, API, Operation).
4. Probar siempre con datos anonimizados en DEV/QA.
5. Si hay payload cifrado extremo a extremo, APIM debe operar en pass-through.

## Orden de composicion recomendado

1. PACK-BASE
2. PACK-AUTH-JWT o PACK-AUTH-OAUTH-OPAQUE
3. PACK-ENCRYPTION-PAYLOAD (si aplica)
4. PACK-THROTTLING
5. PACK-CACHE-GET (solo GET y no sensible)
6. PACK-B2B-STRICT (si aplica)
7. PACK-OBSERVABILITY
8. Transformation policies (solo cuando se requiere mapping/masking)

## Placeholders

- `{{tenant-id}}`
- `{{api-audience-app-id-uri}}`
- `{{backend-base-url}}`
- `{{introspection-url}}`
- `{{introspection-client-id}}`
- `{{introspection-client-secret}}`

Reemplaza placeholders por valores de cada ambiente.
