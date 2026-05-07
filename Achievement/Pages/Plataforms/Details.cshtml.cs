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
    public class DetailsModel : PageModel
    {
        private readonly Achievement.Data.ApplicationDbContext _context;

        public DetailsModel(Achievement.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Plataform Plataform { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plataform = await _context.Plataforms
                .Include(p => p.Games)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (plataform is null)
            {
                return NotFound();
            }

            Plataform = plataform;

            return Page();
        }
    }
}
