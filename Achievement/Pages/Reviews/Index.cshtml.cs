using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Achievement.Data;
using Achievement.Helpers;
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
        public string? Search { get; set; }

        public async Task OnGetAsync(int? rating, int? gameId, string? q, int pageNumber = 1, int pageSize = 10, bool showHidden = false)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            Search = q;

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

            if (string.IsNullOrWhiteSpace(q))
            {
                // Sem pesquisa: feed paginado por data, como antes.
                TotalCount = await query.CountAsync();
                Review = await query
                    .OrderByDescending(r => r.CreatedAt)
                    .ThenByDescending(r => r.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                return;
            }

            // Com pesquisa: filtra em memória sobre conteúdo, nome do jogo e autor,
            // ignorando maiúsculas/acentos (que o SQLite não trata).
            // ponytail: O(n) scan em memória; migrar para coluna normalizada ou FTS5 se as reviews passarem de uns milhares.
            var term = TextSearch.Normalize(q);

            var candidates = await query.ToListAsync();

            var matched = candidates
                .Select(r => new { Review = r, Rank = MatchRank(r, term) })
                .Where(x => x.Rank >= 0)
                .ToList();

            TotalCount = matched.Count;

            Review = matched
                .OrderBy(x => x.Rank)
                .ThenByDescending(x => x.Review.CreatedAt)
                .ThenByDescending(x => x.Review.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => x.Review)
                .ToList();
        }

        /// <summary>
        /// Relevância da correspondência (menor = mais relevante); -1 se não corresponde.
        /// </summary>
        private static int MatchRank(Review r, string term)
        {
            if (TextSearch.Normalize(r.Game?.Name).Contains(term)) return 0;
            if (TextSearch.Normalize(r.User?.Name).Contains(term)) return 1;
            if (TextSearch.Normalize(r.ReviewContent).Contains(term)) return 2;
            return -1;
        }
    }
}
