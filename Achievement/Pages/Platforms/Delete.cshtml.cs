using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Achievement.Data;
using Achievement.Models;

namespace Achievement.Pages.Platforms
{
    public class DeleteModel : PageModel
    {
        private readonly Achievement.Data.ApplicationDbContext _context;

        public DeleteModel(Achievement.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Platform Platform { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var platform = await _context.Platforms
                .Include(p => p.Games)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (platform is not null)
            {
                Platform = platform;

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

            var platform = await _context.Platforms
                .Include(p => p.Games)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (platform == null)
            {
                return NotFound();
            }

            // Bloqueia exclusão se houver jogos vinculados
            if (platform.Games.Any())
            {
                ModelState.AddModelError(string.Empty, "Não é possível excluir esta plataforma porque existem jogos vinculados. Remova as relações antes de excluir.");
                Platform = platform;
                return Page();
            }

            _context.Platforms.Remove(platform);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
