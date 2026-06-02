
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevPath.Models;

public class AreasController : Controller
{
    private readonly DevPathContext _context;

    public AreasController(DevPathContext context)
    {
        _context = context;
    }

    // GET: AREAS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Areas.ToListAsync());
    }

    // GET: AREAS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var area = await _context.Areas
            .FirstOrDefaultAsync(m => m.Id == id);
        if (area == null)
        {
            return NotFound();
        }

        return View(area);
    }

    // GET: AREAS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: AREAS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion,Color,Habilidades")] Area area)
    {
        if (ModelState.IsValid)
        {
            _context.Add(area);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(area);
    }

    // GET: AREAS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var area = await _context.Areas.FindAsync(id);
        if (area == null)
        {
            return NotFound();
        }
        return View(area);
    }

    // POST: AREAS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Nombre,Descripcion,Color,Habilidades")] Area area)
    {
        if (id != area.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(area);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AreaExists(area.Id))
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
        return View(area);
    }

    // GET: AREAS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var area = await _context.Areas
            .FirstOrDefaultAsync(m => m.Id == id);
        if (area == null)
        {
            return NotFound();
        }

        return View(area);
    }

    // POST: AREAS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var area = await _context.Areas.FindAsync(id);
        if (area != null)
        {
            _context.Areas.Remove(area);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool AreaExists(int? id)
    {
        return _context.Areas.Any(e => e.Id == id);
    }
}
