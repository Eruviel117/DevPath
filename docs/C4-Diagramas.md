# Diagramas C4 — DevPath

## C4 Nivel 1 — Contexto del Sistema

**Para quién es:** cualquier persona, sin conocimientos técnicos.  
**Qué responde:** ¿Qué hace el sistema y quién lo usa?

```mermaid
graph TD
    Usuario([👤 Usuario\nCualquier persona\nque aprende sola])
    
    subgraph DevPath [DevPath — Sistema de Software]
        Sistema[DevPath\nGestor de aprendizaje autónomo\nASP.NET Core MVC + C#]
    end
    
    Navegador([🌐 Navegador Web\nChrome / Firefox / Edge])

    Usuario -->|Gestiona sus metas\nde aprendizaje| Sistema
    Sistema -->|Devuelve páginas\nHTML via HTTP| Navegador
```

> El sistema no tiene integraciones con sistemas externos en esta versión.
> Es autocontenido — todo corre dentro de un solo proyecto ASP.NET Core MVC.