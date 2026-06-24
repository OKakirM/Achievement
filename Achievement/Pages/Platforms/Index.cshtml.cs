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
    public class IndexModel : PageModel
    {
        private readonly Achievement.Data.ApplicationDbContext _context;

        public IndexModel(Achievement.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Platform> Platform { get;set; } = default!;

        public async Task OnGetAsync(bool showHidden = false)
        {
            Platform = await _context.Platforms
                .Include(p => p.Games)
                .OrderBy(p => p.Type).ThenBy(p => p.Name)
                .ToListAsync();
        }
    }
}
