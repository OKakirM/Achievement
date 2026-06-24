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
    public class ApiPlataformsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiPlataformsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlataformDto>>> GetPlataforms()
        {
            var list = await _context.Plataforms.ToListAsync();
            return Ok(list.Select(p => MapToDto(p)));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlataformDto>> GetPlataform(int id)
        {
            var p = await _context.Plataforms.FindAsync(id);
            if (p == null) return NotFound();
            return Ok(MapToDto(p));
        }

        [HttpPost]
        public async Task<ActionResult<PlataformDto>> PostPlataform(PlataformCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var exists = await _context.Plataforms.AnyAsync(x => x.Name == dto.Name && x.Type.ToString() == dto.Type);
            if (exists) return BadRequest("Plataform already exists.");

            // try parse enum if possible
            Plataform p = new Plataform { Name = dto.Name };
            // parse type enum if matches
            if (Enum.TryParse<PlataformType>(dto.Type, out var parsed)) p.Type = parsed;

            _context.Plataforms.Add(p);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlataform), new { id = p.Id }, MapToDto(p));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlataform(int id, PlataformCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var p = await _context.Plataforms.FindAsync(id);
            if (p == null) return NotFound();

            p.Name = dto.Name;
            if (Enum.TryParse<PlataformType>(dto.Type, out var parsed)) p.Type = parsed;

            _context.Plataforms.Update(p);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlataform(int id)
        {
            var p = await _context.Plataforms.FindAsync(id);
            if (p == null) return NotFound();

            _context.Plataforms.Remove(p);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static PlataformDto MapToDto(Plataform p) => new PlataformDto { Id = p.Id, Name = p.Name, Type = p.Type.ToString() };
    }
}
