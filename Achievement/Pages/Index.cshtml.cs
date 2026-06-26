using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Achievement.Data;
using Achievement.Models;

namespace Achievement.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>Jogos em destaque no hero (mais jogados de sempre, com banner).</summary>
        public IList<Game> Featured { get; set; } = new List<Game>();

        /// <summary>Populares da semana: mais reviews nos últimos 7 dias.</summary>
        public IList<Game> PopularThisWeek { get; set; } = new List<Game>();

        /// <summary>Adicionados recentemente.</summary>
        public IList<Game> RecentlyAdded { get; set; } = new List<Game>();

        /// <summary>Reviews mais recentes, para o grid.</summary>
        public IList<Review> RecentReviews { get; set; } = new List<Review>();

        public async Task OnGetAsync()
        {
            var weekAgo = DateTime.UtcNow.AddDays(-7);

            Featured = await _context.Games
                .Where(g => g.BannerImage != null)
                .OrderByDescending(g => g.Plays)
                .Take(5)
                .ToListAsync();

            PopularThisWeek = await _context.Games
                .OrderByDescending(g => g.Reviews.Count(r => r.CreatedAt >= weekAgo))
                .ThenByDescending(g => g.Plays)
                .Take(6)
                .ToListAsync();

            RecentlyAdded = await _context.Games
                .OrderByDescending(g => g.CreatedAt)
                .Take(6)
                .ToListAsync();

            RecentReviews = await _context.Reviews
                .Include(r => r.Game)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .Take(8)
                .ToListAsync();
        }
    }
}
