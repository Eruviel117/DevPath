using DevPath.Models;
using Xunit;

namespace DevPath.Tests.ModelsTests
{
    public class HabilidadTests
    {
        [Fact]
        public void PorcentajeProgreso_SinRecursos_RetornaCero()
        {
            // Arrange
            var habilidad = new Habilidad { Titulo = "C#" };

            // Act
            var porcentaje = habilidad.PorcentajeProgreso;

            // Assert
            Assert.Equal(0, porcentaje);
        }

        [Fact]
        public void PorcentajeProgreso_ConAlgunosRecursosCompletados_CalculaPorcentajeCorrecto()
        {
            // Arrange
            var habilidad = new Habilidad { Titulo = "SQL" };
            habilidad.Recursos.Add(new Recurso { Nombre = "Curso A", Completado = true });
            habilidad.Recursos.Add(new Recurso { Nombre = "Curso B", Completado = false });
            habilidad.Recursos.Add(new Recurso { Nombre = "Curso C", Completado = false });
            habilidad.Recursos.Add(new Recurso { Nombre = "Curso D", Completado = true });

            // Act
            var porcentaje = habilidad.PorcentajeProgreso;

            // Assert
            // 2 de 4 recursos completados = 50%
            Assert.Equal(50, porcentaje);
        }

        [Fact]
        public void PorcentajeProgreso_ConTodosLosRecursosCompletados_Retorna100()
        {
            // Arrange
            var habilidad = new Habilidad { Titulo = "Arquitectura de Software" };
            habilidad.Recursos.Add(new Recurso { Nombre = "Libro", Completado = true });
            habilidad.Recursos.Add(new Recurso { Nombre = "Curso", Completado = true });

            // Act
            var porcentaje = habilidad.PorcentajeProgreso;

            // Assert
            Assert.Equal(100, porcentaje);
        }
    }
}
