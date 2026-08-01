# ADR-10: Rediseño de interfaz — sidebar, layout de autenticación y estados vacíos

| Campo  | Valor |
|--------|-------|
| Autor  | Euruviel Marquez |
| Fecha  | 31/07/2026 |
| Estado | `Aceptado` |
| Relacionado con | ADR-02 (arquitectura de capas), ADR-03 (estilo arquitectónico) |

---

## Contexto

Hasta antes de este cambio, DevPath usaba la barra de navegación por
defecto de las plantillas de ASP.NET Core MVC (`_Layout.cshtml` con
`navbar-light` superior) y vistas con estilo de scaffold sin pulir
(formularios en inglés, tablas planas de Bootstrap, emojis como
íconos, mensajes de "no hay datos" en `alert-info` genérico). El
proyecto era funcional pero se veía como un ejercicio de clase, no
como un producto terminado — un objetivo explícito de esta etapa
final era que DevPath se pudiera mostrar como algo profesional.

## Decisión

Se rediseñó la interfaz completa manteniendo Bootstrap 5 como base,
con los siguientes cambios:

1. **Layout de aplicación con sidebar fijo** (`_Layout.cshtml` +
   `_Sidebar.cshtml`): barra lateral oscura persistente con navegación
   por ícono (Bootstrap Icons), en vez del navbar superior por defecto.
   Incluye versión `offcanvas` para dispositivos móviles.
2. **Layout separado para autenticación** (`_AuthLayout.cshtml`): las
   vistas de Login y Register ya no comparten el sidebar de la
   aplicación — usan una pantalla centrada de ancho fijo con fondo
   degradado, sin distraer con navegación que aún no aplica (el
   usuario no ha iniciado sesión).
3. **Tarjetas de resumen (`devpath-stat-card`)**: se reemplazaron las
   tablas y listas planas en Habilidades, Recursos, Registros y
   Estadísticas por tarjetas con ícono, número y etiqueta — el mismo
   componente reutilizado en las cuatro vistas para mantener
   consistencia visual.
4. **Estados vacíos con guía de acción**: los mensajes de "no hay
   datos todavía" pasaron de un `alert-info` de una línea a un
   componente (`devpath-empty`) con ícono, título, texto de ayuda y
   botón de acción hacia el flujo de creación correspondiente.
5. **Reemplazo de emojis por Bootstrap Icons** en toda la aplicación,
   para evitar inconsistencias de renderizado entre sistemas
   operativos y dar una apariencia más uniforme.

## Alternativas consideradas

- **Mantener el navbar superior por defecto**: se descartó porque no
  escala bien a más secciones y no se parece a los paneles de
  administración/dashboards que sirvieron de referencia visual.
- **Usar una librería de UI de terceros (ej. MudBlazor, Material)**:
  se descartó porque implicaría reescribir la capa de presentación
  completa y el curso ya evaluó el proyecto sobre Bootstrap 5 desde
  el inicio; el costo no se justificaba para el tiempo disponible.

## Consecuencias

**Positivas:** la aplicación se ve como un producto terminado, con
una identidad visual consistente entre todas las vistas; la
navegación por sidebar escala mejor si se agregan más secciones a
futuro; los estados vacíos ahora guían al usuario en vez de solo
informar.

**Negativas:** se agregó CSS propio (`site.css` pasó de ~50 líneas a
más de 300) que ahora hay que mantener manualmente; el layout de
sidebar añade una capa extra de partials (`_Sidebar.cshtml`) que
cualquier cambio de navegación debe actualizar en dos lugares
(sidebar de escritorio y `offcanvas` móvil, aunque ambos reutilizan
el mismo partial para evitar duplicar el HTML).

## Relación con otros ADR

Este cambio es puramente de presentación — no modifica el
aislamiento por usuario (ADR-08) ni la arquitectura de capas
(ADR-02); todos los controladores y consultas siguen intactos.

## Referencias

- Diagramas C4 actualizados: [`docs/C4-Diagramas.md`](C4-Diagramas.md)
- Evaluación ATAM de la entrega final: [`docs/ATAM-Euruviel-Marquez.md`](ATAM-Euruviel-Marquez.md)
