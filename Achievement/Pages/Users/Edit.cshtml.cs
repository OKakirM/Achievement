using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Achievement.Data;
using Achievement.Models;
using Microsoft.AspNetCore.Identity;

namespace Achievement.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly Achievement.Data.ApplicationDbContext _context;

        public EditModel(Achievement.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User User { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            User = user;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Verifica se email está usado por outro user
            if (await _context.Users.AnyAsync(u => u.Email == User.Email && u.Id != User.Id))
            {
                ModelState.AddModelError("User.Email", "E-mail já em uso por outro utilizador.");
                return Page();
            }

            // Carrega entidade existente
            var existing = await _context.Users.FirstOrDefaultAsync(predicate: u => u.Id == User.Id);
            if (existing == null)
            {
                return NotFound();
            }

            // Atualiza apenas campos permitidos
            existing.Name = User.Name?.Trim() ?? existing.Name;
            existing.Email = User.Email?.Trim() ?? existing.Email;
            existing.Image = string.IsNullOrWhiteSpace(User.Image) ? null : User.Image.Trim();

            // Se a password foi preenchida, re-hash e atualiza. Caso contrário, mantém a existente.
            if (!string.IsNullOrWhiteSpace(User.Password) && User.Password != existing.Password)
            {
                var hasher = new PasswordHasher<User>();
                existing.Password = hasher.HashPassword(existing, User.Password);
            }

            // Marca a entidade como modificada e salva
            try
            {
                _context.Users.Update(existing);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(User.Id))
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

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
