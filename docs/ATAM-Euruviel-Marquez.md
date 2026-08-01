# Evaluación ATAM — DevPath

| Campo  | Valor |
|--------|-------|
| Autor  | Euruviel Marquez |
| Fecha  | 31/07/2026 |
| Alcance | Evaluación final de arquitectura — versión consolidada del proyecto |

---

## 1. Riesgo

**Decisión evaluada:** el aislamiento de datos por usuario (`UserId`) se
implementa manualmente en cada controlador — cada consulta debe incluir
`.Where(x => x.UserId == CurrentUserId)` a mano, en vez de aplicarse
automáticamente a nivel de `DbContext` (por ejemplo con un
[global query filter de EF Core](https://learn.microsoft.com/ef/core/querying/filters)).

**¿Por qué es un riesgo?** No es teórico — ya ocurrió en este proyecto:
al construir `AreasApiController` y `HabilidadesApiController` (rama
`feature/api-rest`) antes de terminar el aislamiento por usuario, esos
dos controladores quedaron sin el filtro por `UserId`. Cualquier usuario
autenticado podía leer, editar o borrar los datos de cualquier otro
usuario a través de esos endpoints (vulnerabilidad tipo IDOR). El error
no lo detectó el compilador ni las pruebas existentes — se encontró
hasta la revisión manual de la entrega final.

**Impacto si se materializa de nuevo:** cada vez que se agregue un
controlador o una consulta nueva (MVC o API), existe la misma
probabilidad de olvidar el filtro, exponiendo datos de usuarios reales.

**Mitigación recomendada:** mover el filtro de `UserId` al propio
`DevPathContext` usando `HasQueryFilter()` sobre cada `DbSet`, para que
la restricción se aplique automáticamente sin depender de que cada
desarrollador la recuerde en cada consulta nueva.

---

## 2. Trade-off

**Decisión evaluada:** usar el patrón **Strategy** (`INivelStrategy`,
`NivelStrategyFactory`) para resolver el comportamiento asociado a cada
nivel de habilidad (Básico/Intermedio/Avanzado), en vez de un simple
`switch` dentro de `HabilidadController`.

**A favor:** agregar un nuevo nivel en el futuro (por ejemplo "Experto")
solo requiere crear una clase nueva que implemente `INivelStrategy` y
registrarla en la fábrica — no hay que tocar el controlador ni el resto
del código existente (principio abierto/cerrado). También facilita
probar cada nivel de forma aislada con xUnit, como ya se hizo.

**En contra:** para solo 3 niveles fijos que no han cambiado desde el
inicio del cuatrimestre, el patrón introduce más archivos, más
indirección y más curva de aprendizaje para quien lea el código por
primera vez, comparado con un `switch` de 3 casos que sería igual de
funcional y más fácil de leer de un vistazo.

**Conclusión:** se aceptó el costo de complejidad adicional a cambio de
extensibilidad, priorizando además que la materia pedía demostrar un
patrón GOF real — es una decisión correcta para el contexto académico,
aunque en un proyecto de alcance fijo y pequeño el `switch` simple
también hubiera sido válido.

---

## 3. Punto de sensibilidad

**Decisión evaluada:** el orden de los middlewares de autenticación en
`Program.cs` — específicamente que `app.UseAuthentication()` debe
ejecutarse **antes** que `app.UseAuthorization()`.

**¿Por qué es sensible?** Es un solo detalle de ordenamiento, pero un
cambio mínimo ahí rompe todo el flujo de acceso al sistema. De hecho
ocurrió durante el desarrollo: en un commit temprano el orden estaba
invertido (`UseAuthorization()` antes que `UseAuthentication()`), lo
que provocaba un ciclo infinito de redirección al intentar iniciar
sesión — la aplicación nunca lograba autenticar al usuario antes de
verificar sus permisos. El síntoma no era un error de compilación ni
una excepción visible, sino un comportamiento silencioso e imposible
de diagnosticar sin conocer el detalle exacto del pipeline de ASP.NET
Core.

**Por qué es un punto de sensibilidad y no solo un bug puntual:** una
sola línea movida de lugar afecta la calidad de atributo más crítica
del sistema completo —la seguridad y disponibilidad del acceso—, sin
que ninguna otra parte del código lo señale. Cualquier futura
modificación al pipeline de middlewares en `Program.cs` debe revisar
explícitamente este orden.

**Mitigación recomendada:** agregar una prueba de integración que
levante la aplicación y verifique que una ruta protegida redirige
correctamente a `/Account/Login` cuando no hay sesión, de forma que un
futuro cambio accidental en el orden de los middlewares se detecte en
el pipeline de CI antes de llegar a producción.
