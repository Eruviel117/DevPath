using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevPath.Models;

namespace DevPath.Controllers
{
    public class HomeController : Controller
    {
        private readonly DevPathContext _context;

        public HomeController(DevPathContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new EstadisticasViewModel
            {
                TotalAreas = await _context.Areas.CountAsync(),
                TotalHabilidades = await _context.Habilidades.CountAsync(),
                TotalRecursos = await _context.Recursos.CountAsync(),
                HabilidadesPendientes = await _context.Habilidades
                    .CountAsync(h => h.Estado == "Pendiente"),
                HabilidadesEnProgreso = await _context.Habilidades
                    .CountAsync(h => h.Estado == "En progreso"),
                HabilidadesCompletadas = await _context.Habilidades
                    .CountAsync(h => h.Estado == "Completado"),
                RecursosCompletados = await _context.Recursos
                    .CountAsync(r => r.Completado),
                ProgresoPorArea = await _context.Areas
                    .Select(a => new ProgresoAreaViewModel
                    {
                        NombreArea = a.Nombre,
                        Color = a.Color,
                        TotalHabilidades = a.Habilidades.Count,
                        HabilidadesCompletadas = a.Habilidades
                            .Count(h => h.Estado == "Completado")
                    })
                    .ToListAsync()
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}