using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Achievement.Data;
using Achievement.Models;

namespace Achievement.Pages.Genres
{
    public class IndexModel : PageModel
    {
        private readonly Achievement.Data.ApplicationDbContext _context;

        public IndexModel(Achievement.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Genre> Genre { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // Include Games to be able to show counts without additional queries
            Genre = await _context.Genres
                .Include(g => g.Games)
                .ToListAsync();
        }
    }
}
