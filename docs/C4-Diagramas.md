# Diagramas C4 — DevPath

## C4 Nivel 1 — Contexto del Sistema

**Para quién es:** cualquier persona, sin conocimientos técnicos.
**Qué responde:** ¿Qué hace el sistema y quién lo usa?

```mermaid
graph TD
    Usuario([ Usuario\nCualquier persona\nque aprende sola])

    subgraph DevPath [DevPath — Sistema de Software]
        Sistema[DevPath\nGestor de aprendizaje autónomo\nASP.NET Core MVC + C#]
    end

    Navegador([ Navegador Web\nChrome / Firefox / Edge])
    ClienteApi([ Cliente externo\nconsume datos via API REST])

    Usuario -->|Crea cuenta,\ninicia sesión y gestiona\nsus metas de aprendizaje| Sistema
    Sistema -->|Devuelve páginas\nHTML via HTTP| Navegador
    ClienteApi -->|Consulta/gestiona\nÁreas y Habilidades\nvia JSON| Sistema
```

> Cada usuario tiene su propia cuenta (ASP.NET Identity) y solo puede ver
> y modificar sus propias Áreas, Habilidades, Recursos y Registros —
> los datos están aislados por usuario a nivel de base de datos.
> El sistema también expone una API REST para consumo externo,
> protegida con el mismo esquema de autenticación.

---

## C4 Nivel 2 — Contenedores

**Para quién es:** equipo técnico.
**Qué responde:** ¿En qué piezas técnicas está dividido el sistema y cómo se comunican?

```mermaid
graph TD
    Usuario([ Usuario\nNavegador Web])
    ClienteApi([ Cliente externo\nvia API REST])

    subgraph DevPath [DevPath — Sistema]
        WebApp[" Web App\nASP.NET Core MVC + C#\nControllers + Views + Razor"]
        API[" API REST\nASP.NET Core Web API\nAreasApi + HabilidadesApi\nSwagger"]
        Identity[" ASP.NET Identity\nAutenticación por cookie\nUserId por registro"]
        DB[(" SQL Server\nLocalDB\nDevPathDB")]
        EF[" Entity Framework Core\nORM Code First\nMigraciones"]
        CI[" CI Pipeline\nGitHub Actions\nxUnit + build"]
    end

    Usuario -->|HTTP/HTTPS\npeticiones web| WebApp
    ClienteApi -->|HTTP/HTTPS\nconsumo API + cookie auth| API
    WebApp -->|valida sesión| Identity
    API -->|valida sesión| Identity
    WebApp -->|consulta y guarda\ndatos filtrados por UserId| EF
    API -->|consulta y guarda\ndatos filtrados por UserId| EF
    EF -->|SQL queries| DB
    CI -.->|ejecuta pruebas\nen cada push| WebApp
```

> La Web App y la API REST comparten el mismo DevPathContext, los mismos
> modelos y el mismo esquema de autenticación de ASP.NET Identity.
> Todas las consultas —tanto en la Web App como en la API— se filtran por
> el `UserId` del usuario autenticado, garantizando que cada persona solo
> acceda a su propia información.
> El pipeline de CI (GitHub Actions) corre las pruebas xUnit en cada push
> para detectar regresiones antes de fusionar cambios.
> En desarrollo corre con IIS Express + LocalDB. Despliegue: contenedor
> Docker en un servicio gratuito/de bajo costo (AWS/Azure free tier).

---

## C4 Nivel 3 — Componentes

**Para quién es:** desarrolladores que trabajan en el proyecto.
**Qué responde:** ¿Qué hay dentro de la Web App — la pieza principal del sistema?

```mermaid
graph TD
    Usuario([ Usuario\nNavegador Web])

    subgraph WebApp [Web App — ASP.NET Core MVC]

        subgraph Auth [Capa de Autenticación]
            ACC[AccountController\nLogin / Register / Logout]
            ID[ASP.NET Identity\nIdentityUser + IdentityRole]
        end

        subgraph Presentacion [Capa de Presentación]
            AC[AreasController]
            HC[HabilidadController]
            RC[RecursoController]
            RGC[RegistroController]
            EC[EstadisticasController]
            HMC[HomeController]
        end

        subgraph API [Capa API REST]
            AAC["AreasApiController\n[Authorize]"]
            HAC["HabilidadesApiController\n[Authorize]"]
        end

        subgraph Patrones [Patrones GOF — Patterns/]
            STR[" Strategy\nINivelStrategy\nNivelBasicoStrategy\nNivelIntermedioStrategy\nNivelAvanzadoStrategy\nNivelStrategyFactory"]
            DEC[" Decorator\nLoggingHabilidadDecorator"]
        end

        subgraph Dominio [Capa de Dominio — Models/]
            MA["Area.cs\n+ UserId"]
            MH["Habilidad.cs\n+ UserId\n+ PorcentajeProgreso"]
            MR["Recurso.cs\n+ UserId"]
            MRG["Registro.cs\n+ UserId"]
            VM[EstadisticasViewModel\nProgresoAreaViewModel]
        end

        subgraph Datos [Capa de Acceso a Datos]
            CTX[DevPathContext\nDbContext\n4 DbSets]
        end

    end

    DB[(" SQL Server\nDevPathDB")]

    Usuario -->|HTTP| ACC
    Usuario -->|HTTP, requiere sesión| AC
    Usuario -->|HTTP, requiere sesión| HC
    Usuario -->|HTTP, requiere sesión| RC
    Usuario -->|HTTP, requiere sesión| RGC
    Usuario -->|HTTP, requiere sesión| EC
    Usuario -->|HTTP| HMC
    Usuario -->|JSON, requiere sesión| AAC
    Usuario -->|JSON, requiere sesión| HAC

    ACC --> ID
    AC -.->|valida UserId| ID
    HC -.->|valida UserId| ID
    RC -.->|valida UserId| ID
    RGC -.->|valida UserId| ID
    AAC -.->|valida UserId| ID
    HAC -.->|valida UserId| ID

    HC -->|usa| STR
    HC -->|delega escritura| DEC
    DEC -->|guarda y elimina| CTX

    AC --> CTX
    HC --> CTX
    RC --> CTX
    RGC --> CTX
    EC --> CTX
    HMC --> CTX
    AAC --> CTX
    HAC --> CTX
    ID --> CTX

    CTX -->|Entity Framework| DB

    AC --> MA
    HC --> MH
    RC --> MR
    RGC --> MRG
    EC --> VM
    HMC --> VM
    AAC --> MA
    HAC --> MH
```

> Este nivel muestra los componentes internos de la Web App:
> el controlador de cuentas y ASP.NET Identity (autenticación),
> los controladores MVC y de API —todos protegidos y filtrando por
> `UserId`—, los patrones GOF implementados (Strategy para lógica de
> niveles y Decorator para logging), los modelos de dominio (todos con
> `UserId` para aislamiento por usuario) y el `DevPathContext` como
> puente hacia SQL Server.
