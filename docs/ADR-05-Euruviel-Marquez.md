# ADR-05: Integración de Patrones de Diseño GOF en DevPath

| Campo  | Valor |
|--------|-------|
| Autor  | Euruviel Marquez |
| Fecha  | 26/06/2026 |
| Estado | `Propuesto` |

---

## Contexto

DevPath ya tiene una arquitectura en capas funcional con CRUD completo de cuatro entidades, API REST documentada con Swagger  
y filtros de búsqueda. A medida que el sistema crece, aparecen dos problemas concretos que la arquitectura en capas por sí sola no resuelve:

**Problema 1 — Lógica diferenciada por nivel sin estructura**

El modelo `Habilidad` tiene un campo `Nivel` con tres valores posibles: 
Básico, Intermedio y Avanzado. Actualmente ese campo es solo un string 
guardado en la base de datos — no hay ninguna lógica diferenciada según 
el nivel. Si en el futuro se quisiera calcular el tiempo estimado de 
aprendizaje, el número de recursos recomendados, o mostrar una 
descripción distinta según el nivel, habría que agregar condicionales 
(`if/switch`) directamente en el controlador o en la vista. Eso viola 
el principio OCP de SOLID: cada nuevo nivel o regla nueva obligaría a 
modificar código existente.

**Problema 2 — Ausencia de trazabilidad en las operaciones**

No existe ningún mecanismo que registre qué operaciones ocurren en el 
sistema, cuándo ocurren y con qué resultado. Si algo falla — una 
habilidad no se guarda, un recurso se elimina por error — no hay forma 
de saber qué pasó. Agregar esa capacidad directamente en los 
controladores mezclaría lógica de negocio con lógica de registro, 
violando el principio SRP: el controlador tendría dos razones para 
cambiar.

Estas dos necesidades concretas justifican la integración de dos 
patrones de diseño GOF de categorías distintas.

---

## Decisión

Se integran dos patrones GOF en el proyecto DevPath:

- **Strategy** (familia Comportamiento) para encapsular la lógica 
  diferenciada por nivel de habilidad.
- **Decorator** (familia Estructural) para agregar trazabilidad a las 
  operaciones del contexto de datos sin modificar el código existente.

---

## Patrón 1 — Strategy (Comportamiento)

### ¿Qué problema resuelve?

Encapsula la lógica que varía según el nivel de una Habilidad en clases 
separadas e intercambiables, evitando condicionales en el controlador.

### Implementación en DevPath

Se define una interfaz `INivelStrategy` con dos métodos: uno que retorna
una descripción contextual del nivel y otro que indica cuántos recursos
se recomiendan para ese nivel. Tres clases implementan esa interfaz —
`NivelBasicoStrategy`, `NivelIntermedioStrategy` y
`NivelAvanzadoStrategy` — cada una con su propia lógica encapsulada.

Una clase auxiliar `NivelStrategyFactory` actúa como punto único de
selección: recibe el string del nivel almacenado en la base de datos y
devuelve la estrategia correspondiente.

### ¿Por qué Strategy y no un simple switch en el controlador?

Un switch directamente en el controlador funcionaría — pero cada vez 
que se agregue un nuevo nivel o se cambie la lógica de uno existente, 
habría que modificar el controlador. Strategy encapsula ese 
comportamiento en clases independientes: agregar un nivel "Experto" 
significa crear `NivelExpertoStrategy` sin tocar nada más. Eso es OCP 
aplicado.

---

## Patrón 2 — Decorator (Estructural)

### ¿Qué problema resuelve?

Agrega trazabilidad (logging) a las operaciones de la capa de datos sin 
modificar el `DevPathContext` existente ni los controladores.

### Implementación en DevPath

Se crea una interfaz `IDevPathRepository` que define las operaciones 
principales, y una clase `LoggingRepositoryDecorator` que envuelve 
la implementación concreta y agrega logging antes y después de cada 
operación.

El `HabilidadController` delega las operaciones de escritura al 
Decorator en lugar de al contexto directamente.

### ¿Por qué Decorator y no agregar el logging en el controlador?

Agregar logging directamente en el controlador mezclaría dos 
responsabilidades en la misma clase (SRP violado). El Decorator mantiene 
el controlador enfocado en el flujo HTTP, el contexto enfocado en el 
acceso a datos, y el Decorator enfocado exclusivamente en el registro 
de operaciones. Cada clase tiene exactamente una razón para cambiar.

---

## Alternativas consideradas

| Alternativa | Por qué se descartó |
|---|---|
| **Observer (Comportamiento)** | Se evaluó para notificar cuando una Habilidad cambia a "Completado". Se descartó porque no hay un segundo componente real que necesite reaccionar a ese evento en esta versión del sistema — implementarlo sin un caso de uso concreto sería sobreingeniería. |
| **Factory (Creacional) como segundo patrón** | Factory se usa dentro de la implementación de Strategy (NivelStrategyFactory), pero no se documenta como patrón independiente porque su rol es de soporte, no de decisión arquitectónica autónoma. |
| **Singleton para el contexto** | Entity Framework ya maneja el ciclo de vida del DbContext mediante la inyección de dependencias registrada en Program.cs. Agregar un Singleton manualmente duplicaría esa responsabilidad. |


---

## Consecuencias

**Lo que gano:**

- **Técnica (Strategy):** Agregar un nuevo nivel de habilidad en el 
  futuro es tan simple como crear una clase nueva que implemente 
  `INivelStrategy`. El controlador, la vista y el modelo no se tocan.

- **Técnica (Decorator):** Las operaciones críticas de escritura 
  (guardar y eliminar habilidades) quedan trazadas automáticamente 
  con timestamp, sin que los controladores sepan que el logging existe.

- **Proceso:** Ambos patrones demuestran que la arquitectura de DevPath 
  no solo funciona — está diseñada para crecer sin romper lo que ya 
  existe. Eso es lo que distingue un proyecto con arquitectura real de 
  un CRUD básico.

**Lo que sacrifico o asumo:**

- **Complejidad:** Introducir patrones agrega clases al proyecto. Para 
  un sistema del tamaño actual de DevPath, esa complejidad es mayor de 
  lo estrictamente necesario. Se acepta porque el objetivo del proyecto 
  es demostrar comprensión de diseño orientado a objetos, no solo 
  entregar funcionalidad.

- **Deuda técnica:** El Decorator actualmente solo cubre la entidad 
  `Habilidad`. Para ser consistente, debería extenderse a las demás 
  entidades (Area, Recurso, Registro) en una siguiente iteración.

---

  