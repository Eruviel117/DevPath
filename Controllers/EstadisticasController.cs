using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevPath.Models;


namespace DevPath.Controllers
{
    public class EstadisticasController : Controller
    {
        private readonly DevPathContext _context;

        public EstadisticasController(DevPathContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var totalAreas = await _context.Areas.CountAsync();
            var totalHabilidades = await _context.Habilidades.CountAsync();
            var totalRecursos = await _context.Recursos.CountAsync();

            var habilidadesPendientes = await _context.Habilidades
                .CountAsync(h => h.Estado == "Pendiente");
            var habilidadesEnProgreso = await _context.Habilidades
                .CountAsync(h => h.Estado == "En progreso");
            var habilidadesCompletadas = await _context.Habilidades
                .CountAsync(h => h.Estado == "Completado");

            var recursosCompletados = await _context.Recursos
                .CountAsync(r => r.Completado);

            // Progreso por área: cuántas habilidades completadas tiene cada área
            var progresoPorArea = await _context.Areas
                .Select(a => new ProgresoAreaViewModel
                {
                    NombreArea = a.Nombre,
                    Color = a.Color,
                    TotalHabilidades = a.Habilidades.Count,
                    HabilidadesCompletadas = a.Habilidades.Count(h => h.Estado == "Completado")
                })
                .ToListAsync();

            var viewModel = new EstadisticasViewModel
            {
                TotalAreas = totalAreas,
                TotalHabilidades = totalHabilidades,
                TotalRecursos = totalRecursos,
                HabilidadesPendientes = habilidadesPendientes,
                HabilidadesEnProgreso = habilidadesEnProgreso,
                HabilidadesCompletadas = habilidadesCompletadas,
                RecursosCompletados = recursosCompletados,
                ProgresoPorArea = progresoPorArea
            };

            return View(viewModel);
        }
    }
}