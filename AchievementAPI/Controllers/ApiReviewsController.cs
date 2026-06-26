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
    public class ApiReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviews()
        {
            var reviews = await _context.Reviews.AsNoTracking().ToListAsync();
            return Ok(reviews.Select(r => MapToDto(r)));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDto>> GetReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound();
            return Ok(MapToDto(review));
        }

        [HttpPost]
        public async Task<ActionResult<ReviewDto>> PostReview(ReviewCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // verify referenced entities exist
            var game = await _context.Games.FindAsync(dto.GameId);
            if (game == null) return BadRequest("Game not found.");
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null) return BadRequest("User not found.");

            var review = new Review
            {
                ReviewContent = dto.ReviewContent,
                Rating = dto.Rating,
                GameFK = dto.GameId,
                UserFK = dto.UserId
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReview), new { id = review.Id }, MapToDto(review));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutReview(int id, ReviewCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound();

            // allow update of content and rating; optionally change references
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

        [HttpDelete("{id}")]
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
            GameId = r.GameFK,
            UserId = r.UserFK
        };
    }
}
