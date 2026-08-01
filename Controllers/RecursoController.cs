
using DevPath.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[Authorize]
public class RecursoController : Controller
{
    private readonly DevPathContext _context;

    public RecursoController(DevPathContext context)
    {
        _context = context;
    }

    // Id del usuario autenticado. Todas las consultas de este controlador
    // se filtran por este valor para que cada usuario solo vea sus propios
    // Recursos.
    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // GET: RECURSOS
    public async Task<IActionResult> Index()
    {
        var recursos = await _context.Recursos
            .Where(r => r.UserId == CurrentUserId)
            .Include(r => r.Habilidad)
            .ToListAsync();
        return View(recursos);
    }

    // GET: RECURSOS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var recurso = await _context.Recursos
            .Include(r => r.Habilidad)
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == CurrentUserId);

        if (recurso == null)
        {
            return NotFound();
        }

        return View(recurso);
    }

    // GET: RECURSOS/Create
    public IActionResult Create()
    {
        ViewData["HabilidadId"] = new SelectList(
            _context.Habilidades.Where(h => h.UserId == CurrentUserId), "Id", "Titulo");
        return View();
    }

    // POST: RECURSOS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Nombre,Url,Tipo,Completado,HabilidadId")] Recurso recurso)
    {
        // La Habilidad elegida debe pertenecer al usuario actual.
        var habilidadValida = await _context.Habilidades
            .AnyAsync(h => h.Id == recurso.HabilidadId && h.UserId == CurrentUserId);
        if (!habilidadValida)
        {
            ModelState.AddModelError(nameof(Recurso.HabilidadId), "Habilidad inválida.");
        }

        if (ModelState.IsValid)
        {
            recurso.UserId = CurrentUserId;
            _context.Add(recurso);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["HabilidadId"] = new SelectList(
            _context.Habilidades.Where(h => h.UserId == CurrentUserId), "Id", "Titulo", recurso.HabilidadId);
        return View(recurso);
    }


    // GET: RECURSOS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var recurso = await _context.Recursos
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == CurrentUserId);
        if (recurso == null)
        {
            return NotFound();
        }
        ViewData["HabilidadId"] = new SelectList(
            _context.Habilidades.Where(h => h.UserId == CurrentUserId), "Id", "Titulo", recurso.HabilidadId);
        return View(recurso);
    }


    // POST: RECURSOS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Nombre,Url,Tipo,Completado,HabilidadId")] Recurso recurso)
    {
        if (id != recurso.Id)
        {
            return NotFound();
        }

        var existe = await _context.Recursos
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == CurrentUserId);
        if (existe == null)
        {
            return NotFound();
        }

        var habilidadValida = await _context.Habilidades
            .AnyAsync(h => h.Id == recurso.HabilidadId && h.UserId == CurrentUserId);
        if (!habilidadValida)
        {
            ModelState.AddModelError(nameof(Recurso.HabilidadId), "Habilidad inválida.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                recurso.UserId = CurrentUserId;
                _context.Update(recurso);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecursoExists(recurso.Id))
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
            _context.Habilidades.Where(h => h.UserId == CurrentUserId), "Id", "Titulo", recurso.HabilidadId);
        return View(recurso);
    }

    // GET: RECURSOS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var recurso = await _context.Recursos
            .Include(r => r.Habilidad)
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == CurrentUserId);
        if (recurso == null)
        {
            return NotFound();
        }

        return View(recurso);
    }

    // POST: RECURSOS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var recurso = await _context.Recursos
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == CurrentUserId);
        if (recurso != null)
        {
            _context.Recursos.Remove(recurso);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool RecursoExists(int? id)
    {
        return _context.Recursos.Any(e => e.Id == id && e.UserId == CurrentUserId);
    }

    // POST: Recurso/ToggleCompletado/5
    [HttpPost]
    public async Task<IActionResult> ToggleCompletado(int id)
    {
        var recurso = await _context.Recursos
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == CurrentUserId);
        if (recurso == null)
        {
            return NotFound();
        }

        recurso.Completado = !recurso.Completado;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

}
