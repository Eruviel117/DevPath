# ADR-09: Deuda Técnica Identificada en DevPath

| Campo  | Valor |
|--------|-------|
| Autor  | Euruviel Marquez |
| Fecha  | 31/07/2026 |
| Estado | `Aceptado` |

---

## Contexto

A medida que DevPath ha crecido durante el cuatrimestre — pasando de un
CRUD básico a un sistema con autenticación, aislamiento de datos por
usuario, API REST, patrones GOF, pruebas automatizadas, CI y diagramas
C4 — han surgido decisiones técnicas que fueron correctas para su
momento pero que representan costos futuros si no se atienden. Este
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
en Azure, AWS u otro proveedor con credenciales reales (usuario y
contraseña), esas credenciales quedarían expuestas públicamente
en el historial de Git — incluso si se eliminan después, Git
conserva el historial completo.

### ¿Por qué existe?

Fue una decisión consciente para agilizar el desarrollo durante el
cuatrimestre. En entorno local con LocalDB no hay credenciales
sensibles — el servidor usa autenticación de Windows y no requiere
usuario ni contraseña. La deuda se volvió real en el momento en que
se agregó ASP.NET Identity, porque ahora el sistema maneja sesiones
de usuarios reales, y se vuelve crítica ahora que el proyecto se
va a desplegar en un servidor público.

### Costo de no pagarla

Si el proyecto se despliega en producción con esta configuración,
las credenciales de la base de datos quedarían visibles en GitHub.
Un atacante podría conectarse directamente a la base de datos y
leer o modificar todos los datos de los usuarios. Además viola
directamente el **Factor III del 12-factor app**: la configuración
debe vivir en el entorno, no en el código.

### Propuesta de solución

Mover la cadena de conexión a variables de entorno y leerla desde
el código sin exponerla en el repositorio:

**Paso 1** — Agregar `appsettings.Production.json` a `.gitignore`
para que nunca se suba al repositorio.

**Paso 2** — Leer la cadena de conexión desde una variable de
entorno en `Program.cs`:

```csharp
var connectionString =
    Environment.GetEnvironmentVariable("DEVPATH_DB_CONNECTION")
    ?? builder.Configuration.GetConnectionString("DevPathContext");
```

**Paso 3** — En producción, configurar la variable de entorno
`DEVPATH_DB_CONNECTION` directamente en el panel de configuración
del servicio de despliegue — nunca en el código.

Esta solución implementa el **Factor III del 12-factor app** y
elimina el riesgo de exposición de credenciales en el repositorio.

---

## Deuda Técnica #2 — Nivel y Estado como strings sin validación

### ¿Qué es?

Los campos `Nivel` y `Estado` del modelo `Habilidad` están definidos
como `string` en C#:

```csharp
public string Nivel { get; set; } = "Básico";
public string Estado { get; set; } = "Pendiente";
```

Esto significa que no existe ninguna restricción en el código que
impida guardar valores inválidos como `"basico"` (sin mayúscula),
`"Basico"` (sin acento), `"en progreso"` (minúsculas) o cualquier
otro string arbitrario. El compilador no detectaría ese error —
solo se manifestaría en tiempo de ejecución cuando los filtros no
funcionen o los badges de color muestren el estado incorrecto.

### ¿Por qué existe?

Fue una decisión de velocidad tomada al inicio del proyecto cuando
se scaffoldearon los controladores. Usar strings era la forma más
rápida de tener el sistema funcionando. En ese momento el enfoque
era demostrar arquitectura MVC y relaciones entre entidades — no
robustez del modelo de datos. La deuda creció cuando se agregaron
filtros por estado y nivel, y badges de color que dependen de que
esos valores sean exactamente correctos.

### Costo de no pagarla

Si un valor inválido llega a la base de datos los efectos son
silenciosos pero reales:

- El filtro por nivel no encontraría la habilidad aunque exista.
- El badge de color mostraría el estado por defecto (gris) en lugar
  del color correcto.
- El patrón Strategy fallaría silenciosamente — `NivelStrategyFactory`
  devolvería siempre `NivelBasicoStrategy` para cualquier valor no
  reconocido.
- Las estadísticas contarían mal las habilidades por estado.

Ninguno de estos errores lanzaría una excepción — simplemente los
datos se verían incorrectos sin una causa obvia.

### Propuesta de solución

Reemplazar los strings por enumeraciones (`enum`) en C# y agregar
una migración que actualice la base de datos:

**Paso 1** — Definir los enums en el modelo:

```csharp
public enum NivelHabilidad
{
    Basico,
    Intermedio,
    Avanzado
}

public enum EstadoHabilidad
{
    Pendiente,
    EnProgreso,
    Completado
}
```

**Paso 2** — Actualizar el modelo `Habilidad.cs`:

```csharp
public NivelHabilidad Nivel { get; set; } = NivelHabilidad.Basico;
public EstadoHabilidad Estado { get; set; } = EstadoHabilidad.Pendiente;
```

**Paso 3** — Crear una nueva migración para que Entity Framework
actualice las columnas en SQL Server.

**Paso 4** — Actualizar los dropdowns en las vistas Create y Edit
para usar los valores del enum en lugar de strings hardcodeados.

Con este cambio el compilador rechazaría cualquier valor inválido
en tiempo de compilación — el error aparecería antes de ejecutar
la app, no después de que los datos ya estén corruptos en la base
de datos.

---

## Relación entre las deudas y los ADRs anteriores

| Deuda | ADR relacionado | Decisión original |
|---|---|---|
| Cadena de conexión expuesta | ADR-01, ADR-02 | Se priorizó velocidad de desarrollo sobre seguridad de configuración |
| Strings en lugar de enums | ADR-01 | Se priorizó simplicidad del modelo sobre robustez de validación |

Ambas deudas son consecuencia directa de decisiones conscientes
tomadas al inicio del proyecto para cumplir con el alcance del
cuatrimestre. Están documentadas aquí como parte del proceso de
maduración de la arquitectura de DevPath.
