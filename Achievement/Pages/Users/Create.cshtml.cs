using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Achievement.Data;
using Achievement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Achievement.Pages.Users
{
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
        public User User { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Trim inputs
            User.Name = User.Name?.Trim() ?? string.Empty;
            User.Email = User.Email?.Trim() ?? string.Empty;

            // Validação: e-mail único
            if (await _context.Users.AnyAsync(u => u.Email == User.Email))
            {
                ModelState.AddModelError("User.Email", "E-mail já em uso.");
                return Page();
            }

            // Validação da password (regras mínimas)
            var pwd = User.Password ?? string.Empty;
            var pwdPattern = new System.Text.RegularExpressions.Regex(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*\-]).{8,}$");
            if (!pwdPattern.IsMatch(pwd))
            {
                ModelState.AddModelError("User.Password", "A Palavra-Passe tem que ter: mínimo 8 caracteres, pelo menos 1 letra maiúscula, 1 letra minúscula, 1 número e 1 caractere especial.");
                return Page();
            }

            // Hash da password antes de guardar
            var hasher = new PasswordHasher<User>();
            User.Password = hasher.HashPassword(User, User.Password);

            // Normalizar Image
            if (string.IsNullOrWhiteSpace(User.Image))
            {
                User.Image = null;
            }

            _context.Users.Add(User);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                // Log could be added here. For now return a generic error.
                ModelState.AddModelError(string.Empty, "Ocorreu um erro ao salvar o utilizador. Tenta novamente.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
