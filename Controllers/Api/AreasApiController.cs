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
    public class AreasApiController : ControllerBase
    {
        private readonly DevPathContext _context;

        public AreasApiController(DevPathContext context)
        {
            _context = context;
        }

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // GET: api/AreasApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Area>>> GetAreas()
        {
            return await _context.Areas
                .Where(a => a.UserId == CurrentUserId)
                .ToListAsync();
        }

        // GET: api/AreasApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Area>> GetArea(int id)
        {
            var area = await _context.Areas
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == CurrentUserId);

            if (area == null)
            {
                return NotFound();
            }

            return area;
        }

        // POST: api/AreasApi
        [HttpPost]
        public async Task<ActionResult<Area>> PostArea(Area area)
        {
            area.UserId = CurrentUserId;
            _context.Areas.Add(area);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetArea), new { id = area.Id }, area);
        }

        // PUT: api/AreasApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArea(int id, Area area)
        {
            if (id != area.Id)
            {
                return BadRequest();
            }

            var existente = await _context.Areas
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == CurrentUserId);

            if (existente == null)
            {
                return NotFound();
            }

            area.UserId = CurrentUserId;
            _context.Entry(area).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Areas.Any(e => e.Id == id && e.UserId == CurrentUserId))
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

        // DELETE: api/AreasApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArea(int id)
        {
            var area = await _context.Areas
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == CurrentUserId);

            if (area == null)
            {
                return NotFound();
            }

            _context.Areas.Remove(area);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
