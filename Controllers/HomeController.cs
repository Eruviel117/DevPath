using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevPath.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DevPath.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly DevPathContext _context;

        public HomeController(DevPathContext context)
        {
            _context = context;
        }

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        public async Task<IActionResult> Index()
        {
            var viewModel = new EstadisticasViewModel
            {
                TotalAreas = await _context.Areas.CountAsync(a => a.UserId == CurrentUserId),
                TotalHabilidades = await _context.Habilidades.CountAsync(h => h.UserId == CurrentUserId),
                TotalRecursos = await _context.Recursos.CountAsync(r => r.UserId == CurrentUserId),
                HabilidadesPendientes = await _context.Habilidades
                    .CountAsync(h => h.UserId == CurrentUserId && h.Estado == "Pendiente"),
                HabilidadesEnProgreso = await _context.Habilidades
                    .CountAsync(h => h.UserId == CurrentUserId && h.Estado == "En progreso"),
                HabilidadesCompletadas = await _context.Habilidades
                    .CountAsync(h => h.UserId == CurrentUserId && h.Estado == "Completado"),
                RecursosCompletados = await _context.Recursos
                    .CountAsync(r => r.UserId == CurrentUserId && r.Completado),
                ProgresoPorArea = await _context.Areas
                    .Where(a => a.UserId == CurrentUserId)
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
