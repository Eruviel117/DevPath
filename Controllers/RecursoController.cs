
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevPath.Models;

public class RecursoController : Controller
{
    private readonly DevPathContext _context;

    public RecursoController(DevPathContext context)
    {
        _context = context;
    }

    // GET: RECURSOS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Recursos.ToListAsync());
    }

    // GET: RECURSOS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var recurso = await _context.Recursos
            .FirstOrDefaultAsync(m => m.Id == id);
        if (recurso == null)
        {
            return NotFound();
        }

        return View(recurso);
    }

    // GET: RECURSOS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: RECURSOS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Nombre,Url,Tipo,Completado,HabilidadId")] Recurso recurso)
    {


        if (ModelState.IsValid)
        {
            _context.Add(recurso);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(recurso);
    }


    // GET: RECURSOS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var recurso = await _context.Recursos.FindAsync(id);
        if (recurso == null)
        {
            return NotFound();
        }
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

        if (ModelState.IsValid)
        {
            try
            {
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
            .FirstOrDefaultAsync(m => m.Id == id);
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
        var recurso = await _context.Recursos.FindAsync(id);
        if (recurso != null)
        {
            _context.Recursos.Remove(recurso);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool RecursoExists(int? id)
    {
        return _context.Recursos.Any(e => e.Id == id);
    }

}
