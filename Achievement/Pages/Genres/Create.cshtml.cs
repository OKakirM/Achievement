using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Achievement.Data;
using Achievement.Models;

namespace Achievement.Pages.Genres
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
        public Genre Genre { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Ensure slug exists
            if (string.IsNullOrWhiteSpace(Genre.Slug))
            {
                Genre.Slug = GenerateSlug(Genre.Name);
            }

            // Check uniqueness of slug
            if (_context.Genres.Any(g => g.Slug == Genre.Slug))
            {
                ModelState.AddModelError("Genre.Slug", "O slug informado já está em uso. Escolha outro.");
                return Page();
            }

            _context.Genres.Add(Genre);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private static string GenerateSlug(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            var s = value.Trim().ToLowerInvariant();
            // Replace spaces and invalid chars with hyphens
            s = Regex.Replace(s, "[^a-z0-9]+", "-");
            s = s.Trim('-');
            return s;
        }
    }
}
