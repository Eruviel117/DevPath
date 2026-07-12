
using DevPath.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class HabilidadController : Controller
{
    private readonly DevPathContext _context;
    private readonly DevPath.Patterns.LoggingHabilidadDecorator _decorator;

    public HabilidadController(
        DevPathContext context,
        DevPath.Patterns.LoggingHabilidadDecorator decorator)
    {
        _context = context;
        _decorator = decorator;
    }

    // GET: HABILIDADS
    public async Task<IActionResult> Index(int? areaId, string? nivel, string? estado)
    {
        var query = _context.Habilidades
            .Include(h => h.Area)
            .Include(h => h.Recursos)
            .AsQueryable();

        if (areaId.HasValue)
        {
            query = query.Where(h => h.AreaId == areaId.Value);
        }

        if (!string.IsNullOrEmpty(nivel))
        {
            query = query.Where(h => h.Nivel == nivel);
        }

        if (!string.IsNullOrEmpty(estado))
        {
            query = query.Where(h => h.Estado == estado);
        }

        ViewData["Areas"] = new SelectList(_context.Areas, "Id", "Nombre", areaId);
        ViewData["NivelSeleccionado"] = nivel;
        ViewData["EstadoSeleccionado"] = estado;

        var habilidades = await query.ToListAsync();
        return View(habilidades);
    }

    // GET: HABILIDADS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var habilidad = await _context.Habilidades
            .Include(h => h.Area)
            .Include(h => h.Recursos)
            .Include(h => h.Registros)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (habilidad == null)
        {
            return NotFound();
        }

        // Patrón Strategy — selecciona la estrategia según el nivel
        var strategy = DevPath.Patterns.NivelStrategyFactory.Obtener(habilidad.Nivel);
        ViewData["NivelDescripcion"] = strategy.ObtenerDescripcion();
        ViewData["NivelRecursos"] = strategy.RecursosRecomendados();
        ViewData["NivelColor"] = strategy.ObtenerColor();

        return View(habilidad);
    }

    // GET: HABILIDADS/Create
    public IActionResult Create()
    {
        ViewData["AreaId"] = new SelectList(_context.Areas, "Id", "Nombre");
        return View();
    }

    // POST: HABILIDADS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Titulo,Descripcion,Nivel,Estado,AreaId")] Habilidad habilidad)
    {
        if (ModelState.IsValid)
        {
            await _decorator.GuardarHabilidadAsync(habilidad);
            return RedirectToAction(nameof(Index));
        }
        return View(habilidad);
    }

    // GET: HABILIDADS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var habilidad = await _context.Habilidades.FindAsync(id);
        if (habilidad == null)
        {
            return NotFound();
        }
        ViewData["AreaId"] = new SelectList(_context.Areas, "Id", "Nombre", habilidad.AreaId);
        return View(habilidad);
    }

    // POST: HABILIDADS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Titulo,Descripcion,Nivel,Estado,AreaId,Area,Recursos,Registros")] Habilidad habilidad)
    {
        if (id != habilidad.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(habilidad);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HabilidadExists(habilidad.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(habilidad);
    }

    // GET: HABILIDADS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var habilidad = await _context.Habilidades
            .FirstOrDefaultAsync(m => m.Id == id);
        if (habilidad == null)
        {
            return NotFound();
        }

        return View(habilidad);
    }

    // POST: HABILIDADS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var habilidad = await _context.Habilidades.FindAsync(id);
        if (habilidad != null)
        {
            await _decorator.EliminarHabilidadAsync(habilidad);
        }
        return RedirectToAction(nameof(Index));
    }

    private bool HabilidadExists(int? id)
    {
        return _context.Habilidades.Any(e => e.Id == id);
    }
}
