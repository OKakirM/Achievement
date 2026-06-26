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

        public int? GenreId { get; set; }
        public string? GenreName { get; set; }

        public int? PlatformId { get; set; }
        public string? PlatformName { get; set; }

        // Nome do filtro ativo (género ou plataforma), para o cabeçalho
        public string? FilterName => GenreName ?? PlatformName;

        public async Task OnGetAsync(int? pageNumber, string? search, int? genre, int? platform)
        {
            PageNumber = pageNumber ?? 1;
            Search = search;
            GenreId = genre;
            PlatformId = platform;

            var query = _context.Games.AsQueryable();

            // Only active games in admin list? Admin should see all; keep all.

            if (GenreId.HasValue)
            {
                query = query.Where(g => g.Genres.Any(ge => ge.Id == GenreId.Value));
                GenreName = (await _context.Genres.FindAsync(GenreId.Value))?.Name;
            }

            if (PlatformId.HasValue)
            {
                query = query.Where(g => g.Platforms.Any(p => p.Id == PlatformId.Value));
                PlatformName = (await _context.Platforms.FindAsync(PlatformId.Value))?.Name;
            }

            if (string.IsNullOrWhiteSpace(Search))
            {
                // Sem pesquisa: SQL paginado por data, como antes.
                Game = await query
                    .OrderByDescending(g => g.ReleaseDate)
                    .Skip((PageNumber - 1) * PageSize)
                    .Take(PageSize)
                    .ToListAsync();
                return;
            }

            // Com pesquisa: carrega candidatos e filtra/ordena em memória, para
            // ignorar maiúsculas/acentos (que o SQLite não trata) e pesquisar em
            // nome, desenvolvedora e nomes de géneros/plataformas.
            // ponytail: O(n) scan em memória; migrar para coluna normalizada ou FTS5 se a base passar de uns milhares de jogos.
            var term = TextSearch.Normalize(Search);

            var candidates = await query
                .Include(g => g.Genres)
                .Include(g => g.Platforms)
                .ToListAsync();

            Game = candidates
                .Select(g => new { Game = g, Rank = MatchRank(g, term) })
                .Where(x => x.Rank >= 0)
                .OrderBy(x => x.Rank)
                .ThenByDescending(x => x.Game.ReleaseDate)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .Select(x => x.Game)
                .ToList();
        }

        /// <summary>
        /// Relevância da correspondência (menor = mais relevante); -1 se não corresponde.
        /// </summary>
        private static int MatchRank(Game g, string term)
        {
            var name = TextSearch.Normalize(g.Name);
            if (name.StartsWith(term)) return 0;
            if (name.Contains(term)) return 1;
            if (TextSearch.Normalize(g.Developer).Contains(term)) return 2;
            if (g.Genres.Any(ge => TextSearch.Normalize(ge.Name).Contains(term))
                || g.Platforms.Any(p => TextSearch.Normalize(p.Name).Contains(term))) return 2;
            return -1;
        }
    }
}
