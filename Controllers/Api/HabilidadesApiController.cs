using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevPath.Models;
using System.Security.Claims;

namespace DevPath.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HabilidadesApiController : ControllerBase
    {
        private readonly DevPathContext _context;

        public HabilidadesApiController(DevPathContext context)
        {
            _context = context;
        }

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // GET: api/HabilidadesApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Habilidad>>> GetHabilidades()
        {
            return await _context.Habilidades
                .Include(h => h.Area)
                .Where(h => h.UserId == CurrentUserId)
                .ToListAsync();
        }

        // GET: api/HabilidadesApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Habilidad>> GetHabilidad(int id)
        {
            var habilidad = await _context.Habilidades
                .Include(h => h.Area)
                .FirstOrDefaultAsync(h => h.Id == id && h.UserId == CurrentUserId);

            if (habilidad == null)
            {
                return NotFound();
            }

            return habilidad;
        }

        // POST: api/HabilidadesApi
        [HttpPost]
        public async Task<ActionResult<Habilidad>> PostHabilidad(Habilidad habilidad)
        {
            var areaValida = await _context.Areas
                .AnyAsync(a => a.Id == habilidad.AreaId && a.UserId == CurrentUserId);

            if (!areaValida)
            {
                return BadRequest("El área indicada no existe o no pertenece al usuario actual.");
            }

            habilidad.UserId = CurrentUserId;
            _context.Habilidades.Add(habilidad);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHabilidad), new { id = habilidad.Id }, habilidad);
        }

        // PUT: api/HabilidadesApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHabilidad(int id, Habilidad habilidad)
        {
            if (id != habilidad.Id)
            {
                return BadRequest();
            }

            var existente = await _context.Habilidades
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == id && h.UserId == CurrentUserId);

            if (existente == null)
            {
                return NotFound();
            }

            var areaValida = await _context.Areas
                .AnyAsync(a => a.Id == habilidad.AreaId && a.UserId == CurrentUserId);

            if (!areaValida)
            {
                return BadRequest("El área indicada no existe o no pertenece al usuario actual.");
            }

            habilidad.UserId = CurrentUserId;
            _context.Entry(habilidad).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Habilidades.Any(e => e.Id == id && e.UserId == CurrentUserId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/HabilidadesApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHabilidad(int id)
        {
            var habilidad = await _context.Habilidades
                .FirstOrDefaultAsync(h => h.Id == id && h.UserId == CurrentUserId);

            if (habilidad == null)
            {
                return NotFound();
            }

            _context.Habilidades.Remove(habilidad);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
