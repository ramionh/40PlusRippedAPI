using _40PlusRipped.Core.Models;
using _40PlusRipped.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _40PlusRipped.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoreAreasController : ControllerBase
    {
        private readonly FortyPlusRippedDbContext _context;

        public CoreAreasController(FortyPlusRippedDbContext context)
        {
            _context = context;
        }

        // GET: api/CoreAreas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CoreArea>>> GetCoreAreas()
        {
            return await _context.CoreAreas.ToListAsync();
        }

        // GET: api/CoreAreas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CoreArea>> GetCoreArea(int id)
        {
            var coreArea = await _context.CoreAreas.FindAsync(id);

            if (coreArea == null)
            {
                return NotFound();
            }

            return coreArea;
        }

        // POST: api/CoreAreas
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CoreArea>> PostCoreArea(CoreArea coreArea)
        {
            _context.CoreAreas.Add(coreArea);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCoreArea), new { id = coreArea.Id }, coreArea);
        }

        // PUT: api/CoreAreas/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutCoreArea(int id, CoreArea coreArea)
        {
            if (id != coreArea.Id)
            {
                return BadRequest();
            }

            _context.Entry(coreArea).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CoreAreaExists(id))
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

        // DELETE: api/CoreAreas/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCoreArea(int id)
        {
            var coreArea = await _context.CoreAreas.FindAsync(id);
            if (coreArea == null)
            {
                return NotFound();
            }

            _context.CoreAreas.Remove(coreArea);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CoreAreaExists(int id)
        {
            return _context.CoreAreas.Any(e => e.Id == id);
        }
    }
}