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
    public class ApiReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todas as reviews.
        /// </summary>
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviews()
        {
            var reviews = await _context.Reviews.AsNoTracking().ToListAsync();
            return Ok(reviews.Select(r => MapToDto(r)));
        }

        /// <summary>
        /// Obtém uma review pelo Id.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ReviewDto>> GetReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound();
            return Ok(MapToDto(review));
        }

        /// <summary>
        /// Cria uma review. Valida que o jogo e o utilizador existem.
        /// </summary>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<ReviewDto>> PostReview(ReviewCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Valida se o jogo e o utilizador existem
            var game = await _context.Games.FindAsync(dto.GameId);
            if (game == null) return BadRequest("Game not found.");
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null) return BadRequest("User not found.");

            var review = new Review
            {
                ReviewContent = dto.ReviewContent,
                Rating = dto.Rating,
                GameFK = dto.GameId,
                UserFK = dto.UserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReview), new { id = review.Id }, MapToDto(review));
        }

        /// <summary>
        /// Atualiza o conteúdo, avaliação e referências de uma review.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> PutReview(int id, ReviewCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound();

            // Atualiza os campos da review
            review.ReviewContent = dto.ReviewContent;
            review.Rating = dto.Rating;

            if (dto.GameId != review.GameFK)
            {
                var g = await _context.Games.FindAsync(dto.GameId);
                if (g == null) return BadRequest("Game not found.");
                review.GameFK = dto.GameId;
            }

            if (dto.UserId != review.UserFK)
            {
                var u = await _context.Users.FindAsync(dto.UserId);
                if (u == null) return BadRequest("User not found.");
                review.UserFK = dto.UserId;
            }

            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Remove uma review.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound();

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static ReviewDto MapToDto(Review r) => new ReviewDto
        {
            Id = r.Id,
            ReviewContent = r.ReviewContent,
            Rating = r.Rating,
            CreatedAt = r.CreatedAt,
            GameId = r.GameFK,
            UserId = r.UserFK
        };
    }
}
