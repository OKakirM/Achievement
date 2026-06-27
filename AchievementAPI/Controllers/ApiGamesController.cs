using Achievement.Data;
using Achievement.Models;
using AchievementAPI.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        /// <summary>Lista os jogos de forma paginada.</summary>
        /// <param name="pageNumber">Número da página (começa em 1).</param>
        /// <param name="pageSize">Quantidade de itens por página.</param>
        // GET: api/ApiGames
        [HttpGet("/api/games/")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetGames([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var games = await _context.Games
                .Include(g => g.Platforms)
                .Include(g => g.Genres)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = games.Select(g => MapToDto(g)).ToList();

            return Ok(result);
        }

        /// <summary>Obtém um jogo pelo seu Id.</summary>
        /// <param name="id">Id do jogo.</param>
        // GET: api/ApiGames/5
        [HttpGet("/api/games/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<GameDto>> GetGame(int id)
        {
            var game = await _context.Games
                .Include(g => g.Platforms)
                .Include(g => g.Genres)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
                return NotFound();

            return Ok(MapToDto(game));
        }

        /// <summary>Cria um novo jogo.</summary>
        /// <param name="dto">Dados do jogo a criar.</param>
        // POST: api/ApiGames
        [HttpPost("/api/games/create")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
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
                Plays = dto.Plays,
                CreatedAt = DateTime.UtcNow
            };

            // relacionamentos
            if (dto.PlatformIds != null)
            {
                foreach (var pid in dto.PlatformIds.Distinct())
                {
                    var p = await _context.Platforms.FindAsync(pid);
                    if (p != null) game.Platforms.Add(p);
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

        /// <summary>Atualiza parcialmente um jogo (só os campos fornecidos).</summary>
        /// <param name="id">Id do jogo.</param>
        /// <param name="dto">Campos a atualizar.</param>
        // PUT: api/ApiGames/5
        [HttpPut("/api/games/edit/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> PutGame(int id, GameUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var game = await _context.Games
                .Include(g => g.Platforms)
                .Include(g => g.Genres)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
                return NotFound();

            // atualizar campos quando fornecidos
            if (!string.IsNullOrEmpty(dto.Name)) game.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Description)) game.Description = dto.Description;
            if (dto.ReleaseDate.HasValue) game.ReleaseDate = dto.ReleaseDate.Value;
            if (dto.Rating.HasValue) game.Rating = dto.Rating.Value;
            if (dto.Length.HasValue) game.Length = dto.Length.Value;
            if (!string.IsNullOrEmpty(dto.CoverImage)) game.CoverImage = dto.CoverImage;
            if (!string.IsNullOrEmpty(dto.BannerImage)) game.BannerImage = dto.BannerImage;
            if (dto.Plays.HasValue) game.Plays = dto.Plays.Value;

            // atualizar relacionamentos se ids fornecidos
            if (dto.PlatformIds != null)
            {
                game.Platforms.Clear();
                foreach (var pid in dto.PlatformIds.Distinct())
                {
                    var p = await _context.Platforms.FindAsync(pid);
                    if (p != null) game.Platforms.Add(p);
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

        /// <summary>Remove um jogo.</summary>
        /// <param name="id">Id do jogo.</param>
        // DELETE: api/ApiGames/5
        [HttpDelete("/api/games/delete/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
                return NotFound();

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // mapeia um Game para GameDto
        private static GameDto MapToDto(Game g)
        {
            return new GameDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                ReleaseDate = g.ReleaseDate,
                CreatedAt = g.CreatedAt,
                Rating = g.Rating,
                Length = g.Length,
                CoverImage = g.CoverImage,
                BannerImage = g.BannerImage,
                Plays = g.Plays,
                Platforms = g.Platforms?.Select(p => p.Name) ?? Enumerable.Empty<string>(),
                Genres = g.Genres?.Select(x => x.Name) ?? Enumerable.Empty<string>()
            };
        }
    }
}
