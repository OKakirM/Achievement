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
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Achievement.Pages.Genres
{
    [Authorize(Roles = "Admin")]
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            bool duplicatedName = await _context.Genres.AnyAsync(g => g.Name == Genre.Name);

            if (duplicatedName)
            {
                ModelState.AddModelError("Genre.Name", "Já existe um género com este nome. Por favor, escolha outro nome.");
                return Page();
            }

            _context.Genres.Add(Genre);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
