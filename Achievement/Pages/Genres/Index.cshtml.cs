using Achievement.Data;
using Achievement.Helpers;
using Achievement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Achievement.Pages.Genres
{
    public class IndexModel : PageModel
    {
        private readonly Achievement.Data.ApplicationDbContext _context;

        public IndexModel(Achievement.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Genre> Genre { get; set; } = default!;
        public string? Search { get; set; }

        public async Task OnGetAsync(string? search)
        {
            Search = search;

            // Include Games to be able to show counts without additional queries
            var genres = await _context.Genres
                .Include(g => g.Games)
                .OrderBy(g => g.Name)
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                // Filtra em memória por nome, ignorando maiúsculas/acentos.
                var term = TextSearch.Normalize(search);
                genres = genres.Where(g => TextSearch.Normalize(g.Name).Contains(term)).ToList();
            }

            Genre = genres;
        }
    }
}
