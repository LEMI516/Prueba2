# Policy Bank - Azure API Management

Este paquete centraliza escenarios, politicas y documentacion para reducir errores comunes en diseno y operacion de APIs en Azure API Management (APIM).

## Objetivos

- Estandarizar configuraciones de seguridad, transformacion y observabilidad.
- Reutilizar politicas por "packs" segun escenario.
- Evitar exposicion de datos sensibles (PII, PAN, secretos).
- Acelerar implementaciones con una base lista para copiar/adaptar.

## Estructura

```text
policy-bank/
  README.md
  scenarios/
    scenarios.csv
    scenarios.md
  docs/
    apim-manual.md
    products-structure.md
    release-checklist.md
    architecture-notes.md
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

## Como usar este banco

1. Elegir el escenario en `scenarios/scenarios.csv`.
2. Tomar los packs recomendados para ese escenario.
3. Componer una politica final por API/Product/Operation.
4. Ajustar placeholders (`{{tenant-id}}`, `{{backend-base-url}}`, etc.).
5. Probar en DEV/QA con `docs/release-checklist.md`.

## Reglas de oro

- OAuth2 no es lo mismo que JWT (OAuth2 define flujos; JWT es formato de token).
- Nunca hardcodear secretos en XML.
- En datos de tarjeta, exponer solo `last4` o token, nunca PAN completo.
- Si el payload llega cifrado extremo a extremo, APIM debe operar en pass-through.
- Aplicar `rate-limit` y `quota` tambien en endpoints POST.

## Nota sobre cumplimiento

Este paquete es una base tecnica. Debe ser revisado con seguridad/compliance para normativas como PCI DSS, Ley de Proteccion de Datos y politicas internas.
