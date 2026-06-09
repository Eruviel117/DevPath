
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevPath.Models;

public class HabilidadController : Controller
{
    private readonly DevPathContext _context;

    public HabilidadController(DevPathContext context)
    {
        _context = context;
    }

    // GET: HABILIDADS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Habilidades.ToListAsync());
    }

    // GET: HABILIDADS/Details/5
    public async Task<IActionResult> Details(int? id)
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

    // GET: HABILIDADS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: HABILIDADS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Titulo,Descripcion,Nivel,Estado,AreaId,Area,Recursos,Registros")] Habilidad habilidad)
    {
        if (ModelState.IsValid)
        {
            _context.Add(habilidad);
            await _context.SaveChangesAsync();
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
            _context.Habilidades.Remove(habilidad);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool HabilidadExists(int? id)
    {
        return _context.Habilidades.Any(e => e.Id == id);
    }
}
