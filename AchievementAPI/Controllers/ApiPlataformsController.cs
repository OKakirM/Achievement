using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Achievement.Data;
using Achievement.Models;
using AchievementAPI.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AchievementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiPlatformsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiPlatformsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlatformDto>>> GetPlatforms()
        {
            var list = await _context.Platforms.ToListAsync();
            return Ok(list.Select(p => MapToDto(p)));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlatformDto>> GetPlatform(int id)
        {
            var p = await _context.Platforms.FindAsync(id);
            if (p == null) return NotFound();
            return Ok(MapToDto(p));
        }

        [HttpPost]
        public async Task<ActionResult<PlatformDto>> PostPlatform(PlatformCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var exists = await _context.Platforms.AnyAsync(x => x.Name == dto.Name && x.Type.ToString() == dto.Type);
            if (exists) return BadRequest("Platform already exists.");

            // try parse enum if possible
            Platform p = new Platform { Name = dto.Name };
            // parse type enum if matches
            if (Enum.TryParse<PlatformType>(dto.Type, out var parsed)) p.Type = parsed;

            _context.Platforms.Add(p);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlatform), new { id = p.Id }, MapToDto(p));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlatform(int id, PlatformCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var p = await _context.Platforms.FindAsync(id);
            if (p == null) return NotFound();

            p.Name = dto.Name;
            if (Enum.TryParse<PlatformType>(dto.Type, out var parsed)) p.Type = parsed;

            _context.Platforms.Update(p);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlatform(int id)
        {
            var p = await _context.Platforms.FindAsync(id);
            if (p == null) return NotFound();

            _context.Platforms.Remove(p);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static PlatformDto MapToDto(Platform p) => new PlatformDto { Id = p.Id, Name = p.Name, Type = p.Type.ToString() };
    }
}
