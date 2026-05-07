using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Achievement.Data;
using Achievement.Models;

namespace Achievement.Pages.Games
{
    public class IndexModel : PageModel
    {
        private readonly Achievement.Data.ApplicationDbContext _context;

        public IndexModel(Achievement.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Game> Game { get; set; } = default!;

        // Simplified pagination parameters
        public int PageSize { get; set; } = 20;
        public int PageNumber { get; set; } = 1;

        public string? Search { get; set; }

        public async Task OnGetAsync(int? pageNumber, string? search)
        {
            PageNumber = pageNumber ?? 1;
            Search = search;

            var query = _context.Games.AsQueryable();

            // Only active games in admin list? Admin should see all; keep all.

            if (!string.IsNullOrWhiteSpace(Search))
            {
                query = query.Where(g => g.Name.Contains(Search));
            }

            Game = await query
                .OrderByDescending(g => g.ReleaseDate)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}
