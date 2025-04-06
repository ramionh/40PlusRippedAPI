using _40PlusRipped.Core.Models;
using _40PlusRipped.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace _40PlusRipped.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HealthLevelsController : ControllerBase
    {
        private readonly FortyPlusRippedDbContext _context;

        public HealthLevelsController(FortyPlusRippedDbContext context)
        {
            _context = context;
        }

        // GET: api/HealthLevels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HealthLevel>>> GetHealthLevels()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.HealthLevels
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.RecordedDate)
                .ToListAsync();
        }

        // GET: api/HealthLevels/latest
        [HttpGet("latest")]
        public async Task<ActionResult<HealthLevel>> GetLatestHealthLevel()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var healthLevel = await _context.HealthLevels
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.RecordedDate)
                .FirstOrDefaultAsync();

            if (healthLevel == null)
            {
                return NotFound();
            }

            return healthLevel;
        }

        // GET: api/HealthLevels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HealthLevel>> GetHealthLevel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var healthLevel = await _context.HealthLevels
                .FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);

            if (healthLevel == null)
            {
                return NotFound();
            }

            return healthLevel;
        }

        // POST: api/HealthLevels
        [HttpPost]
        public async Task<ActionResult<HealthLevel>> PostHealthLevel(HealthLevel healthLevel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            healthLevel.UserId = userId;
            healthLevel.RecordedDate = DateTime.UtcNow;

            _context.HealthLevels.Add(healthLevel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHealthLevel), new { id = healthLevel.Id }, healthLevel);
        }

        // PUT: api/HealthLevels/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHealthLevel(int id, HealthLevel healthLevel)
        {
            if (id != healthLevel.Id)
            {
                return BadRequest();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingHealthLevel = await _context.HealthLevels
                .FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);

            if (existingHealthLevel == null)
            {
                return NotFound();
            }

            // Update properties
            existingHealthLevel.PhysicalActivityLevel = healthLevel.PhysicalActivityLevel;
            existingHealthLevel.NutritionQuality = healthLevel.NutritionQuality;
            existingHealthLevel.SleepQuality = healthLevel.SleepQuality;
            existingHealthLevel.StressLevel = healthLevel.StressLevel;
            existingHealthLevel.OverallHealth = healthLevel.OverallHealth;
            existingHealthLevel.Notes = healthLevel.Notes;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HealthLevelExists(id))
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

        // DELETE: api/HealthLevels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHealthLevel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var healthLevel = await _context.HealthLevels
                .FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);

            if (healthLevel == null)
            {
                return NotFound();
            }

            _context.HealthLevels.Remove(healthLevel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HealthLevelExists(int id)
        {
            return _context.HealthLevels.Any(e => e.Id == id);
        }
    }
}