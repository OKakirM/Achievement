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
    public class ApiGenresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiGenresController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GenreDto>>> GetGenres()
        {
            var genres = await _context.Genres.ToListAsync();
            return Ok(genres.Select(g => MapToDto(g)));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GenreDto>> GetGenre(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null) return NotFound();
            return Ok(MapToDto(genre));
        }

        [HttpPost]
        public async Task<ActionResult<GenreDto>> PostGenre(GenreCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var exists = await _context.Genres.AnyAsync(g => g.Name == dto.Name);
            if (exists) return BadRequest("Genre with this name already exists.");

            var genre = new Genre { Name = dto.Name };
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGenre), new { id = genre.Id }, MapToDto(genre));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutGenre(int id, GenreCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var genre = await _context.Genres.FindAsync(id);
            if (genre == null) return NotFound();

            genre.Name = dto.Name;
            _context.Genres.Update(genre);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null) return NotFound();

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static GenreDto MapToDto(Genre g) => new GenreDto { Id = g.Id, Name = g.Name };
    }
}
