using Achievement.Data;
using Achievement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Achievement.Pages.Platforms
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
        public Platform Platform { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            bool duplicatedName = await _context.Platforms.AnyAsync(g => g.Name == Platform.Name);

            if (duplicatedName)
            {
                ModelState.AddModelError("Platform.Name", "Já existe uma plataforma  com este nome. Por favor, escolha outro nome.");
                return Page();
            }

            _context.Platforms.Add(Platform);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
