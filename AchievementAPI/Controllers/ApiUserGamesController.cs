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
    public class ApiUserGamesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiUserGamesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todas as entradas de biblioteca (jogos de utilizadores e respetivo estado).
        /// </summary>
        // GET: api/ApiUserGames
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IEnumerable<UserGameDto>>> GetUserGames()
        {
            var entries = await _context.UserGames
                .Include(ug => ug.User)
                .Include(ug => ug.Game)
                .AsNoTracking()
                .ToListAsync();

            return Ok(entries.Select(MapToDto));
        }

        /// <summary>
        /// Obtém a entrada de biblioteca de um utilizador para um jogo (chave composta).
        /// </summary>
        // GET: api/ApiUserGames/5/10
        [HttpGet("{userId}/{gameId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserGameDto>> GetUserGame(int userId, int gameId)
        {
            var entry = await _context.UserGames
                .Include(ug => ug.User)
                .Include(ug => ug.Game)
                .AsNoTracking()
                .FirstOrDefaultAsync(ug => ug.UserFK == userId && ug.GameFK == gameId);

            if (entry == null) return NotFound();
            return Ok(MapToDto(entry));
        }

        /// <summary>
        /// Adiciona um jogo à biblioteca de um utilizador com um estado. Falha se a entrada já existir.
        /// </summary>
        // POST: api/ApiUserGames
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<UserGameDto>> PostUserGame(UserGameDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null) return BadRequest("User not found.");

            var game = await _context.Games.FindAsync(dto.GameId);
            if (game == null) return BadRequest("Game not found.");

            var exists = await _context.UserGames.AnyAsync(ug => ug.UserFK == dto.UserId && ug.GameFK == dto.GameId);
            if (exists) return BadRequest("Entry already exists.");

            var entry = new UserGame
            {
                UserFK = dto.UserId,
                GameFK = dto.GameId,
                Status = dto.Status
            };

            _context.UserGames.Add(entry);
            await _context.SaveChangesAsync();

            var result = MapToDto(entry);
            return CreatedAtAction(nameof(GetUserGame), new { userId = entry.UserFK, gameId = entry.GameFK }, result);
        }

        /// <summary>
        /// Atualiza o estado de um jogo na biblioteca de um utilizador.
        /// </summary>
        // PUT: api/ApiUserGames/5/10
        [HttpPut("{userId}/{gameId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> PutUserGame(int userId, int gameId, UserGameDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (userId != dto.UserId || gameId != dto.GameId) return BadRequest();

            var entry = await _context.UserGames.FindAsync(userId, gameId);
            if (entry == null) return NotFound();

            entry.Status = dto.Status;

            _context.UserGames.Update(entry);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Remove um jogo da biblioteca de um utilizador.
        /// </summary>
        // DELETE: api/ApiUserGames/5/10
        [HttpDelete("{userId}/{gameId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> DeleteUserGame(int userId, int gameId)
        {
            var entry = await _context.UserGames.FindAsync(userId, gameId);
            if (entry == null) return NotFound();

            _context.UserGames.Remove(entry);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static UserGameDto MapToDto(UserGame ug)
        {
            return new UserGameDto
            {
                UserId = ug.UserFK,
                GameId = ug.GameFK,
                Status = ug.Status,
                UserName = ug.User?.Name,
                GameName = ug.Game?.Name
            };
        }
    }
}
