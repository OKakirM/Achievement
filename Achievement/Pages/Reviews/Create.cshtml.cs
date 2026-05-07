using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Achievement.Data;
using Achievement.Models;

namespace Achievement.Pages.Reviews
{
    public class CreateModel : PageModel
    {
        private readonly Achievement.Data.ApplicationDbContext _context;

        public CreateModel(Achievement.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet(int? productId)
        {
            ViewData["GameFK"] = new SelectList(_context.Games, "Id", "Description");
            ViewData["UserFK"] = new SelectList(_context.Users, "Id", "Email");

            if (productId.HasValue)
            {
                // Let the page pre-select the game being reviewed
                ViewData["SelectedGameId"] = productId.Value;
            }

            return Page();
        }

        [BindProperty]
        public Review Review { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Try to resolve the logged-in user by email claim or username
            string? email = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name;
            Models.User? currentUser = null;
            if (!string.IsNullOrWhiteSpace(email))
            {
                currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            }

            // If we found the user, set the foreign key; otherwise require the form to provide a valid UserFK (admin scenario)
            if (currentUser != null)
            {
                Review.UserFK = currentUser.Id;
            }
            else if (Review.UserFK == 0)
            {
                ModelState.AddModelError(string.Empty, "Utilizador não autenticado. Faça login para criar uma review.");
                ViewData["GameFK"] = new SelectList(_context.Games, "Id", "Description");
                ViewData["UserFK"] = new SelectList(_context.Users, "Id", "Email");
                return Page();
            }

            // Validate game exists
            var game = await _context.Games.Include(g => g.Users).FirstOrDefaultAsync(g => g.Id == Review.GameFK);
            if (game == null)
            {
                ModelState.AddModelError(string.Empty, "Jogo inválido.");
                ViewData["GameFK"] = new SelectList(_context.Games, "Id", "Description");
                ViewData["UserFK"] = new SelectList(_context.Users, "Id", "Email");
                return Page();
            }

            // Business rule: user can only review a game they "own". We treat Game.Users as ownership relation.
            if (currentUser != null)
            {
                var owns = game.Users.Any(u => u.Id == currentUser.Id);
                if (!owns)
                {
                    ModelState.AddModelError(string.Empty, "Só pode avaliar jogos que possui.");
                    ViewData["GameFK"] = new SelectList(_context.Games, "Id", "Description");
                    ViewData["UserFK"] = new SelectList(_context.Users, "Id", "Email");
                    return Page();
                }
            }

            // Ensure visible by default
            Review.IsVisible = true;

            _context.Reviews.Add(Review);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
