using _40PlusRipped.Core.Models;
using _40PlusRipped.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    public class FitnessGoalsController : ControllerBase
    {
        private readonly FortyPlusRippedDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FitnessGoalsController(FortyPlusRippedDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/FitnessGoals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FitnessGoal>>> GetFitnessGoals()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.FitnessGoals
                .Where(g => g.UserId == userId)
                .ToListAsync();
        }

        // GET: api/FitnessGoals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FitnessGoal>> GetFitnessGoal(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var fitnessGoal = await _context.FitnessGoals
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (fitnessGoal == null)
            {
                return NotFound();
            }

            return fitnessGoal;
        }

        // POST: api/FitnessGoals
        [HttpPost]
        public async Task<ActionResult<FitnessGoal>> PostFitnessGoal(FitnessGoal fitnessGoal)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            fitnessGoal.UserId = userId;

            _context.FitnessGoals.Add(fitnessGoal);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFitnessGoal), new { id = fitnessGoal.Id }, fitnessGoal);
        }

        // PUT: api/FitnessGoals/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFitnessGoal(int id, FitnessGoal fitnessGoal)
        {
            if (id != fitnessGoal.Id)
            {
                return BadRequest();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingGoal = await _context.FitnessGoals
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (existingGoal == null)
            {
                return NotFound();
            }

            // Update properties
            existingGoal.GoalType = fitnessGoal.GoalType;
            existingGoal.TargetValue = fitnessGoal.TargetValue;
            existingGoal.CurrentValue = fitnessGoal.CurrentValue;
            existingGoal.StartDate = fitnessGoal.StartDate;
            existingGoal.TargetDate = fitnessGoal.TargetDate;
            existingGoal.IsCompleted = fitnessGoal.IsCompleted;
            existingGoal.Notes = fitnessGoal.Notes;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FitnessGoalExists(id))
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

        // DELETE: api/FitnessGoals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFitnessGoal(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var fitnessGoal = await _context.FitnessGoals
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (fitnessGoal == null)
            {
                return NotFound();
            }

            _context.FitnessGoals.Remove(fitnessGoal);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FitnessGoalExists(int id)
        {
            return _context.FitnessGoals.Any(e => e.Id == id);
        }
    }
}