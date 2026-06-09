# ADR-02: Organización del código en arquitectura de capas

| Campo  | Valor |
|--------|-------|
| Autor  | Euruviel Marquez |
| Fecha  | 05/06/2026 |
| Estado | `Aceptado` |
| Relacionado con | ADR-01 (decisión de stack tecnológico) |

---

## Contexto

DevPath ya tiene definido su stack (ADR-01: ASP.NET Core MVC + EF Core + SQL Server). La siguiente decisión es **cómo organizar internamente el código** dentro de ese stack.

En la versión inicial del proyecto, toda la lógica vive en un solo proyecto con una estructura plana: Controllers, Models y Views en la misma solución, sin separación explícita de responsabilidades más allá del patrón MVC básico. Esto funciona para el alcance actual, pero a medida que se agregan entidades (Área, Habilidad, Recurso, Registro) y controladores, se vuelve importante decidir **hasta qué punto separar las capas** para mantener el código mantenible y demostrar comprensión de arquitectura.

### Condiciones que influyeron

- El proyecto es individual y de alcance cuatrimestral — la solución no puede ser tan compleja que consuma más tiempo configurando que desarrollando.
- La materia exige demostrar principios SOLID, bounded contexts y descomposición arquitectónica (temas de Semana 2 y 3).
- El código debe poder evolucionar: si en el futuro se agrega autenticación, una API REST o una app móvil, la arquitectura no debería romperse.

---

## Decisión

Se adopta una **arquitectura en capas lógicas dentro de un solo proyecto MVC**, con responsabilidades claramente delimitadas:

| Capa | Responsabilidad | En DevPath |
|------|----------------|------------|
| **Presentation** | Interacción con el usuario | Controllers + Views (.cshtml) |
| **Domain** | Entidades y reglas de negocio | `Area.cs`, `Habilidad.cs`, `Recurso.cs`, `Registro.cs` |
| **Infrastructure** | Acceso a datos | `DevPathContext.cs` + migraciones EF Core |

La regla que se respeta: **las dependencias apuntan hacia abajo**. Los Controllers conocen los modelos y el contexto, pero los modelos (`Domain`) no importan nada de la capa de presentación ni de infraestructura.



### ¿Por qué esta decisión mejora el sistema?

- **Mantenibilidad (atributo estático):** puedo cambiar cómo se almacenan los datos (por ejemplo, migrar de LocalDB a SQL Server completo) sin tocar los Controllers ni las Views.
- **Testeabilidad (atributo estático):** la lógica de negocio está en los modelos, separada de la presentación, lo que facilita probar el comportamiento sin levantar la aplicación completa.
- **Aplica SRP de SOLID a nivel de capa:** `AreaController` solo orquesta la respuesta HTTP — no decide cómo se guardan los datos. `DevPathContext` solo gestiona la persistencia.
- **Aplica DIP de SOLID:** los controladores dependen de `DevPathContext` (una abstracción del acceso a datos), no de SQL Server directamente.

### Alternativas consideradas

| Alternativa | Por qué la descarté |
|-------------|---------------------|
| Solución con 4 proyectos separados (`Domain`, `Application`, `Infrastructure`, `Presentation`) | Arquitectura más robusta y más alineada con Clean Architecture, pero agrega complejidad de referencias entre proyectos y tiempo de configuración que no se justifica para el alcance del cuatrimestre. |
| Lógica directamente en los Controllers (sin separación) | Más rápido de escribir, pero viola SRP: el Controller termina haciendo validación, acceso a datos y formateo de respuesta. Cualquier cambio en la BD afecta el Controller. |
| Usar servicios/repositorios con interfaces explícitas | Sería el paso siguiente hacia una arquitectura hexagonal. Viable, pero requiere definir interfaces (`IHabilidadRepository`) y clases de servicio adicionales — complejidad que se reserva para una iteración futura. |

---

## Consecuencias

**Lo que gano:**

- **Técnica:** El código es predecible. Cualquier persona que abra el proyecto sabe dónde buscar cada cosa: lógica de presentación en Controllers/Views, entidades en Models, acceso a datos en DevPathContext.
- **Proceso:** El scaffolding de Visual Studio funciona correctamente con esta estructura — generó el CRUD completo de `AreaController` leyendo solo el modelo y el contexto. Eso valida que la separación está bien definida.
- **Escalabilidad futura:** Si en una versión siguiente se agrega autenticación (ASP.NET Identity) o una Web API, la capa de dominio no se toca — solo se agregan Controllers nuevos en Presentation y se extiende Infrastructure.

**Lo que sacrifico o asumo:**

- **Limitación técnica:** Sin interfaces explícitas entre capas, reemplazar EF Core por otro ORM requeriría modificar los Controllers que llaman directamente a `DevPathContext`. En una arquitectura con repositorios esto no sería necesario.
- **Deuda técnica:** Los valores de `Estado` (`Pendiente`, `En progreso`, `Completado`) y `Nivel` (`Básico`, `Intermedio`, `Avanzado`) están como strings en el modelo. Deberían ser `enum` para evitar valores inválidos — esto es trabajo pendiente para la siguiente iteración.
- **Deuda técnica:** La capa de Application (casos de uso) no existe como clase separada — la lógica de orquestación vive en los Controllers. Si el sistema crece, esta mezcla de responsabilidades será el primer punto de refactorización.

---

## Diagrama

Las cuatro vistas arquitectónicas de DevPath (lógica, desarrollo, procesos y despliegue) están documentadas en:



##  Diagrama Vista Logiga 


<img width="910" height="510" alt="Vista_Logica" src="https://github.com/user-attachments/assets/1db8ca66-f5ee-4bd8-8be7-0bce5abdc499" />

##  Vista física

```

DevPath/
│
├── Controllers/
│   ├── AreaController.cs
│   ├── HabilidadController.cs
│   ├── RecursoController.cs
│   ├── RegistroController.cs
│   ├── EstadisticasController.cs
│   └── HomeController.cs
│
├── Models/
│   ├── Area.cs
│   ├── Habilidad.cs
│   ├── Recurso.cs
│   ├── Registro.cs
│   ├── DevPathContext.cs
│   └── ErrorViewModel.cs
│
├── Views/
│   ├── Area/
│   │   ├── Index.cshtml
│   │   ├── Create.cshtml
│   │   ├── Edit.cshtml
│   │   ├── Details.cshtml
│   │   └── Delete.cshtml
│   │
│   ├── Habilidad/
│   │   ├── Index.cshtml
│   │   ├── Create.cshtml
│   │   ├── Edit.cshtml
│   │   ├── Details.cshtml
│   │   └── Delete.cshtml
│   │
│   ├── Recurso/
│   │   ├── Index.cshtml
│   │   ├── Create.cshtml
│   │   ├── Edit.cshtml
│   │   ├── Details.cshtml
│   │   └── Delete.cshtml
│   │
│   ├── Registro/
│   │   ├── Index.cshtml
│   │   ├── Create.cshtml
│   │   ├── Edit.cshtml
│   │   ├── Details.cshtml
│   │   └── Delete.cshtml
│   │
│   ├── Estadisticas/
│   │   └── Index.cshtml
│   │
│   ├── Home/
│   │   ├── Index.cshtml
│   │   └── Privacy.cshtml
│   │
│   └── Shared/
│       ├── _Layout.cshtml
│       ├── _Layout.cshtml.css
│       ├── _ValidationScriptsPartial.cshtml
│       └── Error.cshtml
│
├── Migrations/
│   ├── 20260602004320_InitialCreate.cs
│   ├── 20260602004320_InitialCreate.Designer.cs
│   └── DevPathContextModelSnapshot.cs
│
├── wwwroot/
│   ├── css/
│   │   └── site.css
│   ├── js/
│   │   └── site.js
│   └── lib/
│       ├── bootstrap/
│       ├── jquery/
│       └── jquery-validation/
│
├── docs/
│   ├── ADR-01-Euruviel-Marquez.md
│   └── DevPath-C4-Expo.drawio
│
├── Properties/
│   └── launchSettings.json
│
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
├── DevPath.csproj
├── DevPath.sln
└── .gitignore

```




## Vista de despliegue


<img width="797" height="604" alt="Vista_Despliegue" src="https://github.com/user-attachments/assets/a3a4a7f1-f3e1-45fb-b55a-33c058e4662f" />



##  Vista de procesos 

<img width="804" height="730" alt="Vista_de_Procesos" src="https://github.com/user-attachments/assets/c8e9e0ba-1358-46f8-842b-a6805cc92319" />



