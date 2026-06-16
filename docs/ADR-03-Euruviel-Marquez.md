# ADR-02: Estilo arquitectónico de DevPath

| Campo  | Valor |
|--------|-------|
| Autor  | Euruviel Marquez |
| Fecha  | 12/06/2026 |
| Estado | `Aceptado` |

---

## Contexto

DevPath es un sistema individual con un solo tipo de usuario: la persona que organiza su propio aprendizaje. No hay roles diferenciados (admin, 
usuario, invitado) ni necesidad de múltiples interfaces — todo se consume desde un navegador web mediante páginas renderizadas en el 
servidor (Razor + ASP.NET Core MVC).

El sistema ya tiene 4 entidades con relaciones jerárquicas claras (`Area → Habilidad → Recurso/Registro`) gestionadas con Entity Framework 
Core, y 3 controladores generados con scaffolding que siguen el patrón MVC nativo de ASP.NET Core: Modelo (entidades + DbContext), Vista 
(Razor) y Controlador (lógica de cada entidad).

Las características concretas que hacen relevante esta decisión son:

- **Un solo tipo de usuario**, sin necesidad de autenticación ni roles por ahora.
- **Bajo volumen esperado**: el sistema está pensado para uso personal, no para múltiples usuarios concurrentes.
- **Una sola interfaz**: páginas web renderizadas por el servidor. No hay planes inmediatos de una app móvil o una API consumida por 
  terceros.
- **Las relaciones entre entidades son el núcleo del sistema** — separar esa lógica en servicios distribuidos agregaría comunicación 
  de red donde hoy es una simple consulta con Entity Framework.

---

## Decisión

Se adopta una **arquitectura en capas (layered architecture)**, implementada de forma monolítica sobre ASP.NET Core MVC.

El sistema se organiza en tres capas dentro de un solo proyecto desplegable:

- **Capa de Presentación** — Controllers + Views (Razor + Bootstrap). Recibe las peticiones HTTP y devuelve HTML.
- **Capa de Dominio/Aplicación** — los modelos (`Area`, `Habilidad`, `Recurso`, `Registro`) con sus relaciones y reglas básicas de 
  validación.
- **Capa de Acceso a Datos** — `DevPathContext` (Entity Framework Core)  como puente hacia SQL Server.

Las dependencias fluyen en una sola dirección: Presentación → Dominio → Datos. Los Controllers no acceden a SQL Server directamente, siempre 
pasan por `DevPathContext`.

---

## Consecuencias positivas

- **Técnica:** La separación en capas permite cambiar cualquiera de ellas de forma aislada. Ya se demostró en la práctica: el scaffolding 
  generó el CRUD completo de `AreaController`, `HabilidadController` y `RecursoController` leyendo solo los modelos y el `DevPathContext`, 
  sin tocar manualmente la capa de presentación.

- **Técnica:** Al estar todo en un solo proyecto desplegable, no hay llamadas de red entre capas. Una operación como crear una Habilidad 
  con su Área relacionada es una sola transacción local con Entity Framework — no requiere coordinar múltiples servicios ni manejar 
  fallos de red parciales.

- **Proceso:** Para un desarrollador individual con tiempo limitado, un monolito en capas significa un solo proyecto que correr, depurar y 
  desplegar. No hay que levantar múltiples servicios ni configurar comunicación entre ellos para poder probar el sistema completo.

---

## Consecuencias negativas

- **Limitación técnica:** Todas las capas comparten el mismo proceso y el mismo ciclo de vida. Si en el futuro la capa de presentación 
  necesitara escalar de forma independiente de la capa de datos (por ejemplo, muchas más peticiones de lectura que de escritura), no se 
  puede escalar solo esa parte — habría que escalar todo el monolito.

- **Deuda / riesgo:** Si más adelante se quisiera agregar una app móvil o exponer DevPath como API para otro cliente, la capa de Presentación 
  actual (Controllers que devuelven Vistas Razor) tendría que duplicarse o refactorizarse para devolver JSON en lugar de HTML. La 
  arquitectura en capas no impide esto, pero tampoco lo facilita automáticamente — es trabajo adicional reconocido desde ahora.

---

## Infraestructura: ¿dónde corre el sistema?

Actualmente DevPath corre **en localhost**, usando **IIS Express** (servidor de desarrollo integrado en Visual Studio) y **SQL Server 
LocalDB** como base de datos. No está desplegado en ningún servidor externo.

Esta decisión es coherente con el estilo en capas elegido: un monolito en capas está pensado para desplegarse como **una sola unidad**. Si en 
el futuro se necesitara un entorno accesible públicamente, el siguiente paso natural sería un **Azure App Service** (PaaS) con **Azure SQL 
Database** — ambos siguen soportando el mismo monolito desplegado como una sola unidad, sin requerir cambios en el estilo arquitectónico.

Una arquitectura de microservicios, en cambio, sí habría obligado a pensar en infraestructura más compleja desde el inicio — múltiples 
servicios desplegados por separado (por ejemplo, contenedores en EC2 o funciones Lambda independientes para cada entidad), con su propia base 
de datos y comunicación entre ellos. Para el alcance actual de DevPath, esa complejidad no tiene justificación.

---

## Alternativas consideradas

| Alternativa | Por qué la descarté |
|---|---|
| **Arquitectura hexagonal (Ports & Adapters)** | Hexagonal tiene sentido cuando se anticipa reemplazar piezas externas (base de datos, frameworks de UI) sin tocar el núcleo del negocio. DevPath ya logra ese desacoplamiento básico con Entity Framework + DIP (cambiar de SQL Server a PostgreSQL solo cambia un paquete y una línea). Agregar las interfaces y adaptadores explícitos de hexagonal añadiría una capa de abstracción que el proyecto no necesita para su alcance actual. |
| **Microservicios** | Microservicios resuelven el problema de equipos grandes que necesitan desplegar y escalar partes del sistema de forma independiente. DevPath es un proyecto individual con 4 entidades fuertemente relacionadas entre sí (Area → Habilidad → Recurso/Registro) — separarlas en servicios distintos convertiría consultas locales simples en llamadas de red, agregando latencia y complejidad de coordinación sin ningún beneficio real para un solo usuario. |
| **Arquitectura cliente-servidor con API REST separada** | Se consideró por si en el futuro se agrega una app móvil. Se descartó por ahora porque no hay un segundo cliente real que consumir — construir una API sin un consumidor sería esfuerzo anticipado sin valor inmediato. Si surge esa necesidad, la capa de Presentación actual puede extenderse para exponer endpoints JSON sin rediseñar las capas de Dominio o Datos. |


## Diagrama 


<img width="1015" height="672" alt="Captura de pantalla 2026-06-12 194641" src="https://github.com/user-attachments/assets/a454480d-6307-455c-9329-63945060ca6d" />



---

##  Declaración de uso de Inteligencia Artificial

| Campo | Detalle |
|---|---|
| **Nombre del estudiante** | Euruviel Marquez Martinez |
| **IA utilizada** | Claude  |
| **Fecha de uso** | 10/06/2026 |
| **Propósito** | Investigacion sobre los temas|

### Prompt utilizado:
- *"puede ayudarme a definir bien los El estilo elegido (capas, cliente-servidor, microservicios, event-driven, serverless, hexagonal, etc.), lo necesito oara saber y determinar y explicar de mejor manera un proyeto que estoy haciendo porfavor"*

### Respuesta en crudo (raw):

Arquitectura en capas (Layered/N-Tier)

El sistema se organiza en capas horizontales: presentación, lógica de negocio, acceso a datos. Cada capa solo se comunica con la inmediatamente adyacente. Es el más común en proyectos académicos tipo MVC (como tu CatalogoApp/AnimeVault con Domain, Application, Infrastructure, Presentation).
Cliente-servidor

Un servidor centraliza recursos/lógica y los clientes (web, móvil, desktop) consumen esos servicios mediante peticiones. Casi todo proyecto web entra aquí a nivel general.
Arquitectura hexagonal (Ports & Adapters)

El núcleo del negocio (dominio) queda aislado en el centro, sin depender de frameworks ni bases de datos. Se comunica con el exterior mediante "puertos" (interfaces) y "adaptadores" (implementaciones concretas: API REST, base de datos, etc.). Es muy popular en Spring Boot bien estructurado.
Microservicios

La aplicación se divide en servicios pequeños, independientes, cada uno con su propia base de datos y despliegue, comunicándose por red (HTTP/mensajería).
Event-driven (basada en eventos)

Los componentes se comunican emitiendo y reaccionando a eventos de forma asíncrona, normalmente vía un broker de mensajes (Kafka, RabbitMQ).
Serverless

La lógica se ejecuta en funciones (FaaS) administradas por un proveedor cloud, sin gestionar servidores directamente.
---
