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

    Usuario -->|Gestiona sus metas\nde aprendizaje| Sistema
    Sistema -->|Devuelve páginas\nHTML via HTTP| Navegador
```

> El sistema no tiene integraciones con sistemas externos en esta versión.
> Es autocontenido — todo corre dentro de un solo proyecto ASP.NET Core MVC.

---

## C4 Nivel 2 — Contenedores

**Para quién es:** equipo técnico.  
**Qué responde:** ¿En qué piezas técnicas está dividido el sistema y cómo se comunican?

```mermaid
graph TD
    Usuario([ Usuario\nNavegador Web])

    subgraph DevPath [DevPath — Sistema]
        WebApp[" Web App\nASP.NET Core MVC + C#\nControllers + Views + Razor"]
        API[" API REST\nASP.NET Core Web API\nEndpoints JSON + Swagger"]
        DB[(" SQL Server\nLocalDB\nDevPathDB")]
        EF[" Entity Framework Core\nORM Code First\nMigraciones"]
    end

    Usuario -->|HTTP/HTTPS\npeticiones web| WebApp
    Usuario -->|HTTP/HTTPS\nconsumo API| API
    WebApp -->|consulta y guarda\ndatos| EF
    API -->|consulta y guarda\ndatos| EF
    EF -->|SQL queries| DB
```

> La Web App y la API REST comparten el mismo DevPathContext y los mismos modelos.
> Entity Framework actúa como puente entre ambas y SQL Server.
> En desarrollo corre con IIS Express + LocalDB. Despliegue planeado: Azure App Service + Azure SQL.

---

## C4 Nivel 3 — Componentes

**Para quién es:** desarrolladores que trabajan en el proyecto.  
**Qué responde:** ¿Qué hay dentro de la Web App — la pieza principal del sistema?

```mermaid
graph TD
    Usuario([ Usuario\nNavegador Web])

    subgraph WebApp [Web App — ASP.NET Core MVC]

        subgraph Presentacion [Capa de Presentación]
            AC[AreasController]
            HC[HabilidadController]
            RC[RecursoController]
            RGC[RegistroController]
            EC[EstadisticasController]
            HMC[HomeController]
        end

        subgraph API [Capa API REST]
            AAC[AreasApiController]
            HAC[HabilidadesApiController]
        end

        subgraph Patrones [Patrones GOF — Patterns/]
            STR[" Strategy\nINivelStrategy\nNivelBasicoStrategy\nNivelIntermedioStrategy\nNivelAvanzadoStrategy\nNivelStrategyFactory"]
            DEC[" Decorator\nLoggingHabilidadDecorator"]
        end

        subgraph Dominio [Capa de Dominio — Models/]
            MA[Area.cs]
            MH[Habilidad.cs\n+ PorcentajeProgreso]
            MR[Recurso.cs]
            MRG[Registro.cs]
            VM[EstadisticasViewModel\nProgresoAreaViewModel]
        end

        subgraph Datos [Capa de Acceso a Datos]
            CTX[DevPathContext\nDbContext\n4 DbSets]
        end

    end

    DB[(" SQL Server\nDevPathDB")]

    Usuario -->|HTTP| AC
    Usuario -->|HTTP| HC
    Usuario -->|HTTP| RC
    Usuario -->|HTTP| RGC
    Usuario -->|HTTP| EC
    Usuario -->|HTTP| HMC
    Usuario -->|JSON| AAC
    Usuario -->|JSON| HAC

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

    CTX -->|Entity Framework| DB

    AC --> MA
    HC --> MH
    RC --> MR
    RGC --> MRG
    EC --> VM
    HMC --> VM
```

> Este nivel muestra los componentes internos de la Web App:
> los controladores MVC, los controladores de API, los patrones GOF implementados
> (Strategy para lógica de niveles y Decorator para logging),
> los modelos de dominio y el DevPathContext como puente hacia SQL Server.
