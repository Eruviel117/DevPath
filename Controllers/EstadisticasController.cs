using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevPath.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DevPath.Controllers
{
    [Authorize]
    public class EstadisticasController : Controller
    {
        private readonly DevPathContext _context;

        public EstadisticasController(DevPathContext context)
        {
            _context = context;
        }

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        public async Task<IActionResult> Index()
        {
            var totalAreas = await _context.Areas
                .CountAsync(a => a.UserId == CurrentUserId);
            var totalHabilidades = await _context.Habilidades
                .CountAsync(h => h.UserId == CurrentUserId);
            var totalRecursos = await _context.Recursos
                .CountAsync(r => r.UserId == CurrentUserId);

            var habilidadesPendientes = await _context.Habilidades
                .CountAsync(h => h.UserId == CurrentUserId && h.Estado == "Pendiente");
            var habilidadesEnProgreso = await _context.Habilidades
                .CountAsync(h => h.UserId == CurrentUserId && h.Estado == "En progreso");
            var habilidadesCompletadas = await _context.Habilidades
                .CountAsync(h => h.UserId == CurrentUserId && h.Estado == "Completado");

            var recursosCompletados = await _context.Recursos
                .CountAsync(r => r.UserId == CurrentUserId && r.Completado);

            // Progreso por área: cuántas habilidades completadas tiene cada área
            var progresoPorArea = await _context.Areas
                .Where(a => a.UserId == CurrentUserId)
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
