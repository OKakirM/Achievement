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

namespace Achievement.Pages.Platforms
{
    public class IndexModel : PageModel
    {
        private readonly Achievement.Data.ApplicationDbContext _context;

        public IndexModel(Achievement.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Platform> Platform { get;set; } = default!;
        public string? Search { get; set; }

        public async Task OnGetAsync(string? search, bool showHidden = false)
        {
            Search = search;

            var platforms = await _context.Platforms
                .Include(p => p.Games)
                .OrderBy(p => p.Type).ThenBy(p => p.Name)
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                // Filtra em memória por nome do aparelho ou tipo, ignorando maiúsculas/acentos.
                var term = TextSearch.Normalize(search);
                platforms = platforms
                    .Where(p => TextSearch.Normalize(p.Name).Contains(term)
                             || TextSearch.Normalize(p.Type.DisplayName()).Contains(term))
                    .ToList();
            }

            Platform = platforms;
        }
    }
}
