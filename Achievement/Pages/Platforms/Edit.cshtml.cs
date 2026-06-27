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
    public class EditModel : PageModel
    {
        private readonly Achievement.Data.ApplicationDbContext _context;

        public EditModel(Achievement.Data.ApplicationDbContext context)
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

            var platform = await _context.Platforms.FirstOrDefaultAsync(m => m.Id == id);
            if (platform == null)
            {
                return NotFound();
            }
            Platform = platform;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Platform).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlatformExists(Platform.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool PlatformExists(int id)
        {
            return _context.Platforms.Any(e => e.Id == id);
        }
    }
}
