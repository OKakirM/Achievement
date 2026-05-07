using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Achievement.Data;
using Achievement.Models;
using Microsoft.EntityFrameworkCore;

namespace Achievement.Pages.Plataforms
{
    public class CreateModel : PageModel
    {
        private readonly Achievement.Data.ApplicationDbContext _context;

        public CreateModel(Achievement.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }
        [BindProperty]
        public Plataform Plataform { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Slug generation if not provided
            if (string.IsNullOrWhiteSpace(Plataform.Slug) && !string.IsNullOrWhiteSpace(Plataform.Name))
            {
                Plataform.Slug = Plataform.Name.Trim().ToLower().Replace(" ", "-");
            }

            // Prevent deleting if slug already exists (unique constraint recommended)
            var exists = await _context.Plataforms.AnyAsync(p => p.Slug == Plataform.Slug);
            if (exists)
            {
                ModelState.AddModelError(nameof(Plataform.Slug), "Slug já existe. Escolha outro.");
                return Page();
            }

            _context.Plataforms.Add(Plataform);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
