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
  fragments/
    inbound/
      FRAG-IN-BASE.xml
      FRAG-IN-AUTH-JWT.xml
      FRAG-IN-AUTH-OAUTH-OPAQUE.xml
      FRAG-IN-ENCRYPTION-PAYLOAD.xml
      FRAG-IN-THROTTLING.xml
      FRAG-IN-B2B-STRICT.xml
    outbound/
      FRAG-OUT-BASE.xml
      FRAG-OUT-CACHE-GET.xml
      FRAG-OUT-MAP-ONLY-FIELDS.xml
      FRAG-OUT-MASK-CARD-LAST4.xml
      FRAG-OUT-BLOCK-SENSITIVE-FIELDS.xml
    on-error/
      FRAG-ERR-OBSERVABILITY.xml
  compositions/
    COMP-ESC-001-public-get-cache.xml
    COMP-ESC-006-encrypted-payload.xml
```

## Reglas de uso

1. No subir secretos en texto plano al XML.
2. Usar Named Values y Key Vault para credenciales.
3. Aplicar politicas por scope correcto (Global, Product, API, Operation).
4. Probar siempre con datos anonimizados en DEV/QA.
5. Si hay payload cifrado extremo a extremo, APIM debe operar en pass-through.

## Reutilizacion recomendada (fragments)

1. Crear Policy Fragments en APIM con el contenido de `policies/fragments`.
2. Asignar IDs estables (por ejemplo, `FRAG-IN-AUTH-JWT`).
3. Componer politicas por API/Product usando `include-fragment`.
4. Tomar ejemplos de `policies/compositions`.

Ejemplo:

```xml
<policies>
  <inbound>
    <base />
    <include-fragment fragment-id="FRAG-IN-BASE" />
    <include-fragment fragment-id="FRAG-IN-AUTH-JWT" />
    <include-fragment fragment-id="FRAG-IN-THROTTLING" />
  </inbound>
  <backend>
    <base />
  </backend>
  <outbound>
    <base />
    <include-fragment fragment-id="FRAG-OUT-BASE" />
  </outbound>
  <on-error>
    <base />
    <include-fragment fragment-id="FRAG-ERR-OBSERVABILITY" />
  </on-error>
</policies>
```

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
- `{{oauth-introspection-url}}`
- `{{introspection-basic-auth}}`

Reemplaza placeholders por valores de cada ambiente.
