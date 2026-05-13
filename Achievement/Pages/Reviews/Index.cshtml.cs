using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Achievement.Data;
using Achievement.Models;

namespace Achievement.Pages.Reviews
{
    public class IndexModel : PageModel
    {
        private readonly Achievement.Data.ApplicationDbContext _context;

        public IndexModel(Achievement.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        // Results shown on the page
        public IList<Review> Review { get; set; } = default!;

        // Basic paging and filtering parameters (optional)
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public async Task OnGetAsync(int? rating, int? gameId, string? q, int pageNumber = 1, int pageSize = 10, bool showHidden = false)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;

            var query = _context.Reviews
                .Include(r => r.Game)
                .Include(r => r.User)
                .AsQueryable();

            if (rating.HasValue)
            {
                query = query.Where(r => r.Rating == rating.Value);
            }

            if (gameId.HasValue)
            {
                query = query.Where(r => r.GameFK == gameId.Value);
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                var text = q.Trim();
                query = query.Where(r => r.ReviewContent.Contains(text));
            }

            TotalCount = await query.CountAsync();

            Review = await query
                .OrderByDescending(r => r.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
