using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevPath.Models;

namespace DevPath.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class HabilidadesApiController : ControllerBase
    {
        private readonly DevPathContext _context;

        public HabilidadesApiController(DevPathContext context)
        {
            _context = context;
        }

        // GET: api/HabilidadesApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Habilidad>>> GetHabilidades()
        {
            return await _context.Habilidades
                .Include(h => h.Area)
                .ToListAsync();
        }

        // GET: api/HabilidadesApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Habilidad>> GetHabilidad(int id)
        {
            var habilidad = await _context.Habilidades
                .Include(h => h.Area)
                .FirstOrDefaultAsync(h => h.Id == id);

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

            _context.Entry(habilidad).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Habilidades.Any(e => e.Id == id))
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
            var habilidad = await _context.Habilidades.FindAsync(id);
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