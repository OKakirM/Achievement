using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Achievement.Data;
using Achievement.Models;
using Microsoft.AspNetCore.Authorization;

namespace Achievement.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private const string AdminRole = "Admin";

        private readonly Achievement.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DeleteModel(Achievement.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public User User { get; set; } = default!;

        public int GamesCount { get; set; }
        public bool IsAdmin { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.UserGames)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user is null)
            {
                return NotFound();
            }

            User = user;
            GamesCount = user.UserGames.Count;

            var idUser = await _userManager.FindByEmailAsync(user.Email);
            IsAdmin = idUser != null && await _userManager.IsInRoleAsync(idUser, AdminRole);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                // A tabela User espelha a AspNetUsers (Identity): elimina também a conta de login.
                var idUser = await _userManager.FindByEmailAsync(user.Email);
                if (idUser != null)
                {
                    await _userManager.DeleteAsync(idUser);
                }

                // Remove os ficheiros de imagem do perfil
                DeletePhysicalFile(user.Image);
                DeletePhysicalFile(user.Banner);

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }

        private static void DeletePhysicalFile(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return;
            var full = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);
            if (System.IO.File.Exists(full))
            {
                System.IO.File.Delete(full);
            }
        }
    }
}
