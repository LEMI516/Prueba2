# Paso a paso: crear banco APIM desde cero y reutilizar packs

Esta guia describe como levantar un banco de politicas APIM desde cero y convertirlo en fragments reutilizables.

## 1) Preparar base en repositorio

1. Crear carpeta `policy-bank/` con subcarpetas:
   - `scenarios/`
   - `docs/`
   - `policies/base/`
   - `policies/packs/`
   - `policies/transformations/`
   - `policies/fragments/`
2. Crear matriz de escenarios en `scenarios/scenarios.csv`.
3. Definir politicas base y packs.
4. Versionar todo en Git y aprobar cambios por PR.

## 2) Preparar Azure API Management

1. Crear instancia APIM (o usar una existente).
2. Definir `Named Values`:
   - `tenant-id`
   - `api-audience-app-id-uri`
   - `backend-base-url`
   - `oauth-introspection-url`
   - `introspection-basic-auth`
3. Guardar secretos en Key Vault y referenciarlos desde Named Values.
4. Configurar diagnostico (Application Insights o Log Analytics).

## 3) Crear Products y Subscriptions

1. Crear products recomendados:
   - `public-basic`
   - `partners-standard`
   - `internal`
   - `premium`
2. Asociar APIs a products.
3. Crear subscriptions por app y ambiente:
   - `sub-<org>-<app>-<env>-<product>`

## 4) Importar API y versionado

1. Importar API (OpenAPI/WSDL).
2. Configurar Version Set para cambios breaking.
3. Usar Revisions para cambios no breaking.

## 5) Reutilizar packs con Policy Fragments

1. Ir a APIM -> Policies -> Policy fragments.
2. Crear fragmentos usando archivos de `policies/fragments/`.
3. Nombrar fragmentos igual que los IDs propuestos.
4. Componer politica por API u operacion con `include-fragment`.

Ejemplo de composicion:

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

## 6) Escenario con payload cifrado

Para operaciones POST/PUT/PATCH con payload cifrado:

1. Incluir `FRAG-IN-ENCRYPTION-PAYLOAD`.
2. Exigir header `x-payload-encrypted=true`.
3. Validar content type (`application/jose` o `application/octet-stream`).
4. Mantener APIM en pass-through (sin desencriptar).
5. Desencriptar en backend con Key Vault/HSM.

## 7) Escenario con masking de tarjeta

Para devolver solo ultimos 4 digitos:

1. Incluir fragmento `FRAG-OUT-MASK-CARD-LAST4`.
2. Verificar que PAN completo no salga en respuesta.
3. Asegurar que PAN/CVV no se registren en logs.

## 8) Pruebas minimas antes de promover

1. Auth valida e invalida.
2. Rate limit y cuota.
3. Mapping/masking de campos sensibles.
4. Flujo cifrado/no cifrado.
5. Respuestas 2xx, 4xx, 5xx.

## 9) Promocion de cambios

1. DEV -> QA -> PROD con pipeline CI/CD.
2. Sin cambios manuales en portal en ambientes productivos.
3. Rollback definido (revision/version previa).

## 10) Recomendaciones de gobierno

- Mantener un owner por pack.
- Definir checklist de aprobacion para cambios de seguridad.
- Versionar fragments (`v1`, `v2`) cuando haya cambios de contrato.
- Preferir componer politicas por fragmentos en vez de duplicar XML.
