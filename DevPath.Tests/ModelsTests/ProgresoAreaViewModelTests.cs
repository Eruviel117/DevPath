using DevPath.Models;
using Xunit;

namespace DevPath.Tests.ModelsTests
{
    public class ProgresoAreaViewModelTests
    {
        [Fact]
        public void PorcentajeCompletado_SinHabilidades_RetornaCero()
        {
            // Arrange
            var viewModel = new ProgresoAreaViewModel
            {
                NombreArea = "Backend",
                TotalHabilidades = 0,
                HabilidadesCompletadas = 0
            };

            // Act
            var porcentaje = viewModel.PorcentajeCompletado;

            // Assert
            // Evita división entre cero — si esto se rompe, la vista de
            // Estadísticas explota con una excepción.
            Assert.Equal(0, porcentaje);
        }

        [Fact]
        public void PorcentajeCompletado_ConHabilidadesParcialmenteCompletadas_CalculaPorcentajeCorrecto()
        {
            // Arrange
            var viewModel = new ProgresoAreaViewModel
            {
                NombreArea = "Frontend",
                TotalHabilidades = 5,
                HabilidadesCompletadas = 3
            };

            // Act
            var porcentaje = viewModel.PorcentajeCompletado;

            // Assert
            Assert.Equal(60, porcentaje);
        }

        [Fact]
        public void PorcentajeCompletado_ConTodasCompletadas_Retorna100()
        {
            // Arrange
            var viewModel = new ProgresoAreaViewModel
            {
                NombreArea = "Bases de Datos",
                TotalHabilidades = 4,
                HabilidadesCompletadas = 4
            };

            // Act
            var porcentaje = viewModel.PorcentajeCompletado;

            // Assert
            Assert.Equal(100, porcentaje);
        }
    }
}
