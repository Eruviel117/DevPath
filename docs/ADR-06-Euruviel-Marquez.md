# ADR-06: Documentación de Arquitectura con el Modelo C4

| Campo  | Valor |
|--------|-------|
| Autor  | Euruviel Marquez |
| Fecha  | 08/07/2026 |
| Estado | `Aceptado` |

---

## Contexto

A medida que DevPath creció durante el cuatrimestre — pasando de un
CRUD básico a un sistema con autenticación, API REST y patrones GOF —
se volvió necesario documentar la arquitectura de forma que distintas
audiencias pudieran entenderla sin leer el código fuente.

El problema concreto es que un solo diagrama no puede responderle a
todos:

- Se quiere saber qué decisiones arquitectónicas se tomaron.
- Un desarrollador nuevo necesita saber por dónde empezar y cómo está
  organizado el código.
- Alguien sin contexto técnico necesita entender qué hace el sistema
  y quién lo usa.

Se necesitaba una forma de documentar la arquitectura en múltiples
niveles de detalle, cada uno respondiendo preguntas distintas para
audiencias distintas.

---

## Decisión

Se adopta el **Modelo C4** de Simon Brown para documentar la
arquitectura de DevPath en tres niveles. Los diagramas se escriben
como código en formato **Mermaid** dentro de un archivo `.md`
versionado en el repositorio, en lugar de imágenes sueltas que se
desactualizan con el tiempo.

Los tres niveles se documentan en `docs/C4-Diagramas.md`.

---

## Los tres niveles documentados

### Nivel 1 — System Context
**Para quién:** cualquier persona, sin conocimientos técnicos.
**Qué responde:** ¿Qué hace el sistema y quién lo usa?

Muestra DevPath como una caja negra con sus relaciones con el mundo
exterior: el usuario que lo usa y el navegador web como medio de
acceso. No muestra nada de la tecnología interna.

### Nivel 2 — Container
**Para quién:** equipo técnico.
**Qué responde:** ¿En qué piezas técnicas está dividido el sistema?

Muestra las piezas técnicas principales: la Web App en ASP.NET Core
MVC, la API REST, Entity Framework Core como puente y SQL Server como
base de datos. Incluye cómo se comunican entre sí.

### Nivel 3 — Component
**Para quién:** desarrolladores que trabajan en el proyecto.
**Qué responde:** ¿Qué hay dentro de la Web App?

Muestra los componentes internos de la pieza principal: los
controladores MVC, los controladores de API, los patrones GOF
implementados (Strategy y Decorator), los modelos de dominio y el
DevPathContext.

---

## ¿Por qué Mermaid y no draw.io o imágenes?

| Criterio | Mermaid | draw.io / Imágenes |
|---|---|---|
| Versionado en Git |  Es código — se versiona igual que el resto |  Las imágenes son binarias — difíciles de diff |
| Actualización |  Editar texto es inmediato |  Requiere abrir la herramienta y exportar |
| Consistencia |  El diagrama siempre refleja el código al lado |  La imagen puede quedar desactualizada |
| Visibilidad en GitHub |  GitHub renderiza Mermaid nativamente en `.md` |  Las imágenes también se ven |

Mermaid permite que los diagramas sean **documentación viva** —
viven junto al código y se actualizan en el mismo commit que el
código que describen.

---

## Alternativas consideradas

| Alternativa | Por qué se descartó |
|---|---|
| **Solo draw.io** | Los archivos `.drawio` son XML binario difícil de versionar. Cualquier cambio genera un diff ilegible en Git. |
| **PlantUML** | Similar a Mermaid pero requiere un servidor externo para renderizar. Mermaid se renderiza nativamente en GitHub sin dependencias adicionales. |
| **Un solo diagrama** | Un solo diagrama no puede responderle a todas las audiencias al mismo tiempo. El Modelo C4 resuelve exactamente ese problema con niveles de zoom progresivos. |
| **Solo C4 Nivel 1 y 2** | El Nivel 3 es el más valioso para el desarrollo — muestra exactamente cómo están organizados los componentes internos incluyendo los patrones GOF implementados. Omitirlo dejaría sin documentar la parte más técnica del sistema. |

---

## Consecuencias

**Lo que gano:**

- **Técnica:** Cualquier persona que clone el repositorio puede
  entender la arquitectura de DevPath en tres niveles de detalle
  sin necesidad de leer el código fuente.
- **Proceso:** Los diagramas se actualizan en el mismo commit que
  el código — no existe el riesgo de que la documentación quede
  desincronizada con la implementación real.
- **Trazabilidad:** GitHub renderiza los diagramas Mermaid
  directamente en la vista del archivo `.md`, haciendo la
  documentación accesible sin herramientas adicionales.

**Lo que sacrifico o asumo:**

- **Limitación:** Mermaid tiene menos capacidad de personalización
  visual que draw.io — los diagramas se ven más simples. Para
  documentación técnica eso es aceptable; para una presentación
  ejecutiva no lo sería.
- **Deuda:** El Nivel 3 necesita actualizarse cada vez que se
  agrega un nuevo controlador o patrón al sistema. Si no se
  mantiene sincronizado pierde su valor como documentación.

---

## Ubicación en el repositorio

```
docs/
├── C4-Diagramas.md          ← Niveles 1, 2 y 3 en Mermaid
├── ADR-01-Euruviel-Marquez.md
├── ADR-02-...
├── ADR-03-...
├── ADR-04-...
├── ADR-05-...
└── ADR-06-Diagramas-C4.md   ← este archivo
```