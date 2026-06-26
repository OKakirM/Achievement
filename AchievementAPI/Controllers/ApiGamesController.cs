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
    public class ApiGamesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiGamesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ApiGames
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetGames([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var games = await _context.Games
                .Include(g => g.Plataforms)
                .Include(g => g.Genres)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = games.Select(g => MapToDto(g)).ToList();

            return Ok(result);
        }

        // GET: api/ApiGames/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GameDto>> GetGame(int id)
        {
            var game = await _context.Games
                .Include(g => g.Plataforms)
                .Include(g => g.Genres)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
                return NotFound();

            return Ok(MapToDto(game));
        }

        // POST: api/ApiGames
        [HttpPost]
        public async Task<ActionResult<GameDto>> PostGame(GameCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var game = new Game
            {
                Name = dto.Name,
                Description = dto.Description,
                ReleaseDate = dto.ReleaseDate,
                Rating = dto.Rating,
                Length = dto.Length,
                CoverImage = dto.CoverImage,
                BannerImage = dto.BannerImage,
                Plays = dto.Plays
            };

            // relacionamentos
            if (dto.PlataformIds != null)
            {
                foreach (var pid in dto.PlataformIds.Distinct())
                {
                    var p = await _context.Plataforms.FindAsync(pid);
                    if (p != null) game.Plataforms.Add(p);
                }
            }

            if (dto.GenreIds != null)
            {
                foreach (var gid in dto.GenreIds.Distinct())
                {
                    var gr = await _context.Genres.FindAsync(gid);
                    if (gr != null) game.Genres.Add(gr);
                }
            }

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            var resultDto = MapToDto(game);

            return CreatedAtAction(nameof(GetGame), new { id = game.Id }, resultDto);
        }

        // PUT: api/ApiGames/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGame(int id, GameUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var game = await _context.Games
                .Include(g => g.Plataforms)
                .Include(g => g.Genres)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
                return NotFound();

            // atualizar campos quando fornecidos
            if (!string.IsNullOrEmpty(dto.Name)) game.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Description)) game.Description = dto.Description;
            if (dto.ReleaseDate.HasValue) game.ReleaseDate = dto.ReleaseDate.Value;
            if (dto.Rating.HasValue) game.Rating = dto.Rating.Value;
            if (!string.IsNullOrEmpty(dto.Length)) game.Length = dto.Length;
            if (!string.IsNullOrEmpty(dto.CoverImage)) game.CoverImage = dto.CoverImage;
            if (!string.IsNullOrEmpty(dto.BannerImage)) game.BannerImage = dto.BannerImage;
            if (dto.Plays.HasValue) game.Plays = dto.Plays.Value;

            // atualizar relacionamentos se ids fornecidos
            if (dto.PlataformIds != null)
            {
                game.Plataforms.Clear();
                foreach (var pid in dto.PlataformIds.Distinct())
                {
                    var p = await _context.Plataforms.FindAsync(pid);
                    if (p != null) game.Plataforms.Add(p);
                }
            }

            if (dto.GenreIds != null)
            {
                game.Genres.Clear();
                foreach (var gid in dto.GenreIds.Distinct())
                {
                    var gr = await _context.Genres.FindAsync(gid);
                    if (gr != null) game.Genres.Add(gr);
                }
            }

            _context.Games.Update(game);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/ApiGames/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
                return NotFound();

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // mapping helper
        private static GameDto MapToDto(Game g)
        {
            return new GameDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                ReleaseDate = g.ReleaseDate,
                Rating = g.Rating,
                Length = g.Length,
                CoverImage = g.CoverImage,
                BannerImage = g.BannerImage,
                Plays = g.Plays,
                Platforms = g.Plataforms?.Select(p => p.Name) ?? Enumerable.Empty<string>(),
                Genres = g.Genres?.Select(x => x.Name) ?? Enumerable.Empty<string>()
            };
        }
    }
}
