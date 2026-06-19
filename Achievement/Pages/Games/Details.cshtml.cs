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
    public class DetailsModel : PageModel
    {
        private readonly Achievement.Data.ApplicationDbContext _context;

        public DetailsModel(Achievement.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Game Game { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id, string? slug)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.Genres)
                .Include(g => g.Plataforms)
                .Include(g => g.Reviews)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
            {
                return NotFound();
            }

            // Se o slug na URL não corresponder ao slug real do jogo,
            // redireciona para a URL correta.
            if (string.IsNullOrEmpty(slug) || slug != game.Slug)
            {
                return RedirectToPagePermanent("./Details", new { id = game.Id, slug = game.Slug });
            }

            Game = game;
            return Page();
        }
    }
}
