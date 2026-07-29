# ADR-08: Aislamiento de datos por usuario (multi-tenancy a nivel de fila)

| Campo  | Valor |
|--------|-------|
| Autor  | Euruviel Marquez |
| Fecha  | 28/07/2026 |
| Estado | `Aceptado` |

---

## Contexto

DevPath ya tenía autenticación con ASP.NET Core Identity (`[Authorize]` en
todos los controladores, login y registro funcionando), pero ninguna de
las 4 entidades del dominio (`Area`, `Habilidad`, `Recurso`, `Registro`)
tenía una referencia al usuario dueño del registro.

En la práctica, esto significaba que el login solo controlaba **quién
puede entrar al sitio**, pero no **qué puede ver cada quien una vez
adentro**: cualquier usuario autenticado veía, editaba y eliminaba las
Áreas y Habilidades de absolutamente todos los demás usuarios, porque
todas las consultas (`_context.Areas.ToListAsync()`, etc.) traían la
tabla completa sin ningún filtro. Se detectó este problema al revisar
el proyecto buscando huecos antes de seguir agregando funcionalidad.

---

## Decisión

Se agrega una columna `UserId` (string) a los 4 modelos de dominio, y
se filtran **todas** las operaciones de los controladores por el Id del
usuario autenticado, obtenido con
`User.FindFirstValue(ClaimTypes.NameIdentifier)`.

Reglas aplicadas de forma consistente en `AreasController`,
`HabilidadController`, `RecursoController`, `RegistroController`,
`EstadisticasController` y `HomeController`:

- **Index:** solo trae registros donde `UserId == usuario actual`.
- **Details / Edit / Delete (GET):** el `FirstOrDefaultAsync` filtra por
  `Id` **y** `UserId` a la vez. Si alguien entra directo a
  `/Habilidad/Details/7` intentando ver una Habilidad que no es suya,
  la consulta no encuentra nada y responde `404`, no un error de
  permisos que confirme que el recurso existe.
- **Create (POST):** el `UserId` se asigna en el servidor a partir del
  usuario autenticado — nunca viaja en el formulario ni está en el
  `[Bind]`, así que no se puede falsificar con un formulario
  manipulado.
- **Edit (POST):** antes de aplicar el cambio, se vuelve a comprobar
  que el registro exista y pertenezca al usuario actual.
- **Relaciones cruzadas (`AreaId` en Habilidad, `HabilidadId` en
  Recurso/Registro):** se valida en el servidor que el Área o Habilidad
  elegida en el formulario también pertenezca al usuario actual, para
  evitar que alguien cuelgue una Habilidad de un Área ajena cambiando
  el valor del `<select>` a mano.

Se generó la migración `AgregarUserIdParaAislamiento` con EF Core para
reflejar el cambio en la base de datos.

### Bug encontrado y corregido de paso

Al probar el aislamiento, el login dejaba de funcionar (usuario y
contraseña correctos, pero redirigía de vuelta a `/Account/Login` en
bucle). La causa era el orden del middleware en `Program.cs`:
`UseAuthorization()` estaba registrado **antes** que
`UseAuthentication()`. Como la autorización se evaluaba antes de que el
sistema supiera quién era el usuario, `[Authorize]` siempre fallaba. Se
corrigió el orden (autenticación primero, autorización después), que es
el orden que exige ASP.NET Core.

También se corrigió un `@using` mal escrito (sin la arroba) en
`Views/Habilidad/Index.cshtml`, que hacía que Razor imprimiera el
`using` como texto literal en la página en vez de interpretarlo como
código.

---

## Alternativas consideradas

| Alternativa | Por qué se descartó |
|---|---|
| **Filtrar solo en las vistas** (ocultar botones de Editar/Eliminar si no es tuyo) | No es seguridad real — cualquiera puede llamar a la URL directamente (`/Habilidad/Delete/3`) sin pasar por la vista. El filtrado tiene que vivir en el controlador/consulta, no en el HTML. |
| **UserId solo en `Area`, dejar que Habilidad/Recurso/Registro hereden el dueño vía join** | Simplifica el modelo, pero obliga a hacer `Include` de 2-3 niveles en cada consulta simple (ej. listar Recursos requeriría unir con Habilidad y luego con Área). Se prefirió duplicar el campo en las 4 tablas por simplicidad y velocidad de consulta, a costa de un poco de redundancia. |
| **Roles en vez de dueño por fila** (ej. todos los admins ven todo) | No aplica al caso de uso — DevPath es una herramienta personal de seguimiento, no hay necesidad de compartir datos entre usuarios en esta etapa. |

---

## Consecuencias

**Lo que gano:**

- **Seguridad real:** cada usuario solo puede ver y modificar sus
  propios datos, incluso si manipula URLs o formularios directamente
  (protección contra IDOR — Insecure Direct Object Reference).
- **Multi-usuario de verdad:** el login ahora tiene sentido funcional,
  no solo controla el acceso al sitio sino que separa los datos de cada
  persona.

**Lo que sacrifico o asumo:**

- **Deuda:** cualquier entidad nueva que se agregue en el futuro debe
  recordar incluir `UserId` y aplicar el mismo patrón de filtrado, o el
  problema original vuelve a aparecer silenciosamente.
- **Dato de prueba perdido:** aplicar la migración requirió recrear la
  base de datos local desde cero (`dotnet ef database drop`), ya que
  los registros existentes no tenían dueño asignado.
