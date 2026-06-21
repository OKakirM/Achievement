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
    public class DetailsModel : PageModel
    {
        private readonly Achievement.Data.ApplicationDbContext _context;

        public DetailsModel(Achievement.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Platform Platform { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Platform = await _context.Platforms
                .Include(p => p.Games)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Platform is null)
            {
                return NotFound();
            }

            Platform = Platform;

            return Page();
        }
    }
}
