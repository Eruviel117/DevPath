# ADR-07: Deuda Técnica Identificada en DevPath

| Campo  | Valor |
|--------|-------|
| Autor  | Euruviel Marquez |
| Fecha  | 15/07/2026 |
| Estado | `Aceptado` |

---

## Contexto

A medida que DevPath ha crecido durante el cuatrimestre — pasando de un
CRUD básico a un sistema con autenticación, API REST, patrones GOF y
diagramas C4 — han surgido decisiones técnicas que fueron correctas para
su momento pero que representan costos futuros si no se atienden. Este
ADR documenta dos deudas técnicas reales identificadas en el proyecto,
con su propuesta de solución concreta.

---

## Deuda Técnica #1 — Cadena de conexión expuesta en el repositorio

### ¿Qué es?

La cadena de conexión a SQL Server está escrita directamente en
`appsettings.json`, que es un archivo versionado en el repositorio
de GitHub:

```json
"ConnectionStrings": {
  "DevPathContext": "Server=(localdb)\\mssqllocaldb;
                     Database=DevPathDB;
                     Trusted_Connection=True;"
}
```

Cualquier persona que clone el repositorio puede ver exactamente
a qué servidor apunta la aplicación. Si en el futuro se desplegara
en Azure u otro proveedor con credenciales reales (usuario y
contraseña), esas credenciales quedarían expuestas públicamente
en el historial de Git — incluso si se eliminan después, Git
conserva el historial completo.

### ¿Por qué existe?

Fue una decisión consciente para agilizar el desarrollo durante el
cuatrimestre. En entorno local con LocalDB no hay credenciales
sensibles — el servidor usa autenticación de Windows y no requiere
usuario ni contraseña. La deuda se volvió real en el momento en que
se agregó ASP.NET Identity, porque ahora el sistema maneja sesiones
de usuarios reales.

### Costo de no pagarla

Si el proyecto se desplegara en producción con esta configuración,
las credenciales de la base de datos quedarían visibles en GitHub.
Un atacante podría conectarse directamente a la base de datos y
leer o modificar todos los datos de los usuarios. Además viola
directamente el **Factor III del 12-factor app**: la configuración
debe vivir en el entorno, no en el código.

### Propuesta de solución

Mover la cadena de conexión a variables de entorno y leerla desde
el código sin exponerla en el repositorio:

**Paso 1** — Agregar `appsettings.json` al `.gitignore` o crear
un `appsettings.Production.json` separado que nunca se suba al
repositorio.

**Paso 2** — Leer la cadena de conexión desde una variable de
entorno en `Program.cs`:

```csharp
var connectionString =
    Environment.GetEnvironmentVariable("DEVPATH_DB_CONNECTION")
    ?? builder.Configuration.GetConnectionString("DevPathContext");
```

**Paso 3** — En producción (Azure App Service), configurar la
variable de entorno `DEVPATH_DB_CONNECTION` directamente en el
panel de configuración del servicio — nunca en el código.

Esta solución implementa el **Factor III del 12-factor app** y
elimina el riesgo de exposición de credenciales en el repositorio.