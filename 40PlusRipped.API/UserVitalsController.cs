using _40PlusRipped.Core.Models;
using _40PlusRipped.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace _40PlusRipped.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserVitalsController : ControllerBase
    {
        private readonly FortyPlusRippedDbContext _context;

        public UserVitalsController(FortyPlusRippedDbContext context)
        {
            _context = context;
        }

        // GET: api/UserVitals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserVitals>>> GetUserVitals()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.UserVitals
                .Where(v => v.UserId == userId)
                .OrderByDescending(v => v.RecordedDate)
                .ToListAsync();
        }

        // GET: api/UserVitals/latest
        [HttpGet("latest")]
        public async Task<ActionResult<UserVitals>> GetLatestUserVitals()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userVitals = await _context.UserVitals
                .Where(v => v.UserId == userId)
                .OrderByDescending(v => v.RecordedDate)
                .FirstOrDefaultAsync();

            if (userVitals == null)
            {
                return NotFound();
            }

            return userVitals;
        }

        // GET: api/UserVitals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserVitals>> GetUserVitals(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userVitals = await _context.UserVitals
                .FirstOrDefaultAsync(v => v.Id == id && v.UserId == userId);

            if (userVitals == null)
            {
                return NotFound();
            }

            return userVitals;
        }

        // POST: api/UserVitals
        [HttpPost]
        public async Task<ActionResult<UserVitals>> PostUserVitals(UserVitals userVitals)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            userVitals.UserId = userId;
            userVitals.RecordedDate = DateTime.UtcNow;

            // Calculate BMI
            if (userVitals.Height > 0)
            {
                // Height in meters, weight in kg
                decimal heightInMeters = userVitals.Height / 100; // Convert cm to meters
                userVitals.BMI = userVitals.Weight / (heightInMeters * heightInMeters);
            }

            _context.UserVitals.Add(userVitals);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserVitals), new { id = userVitals.Id }, userVitals);
        }

        // PUT: api/UserVitals/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserVitals(int id, UserVitals userVitals)
        {
            if (id != userVitals.Id)
            {
                return BadRequest();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingVitals = await _context.UserVitals
                .FirstOrDefaultAsync(v => v.Id == id && v.UserId == userId);

            if (existingVitals == null)
            {
                return NotFound();
            }

            // Update properties
            existingVitals.Height = userVitals.Height;
            existingVitals.Weight = userVitals.Weight;
            existingVitals.BodyFatPercentage = userVitals.BodyFatPercentage;
            existingVitals.Notes = userVitals.Notes;

            // Recalculate BMI
            if (existingVitals.Height > 0)
            {
                decimal heightInMeters = existingVitals.Height / 100; // Convert cm to meters
                existingVitals.BMI = existingVitals.Weight / (heightInMeters * heightInMeters);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserVitalsExists(id))
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

        // DELETE: api/UserVitals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserVitals(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userVitals = await _context.UserVitals
                .FirstOrDefaultAsync(v => v.Id == id && v.UserId == userId);

            if (userVitals == null)
            {
                return NotFound();
            }

            _context.UserVitals.Remove(userVitals);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserVitalsExists(int id)
        {
            return _context.UserVitals.Any(e => e.Id == id);
        }
    }
}