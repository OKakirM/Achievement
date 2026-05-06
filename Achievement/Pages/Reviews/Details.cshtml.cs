using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Achievement.Data;
using Achievement.Models;

namespace Achievement.Pages.Reviews
{
    public class DetailsModel : PageModel
    {
        private readonly Achievement.Data.ApplicationDbContext _context;

        public DetailsModel(Achievement.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Review Review { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FirstOrDefaultAsync(m => m.Id == id);

            if (review is not null)
            {
                Review = review;

                return Page();
            }

            return NotFound();
        }
    }
}
