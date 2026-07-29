using DevPath.Patterns;
using Xunit;

namespace DevPath.Tests.PatternsTests
{
    public class NivelStrategyFactoryTests
    {
        [Fact]
        public void Obtener_ConNivelBasico_RetornaNivelBasicoStrategy()
        {
            // Arrange
            var nivel = "Básico";

            // Act
            var strategy = NivelStrategyFactory.Obtener(nivel);

            // Assert
            Assert.IsType<NivelBasicoStrategy>(strategy);
            Assert.Equal(3, strategy.RecursosRecomendados());
        }

        [Fact]
        public void Obtener_ConNivelIntermedio_RetornaNivelIntermedioStrategy()
        {
            // Arrange
            var nivel = "Intermedio";

            // Act
            var strategy = NivelStrategyFactory.Obtener(nivel);

            // Assert
            Assert.IsType<NivelIntermedioStrategy>(strategy);
            Assert.Equal(5, strategy.RecursosRecomendados());
        }

        [Fact]
        public void Obtener_ConNivelAvanzado_RetornaNivelAvanzadoStrategy()
        {
            // Arrange
            var nivel = "Avanzado";

            // Act
            var strategy = NivelStrategyFactory.Obtener(nivel);

            // Assert
            Assert.IsType<NivelAvanzadoStrategy>(strategy);
            Assert.Equal(8, strategy.RecursosRecomendados());
        }

        [Fact]
        public void Obtener_ConNivelDesconocido_RetornaNivelBasicoStrategyPorDefecto()
        {
            // Arrange
            var nivel = "ValorQueNoExiste";

            // Act
            var strategy = NivelStrategyFactory.Obtener(nivel);

            // Assert
            // Si esto cambia, la promesa de la factory se rompe: siempre
            // debe devolver una estrategia válida, nunca null.
            Assert.IsType<NivelBasicoStrategy>(strategy);
        }
    }
}
