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

namespace Achievement.Pages.Platforms
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

            // Limpa as relações N-N (linhas da tabela de junção) antes de remover a plataforma
            platform.Games.Clear();
            _context.Platforms.Remove(platform);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
