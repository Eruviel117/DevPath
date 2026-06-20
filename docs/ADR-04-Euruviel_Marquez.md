# ADR-03: Incorporación de una API REST a DevPath

| Campo  | Valor |
|--------|-------|
| Autor  | Euruviel Marquez |
| Fecha  | 19/06/2026 |
| Estado | `Aceptado` |

---

## Contexto

DevPath funciona actualmente como una aplicación ASP.NET Core MVC monolítica en capas, donde los Controllers reciben peticiones HTTP y devuelven vistas Razor renderizadas en HTML. Toda la 
interacción con el sistema ocurre a través del navegador web.

En el ADR-02 ya se identificó esta limitación como una consecuencia negativa conocida: *"si en el futuro se quisiera agregar una app móvil o exponer DevPath como API para otro cliente, la capa de Presentación 
actual tendría que duplicarse o refactorizarse para devolver JSON en lugar de HTML"*. Esta es exactamente esa siguiente iteración.


- El proyecto ya tiene un modelo de datos estable (`Area`, `Habilidad`, `Recurso`, `Registro`) con `DevPathContext` configurado.
- El alcance de esta entrega se limita a las dos entidades principales: `Area` y `Habilidad`, dejando `Recurso` y `Registro` para una siguiente iteración.
- La industria documenta APIs REST con Swagger / OpenAPI como estándar, y se busca aplicar esa misma práctica profesional en el proyecto.

---

## Decisión

Se incorpora una **API REST** usando ASP.NET Core Web API, implementada dentro del mismo proyecto DevPath, en una carpeta separada (`Controllers/Api/`) para distinguirla claramente de los controladores 
MVC existentes.

Se crean dos controladores de API:

- `AreasApiController` — expone operaciones CRUD sobre `Area`.
- `HabilidadesApiController` — expone operaciones CRUD sobre 
  `Habilidad`.

Cada uno implementa los verbos HTTP estándar de REST:

| Verbo | Ruta | Acción |
|---|---|---|
| GET | `/api/AreasApi` | Lista todas las áreas |
| GET | `/api/AreasApi/{id}` | Obtiene un área por Id |
| POST | `/api/AreasApi` | Crea una nueva área |
| PUT | `/api/AreasApi/{id}` | Actualiza un área existente |
| DELETE | `/api/AreasApi/{id}` | Elimina un área |

(La misma estructura aplica para `/api/HabilidadesApi`.)

La documentación de estos endpoints se genera automáticamente con **Swagger** (paquete `Swashbuckle.AspNetCore`), accesible en `/swagger` durante el desarrollo.

### ¿Por qué REST?

- **Encaja con la arquitectura en capas ya definida.** 
  REST es fundamentalmente cliente-servidor sobre  HTTP — el mismo protocolo que ya usa ASP.NET Core MVC. No requiere introducir un paradigma de comunicación nuevo (como mensajería asíncrona o gRPC), solo añadir una capa de Presentación alterna que devuelve JSON en lugar de HTML, reutilizando el mismo `DevPathContext` y los mismos modelos.

- **Es el estándar más simple para el alcance actual.** 
  DevPath no necesita streaming en tiempo real,   ni comunicación bidireccional, ni operaciones complejas encadenadas — necesita exponer datos estructurados (Áreas y Habilidades) para que un cliente externo los consuma. REST resuelve exactamente eso sin complejidad innecesaria.

- **Swagger es el estándar de la industria para documentar APIs REST.** 
  Genera documentación interactiva directamente desde el código (atributos `[HttpGet]`, `[HttpPost]`, tipos de retorno), sin tener que mantener documentación separada que se desactualiza con el tiempo.

- **No rompe nada de lo ya construido.**
  Los Controllers MVC (`AreaController`, `HabilidadController`) siguen funcionando 
  exactamente igual. La API es aditiva, no un reemplazo — valida directamente la arquitectura en capas: la capa de Dominio y la capa de Datos se reutilizan sin 
  cambios, solo se agregó una segunda forma de acceder a ellas desde Presentación.

### Alternativas consideradas

| Alternativa | Por qué la descarté |
|---|---|
| **GraphQL** | Permite consultas más flexibles desde el cliente, pero agrega complejidad de configuración (resolvers, schema) que no se justifica para un sistema con solo 4 entidades y relaciones simples. REST con rutas predecibles es más que suficiente y más fácil de documentar con Swagger. |
| **No implementar API y mantener solo MVC** | Se descartó porque ya se identificó esta limitación como una deuda conocida, y esta actividad es la oportunidad concreta de resolverla de forma incremental, sin reescribir el sistema existente. |
| **Exponer la API en un proyecto separado (microservicio aparte)** | Implicaría duplicar el `DevPathContext` o comunicar dos proyectos por red, contradiciendo directamente la decisión de arquitectura en capas monolítica. Mantener la API dentro del mismo proyecto, en una carpeta separada, logra la separación lógica sin la complejidad de despliegue de un servicio distribuido. |

---

## Consecuencias

**Lo que gano:**

- **Técnica:** 
  Cualquier cliente externo (una futura app móvil, un script, Postman) puede ahora leer y modificar Áreas y Habilidades sin pasar por las vistas Razor pensadas para humanos. Esto es la base 
  necesaria para cualquier expansión futura del sistema fuera del navegador.

- **Técnica:** 
  Swagger genera documentación interactiva y siempre actualizada de los endpoints directamente desde el código, sin mantenimiento manual de documentación externa.

- **Proceso:** 
  La API reutiliza el mismo `DevPathContext` y los mismos modelos que ya existían — no fue necesario duplicar lógica de acceso a datos, lo que confirma que la arquitectura en capas elegida y sí facilita extender el sistema sin reescribirlo.

**Lo que sacrifico o asumo:**

- **Limitación técnica:** 
  Los endpoints de la API no tienen autenticación ni autorización en esta versión. Cualquiera que conozca 
  la URL puede leer o modificar datos. Esto es aceptable para una entrega de aprendizaje/demostración, pero es una deuda técnica real que debe resolverse con un mecanismo como API Keys o JWT antes de cualquier uso fuera de un entorno controlado.

- **Deuda técnica:** 
  Por ahora la API solo cubre `Area` y `Habilidad`. `Recurso` y `Registro` quedan pendientes para una siguiente iteración, siguiendo exactamente el mismo patrón ya establecido.

---

## Infraestructura

La API corre dentro del mismo proceso que la aplicación MVC — mismo `localhost`, mismo servidor IIS Express en desarrollo. No requiere infraestructura adicional ni un despliegue separado. Esto es coherente 
con la arquitectura en capas monolítica: añadir una API REST no cambió el estilo arquitectónico, solo extendió la capa de Presentación con una segunda forma de exponer los mismos datos.

---