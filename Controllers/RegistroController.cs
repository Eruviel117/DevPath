
using DevPath.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[Authorize]
public class RegistroController : Controller
{
    private readonly DevPathContext _context;

    public RegistroController(DevPathContext context)
    {
        _context = context;
    }

    // Id del usuario autenticado. Todas las consultas de este controlador
    // se filtran por este valor para que cada usuario solo vea sus propios
    // Registros.
    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // GET: REGISTROS
    public async Task<IActionResult> Index()
    {
        var registros = await _context.Registros
            .Where(r => r.UserId == CurrentUserId)
            .Include(r => r.Habilidad)
            .ToListAsync();
        return View(registros);
    }

    // GET: REGISTROS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var registro = await _context.Registros
            .Include(r => r.Habilidad)
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == CurrentUserId);

        if (registro == null)
        {
            return NotFound();
        }

        return View(registro);
    }

    // GET: REGISTROS/Create
    public IActionResult Create()
    {
        ViewData["HabilidadId"] = new SelectList(
            _context.Habilidades.Where(h => h.UserId == CurrentUserId), "Id", "Titulo");
        return View();
    }

    // POST: REGISTROS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Nota,Fecha,HabilidadId")] Registro registro)
    {
        // La Habilidad elegida debe pertenecer al usuario actual.
        var habilidadValida = await _context.Habilidades
            .AnyAsync(h => h.Id == registro.HabilidadId && h.UserId == CurrentUserId);
        if (!habilidadValida)
        {
            ModelState.AddModelError(nameof(Registro.HabilidadId), "Habilidad inválida.");
        }

        if (ModelState.IsValid)
        {
            registro.UserId = CurrentUserId;
            _context.Add(registro);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["HabilidadId"] = new SelectList(
            _context.Habilidades.Where(h => h.UserId == CurrentUserId), "Id", "Titulo", registro.HabilidadId);
        return View(registro);
    }

    // GET: REGISTROS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var registro = await _context.Registros
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == CurrentUserId);
        if (registro == null)
        {
            return NotFound();
        }
        ViewData["HabilidadId"] = new SelectList(
            _context.Habilidades.Where(h => h.UserId == CurrentUserId), "Id", "Titulo", registro.HabilidadId);
        return View(registro);
    }

    // POST: REGISTROS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Nota,Fecha,HabilidadId")] Registro registro)
    {
        if (id != registro.Id)
        {
            return NotFound();
        }

        var existe = await _context.Registros
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == CurrentUserId);
        if (existe == null)
        {
            return NotFound();
        }

        var habilidadValida = await _context.Habilidades
            .AnyAsync(h => h.Id == registro.HabilidadId && h.UserId == CurrentUserId);
        if (!habilidadValida)
        {
            ModelState.AddModelError(nameof(Registro.HabilidadId), "Habilidad inválida.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                registro.UserId = CurrentUserId;
                _context.Update(registro);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RegistroExists(registro.Id))
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
        ViewData["HabilidadId"] = new SelectList(
            _context.Habilidades.Where(h => h.UserId == CurrentUserId), "Id", "Titulo", registro.HabilidadId);
        return View(registro);
    }

    // GET: REGISTROS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var registro = await _context.Registros
            .Include(r => r.Habilidad)
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == CurrentUserId);
        if (registro == null)
        {
            return NotFound();
        }

        return View(registro);
    }

    // POST: REGISTROS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var registro = await _context.Registros
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == CurrentUserId);
        if (registro != null)
        {
            _context.Registros.Remove(registro);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool RegistroExists(int? id)
    {
        return _context.Registros.Any(e => e.Id == id && e.UserId == CurrentUserId);
    }
}
