using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Achievement.Data;
using Achievement.Models;

namespace Achievement.Pages.Genres
{
    public class DetailsModel : PageModel
    {
        private readonly Achievement.Data.ApplicationDbContext _context;

        public DetailsModel(Achievement.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Genre Genre { get; set; } = default!;
        public IList<Game> Games { get; set; } = new List<Game>();

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
                Games = genre.Games.ToList();

                return Page();
            }

            return NotFound();
        }
    }
}
