using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Achievement.Data;
using Achievement.Models;

namespace Achievement.Pages.Plataforms
{
    public class DeleteModel : PageModel
    {
        private readonly Achievement.Data.ApplicationDbContext _context;

        public DeleteModel(Achievement.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Plataform Plataform { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plataform = await _context.Plataforms.FirstOrDefaultAsync(m => m.Id == id);

            if (plataform is not null)
            {
                Plataform = plataform;

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

            var plataform = await _context.Plataforms
                .Include(p => p.Games)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (plataform == null)
            {
                return NotFound();
            }

            // If platform has games, prevent physical delete and mark as not visible
            if (plataform.Games != null && plataform.Games.Any())
            {
                plataform.IsVisible = false;
                _context.Attach(plataform).State = EntityState.Modified;
            }
            else
            {
                _context.Plataforms.Remove(plataform);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
