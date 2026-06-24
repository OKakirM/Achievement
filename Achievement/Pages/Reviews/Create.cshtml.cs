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
using System.Security.Claims;
using System.Threading.Tasks;

namespace Achievement.Pages.Reviews
{
    public class CreateModel : PageModel
    {
        private readonly Achievement.Data.ApplicationDbContext _context;

        public CreateModel(Achievement.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet(int? id)
        {
            // Prepara os dados para os dropdowns de seleção de jogos e utilizadores
            ViewData["GameFK"] = new SelectList(_context.Games, "Id", "Description");
            ViewData["UserFK"] = new SelectList(_context.Users, "Id", "Email");

            if (id.HasValue)
            {
                ViewData["SelectedGameId"] = id.Value;
            }

            return Page();
        }

        [BindProperty]
        public Review Review { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Obtém o email do utilizador autenticado
            string? email = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name;
            Models.User? currentUser = null;

            // Se o email não for nulo ou vazio, procura o utilizador na base de dados
            if (!string.IsNullOrWhiteSpace(email))
            {
                currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            }

            // Se o utilizador autenticado for encontrado, define a chave estrangeira do utilizador na review
            if (currentUser != null)
            {
                Review.UserFK = currentUser.Id;
            }
            // Se o utilizador autenticado não for encontrado, adiciona um erro ao ModelState
            else if (Review.UserFK == 0)
            {
                ModelState.AddModelError(string.Empty, "Utilizador não autenticado. Faça login para criar uma review.");
                ViewData["GameFK"] = new SelectList(_context.Games, "Id", "Description");
                ViewData["UserFK"] = new SelectList(_context.Users, "Id", "Email");
                return Page();
            }

            // Verifica se o jogo existe na base de dados
            var game = await _context.Games.Include(g => g.UserGames).FirstOrDefaultAsync(g => g.Id == Review.GameFK);
            // Se o jogo não for encontrado, adiciona um erro ao ModelState
            if (game == null)
            {
                ModelState.AddModelError(string.Empty, "Jogo inválido.");
                ViewData["GameFK"] = new SelectList(_context.Games, "Id", "Description");
                ViewData["UserFK"] = new SelectList(_context.Users, "Id", "Email");
                return Page();
            }

            // Verifica se o utilizador autenticado possui o jogo antes de permitir a criação da review
            if (currentUser != null)
            {
                var owns = game.UserGames.Any(ug => ug.UserFK == currentUser.Id);
                if (!owns)
                {
                    ModelState.AddModelError(string.Empty, "Só pode avaliar jogos que possui.");
                    ViewData["GameFK"] = new SelectList(_context.Games, "Id", "Description");
                    ViewData["UserFK"] = new SelectList(_context.Users, "Id", "Email");
                    return Page();
                }
            }

            Review.CreatedAt = DateTime.UtcNow;
            _context.Reviews.Add(Review);
            await _context.SaveChangesAsync();

            await _context.RecalculateGameRatingAsync(Review.GameFK);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
