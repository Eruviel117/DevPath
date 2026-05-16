# ADR-01: DevPath

| Campo  | Valor |
|--------|-------|
| Autor  | Euruviel Marquez |
| Fecha  | 15/05/2026 |
| Estado | `Propuesto` |

---

## 1.Contexto

DevPath es una aplicación web diseñada para que cualquier persona que quiera aprender de forma autónoma pueda registrar, organizar y dar seguimiento a sus metas de aprendizaje.No está pensada para un sector específico: puede usarla un estudiante universitario que quiere ordenar su ruta académica, una persona que aprende un idioma nuevo, alguien que quiere dominar un instrumento musical, o un profesional que busca adquirir nuevas habilidades para crecer en su área. El punto central de DevPath es que el aprendizaje autónomo —sin un plan claro— suele ser desorganizado y frustrante, y esta herramienta busca cambiar eso.

El sistema permite al usuario definir áreas de interés (por ejemplo: tecnología, idiomas, música, diseño), crear habilidades o temas dentro de cada área, vincular recursos de aprendizaje como cursos, libros o videos, y registrar su avance conforme va progresando. Las habilidades se organizan por nivel de dificultad — de lo más básico a lo más avanzado — para que el usuario tenga siempre claro qué aprender primero y qué sigue después.

El problema que busca resolver es que cuando aprendes solo, es fácil perderte: empiezas muchas cosas a la vez, olvidas lo que ya avanzaste o no sabes bien qué sigue. DevPath propone ordenar ese proceso en áreas de interés, con habilidades concretas dentro de cada área y recursos como cursos o videos vinculados a cada una. Así el usuario siempre sabe dónde está parado y qué viene después.

---

### Condiciones y restricciones

- El proyecto es individual, sin equipo de desarrollo.
- El desarrollador está en primer año de la carrera, por lo que la tecnología elegida debe ser conocida o de curva de aprendizaje baja.
- El tiempo disponible es el cuatrimestre académico completo.
- El proyecto debe demostrar comprensión de arquitectura de software (patrón MVC, relaciones entre entidades).
- El sistema debe ser sencillo de entender y usar, independientemente de si el usuario tiene o no conocimientos técnicos.

---

## 2.Decisión

Voy a construir DevPath usando las herramientas que he trabajado en clase: ASP.NET Core MVC con C#como framework principal para el backend, las vistas y la lógica de la aplicación ,El frontend se construirá con Razor Pages + Bootstrap HTML + CSS,con JavaScript básico para pequeñas interacciones en la interfaz, SQL Server para guardar los datos, y Git con GitHub para el control de versiones del proyecto.

---

### ¿Por qué estas herramientas?

- **ASP.NET MVC** separa de forma explícita la lógica de negocio (modelos), la presentación (vistas) y el control del flujo de la aplicación (controladores). Esa separación es exactamente lo que el curso de Arquitectura de Software busca que se demuestre, y además ya tengo experiencia con este stack del proyecto anterior.
- **Bootstrap** me permite construir una interfaz presentable sin escribir todo el CSS a mano, lo que me deja enfocarme en la lógica del sistema.
- **SQL Server + Entity Framework** funciona bien con ASP.NET y me permite manejar la base de datos desde C#, sin tener que escribir SQL puro desde el principio.
- **Git y GitHub** ya los uso en clase. Me permiten llevar un historial del proyecto, no perder trabajo y tener todo documentado.

---


## 3. Alternativas consideradas

| Alternativa | Por qué la descarté |
|---|---|
| Java + Spring Boot | Aunque Java es el lenguaje que estoy aprendiendo actualmente, Spring Boot tiene una curva de configuración inicial mucho mayor. El riesgo de invertir tiempo en configuración en lugar de arquitectura es alto para un primer proyecto de cuatrimestre. |
| Node.js + Express | Es un framework minimalista que no impone estructura, lo que dificulta demostrar patrones arquitectónicos de forma explícita. Para un proyecto cuyo objetivo es mostrar decisiones de arquitectura, un framework sin convenciones claras no es conveniente. |
| Django (Python) | Tiene buena estructura MVC pero requiere aprender Python y el ecosistema Django desde cero, lo cual no es viable en el tiempo disponible sin sacrificar calidad en las decisiones arquitectónicas. |

---

## 4. Consecuencias

### Lo que gano

- Las vistas Razor con layouts compartidos permiten cambiar el diseño global de la app desde un solo archivo (`_Layout.cshtml`), lo que facilita el mantenimiento a lo largo del cuatrimestre.
- La estructura de carpetas de ASP.NET MVC (`Controllers/`, `Models/`, `Views/`) hace que el código sea predecible y fácil de presentar y defender en clase.
- Tener el proyecto en GitHub desde el inicio me da un historial de cambios y un respaldo constante del trabajo.

### Lo que sacrifico o asumo

- ASP.NET Core MVC genera páginas completas en cada request, lo que hace más difícil implementar actualizaciones parciales en tiempo real (como marcar un recurso como completado sin recargar la página) sin agregar JavaScript adicional.
- La arquitectura monolítica actual funcionará bien para el cuatrimestre, pero si en el futuro se quisiera escalar a una API consumida por una app móvil, habría que refactorizar hacia una arquitectura cliente-servidor separada.

---

## 5. Diagrama
