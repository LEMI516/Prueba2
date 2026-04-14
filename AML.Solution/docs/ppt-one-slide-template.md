# Plantilla Visual para PowerPoint (1 Slide) — Arquitectura AML

Esta guía te permite armar una lámina ejecutiva en 5-7 minutos, con diseño limpio y mensaje de negocio.

## 1) Configuración de la diapositiva

- **Formato:** 16:9 (pantalla ancha)
- **Fondo:** blanco (`#FFFFFF`)
- **Márgenes internos sugeridos:** 0.7 cm

### Tipografía recomendada
- **Título:** Segoe UI Semibold 34 pt (o Calibri Bold 34 pt)
- **Subtítulo:** Segoe UI 16 pt
- **Caja (título interno):** Segoe UI Semibold 13 pt
- **Caja (contenido):** Segoe UI 11 pt
- **Pie/KPI:** Segoe UI 10 pt

## 2) Paleta de colores (copiar/pegar en PPT)

- **Primario AML (azul):** `#0B5FFF`
- **Azul oscuro:** `#123B7A`
- **Cian acento:** `#00A3FF`
- **Verde resultado:** `#1B9E5A`
- **Gris texto secundario:** `#5F6B7A`
- **Gris borde suave:** `#D9E1EC`
- **Fondo tarjetas suaves:** `#F6F9FF`

## 3) Estructura visual de la lámina

## Encabezado (arriba, ancho completo)
- **Título:** `Arquitectura AML — Vista Ejecutiva`
- **Subtítulo:** `Integración multi-cliente por configuración, no por desarrollo específico`

## Cuerpo (centro, flujo horizontal con 6 bloques)
Distribuye 6 cajas horizontales con flechas entre ellas:

1. **Canales**  
   `Genesys + IA`
2. **Gateway AML**  
   `API única de entrada`
3. **Orquestador AML**  
   `Reglas + resolución por cliente/intención`
4. **Dynamic Adapter**  
   `Auth + Headers + Mapping dinámico`
5. **APIs Cliente BPO**  
   `Sistemas externos del cliente`
6. **Base AML (abajo conectado al Orquestador)**  
   `Servicios configurados + trazabilidad`

### Recomendación visual por bloque
- Bordes redondeados (radio 8-12 px)
- Relleno claro (`#F6F9FF`)
- Borde `#D9E1EC`
- Título en azul oscuro (`#123B7A`)
- Texto interno en gris secundario (`#5F6B7A`)

### Flechas
- Color: `#0B5FFF`
- Grosor: 2.25 pt
- Estilo: simple, de izquierda a derecha

## Pie (parte inferior)
Agregar 3 mini-KPIs en horizontal:

- **Onboarding:** `Más rápido`
- **Costo integración:** `Menor`
- **Escalabilidad:** `Mayor`

Usar verde `#1B9E5A` para resaltar cada valor.

---

## 4) Texto exacto para copiar en las cajas

### Caja 1
**Canales de Atención**  
Genesys + IA

### Caja 2
**AML Gateway**  
API única de entrada

### Caja 3
**Orquestador AML**  
Reglas de negocio + resolución de servicio

### Caja 4
**Dynamic Adapter**  
Autenticación + headers + mapeo de campos

### Caja 5
**APIs Cliente BPO**  
Servicios externos del cliente

### Caja 6 (debajo de Orquestador)
**Base de Datos AML**  
Clientes, servicios, configuración y logs

---

## 5) Guion de exposición (45 segundos)

> “AML centraliza la integración entre canales de atención (Genesys e IA) y los sistemas del cliente.  
> Todo entra por el Gateway, luego el Orquestador decide qué servicio ejecutar según cliente e intención.  
> El Dynamic Adapter aplica autenticación, headers y mapeo de campos de forma configurable, sin construir integraciones a medida por cada cliente.  
> La base AML conserva la configuración y la trazabilidad operativa.  
> El resultado: onboarding más rápido, menor costo de integración y mejor escalabilidad.”

---

## 6) Variante “más ejecutiva” (opcional)

Si quieres una lámina aún más simple, deja solo 4 bloques:

`Canales -> AML Platform -> APIs Cliente`  
y una caja inferior de soporte: `Configuración + Trazabilidad`.

Esto funciona bien para audiencias no técnicas (dirección/comercial).
