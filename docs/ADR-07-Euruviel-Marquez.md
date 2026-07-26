# ADR-07: Pruebas Unitarias con xUnit e Integración Continua con GitHub Actions

| Campo  | Valor |
|--------|-------|
| Autor  | Euruviel Marquez |
| Fecha  | 24/07/2026 |
| Estado | `Aceptado` |

---

## Contexto

Hasta ahora, cada cambio en DevPath (refactorizaciones, nuevos patrones
GOF, ajustes en controladores) se verificaba manualmente: correr la
aplicación y clickear a mano. Eso funciona mientras el proyecto es
pequeño, pero no escala — cada vez que se toca algo hay que volver a
probar todo a mano, y es fácil olvidar un caso.

Se necesita una forma de verificar, en segundos y de manera repetible,
que la lógica del sistema sigue comportándose como se espera después
de cada cambio, y que esa verificación ocurra automáticamente en cada
`push` sin depender de que alguien se acuerde de correrla.

---

## Decisión

Se agregan **pruebas unitarias con xUnit** en un proyecto separado
(`DevPath.Tests`) y un **pipeline de Integración Continua (CI)** con
**GitHub Actions** que compila y corre esas pruebas automáticamente en
cada `push`.

### Qué se prueba y por qué se eligieron esas clases

| Clase probada | Por qué |
|---|---|
| `NivelStrategyFactory` (+ `NivelBasicoStrategy`, `NivelIntermedioStrategy`, `NivelAvanzadoStrategy`) | Es el patrón **Strategy + Factory** del sistema. Si la fábrica deja de devolver la estrategia correcta según el nivel, toda la vista de Detalles de una Habilidad muestra información incorrecta (recursos recomendados, color, descripción) sin que se note a simple vista. |
| `Habilidad.PorcentajeProgreso` | Es una propiedad calculada que se usa directamente en la UI para mostrar el avance de una habilidad. Incluye una división que debe manejar el caso de cero recursos sin lanzar excepción — un caso fácil de romper sin darse cuenta al refactorizar. |
| `ProgresoAreaViewModel.PorcentajeCompletado` | Es la lógica detrás del dashboard de Estadísticas. Igual que el caso anterior, protege contra división entre cero y valida el cálculo de porcentaje que ve el usuario en la pantalla principal. |

Las tres clases se eligieron porque son **lógica de dominio pura**:
no dependen de `DevPathContext` ni de la base de datos, por lo que se
pueden probar de forma rápida y aislada, sin necesidad de mocks ni de
una base de datos real. Esto mantiene las pruebas simples y veloces,
ideales para correr en cada push del pipeline.

Todas las pruebas siguen el patrón **Arrange-Act-Assert**.

### Pipeline de CI

El workflow `.github/workflows/ci.yml` se dispara en cada `push` y
ejecuta tres pasos:

1. Restaurar dependencias del proyecto de pruebas (que a su vez
   restaura `DevPath.csproj` por la referencia de proyecto).
2. Compilar.
3. Correr `dotnet test`.

Si cualquier prueba falla, el pipeline marca el commit o Pull Request
en rojo con el log del error exacto — ya no se depende de la confianza
de que "funciona en mi máquina".

---

## Alternativas consideradas

| Alternativa | Por qué se descartó |
|---|---|
| **NUnit o MSTest** | xUnit es el estándar más moderno en la comunidad .NET y es el que se usó en el curso; mantener consistencia entre proyectos facilita el mantenimiento. |
| **Probar los controladores directamente** | Los controladores dependen de `DevPathContext` (EF Core + SQL Server), lo que requeriría una base de datos en memoria o mocks para cada prueba. Se dejó fuera de esta entrega para enfocar las primeras pruebas en la lógica de dominio más crítica y de menor complejidad de setup. Queda como trabajo futuro. |
| **Solo pruebas manuales documentadas en un checklist** | No es repetible ni automático — el problema que se busca resolver es exactamente que nadie se acuerde de correrlas antes de subir un cambio. |
| **CD (despliegue automático) en el mismo pipeline** | Este ADR se enfoca en CI (verificar que el cambio no rompe nada). El despliegue a EC2 ya se hace de forma manual y queda fuera de alcance de esta actividad. |

---

## Consecuencias

**Lo que gano:**

- **Técnica:** Cada `push` verifica automáticamente que el patrón
  Strategy/Factory y los cálculos de progreso siguen funcionando como
  se espera, sin intervención manual.
- **Proceso:** Se elimina la dependencia de la memoria humana para
  acordarse de probar antes de subir un cambio — la máquina lo hace
  siempre, en segundos.
- **Confianza:** Un Pull Request con el check en verde es evidencia
  objetiva de que el cambio no rompió la lógica ya probada.

**Lo que sacrifico o asumo:**

- **Cobertura parcial:** Los controladores y la capa de acceso a
  datos (EF Core) no están cubiertos todavía. Un cambio que rompa un
  controlador no será detectado por este pipeline.
- **Deuda:** Cada nueva clase de lógica de dominio que se agregue
  debería venir acompañada de sus propias pruebas para no perder esta
  cobertura con el tiempo.

---

## Ubicación en el repositorio

```
DevPath.Tests/
├── DevPath.Tests.csproj
├── PatternsTests/
│   └── NivelStrategyFactoryTests.cs
└── ModelsTests/
    ├── HabilidadTests.cs
    └── ProgresoAreaViewModelTests.cs

.github/
└── workflows/
    └── ci.yml

docs/
└── ADR-07-Euruviel-Marquez.md   ← este archivo
```
