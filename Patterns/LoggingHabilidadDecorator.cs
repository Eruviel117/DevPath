using DevPath.Models;
using Microsoft.Extensions.Logging;

namespace DevPath.Patterns
{
    public class LoggingHabilidadDecorator
    {
        private readonly DevPathContext _context;
        private readonly ILogger<LoggingHabilidadDecorator> _logger;

        public LoggingHabilidadDecorator(
            DevPathContext context,
            ILogger<LoggingHabilidadDecorator> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task GuardarHabilidadAsync(Habilidad habilidad)
        {
            _logger.LogInformation(
                "[{Tiempo}] Guardando habilidad: {Titulo} (Nivel: {Nivel}, Estado: {Estado})",
                DateTime.Now, habilidad.Titulo, habilidad.Nivel, habilidad.Estado);

            _context.Add(habilidad);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "[{Tiempo}] Habilidad guardada correctamente con Id: {Id}",
                DateTime.Now, habilidad.Id);
        }

        public async Task EliminarHabilidadAsync(Habilidad habilidad)
        {
            _logger.LogWarning(
                "[{Tiempo}] Eliminando habilidad Id: {Id} — {Titulo}",
                DateTime.Now, habilidad.Id, habilidad.Titulo);

            _context.Habilidades.Remove(habilidad);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "[{Tiempo}] Habilidad Id: {Id} eliminada correctamente",
                DateTime.Now, habilidad.Id);
        }
    }
}