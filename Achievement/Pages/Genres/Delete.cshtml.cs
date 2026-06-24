using Achievement.Data;
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
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly Achievement.Data.ApplicationDbContext _context;

        public DeleteModel(Achievement.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Genre Genre { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres
                .Include(g => g.Games)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (genre is not null)
            {
                Genre = genre;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres
                .Include(g => g.Games)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (genre != null)
            {
                // Block deletion if there are linked games
                if (genre.Games.Any())
                {
                    ModelState.AddModelError(string.Empty, "Não é possível excluir este gênero porque existem jogos vinculados. Remova as relações antes de excluir.");
                    Genre = genre;
                    return Page();
                }

                Genre = genre;
                _context.Genres.Remove(Genre);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
